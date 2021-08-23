using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace Canvasmart.Editor
{
    [CustomEditor(typeof(Canvasmart), true)]
    [CanEditMultipleObjects]
    public class CanvasmartEditor : UnityEditor.Editor
    {
        SerializedProperty SectorSize;
        SerializedProperty ModeName;
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            Canvasmart canvasmart = target as Canvasmart;
            RectTransform rt = canvasmart.transform as RectTransform;

            EditorGUILayout.BeginHorizontal();
            Canvasmart.EnumLayoutMode(m =>
            {
                LayoutMode mode = canvasmart.GetLayoutMode(m);
                if (GUILayout.Button(mode.Name))
                {
                    canvasmart.SetLayoutMode(m);
                    canvasmart.ToAll(go => canvasmart.SetBehaviour(go, canvasmart.InferAutoBehaviour(go)), true);
                    // canvasmart.ToAll(go => Debug.Log(go), true);
                }
            });
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.PropertyField(SectorSize);
            EditorGUILayout.PropertyField(ModeName);
            if (GUILayout.Button("布局编辑器"))
            {
                CanvasmartEditorWindow.ShowWindow(canvasmart);
            }
            serializedObject.ApplyModifiedProperties();
        }

        void OnEnable()
        {
            SectorSize = serializedObject.FindProperty("SectorSize");
            ModeName = serializedObject.FindProperty("ModeName");
        }
    }
}

