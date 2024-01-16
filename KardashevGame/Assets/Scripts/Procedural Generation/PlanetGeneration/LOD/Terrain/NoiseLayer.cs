
using System;

namespace PlanetGeneration.TerrainGeneration {
    [Serializable]
    public class NoiseLayer {
        public float amplitude, frequency, dimension;
        public const int maxNoiseLayers = 10;

        public NoiseLayer(float amplitude, float frequency, float dimension) {
            this.amplitude = amplitude;
            this.frequency = frequency;
            this.dimension = dimension;
        }
    }
}
