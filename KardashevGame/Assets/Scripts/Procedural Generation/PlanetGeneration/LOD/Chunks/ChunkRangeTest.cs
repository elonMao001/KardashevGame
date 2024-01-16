
using UnityEngine;
using PlanetGeneration.TerrainGeneration;

using static UnityEngine.Mathf;
using System.Security.Cryptography;

namespace PlanetGeneration.Chunks {
    public abstract class ChunkRangeTest {
        public abstract void Init(InitData initData);
        public abstract bool IsInRange(Chunk chunk);
    }

    public class Distance : ChunkRangeTest {
        private float threshold;
        private Vector3 observerPosition;

        public override void Init(InitData initData) {
            observerPosition = initData.observer.position;
            threshold = initData.threshhold;
        }

        public override bool IsInRange(Chunk chunk) {
            if ((chunk.mesh.bounds.center - observerPosition).magnitude < threshold) 
                return true;
            return false;
        }
    }

    public class ClosestDistance : ChunkRangeTest {
        private float threshhold;
        private Vector3 observerPosition;

        public override void Init(InitData initData) {
            observerPosition = initData.observer.position;
            threshhold = initData.threshhold;
        }

        public override bool IsInRange(Chunk chunk) {
            if ((chunk.mesh.bounds.ClosestPoint(observerPosition) - observerPosition).magnitude < threshhold) 
                return true;
            return false;
        }
    }

    public class UsingHelpSphere : ChunkRangeTest {
        private Vector3 closestPointOnSurface;
        private float threshholdToSurfacePoint;

        public override void Init(InitData initData) {
            Vector3 diff = initData.observer.position - initData.planet.position;
            closestPointOnSurface = Vector3.Normalize(diff) * initData.settings.radius;

            Camera camera = initData.observer.GetComponentInChildren<Camera>();

            threshholdToSurfacePoint = Tan(camera.fieldOfView * 0.5f * Deg2Rad) * (diff.magnitude - initData.settings.radius * initData.planet.localScale.x);
            threshholdToSurfacePoint *= threshholdToSurfacePoint;
        }

        public override bool IsInRange(Chunk chunk) {
            float dist = chunk.mesh.bounds.SqrDistance(closestPointOnSurface);
            if (dist < threshholdToSurfacePoint) return true;
            return false;
        }
    }

    public struct InitData {
        public Transform observer, planet;
        public float threshhold;
        public ShapeSettings settings;

        public InitData(Transform observer, Transform planet, float threshhold, ShapeSettings settings) {
            this.observer = observer;
            this.planet = planet;
            this.threshhold = threshhold;
            this.settings = settings;
        }
    }
}
