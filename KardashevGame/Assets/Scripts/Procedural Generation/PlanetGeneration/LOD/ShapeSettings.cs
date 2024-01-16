
using System;
using PlanetGeneration.TerrainGeneration;
using UnityEngine;
using UnityEngine.UIElements;

namespace PlanetGeneration {

    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ShapeSettings", order = 1)]
    public class ShapeSettings : ScriptableObject {
        [Header("Planet"), Min(0)]
        public float radius, seaDepth;

        [HideInInspector]
        public TerrainLayer[] terrainLayers;
        public TerrainLayer baseLayer, layer01, layer02, layer03, layer04;

        public void Init() {
            terrainLayers = new TerrainLayer[5];

            terrainLayers[0] = baseLayer;
            terrainLayers[1] = layer01;
            terrainLayers[2] = layer02;
            terrainLayers[3] = layer03;
            terrainLayers[4] = layer04;

            for (int i = 0; i < terrainLayers.Length; i++) {
                if (terrainLayers[i] == null) {
                    terrainLayers[i] = new TerrainLayer();
                }
            }

        }

        public void InitTerrainLayers() {
            for (int i = 0; i < 5; i++) {
                terrainLayers[i].noiseSettings.Init();
                terrainLayers[i].maskSettings.Init();
            }
        }

        [Serializable]
        public class TerrainLayer {
            public bool enabled, useMask, showMask;
            public NoiseSettings noiseSettings;
            public NoiseSettings maskSettings;

            public TerrainLayer() {
                noiseSettings = new NoiseSettings();
                maskSettings = new NoiseSettings();
            }
        }
    }
}