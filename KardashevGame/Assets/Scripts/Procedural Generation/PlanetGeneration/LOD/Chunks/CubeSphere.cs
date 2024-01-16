
using System;
using PlanetGeneration;
using PlanetGeneration.Chunks;
using PlanetGeneration.TerrainGeneration;
using UnityEngine;

using static UnityEngine.Mathf;

namespace PlanetGeneration.Chunks {
    public class CubeSphere {
        public Chunk[] faces = new Chunk[6];
        public Vector3Int[] localUps ={ 
            Vector3Int.right, Vector3Int.up, Vector3Int.forward,
            Vector3Int.back, Vector3Int.down, Vector3Int.left
        };
        
        private TerrainGenerator terrainGenerator;
        private int[] universalTriangles = new int[Chunk.IndexCount];
        
        public CubeSphere(TerrainGenerator terrainGenerator) {
            if (Chunk.Resolution % 2 != 0)
                throw new Exception("Resolution not even");

            this.terrainGenerator = terrainGenerator;

            CreateUniversalTriangles();

            for (int i = 0; i < 6; i++) {
                Vector3 localUp = localUps[i];

                Vector3 localRight = new Vector3(localUp.y, localUp.z, localUp.x);
                Vector3 localDown = Vector3.Cross(localRight, localUp);
                
                faces[i] = new Chunk() {
                    parentMesh = null,
                    ParentMeshStart = -1,
                    Norm = localUp,
                    Start = (localUp - localDown - localRight) * 0.5f,
                    UInc = localRight * ((float)1f / Chunk.Resolution),
                    VInc = localDown * ((float)1f / Chunk.Resolution),
                    Depth = 0,
                    Index = i
                };

                faces[i].GenerateFace(terrainGenerator, universalTriangles);
            }
        } 

        public void GenerateSubChunk(Chunk chunk) => chunk.Generate(terrainGenerator, universalTriangles);

        private void CreateUniversalTriangles() {
            int tindex = 0, vindex = 0, nextRow = Chunk.VertexResolution;
            
            for (int u = 0; u < Chunk.VertexResolution; u++) {
                for (int v = 0; v < Chunk.VertexResolution; v++) {
                    if (u < Chunk.Resolution && v < Chunk.Resolution) {
                       universalTriangles[tindex++] = vindex;
                       universalTriangles[tindex++] = nextRow + 1;
                       universalTriangles[tindex++] = nextRow;
                       universalTriangles[tindex++] = vindex;
                       universalTriangles[tindex++] = vindex + 1;
                       universalTriangles[tindex++] = nextRow + 1;
                    }

                    vindex++;
                    nextRow++;
                }
            }
        }

        public void GenerateFaces() {
            foreach (Chunk chunk in faces)
                chunk.GenerateFace(terrainGenerator, universalTriangles);
        }

        public void GenerateFace(int i) {
            faces[i].GenerateFace(terrainGenerator, universalTriangles);
        }

        public Chunk GetFace(int index) {
            return faces[index];
        }
    }
}
