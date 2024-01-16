
using NoiseTest;
using UnityEngine;
using static UnityEngine.Mathf;

namespace PlanetGeneration.TerrainGeneration {
    public class LayeredNoiseFilter : ITerrainFilter {
        private NoiseSettings.LayeredNoiseSettings layeredNoiseSettings;
        private OpenSimplexNoise noise = new OpenSimplexNoise(0);

        public LayeredNoiseFilter(NoiseSettings.LayeredNoiseSettings layeredNoiseSettings) {
            this.layeredNoiseSettings = layeredNoiseSettings;
        }

        public float GetAmplitude(Vector3 position) {
            float amplitude = 0;

            for (int i = 0; i < layeredNoiseSettings.noiseLayerCount; i++) {
                NoiseLayer noiseLayer = layeredNoiseSettings.noiseLayers[i];
                
                Vector3 pos = position * noiseLayer.frequency + layeredNoiseSettings.center;
                amplitude += (float)noise.Evaluate(pos.x, pos.y, pos.z) * noiseLayer.amplitude;
                //amplitude += (float)noise.Evaluate(pos.x, pos.y, pos.z, noiseLayer.dimension) * noiseLayer.amplitude;
            }

            return amplitude - layeredNoiseSettings.sealevel;
        }
    }
}
