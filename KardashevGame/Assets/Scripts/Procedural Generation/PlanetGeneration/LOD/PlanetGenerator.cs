
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using PlanetGeneration;
using PlanetGeneration.Chunks;
using PlanetGeneration.TerrainGeneration;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph;
using UnityEditorInternal;
using UnityEngine;

public class PlanetGenerator : MonoBehaviour {
    [Header("General")]

    [SerializeField]
    private Transform chunkPrefab;

    public Transform observer;

    public ShapeSettings shapeSettings;
    public ColorSettings colorSettings;

    [Header("Settings")]

    [SerializeField]
    private bool testPlanet;
    private bool testPlanetInited;

    [SerializeField]
    private TestMode testMode;
    private TestMode lastTestMode;
    [Flags]
    public enum TestMode {
        Nothing = 0, Right = 1, Top = 2, Front = 4, Back = 8, Bottom = 16, Left = 32
    }

    private TerrainGenerator terrainGenerator;
    private ColorGenerator colorGenerator;
    private CubeSphere cubeSphere;
    private ChunkHandler chunkHandler;
    private Transform[] levels;

    [HideInInspector]
    public const int maxDepth = 3;
    private bool shapeNeedsUpdating = false;
    private int updateCounter = 0;
    [SerializeField]
    private int updateDelay = 10;

    [Range(0.01f, 1f)]
    public float maxChunkViewPercentage;
    
    void Start() { 
        chunkPrefab.GetComponent<MeshRenderer>().sharedMaterial = colorSettings.planetMaterial;

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

        colorGenerator.UpdateElevationMinMax(terrainGenerator.minmax);
        colorGenerator.UpdateSurfaceGradient();
        colorGenerator.UpdateOceanfloorGradient();
        colorSettings.planetMaterial.SetFloat("_planetRadius", shapeSettings.radius);


    }

    private void FixedUpdate() {
        if (shapeNeedsUpdating && updateCounter > updateDelay) { 
            UpdateShape();

            shapeNeedsUpdating = false;
            updateCounter = 0;
        } else updateCounter++;

        if (testPlanet) {
            if (lastTestMode != testMode || !testPlanetInited) {
                chunkHandler.PlanetTestMode((int)testMode);
                CreateChunks();
                DestroyChunks();

                lastTestMode = testMode;
                testPlanetInited = true;
            }
        } else {
            chunkHandler.UpdateLoadedChunks();
            CreateChunks();
            DestroyChunks();
        }
    }

    private void CreateChunks() {
        List<Chunk> chunks = chunkHandler.GetChunksToCreate();

        while (chunks.Any()) {
            CreateChunkObject(chunks[0]);
            chunks.RemoveAt(0);
        }
    }

    private void DestroyChunks() {
        List<Chunk> chunks = chunkHandler.GetChunksToDestroy();

        while (chunks.Any()) {
            Destroy(chunks[0].myObject.gameObject);
            chunks.RemoveAt(0);
        }
    } 

    private void CreateChunkObject(Chunk chunk) {
        chunk.myObject = Instantiate(chunkPrefab, levels[chunk.Depth]);


        //temporärer (?) Timon-Eingriff
        chunk.myObject.GetComponent<MeshCollider>().sharedMesh = chunk.mesh;


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

    public void UpdateShape() {
        UpdateLayersAndGenerators();
        UpdateChunks();
        colorSettings.planetMaterial.SetFloat("_PlanetRadius", shapeSettings.radius);
    }

    public void UpdateChunks() {
        List<Chunk> chunks = chunkHandler.GetGeneratedChunks();
        chunks = chunks.OrderBy(c => c.Depth).ToList();
        
        cubeSphere.GenerateFaces();
        foreach (Chunk chunk in chunkHandler.GetGeneratedChunks())
            if (chunk.Depth != 0)
                cubeSphere.GenerateSubChunk(chunk); 
    }

    public void OnColorSettingsUpdated() {
        if (chunkHandler == null) return;

        chunkPrefab.GetComponent<MeshRenderer>().sharedMaterial = colorSettings.planetMaterial;
        foreach (Chunk chunk in chunkHandler.GetGeneratedChunks()) {
            Debug.Log(chunk.Index);
            chunk.myObject.GetComponent<MeshRenderer>().sharedMaterial = colorSettings.planetMaterial;
        }
        
        colorGenerator.UpdateSurfaceGradient();
        colorGenerator.UpdateOceanfloorGradient();
    }

    public float GetRadius() {
        return transform.localScale.x * shapeSettings.radius;
    }

    public void OnValidate() {
        if (!testPlanet) testPlanetInited = testPlanet;
    }
}
