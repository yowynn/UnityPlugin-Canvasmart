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
        public override void OnInspectorGUI()
        {
            Canvasmart canvasmart = target as Canvasmart;
            RectTransform rt = canvasmart.transform as RectTransform;

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("轴线定位"))
            {
                canvasmart.SetLayoutMode(Canvasmart.LayoutModeTag.CrossLine);
                canvasmart.ToAll(go => canvasmart.SetBehaviour(go, canvasmart.InferAutoBehaviour(go)), true);
                // canvasmart.ToAll(go => Debug.Log(go), true);

            }
            if (GUILayout.Button("均匀扩展"))
            {
                canvasmart.SetLayoutMode(Canvasmart.LayoutModeTag.UniformExpand);
                canvasmart.ToAll(go => canvasmart.SetBehaviour(go, canvasmart.InferAutoBehaviour(go)), true);
            }
            EditorGUILayout.EndHorizontal();
            if (GUILayout.Button("布局编辑器"))
            {
                CanvasmartEditorWindow.ShowWindow(canvasmart);
            }
        }
    }
}

