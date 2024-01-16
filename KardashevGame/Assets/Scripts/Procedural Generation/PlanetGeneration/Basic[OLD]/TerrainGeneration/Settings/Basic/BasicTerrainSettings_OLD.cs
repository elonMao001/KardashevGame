using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlanetGeneration_OLD.Settings {

    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/TerrainSettings/Basic_OLD", order = 1)]
    public class BasicTerrainSettings_OLD : AbstractTerrainSettings {
        public long seed;
        public float noiseIncrement; 
        [Range(0, 1)]
        public float noiseIntensity;

        public override bool CheckForChanges(AbstractTerrainSettings asettings) {
            BasicTerrainSettings_OLD settings = (BasicTerrainSettings_OLD)asettings;

            if (settings.noiseIncrement != noiseIncrement ||
                settings.noiseIntensity != noiseIntensity ||
                settings.seed != seed) 
            { return true; }

            return false;
        }
    }
}