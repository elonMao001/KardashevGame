
using PlanetGeneration;
using PlanetGeneration.TerrainGeneration;
using UnityEngine;

using static UnityEngine.Mathf;
using TerrainLayer = PlanetGeneration.ShapeSettings.TerrainLayer;

namespace PlanetGeneration.TerrainGeneration {
    public class ColorGenerator {
        private ColorSettings colorSettings;
        private Texture2D surfaceGradientTexture = new Texture2D(gradientResolution, 1);
        private Texture2D oceanfloorGradientTexture = new Texture2D(gradientResolution, 1);

        public const int gradientResolution = 50;
        private const float stepSize = (float)1f / (gradientResolution - 1);

        public ColorGenerator(ColorSettings colorSettings) {
            this.colorSettings = colorSettings;
        }
        
        public void UpdateElevationMinMax(MinMax minmax) {
            colorSettings.planetMaterial.SetVector("_ElevationMinMax", new Vector4(minmax.min, minmax.max));
        }

        public void UpdatePlanetRadius(float radius) {
            colorSettings.planetMaterial.SetFloat("_PlanetRadius", radius);
        }

        public void UpdateSurfaceGradient() {
            Color[] colors = new Color[gradientResolution];
            for (int i = 0; i < gradientResolution; i++) {
                colors[i] = colorSettings.surfaceGradient.Evaluate(i * stepSize);
            }
            surfaceGradientTexture.SetPixels(colors);
            surfaceGradientTexture.Apply();

            colorSettings.planetMaterial.SetTexture("_SurfaceGradient", surfaceGradientTexture);
        }

        public void UpdateOceanfloorGradient() {
            Color[] colors = new Color[gradientResolution];
            for (int i = 0; i < gradientResolution; i++) {
                colors[i] = colorSettings.oceanfloorGradient.Evaluate(i * stepSize);
            }
            oceanfloorGradientTexture.SetPixels(colors);
            oceanfloorGradientTexture.Apply();

            colorSettings.planetMaterial.SetTexture("_OceanfloorGradient", oceanfloorGradientTexture);
        }
    }
}