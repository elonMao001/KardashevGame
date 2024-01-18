
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
    [SerializeField]
    private Transform observer;

    public ShapeSettings shapeSettings;
    public ColorSettings colorSettings;

    public enum PlanetViewMode {
        Normal, Front
    }
    [SerializeField]
    private PlanetViewMode planetViewMode;

    [Header("Settings")]

    [SerializeField, Range(0.01f, 10f)]
    private float distanceThreshold;

    public enum ChunkRangeTestMode { Distance, ClosestDistance, UsingHelpSphere }
    [SerializeField]
    public ChunkRangeTest[] chunkRangeTests = new ChunkRangeTest[] {
        new Distance(), new ClosestDistance(), new UsingHelpSphere()
    };
    [SerializeField]
    private ChunkRangeTestMode chunkRangeTestMode = 0;

    private TerrainGenerator terrainGenerator;
    private ColorGenerator colorGenerator;
    private CubeSphere cubeSphere;
    private ChunkHandler chunkHandler;
    private Transform[] levels;

    private int maxDepth = 5;
    private bool shapeNeedsUpdating = false;
    private int updateCounter = 0;
    [SerializeField]
    private int updateDelay = 10;
    
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
        chunkHandler = new ChunkHandler(cubeSphere, maxDepth, levels, chunkRangeTests[(int)chunkRangeTestMode]);

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

        if (planetViewMode == PlanetViewMode.Normal) {
            chunkHandler.UpdateLoadedChunks(new InitData(observer, transform, distanceThreshold, shapeSettings));
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

    private void UpdateLayeresAndGenerators() {
        shapeSettings.InitTerrainLayers();
        terrainGenerator.InitTerrainBundles();
        terrainGenerator.minmax.Reset();
    }

    public void UpdateShape() {
        UpdateLayeresAndGenerators();

        if (planetViewMode == PlanetViewMode.Normal)
            UpdateChunks();
        else {
            int index = (int)planetViewMode - 1;
            foreach (Chunk chunk in chunkHandler.GetGeneratedChunks()) 
                Destroy(chunk.myObject.gameObject);
            chunkHandler.GetGeneratedChunks().Clear();
            chunkHandler.GetLoadedChunks().Clear();

            cubeSphere.GenerateFace(index);
            CreateChunkObject(cubeSphere.faces[index]);
            chunkHandler.GetGeneratedChunks().Add(cubeSphere.faces[index]);
            chunkHandler.GetLoadedChunks().Add(cubeSphere.faces[index]);
        } 

        colorGenerator.UpdateElevationMinMax(terrainGenerator.minmax);
        colorGenerator.UpdatePlanetRadius(shapeSettings.radius);
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
            chunk.myObject.GetComponent<MeshRenderer>().sharedMaterial = colorSettings.planetMaterial;
        }
        
        colorGenerator.UpdateSurfaceGradient();
        colorGenerator.UpdateOceanfloorGradient();
    }

    public float GetRadius() {
        return transform.localScale.x * shapeSettings.radius;
    }
}
