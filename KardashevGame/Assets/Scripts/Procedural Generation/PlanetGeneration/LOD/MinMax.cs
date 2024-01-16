using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlanetGeneration {
    public class MinMax {
        public float min { get; private set; }
        public float max { get; private set; }

        public MinMax() {
            Reset();
        }

        public void Reset() {
            min = float.MaxValue;
            max = float.MinValue;
        }

        public void AddValue(float value) {
            if (value < min) min = value;
            else 
            if (value > max) max = value;
        }
    }
}

