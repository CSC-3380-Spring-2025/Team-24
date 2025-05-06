using UnityEditor;
using UnityEngine;

/// <summary>
/// 
/// </summary>
[CustomEditor(typeof(FishGenerator))]
public class FishGeneratorEditor : Editor
{
    private FishType testType;
    private int testLevel = 1;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        FishGenerator generator = (FishGenerator)target;

        GUILayout.Space(10);
        GUILayout.Label("Test Fish Generation", EditorStyles.boldLabel);

        testLevel = EditorGUILayout.IntSlider("Fish Level", testLevel, 1, 30);

        if (GUILayout.Button("Generate Fish"))
        {
            generator.DebugGenerateAndPrint(testType, testLevel);
        }
    }
}