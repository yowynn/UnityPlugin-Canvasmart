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
            EditorGUILayout.PropertyField(ModeName);
            if ((Canvasmart.LayoutModeTag)ModeName.enumValueIndex == Canvasmart.LayoutModeTag.CentralSector)
            {
                canvasmart.ShowGizmos = EditorGUILayout.Foldout(canvasmart.ShowGizmos, "显示中宫辅助线");
                if (canvasmart.ShowGizmos)
                {
                        EditorGUILayout.PropertyField(SectorSize);
                }
            }
            if (GUILayout.Button("自动设置Canvas"))
            {
                InitEditParams();
            }
            if (GUILayout.Button("自动适应Rect"))
            {
                foreach (var child in canvasmart.transform as RectTransform)
                    RectTransformUtil.AutoSize(child as RectTransform, true);
            }
            if (GUILayout.Button("布局编辑器"))
            {
                CanvasmartEditorWindow.ShowWindow(canvasmart);
            }
            serializedObject.ApplyModifiedProperties();
        }

        public void InitEditParams()
        {
            Canvasmart canvasmart = target as Canvasmart;
            var canvas = canvasmart.GetComponent<Canvas>();
            if (canvas != null)
            {
                canvas.renderMode = RenderMode.ScreenSpaceCamera;
                canvas.planeDistance = 10;
            }
            var canvasScaler = canvasmart.GetComponent<UnityEngine.UI.CanvasScaler>();
            if (canvasScaler != null)
            {
                canvasScaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
                canvasScaler.referenceResolution = canvasmart.ReferenceResolution;
                canvasScaler.screenMatchMode = UnityEngine.UI.CanvasScaler.ScreenMatchMode.Expand;
                canvasScaler.referencePixelsPerUnit = 100;
            }

            int w = (int)canvasmart.ReferenceResolution.x;
            int h = (int)canvasmart.ReferenceResolution.y;
            int i = GameViewUtils.FindSize(GameViewSizeGroupType.Standalone, w, h);
            if (i == -1){
                GameViewUtils.AddCustomSize(GameViewUtils.GameViewSizeType.FixedResolution, GameViewSizeGroupType.Standalone, w, h, "UIEditerMode");
                i = GameViewUtils.FindSize(GameViewSizeGroupType.Standalone, "UIEditerMode");
            }
            if (i != -1){
                GameViewUtils.SetSize(i);
            }

        }

        void OnEnable()
        {
            SectorSize = serializedObject.FindProperty("SectorSize");
            ModeName = serializedObject.FindProperty("ModeName");
        }
    }
}

