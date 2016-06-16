using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditor.UI;

[CanEditMultipleObjects]
public class BLUIInspector : Editor {

    [MenuItem("BL/EmoticonEditor")]
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Open EditorWindow"))
        {
            BLUIDirectoryWindow.AddWindow();
        }

        EditorGUILayout.EndHorizontal();
    }
}
