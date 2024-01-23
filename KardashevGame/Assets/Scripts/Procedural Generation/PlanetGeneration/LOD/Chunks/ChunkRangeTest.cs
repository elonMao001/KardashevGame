
using UnityEngine;
using PlanetGeneration.TerrainGeneration;

using static UnityEngine.Mathf;
using System.Security.Cryptography;
using System;
using Unity.Mathematics;

namespace PlanetGeneration.Chunks {
    public class ChunkRangeTest {
        private PlanetGenerator planetGenerator;

        private Vector3 closestPointOnSurface;
        private float sqrObservationRadius, planetRadius;

        public int depth;

        private Plane[] frustumPlanes;

        public ChunkRangeTest(PlanetGenerator planetGenerator) {
            this.planetGenerator = planetGenerator;
        }
        public void Init() {
            planetRadius = planetGenerator.GetRadius();

            Vector3 diff = planetGenerator.observer.position - planetGenerator.transform.position;
            closestPointOnSurface = Vector3.Normalize(diff) * planetRadius;

            Camera camera = planetGenerator.observer.GetComponentInChildren<Camera>();

            float distToSurface = (closestPointOnSurface - planetGenerator.observer.position).magnitude;
            float largestChunkSize = 2 * distToSurface * Tan(camera.fieldOfView * Deg2Rad *0.5f) * planetGenerator.maxChunkViewPercentage;

            float chunkSize = planetRadius;
            depth = 0;
            while (chunkSize > largestChunkSize) {
                chunkSize *= 0.5f;
                depth++;
            }
            
            sqrObservationRadius = GetSqrObservationDistance(planetRadius, distToSurface, camera.fieldOfView * Deg2Rad);

            frustumPlanes = GeometryUtility.CalculateFrustumPlanes(camera);
        }

        private float GetSqrObservationDistance(float r, float dist, float angle) {
            float dx = -(r + dist);
            float dy = Tan(angle) * -dx;
            float dr = Sqrt(dx*dx + dy*dy);

            float D = -dx * dy;

            float discr = r*r * dr*dr - D*D;

            if (discr < 0)
                return dx*dx;
            else {
                float add = Sgn(dy) * dx *Sqrt(r*r * dr*dr - D*D);
                float x1 = (D * dy + add) / (dr*dr);
                float x2 = (D * dy - add) / (dr*dr);

                add = Abs(dy) * Sqrt(r*r * dr*dr - D*D);
                float y1 = (-D * dx + add) / (dr*dr);
                float y2 = (-D * dx - add) / (dr*dr);

                if (discr == 0) {
                    dx = x1 - (r + dist);
                    dy = y1;
                    return dx*dx + dy*dy;
                } else {
                    dx = x1 - (r + dist);
                    dy = y1;
                    float dist1 = dx*dx + dy*dy;
                    
                    dx = x2 - (r + dist);
                    dy = y2;
                    return Min(dist1, dx*dx + dy*dy);
                }
            }
        }

        private float Sgn(float x) {
            if (x < 0) return -1;
            else return 1;
        }

        public bool IsInRange(Chunk chunk) {
            if (GeometryUtility.TestPlanesAABB(frustumPlanes, chunk.mesh.bounds) && 
                chunk.mesh.bounds.SqrDistance(planetGenerator.observer.position) < sqrObservationRadius)
                return true;
            return false;
        }
    }
}
