
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

namespace PlanetGeneration.TerrainGeneration {
    [Serializable]
    public class NoiseSettings {
        public enum FilterType {
            LayeredNoise, RigidNoise, DistortedNoise, StepNoise
        }
        public FilterType filterType;

        public LayeredNoiseSettings layeredNoiseSettings;
        public RigidNoiseSettings rigidNoiseSettings;
        public DistortedNoiseSettings distortedNoiseSettings;
        public StepNoiseSettings stepNoiseSettings;

        public void Init() {
            switch (filterType) {
                case FilterType.LayeredNoise:
                    layeredNoiseSettings.Init();
                    break;
                case FilterType.RigidNoise:
                    rigidNoiseSettings.Init();
                    break;
                case FilterType.DistortedNoise:
                    distortedNoiseSettings.Init();
                    break;
                case FilterType.StepNoise:
                    stepNoiseSettings.Init();
                    break;
            }
        }

        [Serializable]
        public class LayeredNoiseSettings {
            public Vector3 center;

            [Min(0)]
            public float amplitude = 5f;
            [Min(0)]
            public float frequency = 0.2f;
            [Range(0.0001f, 1f)]
            public float persistance = 0.5f;
            [Range(1f, 8f)]
            public float lacunarity = 2f;
            [Range(1, NoiseLayer.maxNoiseLayers)]
            public int noiseLayerCount = 1;
            public float sealevel;

            public NoiseLayer[] noiseLayers;

            public void Init() {
                float a = amplitude, f = frequency;
                noiseLayers = new NoiseLayer[NoiseLayer.maxNoiseLayers];

                for (int i = 0; i < NoiseLayer.maxNoiseLayers; i++) {
                    noiseLayers[i] = new NoiseLayer(a, f, (i + 0.5f) * 1000);
                    
                    a *= persistance;
                    f *= lacunarity;
                }
            }

        }
        [Serializable]
        public class RigidNoiseSettings {
            public Vector3 center;

            [Min(0)]
            public float amplitude = 5f;
            [Min(0)]
            public float frequency = 0.2f;
            [Range(0.0001f, 1f)]
            public float persistance = 0.5f;
            [Range(1f, 8f)]
            public float lacunarity = 2f;
            [Range(1, NoiseLayer.maxNoiseLayers)]
            public int noiseLayerCount = 1;
            public float sealevel;

            public NoiseLayer[] noiseLayers;

            public void Init() {
                float a = amplitude, f = frequency;
                noiseLayers = new NoiseLayer[NoiseLayer.maxNoiseLayers];

                for (int i = 0; i < NoiseLayer.maxNoiseLayers; i++) {
                    noiseLayers[i] = new NoiseLayer(a, f, (i + 0.5f) * 1000);
                    
                    a *= persistance;
                    f *= lacunarity;
                }
            }

        }
        [Serializable]
        public class DistortedNoiseSettings {
            public Vector3 center;
            [Min(0)]
            public float amplitude = 5f;
            [Min(0)]
            public float frequency = 0.2f;
            [Range(0.0001f, 1f)]
            public float persistance = 0.5f;
            [Range(1f, 8f)]
            public float lacunarity = 2f;
            [Range(1, NoiseLayer.maxNoiseLayers)]
            public int noiseLayerCount = 1;

            public Vector3 distortionCenter;
            [Min(0)]
            public float distortionAmplitude = 5f;
            [Min(0)]
            public float distortionFrequency = 0.2f;
            [Range(0.0001f, 2f)]
            public float distortionPersistance = 0.5f;
            [Range(0f, 6f)]
            public float distortionLacunarity = 2f;
            public float sealevel;

            public NoiseLayer[] noiseLayers, distortionNoiseLayers;

            public void Init() {
                float a = amplitude, f = frequency;
                noiseLayers = new NoiseLayer[NoiseLayer.maxNoiseLayers];

                for (int i = 0; i < NoiseLayer.maxNoiseLayers; i++) {
                    noiseLayers[i] = new NoiseLayer(a, f, (i + 0.5f) * 1000);
                    
                    a *= persistance;
                    f *= lacunarity;
                }

                a = distortionAmplitude; 
                f = distortionFrequency;
                distortionNoiseLayers = new NoiseLayer[NoiseLayer.maxNoiseLayers];

                for (int i = 0; i < NoiseLayer.maxNoiseLayers; i++) {
                    distortionNoiseLayers[i] = new NoiseLayer(a, f, (i + 50.5f) * 1000);
                    
                    a *= distortionPersistance;
                    f *= distortionLacunarity;
                }
            }

        }
        [Serializable]
        public class StepNoiseSettings {
            public Vector3 center;
            [Min(0)]
            public float amplitude = 5f;
            [Min(0)]
            public float frequency = 0.2f;
            [Range(0.0001f, 1f)]
            public float persistance = 0.5f;
            [Range(1f, 8f)]
            public float lacunarity = 2f;
            [Range(1, NoiseLayer.maxNoiseLayers)]
            public int noiseLayerCount = 1;
            [Min(1)]
            public int stepCount = 1;
            [HideInInspector]
            public float stepSize;

            public float sealevel;

            public NoiseLayer[] noiseLayers;

            public void Init() {
                float a = amplitude, f = frequency;
                noiseLayers = new NoiseLayer[NoiseLayer.maxNoiseLayers];

                for (int i = 0; i < NoiseLayer.maxNoiseLayers; i++) {
                    noiseLayers[i] = new NoiseLayer(a, f, (i + 0.5f) * 1000);
                    
                    a *= persistance;
                    f *= lacunarity;
                }

                stepSize = (float)1f / stepCount;
            }
        }
    }
}

