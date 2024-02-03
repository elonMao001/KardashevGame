
using NoiseTest;
using UnityEngine;
using static UnityEngine.Mathf;

namespace PlanetGeneration.TerrainGeneration {
    public class DistortedNoiseFilter : ITerrainFilter {
        private NoiseSettings.DistortedNoiseSettings distortedNoiseSettings;
        private OpenSimplexNoise noise = new OpenSimplexNoise(0);
        
        public DistortedNoiseFilter(NoiseSettings.DistortedNoiseSettings distortedNoiseSettings) {
            this.distortedNoiseSettings = distortedNoiseSettings;
        }

        public float GetAmplitude(Vector3 position) {
            float amplitude = 0;

            for (int i = 0; i < distortedNoiseSettings.noiseLayerCount; i++) {
                NoiseLayer noiseLayer = distortedNoiseSettings.noiseLayers[i];
                NoiseLayer distortionNoiseLayer = distortedNoiseSettings.distortionNoiseLayers[i];

                Vector3 pos = position * distortionNoiseLayer.frequency + distortedNoiseSettings.distortionCenter;
                float distortionValue = (float)noise.Evaluate(pos.x, pos.y, pos.z) * distortionNoiseLayer.amplitude;
                
                pos = position * noiseLayer.frequency + distortedNoiseSettings.center + Vector3.one * distortionValue;
                amplitude += (float)noise.Evaluate(pos.x, pos.y, pos.z) * noiseLayer.amplitude;
                //amplitude += (float)noise.Evaluate(pos.x, pos.y, pos.z, noiseLayer.dimension) * noiseLayer.amplitude;
            }
            
            return amplitude - distortedNoiseSettings.sealevel;
        }

        public Vector2 ApproximateMinMax() {
            return Vector2.zero;
        }
    }
}
