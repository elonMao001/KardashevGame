
using UnityEngine;

namespace PlanetGeneration_OLD {
    public abstract class AbstractTerrainSettings : ScriptableObject { 
        public abstract bool CheckForChanges(AbstractTerrainSettings asettings);
    }
}

