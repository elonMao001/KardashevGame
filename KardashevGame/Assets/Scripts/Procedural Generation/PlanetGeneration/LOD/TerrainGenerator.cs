
using PlanetGeneration;
using PlanetGeneration.TerrainGeneration;
using UnityEngine;

using static UnityEngine.Mathf;
using TerrainLayer = PlanetGeneration.ShapeSettings.TerrainLayer;

namespace PlanetGeneration.TerrainGeneration {
    public class TerrainGenerator {
        private TerrainBundle[] terrainBundles;
        private ShapeSettings shapeSettings;
        public MinMax minmax;

        public TerrainGenerator(ShapeSettings shapeSettings) {
            this.shapeSettings = shapeSettings;
            terrainBundles = new TerrainBundle[shapeSettings.terrainLayers.Length];
            InitTerrainBundles();

            minmax = new MinMax();
        }

        public void InitTerrainBundles() {
            for (int i = 0; i < terrainBundles.Length; i++) {
                terrainBundles[i] = new TerrainBundle(shapeSettings.terrainLayers[i]);;
            }
        }

        public Vector3 GetPosition(Vector3 pointOnUnitSphere, bool addToMinMax) {
            float amplitude = 0;

            float baseMask = terrainBundles[0].terrainFilter.GetAmplitude(pointOnUnitSphere);
            
            for (int i = 1; i < terrainBundles.Length; i++)
                if (shapeSettings.terrainLayers[i].enabled) {
                    if (shapeSettings.terrainLayers[i].maskMode == TerrainLayer.MaskMode.ShowCustom) {
                        amplitude += terrainBundles[i].mask.GetAmplitude(pointOnUnitSphere);
                    } else {
                        float currentAmplitude = terrainBundles[i].terrainFilter.GetAmplitude(pointOnUnitSphere);

                        if (shapeSettings.terrainLayers[i].affectsOcean || currentAmplitude >= 0) {

                            switch (shapeSettings.terrainLayers[i].maskMode) {
                                case TerrainLayer.MaskMode.None:
                                    amplitude += currentAmplitude;
                                    break;
                                case TerrainLayer.MaskMode.UseBase:
                                    amplitude += currentAmplitude * Max(0, baseMask);
                                    break;
                                case TerrainLayer.MaskMode.UseCustom:
                                    amplitude += currentAmplitude * Max(0, terrainBundles[i].mask.GetAmplitude(pointOnUnitSphere));
                                    break;
                                default: break;
                            }
                        }   
                    }
                }

            if (shapeSettings.terrainLayers[0].enabled) {
                if (!shapeSettings.terrainLayers[0].affectsOcean)
                    baseMask = Max(0, baseMask);
                amplitude += baseMask;
            }

            if (amplitude < 0)
                amplitude *= shapeSettings.seaDepth;

            amplitude = (1 + amplitude) * shapeSettings.radius;
            if (addToMinMax) 
                minmax.AddValue(amplitude);

            return pointOnUnitSphere * amplitude;
        }

        public class TerrainBundle {
            public ITerrainFilter terrainFilter, mask;

            public TerrainBundle(TerrainLayer terrainLayer) {
                terrainFilter = TerrainFilterFactory.CreateTerrainFilter(terrainLayer.noiseSettings);
                mask = TerrainFilterFactory.CreateTerrainFilter(terrainLayer.maskSettings);
            }
        }
    }
}