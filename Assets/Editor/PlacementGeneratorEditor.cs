using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlacementGenerator))]
public class PlacementGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        PlacementGenerator placementGenerator = (PlacementGenerator)target;

        if (GUILayout.Button("Generate"))
        {
            placementGenerator.Generate();
        }
    }
}
