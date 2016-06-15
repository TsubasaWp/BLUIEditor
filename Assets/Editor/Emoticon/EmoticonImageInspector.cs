using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.AnimatedValues;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.UI;

namespace uizh.UI
{
    [CustomEditor(typeof(EmoticonImage), true)]
    [CanEditMultipleObjects]
    public class EmoticonImageEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Open EditorWindow"))
            {
                EmoticonEditorWindow.AddWindow();
            }

            EditorGUILayout.EndHorizontal();
        }
    }
}
