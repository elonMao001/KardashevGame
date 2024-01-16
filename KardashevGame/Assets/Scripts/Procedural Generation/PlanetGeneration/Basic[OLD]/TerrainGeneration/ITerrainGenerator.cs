
using NoiseTest;
using Unity.Collections;
using UnityEngine;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine.Rendering;

namespace PlanetGeneration_OLD {
    public interface ITerrainGenerator {
        public void Generate(ref Mesh mesh, Vector3[] sphereVertices, AbstractTerrainSettings asettings);
    }
}
