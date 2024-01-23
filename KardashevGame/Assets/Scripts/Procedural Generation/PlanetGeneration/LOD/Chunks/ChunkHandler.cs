
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using PlanetGeneration;
using PlanetGeneration.Chunks;
using PlanetGeneration.TerrainGeneration;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

namespace PlanetGeneration.Chunks {
    public class ChunkHandler {
        private List<Chunk> generatedChunks;
        private HashSet<Chunk> loadedChunks;
        private List<Chunk> createChunks, deactivatedChunks;

        private CubeSphere cubeSphere;
        private ChunkRangeTest chunkRangeTest;

        private int maxDepth = 0;
        private Transform[] levels;

        private int meshWorkload;

        public ChunkHandler(CubeSphere cubeSphere, int maxDepth, Transform[] levels, ChunkRangeTest chunkRangeTest) {
            this.cubeSphere = cubeSphere;
            this.maxDepth = maxDepth;
            this.levels = levels;
            this.chunkRangeTest = chunkRangeTest;

            loadedChunks = new HashSet<Chunk>();
            generatedChunks = new List<Chunk>(cubeSphere.faces);
            createChunks = new List<Chunk>();
            deactivatedChunks = new List<Chunk>();
        }

        public void UpdateLoadedChunks() {
            meshWorkload = 0;

            List<Chunk> chunksInRange = GetChunksInRange();
            List<Chunk> toBeLoaded = FilterLoadedChunks(chunksInRange);

            RecycleAndLoad(toBeLoaded);
            loadedChunks.AddRange(chunksInRange);
        }

        // Finds chunks in Range of the observer
        private List<Chunk> GetChunksInRange() {
            List<Chunk> quadTree = new List<Chunk>(cubeSphere.faces);
            List<Chunk> chunksInRange = new List<Chunk>();

            chunkRangeTest.Init();

            while (quadTree.Any()) {
                Chunk current = quadTree[0];
                quadTree.RemoveAt(0);

                if (current.mesh == null) {
                    cubeSphere.GenerateSubChunk(current);
                    generatedChunks.Add(current);

                    meshWorkload++;
                }
                
                if (chunkRangeTest.IsInRange(current)) {
                    if (current.Depth == chunkRangeTest.depth) {
                        chunksInRange.Add(current);
                    } else {
                        if (current.subChunks == null)
                            current.SubDivide();

                        quadTree.AddRange(current.subChunks);
                    }
                }
            }

            return chunksInRange;
        }

        // Filters out already loaded chunks, returns List of chunks to be loaded
        private List<Chunk> FilterLoadedChunks(List<Chunk> chunksInRange) {
            List<Chunk> toBeLoaded = new List<Chunk>();

            foreach (Chunk chunk in chunksInRange) {
                if (loadedChunks.Contains(chunk)) {
                    loadedChunks.Remove(chunk);
                } else {
                    toBeLoaded.Add(chunk);
                }
            }

            foreach (Chunk chunk in loadedChunks) {
                chunk.myObject.gameObject.SetActive(false);
                deactivatedChunks.Add(chunk);
            }   
            loadedChunks.Clear();

            return toBeLoaded;
        }

        // Loads all chunks to be loaded, reusing chunks that can be unloaded
        // Fully loads and unloads all chunks
        private void RecycleAndLoad(List<Chunk> toBeLoaded) {
            foreach (Chunk chunk in toBeLoaded) {
                if (chunk.myObject != null)
                    chunk.myObject.gameObject.SetActive(true);
                else if (deactivatedChunks.Any()) {
                    Chunk deactivatedChunk = deactivatedChunks[0];
                    deactivatedChunks.RemoveAt(0);

                    chunk.myObject = deactivatedChunk.myObject;
                    deactivatedChunk.myObject = null;
                    
                    chunk.myObject.gameObject.SetActive(true);
                    chunk.myObject.transform.SetParent(levels[chunk.Depth]);
                    chunk.ApplyMesh();
                } else {
                    createChunks.Add(chunk);
                }
            }
        }

        public void PlanetTestMode(int faces) {
            foreach (Chunk chunk in loadedChunks) {
                chunk.myObject.gameObject.SetActive(false);
                deactivatedChunks.Add(chunk);
            }

            loadedChunks.Clear();
            generatedChunks.Clear();
            for (int i = 0; i < 6; i++) {
                if ((faces & Func.ThePowersThatB2[i]) != 0) {
                    loadedChunks.Add(cubeSphere.faces[i]);
                    generatedChunks.Add(cubeSphere.faces[i]);

                    if (cubeSphere.faces[i].myObject == null) {
                        createChunks.Add(cubeSphere.faces[i]);
                    } else {
                        cubeSphere.faces[i].myObject.gameObject.SetActive(true);
                        deactivatedChunks.Remove(cubeSphere.faces[i]);
                    }
                }
            }
        }

        public List<Chunk> GetChunksToCreate() => createChunks;
        public HashSet<Chunk> GetLoadedChunks() => loadedChunks;
        public List<Chunk> GetGeneratedChunks() => generatedChunks;
        public void SetChunkRangeTest(ChunkRangeTest chunkRangeTest) => this.chunkRangeTest = chunkRangeTest;
    }
}
