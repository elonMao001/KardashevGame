
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using PlanetGeneration;
using PlanetGeneration.Chunks;
using PlanetGeneration.TerrainGeneration;
using Unity.VisualScripting;
using UnityEngine;

public class PlanetGenerator : MonoBehaviour {
    [Header("General")]

    [SerializeField]
    private Transform chunkPrefab;
    [SerializeField]
    private Transform ocean;
    [SerializeField]
    private Transform colliders;

    public Transform observer;

    public ShapeSettings shapeSettings;
    public ColorSettings colorSettings;

    [Header("Settings")]

    [SerializeField]
    private float rotationSpeed;

    private bool testPlanetInited;

    [Flags]
    private enum GizmoMode {
        Nothing, Vertices = 1, Triangles = 2, Normals = 4, Tangents = 8
    }
    [SerializeField]
    private GizmoMode gizmoMode;

    [SerializeField]
    private TestMode testMode;
    private enum TestMode {
        Off, Faces, Depth
    }

    [SerializeField]
    private FaceMode faceMode;
    [Flags]
    public enum FaceMode {
        None = 0, Right = 1, Top = 2, Front = 4, Back = 8, Bottom = 16, Left = 32
    }

    [SerializeField, Min(1)]
    private int testDepth;
    [SerializeField, Min(0)]
    private float chunkDispersion;

    private TerrainGenerator terrainGenerator;
    private ColorGenerator colorGenerator;
    private CubeSphere cubeSphere;
    private ChunkHandler chunkHandler;
    private Transform[] levels;

    [HideInInspector]
    public const int maxDepth = 3;
    private bool shapeNeedsUpdating = false;
    private float updateCounter = 0;
    [SerializeField]
    private float updateDelay = 10;

    [Range(0.01f, 1f)]
    public float maxChunkViewPercentage;

    [SerializeField, Range(1, 50)]
    private int chunkTestIts;
    
    void Start() { 
        chunkPrefab.GetComponent<MeshRenderer>().sharedMaterial = colorSettings.planetMaterial;
        ocean.transform.localScale = 2f * transform.localScale * shapeSettings.radius;

        levels = new Transform[maxDepth + 1];
        for (int i = 0; i < levels.Length; i++) {
            levels[i] = new GameObject("Depth " + i).transform;
            levels[i].SetParent(transform);
        }
        
        shapeSettings.Init();
        shapeSettings.InitTerrainLayers();

        terrainGenerator = new TerrainGenerator(shapeSettings);
        colorGenerator = new ColorGenerator(colorSettings);

        cubeSphere = new CubeSphere(terrainGenerator);
        chunkHandler = new ChunkHandler(cubeSphere, maxDepth, levels, new ChunkRangeTest(this));

        GenerateColliders();

        colorGenerator.UpdateElevationMinMax(terrainGenerator.minmax);
        colorGenerator.UpdateSurfaceGradient();
        colorGenerator.UpdateOceanfloorGradient();
        colorGenerator.UpdatePlanetRadius(shapeSettings.radius);

        cubeSphere.faces[0].SubDivide();
        cubeSphere.GenerateSubChunk(cubeSphere.faces[0].subChunks[0]);
    }

    private void Update() {
        if (testMode == TestMode.Off) {
            if (updateCounter >= updateDelay) {
                chunkHandler.UpdateLoadedChunks();
                CreateChunks();

                foreach (Chunk chunk in chunkHandler.GetLoadedChunks()) {
                    chunk.myObject.position = transform.TransformPoint(Vector3.Normalize(chunk.mesh.bounds.center) * chunkDispersion);
                }

                updateCounter = 0;
            }
        } else {
            if (!testPlanetInited) {
                if (testMode == TestMode.Faces) {
                    chunkHandler.PlanetTestMode((int)faceMode, 0);
                    CreateChunks();

                    testPlanetInited = true;
                } else 
                if (testMode == TestMode.Depth) {
                    chunkHandler.PlanetTestMode((int)faceMode, testDepth);
                    CreateChunks();

                    testPlanetInited = true;
                }

                foreach (Chunk chunk in chunkHandler.GetLoadedChunks()) {
                    chunk.myObject.position = transform.TransformPoint(Vector3.Normalize(chunk.mesh.bounds.center) * chunkDispersion);
                }
            }

            if (shapeNeedsUpdating && updateCounter >= updateDelay) { 
                UpdateShape();

                ocean.transform.localScale = 2f * transform.localScale * shapeSettings.radius;

                shapeNeedsUpdating = false;
                updateCounter = 0;
            }
        }

        transform.localEulerAngles += new Vector3(0, rotationSpeed * Time.deltaTime, 0);
        updateCounter += Time.deltaTime;
    }

    private void CreateChunks() {
        List<Chunk> chunks = chunkHandler.GetChunksToCreate();

        while (chunks.Any()) {
            CreateChunkObject(chunks[0]);
            chunks.RemoveAt(0);
        }
    }

    private void CreateChunkObject(Chunk chunk) {
        chunk.myObject = Instantiate(chunkPrefab, levels[chunk.Depth]);

        chunk.ApplyMesh();
    }

    public void OnShapeSettingsUpdated() {
        if (chunkHandler == null) return;

        shapeNeedsUpdating = true;
    }

    private void UpdateLayersAndGenerators() {
        shapeSettings.InitTerrainLayers();
        terrainGenerator.InitTerrainBundles();
        terrainGenerator.minmax.Reset();
    }   

    private void UpdateShape() {
        UpdateLayersAndGenerators();
        UpdateLoadedFaces();
        colorGenerator.UpdatePlanetRadius(shapeSettings.radius);
        colorGenerator.UpdateElevationMinMax(terrainGenerator.minmax);
    }

    private void UpdateLoadedFaces() {
        Debug.Log(chunkHandler.GetLoadedChunks().Count);
        foreach (Chunk chunk in chunkHandler.GetLoadedChunks()) {
            cubeSphere.GenerateFace(chunk.Index);
        }
    }

    public void OnColorSettingsUpdated() {
        if (chunkHandler == null) return;

        chunkPrefab.GetComponent<MeshRenderer>().sharedMaterial = colorSettings.planetMaterial;
        foreach (Chunk chunk in chunkHandler.GetLoadedChunks()) {
            if (chunk.myObject != null)
                chunk.myObject.GetComponent<MeshRenderer>().sharedMaterial = colorSettings.planetMaterial;
        }
        
        colorGenerator.UpdateSurfaceGradient();
        colorGenerator.UpdateOceanfloorGradient();
    }

    public void TestFaceGeneration() {
        float time = Time.realtimeSinceStartup;
        for (int i = 0; i < chunkTestIts; i++) {
            cubeSphere.faces[0].GenerateFace(terrainGenerator, cubeSphere.universalTriangles);
        }
        Debug.Log((Time.realtimeSinceStartup - time) / chunkTestIts);
    }

    public void TestSubChunkGeneration() {
        float time = Time.realtimeSinceStartup;
        for (int i = 0; i < chunkTestIts; i++) {
            cubeSphere.faces[0].subChunks[0].Generate(terrainGenerator, cubeSphere.universalTriangles);
        }
        Debug.Log((Time.realtimeSinceStartup - time) / chunkTestIts);
    }

    public void GenerateColliders() {
        if (colliders == null) return;

        for (int i = 0; i < cubeSphere.faces.Length; i++) {
            Transform collider = new GameObject("Collider " + i, typeof(MeshCollider)).transform;
            collider.GetComponent<MeshCollider>().sharedMesh = cubeSphere.GetFace(i).mesh;
            collider.SetParent(colliders);
            collider.tag = "Ground";
        }
    }

    public float GetRadius() {
        return transform.localScale.x * shapeSettings.radius;
    }

    public void OnValidate() {
        testPlanetInited = false;
    }

    public void OnDrawGizmos() {
        if (testMode == TestMode.Off || gizmoMode == GizmoMode.Nothing) return;

        HashSet<Chunk> chunks = chunkHandler.GetLoadedChunks();

        bool bvertices = (gizmoMode & GizmoMode.Vertices) != 0;
        bool bnormals = (gizmoMode & GizmoMode.Normals) != 0;
        bool btriangles = (gizmoMode & GizmoMode.Triangles) != 0;

        foreach (Chunk chunk in chunks) {
            Mesh mesh = chunk.mesh;

            Vector3[] verts = mesh.vertices;

            if (bvertices) {
                Gizmos.color = Color.red;

                int index = 0;
                foreach (Vector3 vert in verts) {
                    Gizmos.DrawSphere(vert, 0.02f);

                    index++;
                }
            }

            if (btriangles) {
                Gizmos.color = Color.red;

                int[] triangles = mesh.triangles;
                for (int i = 0; i < triangles.Length; i += 3) {
                    Gizmos.DrawLine(verts[triangles[i]], verts[triangles[i + 1]]);
                    Gizmos.DrawLine(verts[triangles[i]], verts[triangles[i + 2]]);
                    Gizmos.DrawLine(verts[triangles[i + 1]], verts[triangles[i + 2]]);
                }
            }

            if (bnormals) {
                Gizmos.color = Color.green;

                Vector3[] normals = mesh.normals;
                for (int i = 0; i < normals.Length; i++) {
                    Gizmos.DrawRay(verts[i], normals[i] * 2);
                }
            }
        }
    }
}
