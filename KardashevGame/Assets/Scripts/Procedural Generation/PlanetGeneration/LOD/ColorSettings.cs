
using System;
using PlanetGeneration.TerrainGeneration;
using UnityEngine;

namespace PlanetGeneration {

    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ColorSettings", order = 1)]
    public class ColorSettings : ScriptableObject {
        [Header("Color")]
        public Gradient surfaceGradient; 
        public Gradient oceanfloorGradient;
        public Material planetMaterial;
    }
}
