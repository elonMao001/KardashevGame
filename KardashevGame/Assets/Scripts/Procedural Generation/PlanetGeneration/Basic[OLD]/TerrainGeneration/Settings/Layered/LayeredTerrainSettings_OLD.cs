using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlanetGeneration_OLD.Settings {

    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/TerrainSettings/Layered_OLD", order = 1)]
    public class LayeredTerrainSettings_OLD : AbstractTerrainSettings {
        public long seed;
        
        [Range(1, 10)]
        public int heightLayerNum;
        
        public float heightFrequency;
        [Range(0, 1)]
        public float heightAmplitude;
        public float heightLacurancy;
        [Range(0, 1)]
        public float heightPersistance;

        [Range(1, 10)]
        public int humidityLayerNum;
        public float humidityFrequency;
        public float humidityLacurancy;
        [Range(0, 1)]
        public float humidityPersistence;

        public float heightOffset = 0.5f;
        public float humidityOffset = 0.5f;

        [HideInInspector]
        public enum Display {
            Result, Height, HeightMap, HumidityMap
        }
        public Display display;

        public override bool CheckForChanges(AbstractTerrainSettings asettings) {
            LayeredTerrainSettings_OLD settings = (LayeredTerrainSettings_OLD)asettings;

            if (settings.seed != seed ||
                settings.heightLayerNum != heightLayerNum ||
                settings.heightFrequency != heightFrequency ||
                settings.heightAmplitude != heightAmplitude ||
                settings.heightLacurancy != heightLacurancy ||
                settings.heightPersistance != heightPersistance ||
                settings.humidityLayerNum != humidityLayerNum ||
                settings.humidityFrequency != humidityFrequency ||
                settings.humidityLacurancy != humidityLacurancy ||
                settings.humidityPersistence != humidityPersistence ||
                settings.heightOffset != heightOffset ||
                settings.humidityOffset != humidityOffset ||
                settings.display != display) 
            { return true; }

            return false;
        }
    }
}