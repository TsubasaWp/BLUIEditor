using UnityEngine;
using System.Collections;
using UnityEditor;
using uizh.UI;

[CustomEditor(typeof(EmoticonDB))]
public class EmoticonDBInspector : Editor
{
    void OnEnable()
    {
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        if (GUILayout.Button("Open EditorWindow"))
        {
            EmoticonEditorWindow.AddWindow();
        }

        serializedObject.ApplyModifiedProperties();
        base.OnInspectorGUI();
    }
}
