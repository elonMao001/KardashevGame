
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using PlanetGeneration;
using PlanetGeneration.Chunks;
using PlanetGeneration.TerrainGeneration;
using Unity.VisualScripting;
using UnityEngine;

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

        public void UpdateLoadedChunks(InitData initData) {
            List<Chunk> chunksInRange = GetChunksInRange(initData);
            List<Chunk> toBeLoaded = FilterLoadedChunks(chunksInRange);

            RecycleAndLoad(toBeLoaded);

            loadedChunks.Clear();
            loadedChunks.AddRange(chunksInRange);
        }

        // Finds chunks in Range of the observer
        private List<Chunk> GetChunksInRange(InitData initData) {
            List<Chunk> quadTree = new List<Chunk>(cubeSphere.faces);
            List<Chunk> chunksInRange = new List<Chunk>();

            chunkRangeTest.Init(initData);

            while (quadTree.Any()) {
                Chunk current = quadTree[0];
                quadTree.RemoveAt(0);

                if (current.mesh == null) {
                    cubeSphere.GenerateSubChunk(current);
                    generatedChunks.Add(current);
                }
                
                if (current.Depth < maxDepth && chunkRangeTest.IsInRange(current)) {
                    if (current.subChunks == null)
                        current.SubDivide();

                    quadTree.AddRange(current.subChunks);
                } else chunksInRange.Add(current);
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

        public List<Chunk> GetChunksToCreate() => createChunks;
        public List<Chunk> GetChunksToDestroy() => destroyChunks;
        public HashSet<Chunk> GetLoadedChunks() => loadedChunks;
        public List<Chunk> GetGeneratedChunks() => generatedChunks;
        public void SetChunkRangeTest(ChunkRangeTest chunkRangeTest) => this.chunkRangeTest = chunkRangeTest;
    }
}
