
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace PlanetGeneration.Chunks {
    public class ChunkHandler {
        private List<Chunk> generatedChunks;
        private HashSet<Chunk> loadedChunks;
        private List<Chunk> createChunks, deactivatedChunks;

        private CubeSphere cubeSphere;
        private ChunkRangeTest chunkRangeTest;

        private Transform[] levels;

        private int meshWorkload;

        public ChunkHandler(CubeSphere cubeSphere, int maxDepth, Transform[] levels, ChunkRangeTest chunkRangeTest) {
            this.cubeSphere = cubeSphere;
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
                if (chunk.myObject != null) {
                    chunk.myObject.gameObject.SetActive(true);
                    deactivatedChunks.Remove(chunk);
                } else if (deactivatedChunks.Any()) {
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

        public void PlanetTestMode(int faces, int depth) {
            foreach (Chunk chunk in loadedChunks) {
                chunk.myObject.gameObject.SetActive(false);
                deactivatedChunks.Add(chunk);
            }

            List<Chunk> newChunks = new List<Chunk>(), tempChunks = new List<Chunk>();

            loadedChunks.Clear();
            generatedChunks.Clear();
            for (int i = 0; i < 6; i++) {
                if ((faces & Func.ThePowersThatB2[i]) != 0) {
                    newChunks.Add(cubeSphere.faces[i]);
                }
            }

            if (depth > 0) {
                for (int i = 0; i < 6; i++) {
                    if (newChunks[i].myObject != null) {
                        deactivatedChunks.Add(newChunks[i]);
                        newChunks[i].myObject.gameObject.SetActive(false);
                    }

                    if (newChunks[i].mesh == null)
                        cubeSphere.GenerateFace(newChunks[i].Index);

                    if (newChunks[i].subChunks == null)
                        newChunks[i].SubDivide();

                    tempChunks.AddRange(newChunks[i].subChunks);
                }
                newChunks = tempChunks;
                tempChunks = new List<Chunk>();

                for (int i = 1; i < depth; i++) {
                    while (newChunks.Any()) {
                        Chunk chunk = newChunks[0];
                        newChunks.RemoveAt(0);

                        if (chunk.mesh == null)
                            cubeSphere.GenerateSubChunk(chunk);

                        if (chunk.subChunks == null)
                            chunk.SubDivide();

                        tempChunks.AddRange(chunk.subChunks);
                    }

                    newChunks = tempChunks;
                    tempChunks = new List<Chunk>();
                }
            }

            foreach (Chunk chunk in newChunks) {
                loadedChunks.Add(chunk);
                generatedChunks.Add(chunk);

                if (depth > 0)
                    cubeSphere.GenerateSubChunk(chunk);

                if (chunk.myObject == null) {
                    createChunks.Add(chunk);
                } else {
                    chunk.myObject.gameObject.SetActive(true);
                    deactivatedChunks.Remove(chunk);
                }
            }
        }

        public List<Chunk> GetChunksToCreate() => createChunks;
        public HashSet<Chunk> GetLoadedChunks() => loadedChunks;
        public List<Chunk> GetGeneratedChunks() => generatedChunks;
        public void SetChunkRangeTest(ChunkRangeTest chunkRangeTest) => this.chunkRangeTest = chunkRangeTest;
    }
}
