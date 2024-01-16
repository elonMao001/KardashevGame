
using System.Collections.Generic;
using PlanetGeneration.Chunks;
using UnityEngine;

namespace PlanetGeneration.TerrainGeneration {
    public interface ITerrainFilter {
        public float GetAmplitude(Vector3 position);
    }
}
