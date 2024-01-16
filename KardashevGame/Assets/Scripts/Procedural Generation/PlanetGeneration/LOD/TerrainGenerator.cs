
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
            
            for (int i = 0; i < terrainBundles.Length; i++) {
                if (shapeSettings.terrainLayers[i].enabled) {
                    if (shapeSettings.terrainLayers[i].showMask) {
                        amplitude += terrainBundles[i].mask.GetAmplitude(pointOnUnitSphere);
                    } else {
                        float currentAmplitude = terrainBundles[i].terrainFilter.GetAmplitude(pointOnUnitSphere);

                        if (i == 0 || currentAmplitude > 0) {
                            if (shapeSettings.terrainLayers[i].useMask)
                                currentAmplitude *= terrainBundles[i].mask.GetAmplitude(pointOnUnitSphere);

                            amplitude += currentAmplitude;
                        }
                    }   
                }
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