
using System;
using System.Collections.Generic;
using PlanetGeneration_OLD;
using PlanetGeneration_OLD.SphereGenerators;
using PlanetGeneration_OLD.Streams;
using PlanetGeneration_OLD.TerrainGenerators;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class PlanetGenerator_OLD : MonoBehaviour {
    static MeshJobScheduleDelegate sphereJob = SphereJob<GeoIcosphere, PositionStream>.ScheduleParallel;
    
    [Header("Terrain")]

    [SerializeField] TerrainGenerators terrainGenerator;
    enum TerrainGenerators {
        Basic, Layered
    }

    ITerrainGenerator[] terrainGenerators ={
        new BasicTerrainGenerator(),
        new LayeredTerrainGenerator()
    };

    [SerializeField]
    AbstractTerrainSettings[] settings;

    [Header("Sphere")]

    [SerializeField, Range(1, 80)]
    int resolution;
    int resolutionTest;

    [SerializeField]
    Mesh sphereMesh;
    Mesh mesh;

    [Flags]
    enum GizmoMode { Nothing = 0, Vertices = 1, Triangles = 2, Normals = 4, Tangents = 8, OverLapped = 16 }
    [SerializeField]
    GizmoMode gizmos;
    
    [NonSerialized]
    Vector3[] vertices, normals;
    [NonSerialized]
    Vector4[] tangents;
    [NonSerialized]
    int[] triangles;

    Vector3[] sphereVertices;

    bool updateSphere = true, updateTerrain = true;

    TerrainGenerators terrainGeneratorCheck;
    AbstractTerrainSettings settingsCheck;
    int resolutionCheck;

    void Awake() {
        mesh = new Mesh() {
            name = "PlanetMesh"
        };
        transform.GetComponent<MeshFilter>().mesh = mesh;

        terrainGeneratorCheck = terrainGenerator;
        settingsCheck = Instantiate(settings[(int)terrainGenerator]);
        resolutionCheck = resolution;
    }

	void Update () {
        CheckForChanges();
		GenerateMesh();

        vertices = null;
        normals = null;
        tangents = null;
        triangles = null;
	}

    void CheckForChanges() {
        if (terrainGenerator != terrainGeneratorCheck) {
            terrainGeneratorCheck = terrainGenerator;
            Destroy(settingsCheck);
            settingsCheck = Instantiate(settings[(int)terrainGenerator]);

            updateTerrain = true;
        } else
        if (settings[(int)terrainGenerator].CheckForChanges(settingsCheck)) {
            Destroy(settingsCheck);
            settingsCheck = Instantiate(settings[(int)terrainGenerator]);

            updateTerrain = true;
        } else
        if (resolutionCheck != resolution) {
            resolutionCheck = resolution;

            updateSphere = true;
        }
    }

	void GenerateMesh () {
        if (updateSphere) {
            GenerateSphere();
        }
        if (updateSphere || updateTerrain) {
            GenerateTerrain();

            updateTerrain = false;
            updateSphere = false;
        }
	}

    void GenerateSphere() {
        if (sphereMesh == null) {
            Mesh.MeshDataArray meshDataArray = Mesh.AllocateWritableMeshData(1);
            Mesh.MeshData meshData = meshDataArray[0];

            sphereJob(meshData, resolution, default).Complete();
            Mesh.ApplyAndDisposeWritableMeshData(meshDataArray, mesh);

            //MeshSaverEditor.SaveMesh(mesh, "SphereMesh" + resolution, true, false);
            CopySphereVerticeData(mesh);
        } else {
            mesh = Instantiate(sphereMesh);
            CopySphereVerticeData(mesh);
            transform.GetComponent<MeshFilter>().mesh = mesh;
        }
    }

    void GenerateTerrain() {
        terrainGenerators[(int)terrainGenerator].Generate(ref mesh, sphereVertices, settings[(int)terrainGenerator]);

        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        mesh.RecalculateBounds();
    }

    [SerializeField] 
    float approxFactor = 1000;

    void OnDrawGizmos () {
		if (gizmos == GizmoMode.Nothing || mesh == null) return;
        if (vertices == null) vertices = mesh.vertices;

        bool drawVertices = (gizmos & GizmoMode.Vertices) != 0;
        bool drawNormals = (gizmos & GizmoMode.Normals) != 0;
        bool drawTangents = (gizmos & GizmoMode.Tangents) != 0;
        bool drawTriangles = (gizmos & GizmoMode.Triangles) != 0;

        if (drawNormals && normals == null) normals = mesh.normals;
        if (drawTangents && tangents == null) tangents = mesh.tangents;
        if (drawTriangles && triangles == null) triangles = mesh.triangles;

        Transform t = transform;

        HashSet<Vector3Int> positions = new HashSet<Vector3Int>();

        for (int i = 0; i < vertices.Length; i++) {
            Vector3 position = t.TransformPoint(vertices[i]);
            if (drawVertices) {
                Vector3Int approx = new Vector3Int((int)(vertices[i].x * approxFactor), 
                                                   (int)(vertices[i].y * approxFactor),
                                                   (int)(vertices[i].z * approxFactor)
                                                   );

                if (positions.Contains(approx)) {
                    Gizmos.color = Color.cyan;
                    Gizmos.DrawSphere(position, 0.016f);
                } else {
                    Gizmos.color = Color.red;
                    Gizmos.DrawSphere(position, 0.02f);

                    positions.Add(approx);
                }
            }
            if (drawNormals) {
                Gizmos.color = Color.green;
                Gizmos.DrawRay(position, t.TransformDirection(normals[i]) * 0.2f);
            }
            if (drawTangents) {
                Gizmos.color = Color.red;
                Gizmos.DrawRay(position, t.TransformDirection(tangents[i]) * 0.2f);
            }
        }

        if (drawTriangles) {
            float colorStep = 1f / (triangles.Length - 3);
            for (int i = 0; i < triangles.Length; i += 3) {
                float c = i * colorStep;
                Gizmos.color = new Color(c, 0f, c);
                Gizmos.DrawSphere(
                    t.TransformPoint((
                        vertices[triangles[i]] +
                        vertices[triangles[i + 1]] +
                        vertices[triangles[i + 2]]
                    ) * (1f / 3f)),
                    0.02f
                );

                Gizmos.color = Color.red;
                Gizmos.DrawLine(t.TransformPoint(vertices[triangles[i    ]]), t.TransformPoint(vertices[triangles[i + 1]]));
                Gizmos.DrawLine(t.TransformPoint(vertices[triangles[i    ]]), t.TransformPoint(vertices[triangles[i + 2]]));
                Gizmos.DrawLine(t.TransformPoint(vertices[triangles[i + 1]]), t.TransformPoint(vertices[triangles[i + 2]]));
            }
        }
	}

    void OnValidate() {
        updateSphere = updateTerrain = true;
    }

    void CopySphereVerticeData(Mesh m) {
        sphereVertices = new Vector3[m.vertices.Length];
        Array.Copy(m.vertices, sphereVertices, m.vertices.Length);
    }
}


