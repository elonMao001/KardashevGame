using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlanetGeneration.TerrainGeneration {
    public static class TerrainFilterFactory {
        public static ITerrainFilter CreateTerrainFilter(NoiseSettings settings) {
            switch (settings.filterType) {
                case NoiseSettings.FilterType.LayeredNoise:
                    return new LayeredNoiseFilter(settings.layeredNoiseSettings);
                case NoiseSettings.FilterType.RigidNoise:
                    return new RigidNoiseFilter(settings.rigidNoiseSettings);
                case NoiseSettings.FilterType.DistortedNoise:
                    return new DistortedNoiseFilter(settings.distortedNoiseSettings);
                case NoiseSettings.FilterType.StepNoise:
                    return new StepNoiseFilter(settings.stepNoiseSettings);
            }
            
            return null;
        }
    }
}
