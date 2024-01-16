
using System.Collections.Generic;
using PlanetGeneration.TerrainGeneration;
using UnityEngine;

using static UnityEngine.Mathf;

namespace PlanetGeneration.Chunks {
    public class Chunk {
        public Chunk[] subChunks;
        public Mesh mesh, parentMesh;
        public Transform myObject;

        public Vector3 Norm;
        public Vector3 Start;
        public Vector3 UInc;
        public Vector3 VInc;
        public int Depth;
        public int Index;
        public int ParentMeshStart;

        public static int Resolution = 100; // Max 255, must be even
        public static int ParentIndexIncrement = CeilToInt(Resolution / 2f) + 1;
        public static float HalfResolution = Resolution * 0.5f;
        public static int VertexResolution = Resolution + 1;
        public static int VertexCount = VertexResolution * VertexResolution;
        public static int IndexCount = Resolution * Resolution * 6;
        public static float[] diagonals ={
            0.70710678118f, 0.35355339059f, 0.17677669529f, 0.08838834764f, 
            0.04419417382f, 0.02209708691f, 0.01104854345f, 0.00552427172f,
            0.00276213586f, 0.00138106793f, 0.00069053396f, 0.00034526698f
        };

        public static int[] ParentMeshStartIndices = new int[4] {
            0, 
            ParentIndexIncrement - 1, 
            VertexResolution * (ParentIndexIncrement - 1), 
            (VertexResolution + 1) * (ParentIndexIncrement - 1)
        };

        public bool ApplyMesh() {
            if (myObject != null) {
                myObject.GetComponent<MeshFilter>().sharedMesh = mesh;
                return true;
            }

            return false;
        }

        public void GenerateFace(TerrainGenerator terrainGenerator, int[] triangles) {
            Vector3[] vertices = new Vector3[VertexCount];
            
            Vector3 position = Start;
            for (int u = 0; u < VertexResolution; u++) {
                FillRow(terrainGenerator, vertices, position, VInc, u * VertexResolution, true);
                position += UInc;
            }

            if (mesh == null)
                mesh = new Mesh() {
                    vertices = vertices,
                    triangles = triangles
                };
            else 
                mesh.SetVertices(vertices);

            mesh.RecalculateNormals();
            mesh.RecalculateTangents();
        }

        public void Generate(TerrainGenerator terrainGenerator, int[] triangles) {
            Vector3[] vertices = new Vector3[VertexCount];
            Vector3[] parentVertices = parentMesh.vertices;
            
            Vector3 position = Start;

            int index = 0, parentIndex = ParentMeshStart; 
            for (int u = 0; u < Resolution; u += 2) {
                FillDottedRow(terrainGenerator, vertices, parentVertices, position, VInc, index, parentIndex);
                position += UInc;
                index += VertexResolution;
                parentIndex += VertexResolution;

                FillRow(terrainGenerator, vertices, position, VInc, index, false);
                position += UInc;
                index += VertexResolution;
            }
            FillDottedRow(terrainGenerator, vertices, parentVertices, position, VInc, index, parentIndex);
            
            if (mesh == null)
                mesh = new Mesh() {
                    vertices = vertices,
                    triangles = triangles
                };
            else 
                mesh.SetVertices(vertices);

            mesh.RecalculateNormals();
            mesh.RecalculateTangents();
        }

        private void FillRow(TerrainGenerator terrainGenerator, Vector3[] vertices, Vector3 position, Vector3 vinc, int index, bool face) {
            for (int v = 0; v < VertexResolution; v++) {
                vertices[index] = terrainGenerator.GetPosition(Vector3.Normalize(position), face);
                
                position += vinc;
                index++;
            }
        }

        private void FillDottedRow(TerrainGenerator terrainGenerator, Vector3[] vertices, Vector3[] parentVertices, Vector3 position, Vector3 vinc, int index, int parentIndex) {
            position += vinc;
            for (int v = 0; v < Resolution; v += 2) {
                vertices[index] = parentVertices[parentIndex];
                index++;
                parentIndex++;

                vertices[index] = terrainGenerator.GetPosition(Vector3.Normalize(position), false);

                position += 2 * vinc;
                index++;
            }
            
            vertices[index] = parentVertices[parentIndex];
        }

        public void SubDivide() {
            Vector3 newUInc = UInc * 0.5f, newVInc = VInc * 0.5f;
            Vector3 HalfU = newUInc * HalfResolution, HalfV = newVInc * HalfResolution;
            int newDepth = Depth + 1;
            int newIndex = Index << 2;

            subChunks = new Chunk[4];

            Vector3[] newStarts = new Vector3[4] {
                Start,
                Start + VInc * HalfResolution,
                Start + UInc * HalfResolution,
                Start + UInc * HalfResolution + VInc * HalfResolution
            };

            for (int i = 0; i < 4; i++) {
                subChunks[i]= new Chunk {
                    parentMesh = mesh,
                    ParentMeshStart = ParentMeshStartIndices[i],
                    Norm = Vector3.Normalize(newStarts[i] + HalfU + HalfV),
                    Start = newStarts[i],
                    UInc = newUInc,
                    VInc = newVInc,
                    Depth = newDepth,
                    Index = newIndex + i
                };
            }
        }

        public void PassDownMesh() {
            if (subChunks == null) return;

            foreach (Chunk chunk in subChunks) 
                chunk.parentMesh = mesh;
        }
    }
}
