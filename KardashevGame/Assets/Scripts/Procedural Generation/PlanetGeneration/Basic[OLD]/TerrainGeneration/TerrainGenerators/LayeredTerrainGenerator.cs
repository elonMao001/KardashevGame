
using PlanetGeneration_OLD.Settings;
using Unity.Mathematics;
using UnityEngine;

using static Unity.Mathematics.math;


namespace PlanetGeneration_OLD.TerrainGenerators {
    public struct LayeredTerrainGenerator : ITerrainGenerator {
        public void Generate(ref Mesh mesh, Vector3[] sphereVertices, AbstractTerrainSettings asettings) {
            LayeredTerrainSettings_OLD settings = (LayeredTerrainSettings_OLD)asettings;

            LayeredNoise layeredNoise = new LayeredNoise(settings.seed);
            layeredNoise.Init(settings.heightFrequency, settings.heightAmplitude, settings.heightLacurancy, settings.heightPersistance, settings.heightLayerNum);
            float[] heightValues = layeredNoise.GetLayeredNoise(sphereVertices);
            float heightden = 2f / (2f * layeredNoise.totalAmplitude);
            
            layeredNoise = new LayeredNoise(12512412);
            layeredNoise.Init(settings.humidityFrequency, 1, settings.humidityLacurancy, settings.humidityPersistence, settings.humidityLayerNum);
            float[] humidityValues = layeredNoise.GetLayeredNoise(sphereVertices);
            float humidityden = 2f / (2 * layeredNoise.totalAmplitude);
            
            Vector3 centre = Vector3.zero;

            Vector3[] terrainVertices = new Vector3[sphereVertices.Length];
            Vector2[] uvs = new Vector2[sphereVertices.Length];

            float maxIntensity = 1;
            for (int i = 1; i < settings.heightLayerNum; i++)
                maxIntensity += pow(settings.heightPersistance, i);
            
            maxIntensity *= settings.heightAmplitude;
            float den = settings.heightLayerNum / (2f * maxIntensity);

            for (int i = 0; i < terrainVertices.Length; i++) {
                float humidityVal = max(0.01f, humidityValues[i] * humidityden + settings.humidityOffset);
                float heightVal = max(0.01f, heightValues[i] * heightden + settings.heightOffset);

                if (settings.display == LayeredTerrainSettings_OLD.Display.Result){
                    terrainVertices[i] = sphereVertices[i] * (1 + heightValues[i]);
                    uvs[i] = new Vector2(humidityVal, heightVal);
                } else 
                if (settings.display == LayeredTerrainSettings_OLD.Display.Height){
                    terrainVertices[i] = sphereVertices[i] * (1 + heightValues[i]);
                    uvs[i] = new Vector2(0.5f, heightVal);
                } else 
                if (settings.display == LayeredTerrainSettings_OLD.Display.HeightMap) {
                    terrainVertices[i] = sphereVertices[i];
                    uvs[i] = new Vector2(0.5f, heightVal);
                } else 
                if (settings.display == LayeredTerrainSettings_OLD.Display.HumidityMap) {
                    terrainVertices[i] = sphereVertices[i];
                    uvs[i] = new Vector2(humidityValues[i] * humidityden + settings.humidityOffset, 0.1f);
                }

                centre += terrainVertices[i];   
            }
            centre /= terrainVertices.Length;

            for (int i = 0; i < terrainVertices.Length; i++) {
                terrainVertices[i] -= centre;
            }

            mesh.SetVertices(terrainVertices);
            mesh.SetUVs(0, uvs);
        }
    }
}
