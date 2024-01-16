using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using NoiseTest;
using UnityEngine;

namespace PlanetGeneration_OLD.TerrainGenerators {
    public class LayeredNoise {
        OpenSimplexNoise noise;
        NoiseLayer[] noiseLayers;

        public float totalAmplitude;

        public LayeredNoise() => noise = new OpenSimplexNoise();
        public LayeredNoise(long seed) => noise = new OpenSimplexNoise(seed);

        public void Init(float frequency, float amplitude, float lacurancy, float persistence, int layerNum) {
            noiseLayers = new NoiseLayer[layerNum];
            totalAmplitude = 0;

            for (int i = 0; i < layerNum; i++) {
                noiseLayers[i] = new NoiseLayer() {
                    frequency = frequency,
                    amplitude = amplitude
                };

                totalAmplitude += amplitude;

                frequency *= lacurancy;
                amplitude *= persistence;
            }
        }

        public float GetLayeredNoise(Vector3 pos) {
            float noiseVal = 0;
            foreach (NoiseLayer layer in noiseLayers)
                noiseVal += (float)noise.Evaluate(pos.x * layer.frequency, pos.y * layer.frequency, pos.z * layer.frequency) * layer.amplitude;

            return noiseVal;
        }

        public float[] GetLayeredNoise(Vector3[] poss) {
            float[] noiseVals = new float[poss.Length];

            for (int i = 0; i < poss.Length; i++)
                noiseVals[i] = GetLayeredNoise(poss[i]);

            return noiseVals;
        }

        public struct NoiseLayer {
            public float frequency, amplitude;
        }
    }
}
