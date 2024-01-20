
using UnityEngine;
using PlanetGeneration.TerrainGeneration;

using static UnityEngine.Mathf;
using System.Security.Cryptography;
using System;

namespace PlanetGeneration.Chunks {
    public class ChunkRangeTest {
        private PlanetGenerator planetGenerator;

        private Vector3 closestPointOnSurface;
        private float threshholdToSurfacePoint;

        public int depth;

        private Plane[] frustumPlanes;

        public ChunkRangeTest(PlanetGenerator planetGenerator) {
            this.planetGenerator = planetGenerator;
        }

        public void Init() {
            Vector3 diff = planetGenerator.observer.position - planetGenerator.transform.position;
            closestPointOnSurface = Vector3.Normalize(diff) * planetGenerator.GetRadius();

            Camera camera = planetGenerator.observer.GetComponentInChildren<Camera>();

            float distToSurface = (closestPointOnSurface - planetGenerator.observer.position).magnitude;
            depth = Clamp((int)Min(Log(2 * distToSurface * Tan(camera.fieldOfView) * planetGenerator.maxChunkViewPercentage) * -1.44269504089f), 0, PlanetGenerator.maxDepth);
            
            frustumPlanes = GeometryUtility.CalculateFrustumPlanes(camera);
        }

        public bool IsInRange(Chunk chunk) {
            if (GeometryUtility.TestPlanesAABB(frustumPlanes, chunk.mesh.bounds) && 
                Vector3.Angle(closestPointOnSurface, chunk.mesh.bounds.center) < 120) // should be 135
                return true;
            return false;
        }
    }
}
