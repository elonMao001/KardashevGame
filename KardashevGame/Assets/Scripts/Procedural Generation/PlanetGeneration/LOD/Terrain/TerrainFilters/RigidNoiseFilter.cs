
using NoiseTest;
using UnityEngine;
using static UnityEngine.Mathf;

namespace PlanetGeneration.TerrainGeneration {
    public class RigidNoiseFilter : ITerrainFilter {
        private NoiseSettings.RigidNoiseSettings rigidNoiseSettings;
        private OpenSimplexNoise noise = new OpenSimplexNoise(0);

        public RigidNoiseFilter(NoiseSettings.RigidNoiseSettings rigidNoiseSettings) {
            this.rigidNoiseSettings = rigidNoiseSettings;
        }

        public float GetAmplitude(Vector3 position) {
            float amplitude = 0;
            float weight = 1;

            for (int i = 0; i < rigidNoiseSettings.noiseLayerCount; i++) {
                NoiseLayer noiseLayer = rigidNoiseSettings.noiseLayers[i];
                
                Vector3 pos = position * noiseLayer.frequency + rigidNoiseSettings.center;
                float noiseValue = 1 - Abs((float)noise.Evaluate(pos.x, pos.y, pos.z));
                //float noiseValue = 1 - Abs((float)noise.Evaluate(pos.x, pos.y, pos.z, noiseLayer.dimension));
                noiseValue *= noiseValue * weight;
                weight = noiseValue;
                amplitude += noiseValue * noiseLayer.amplitude;
            }

            amplitude = amplitude - rigidNoiseSettings.sealevel;
            return amplitude;
        }

        public Vector2 ApproximateMinMax() {
            return Vector2.zero;
        }
    }
}
