
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
        private List<Chunk> createChunks, destroyChunks;

        private CubeSphere cubeSphere;
        private ChunkRangeTest chunkRangeTest;

        private int maxDepth = 0;
        private Transform[] levels;

        public ChunkHandler(CubeSphere cubeSphere, int maxDepth, Transform[] levels, ChunkRangeTest chunkRangeTest) {
            this.cubeSphere = cubeSphere;
            this.maxDepth = maxDepth;
            this.levels = levels;
            this.chunkRangeTest = chunkRangeTest;

            loadedChunks = new HashSet<Chunk>();
            generatedChunks = new List<Chunk>(cubeSphere.faces);
            createChunks = new List<Chunk>();
            destroyChunks = new List<Chunk>();
        }

        public void UpdateLoadedChunks() {
            List<Chunk> chunksInRange = GetChunksInRange();
            List<Chunk> toBeLoaded = FilterLoadedChunks(chunksInRange);

            RecycleAndLoad(toBeLoaded);

            loadedChunks.Clear();
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

            return toBeLoaded;
        }

        // Loads all chunks to be loaded, reusing chunks that can be unloaded
        // Fully loads and unloads all chunks
        private void RecycleAndLoad(List<Chunk> toBeLoaded) {
            int tblcount = toBeLoaded.Count, tblindex = 0;

            foreach (Chunk chunk in loadedChunks) {
                if (tblindex < tblcount) {
                    toBeLoaded[tblindex].myObject = chunk.myObject;
                    chunk.myObject = null;

                    toBeLoaded[tblindex].myObject.transform.SetParent(levels[toBeLoaded[tblindex].Depth]);
                    toBeLoaded[tblindex].ApplyMesh();

                    tblindex++;
                } else {
                    destroyChunks.Add(chunk);
                }
            }

            while (tblindex < tblcount) {
                createChunks.Add(toBeLoaded[tblindex]);
                
                tblindex++;
            }
        }

        public void PlanetTestMode(int faces) {
            destroyChunks.AddRange(loadedChunks);

            loadedChunks.Clear();
            generatedChunks.Clear();
            for (int i = 0; i < 6; i++) {
                if ((faces & Func.ThePowersThatB2[i]) != 0) {
                    loadedChunks.Add(cubeSphere.faces[i]);
                    generatedChunks.Add(cubeSphere.faces[i]);
                    createChunks.Add(cubeSphere.faces[i]);
                    destroyChunks.Remove(cubeSphere.faces[i]);
                }
            }
        }

        public List<Chunk> GetChunksToCreate() => createChunks;
        public List<Chunk> GetChunksToDestroy() => destroyChunks;
        public HashSet<Chunk> GetLoadedChunks() => loadedChunks;
        public List<Chunk> GetGeneratedChunks() => generatedChunks;
        public void SetChunkRangeTest(ChunkRangeTest chunkRangeTest) => this.chunkRangeTest = chunkRangeTest;
    }
}
