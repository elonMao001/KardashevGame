
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
    private float updateCounter = 0;
    [SerializeField]
    private float updateDelay = 10;

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
        colorGenerator.UpdatePlanetRadius(shapeSettings.radius);
    }

    private void Update() {
        if (testPlanet) {
            if (lastTestMode != testMode || !testPlanetInited) {
                chunkHandler.PlanetTestMode((int)testMode);
                CreateChunks();

                lastTestMode = testMode;
                testPlanetInited = true;
            }

            if (shapeNeedsUpdating && updateCounter > updateDelay) { 
                UpdateShape();

                shapeNeedsUpdating = false;
                updateCounter = 0;
            }
        } else if (updateCounter >= updateDelay) {
            chunkHandler.UpdateLoadedChunks();
            CreateChunks();

            updateCounter = 0;
        }

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


        //tempor�rer (?) Timon-Eingriff
        //chunk.myObject.GetComponent<MeshCollider>().sharedMesh = chunk.mesh;


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

    public float GetRadius() {
        return transform.localScale.x * shapeSettings.radius;
    }

    public void OnValidate() {
        if (!testPlanet) testPlanetInited = false;
    }
}
