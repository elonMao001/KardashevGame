
using System;
using PlanetGeneration.TerrainGeneration;
using UnityEngine;
using UnityEditorInternal;
using System.IO;
using System.Collections.Generic;

namespace PlanetGeneration {

    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ShapeSettings", order = 1)]
    public class ShapeSettings : ScriptableObject {
        [Header("Planet"), Min(0)]
        public float radius, seaDepth;

        [HideInInspector]
        public TerrainLayer[] terrainLayers;
        public TerrainLayer baseLayer, layer01, layer02, layer03;

        public void Init() {
            terrainLayers = new TerrainLayer[] {
                baseLayer, layer01, layer02, layer03
            };

            for (int i = 0; i < terrainLayers.Length; i++) {
                if (terrainLayers[i] == null) {
                    terrainLayers[i] = new TerrainLayer();
                }
            }

        }

        public void InitTerrainLayers() {
            for (int i = 0; i < terrainLayers.Length; i++) {
                terrainLayers[i].noiseSettings.Init();
                terrainLayers[i].maskSettings.Init();
            }
        }

        [Serializable]
        public class TerrainLayer {
            public enum MaskMode {
                None, UseBase, UseCustom, ShowCustom
            }
            public bool enabled, affectsOcean;
            public MaskMode maskMode;
            public NoiseSettings noiseSettings;
            public NoiseSettings maskSettings;

            public TerrainLayer() {
                noiseSettings = new NoiseSettings();
                maskSettings = new NoiseSettings();
            }
        }
    }
}