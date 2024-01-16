using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(PlanetGenerator))]
public class PlanetEditor : Editor {
    PlanetGenerator planetGenerator;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        DrawSettingsEditor(planetGenerator.shapeSettings, planetGenerator.OnShapeSettingsUpdated);
        DrawSettingsEditor(planetGenerator.colorSettings, planetGenerator.OnColorSettingsUpdated);
    }

    private void DrawSettingsEditor(Object settings, System.Action onSettingsUpdated) {
        using (var check = new EditorGUI.ChangeCheckScope()) {
            Editor editor = CreateEditor(settings);
            editor.OnInspectorGUI();
            
            if (check.changed) {
                if (onSettingsUpdated != null) {
                    onSettingsUpdated();
                }
            }
        }
    }

    private void OnEnable() {
        planetGenerator = (PlanetGenerator)target;
    }
}
