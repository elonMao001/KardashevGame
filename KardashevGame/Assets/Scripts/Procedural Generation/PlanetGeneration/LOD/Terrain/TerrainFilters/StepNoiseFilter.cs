
using NoiseTest;
using UnityEngine;
using static UnityEngine.Mathf;

namespace PlanetGeneration.TerrainGeneration {
    public class StepNoiseFilter : ITerrainFilter {
        private NoiseSettings.StepNoiseSettings stepNoiseSettings;
        private OpenSimplexNoise noise = new OpenSimplexNoise(0);
        
        public StepNoiseFilter(NoiseSettings.StepNoiseSettings stepNoiseSettings) {
            this.stepNoiseSettings = stepNoiseSettings;
        }

        public float GetAmplitude(Vector3 position) {
            float amplitude = 0;

            for (int i = 0; i < stepNoiseSettings.noiseLayerCount; i++) {
                NoiseLayer noiseLayer = stepNoiseSettings.noiseLayers[i];
                
                Vector3 pos = position * noiseLayer.frequency + stepNoiseSettings.center;
                amplitude += (float)noise.Evaluate(pos.x, pos.y, pos.z) * noiseLayer.amplitude;
                //amplitude += (float)noise.Evaluate(pos.x, pos.y, pos.z, noiseLayer.dimension) * noiseLayer.amplitude;
            }

            amplitude = (int)((float)amplitude / stepNoiseSettings.stepCount) * stepNoiseSettings.stepSize;
            
            return amplitude - stepNoiseSettings.sealevel;
        }
    }
}
