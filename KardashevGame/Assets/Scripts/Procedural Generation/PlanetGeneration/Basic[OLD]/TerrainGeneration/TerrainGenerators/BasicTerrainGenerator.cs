
using NoiseTest;
using PlanetGeneration_OLD.Settings;
using UnityEngine;

namespace PlanetGeneration_OLD.TerrainGenerators {
    public struct BasicTerrainGenerator : ITerrainGenerator {
        public void Generate(ref Mesh mesh, Vector3[] sphereVertices, AbstractTerrainSettings asettings) {
            BasicTerrainSettings_OLD settings = (BasicTerrainSettings_OLD)asettings;

            OpenSimplexNoise noise = new OpenSimplexNoise(settings.seed);
            Vector3[] terrainVertices = new Vector3[sphereVertices.Length];

            for (int i = 0; i < sphereVertices.Length; i++) {
                Vector3 noisePosition = sphereVertices[i] * settings.noiseIncrement;
                float noiseValue = (float)noise.Evaluate(noisePosition.x, noisePosition.y, noisePosition.z);

                terrainVertices[i] = (1 + noiseValue * settings.noiseIntensity) * sphereVertices[i];
            }

            mesh.SetVertices(terrainVertices);
        }
    }
}
