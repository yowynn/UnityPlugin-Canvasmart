using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Canvasmart
{
    [RequireComponent(typeof(Canvas))]
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public class Canvasmart : MonoBehaviour
    {
        public delegate void OnEnum(GameObject go);
        public enum LayoutModeTag
        {
            UniformExpand,
            CrossLine,
            CrossMesh,
        }

        [Tooltip("自动布局模式")]
        public LayoutModeTag ModeName = LayoutModeTag.UniformExpand;
        protected LayoutMode Mode;
        [Tooltip("UI的参考分辨率")]
        public Vector2 ReferenceResolution = new Vector2(2048, 1151);
        [Tooltip("忽略物件列表")]
        public List<GameObject> IgnoredList = new List<GameObject>();

        public bool IsLegal(GameObject go, bool canvasIllegal = false)
        {
            if(go == gameObject)
            {
                return !canvasIllegal;
            }
            else
            {
                return Mode.IsLayoutNode(go);
            }
        }
        public void AddToIgnoredList(GameObject go)
        {
            if (IsLegal(go, true) && !IsIgnore(go))
                IgnoredList.Add(go);
        }

        public void RemoveFromIgnoredList(GameObject go)
        {
            IgnoredList.Remove(go);
        }
        public bool IsIgnore(GameObject go)
        {
            return IgnoredList.Exists(g => g == go);
        }

        public LayoutBehaviour InferBehaviour(GameObject go)
        {
            return Mode.InferBehaviour(go);
        }

        public LayoutBehaviour InferAutoBehaviour(GameObject go)
        {
            return Mode.InferAutoBehaviour(go);
        }

        public LayoutBehaviour SetBehaviour(GameObject go, LayoutBehaviour b)
        {
            EnterProtectScreenMode();
            if (IsLegal(go, true))
            {
                Mode.SetBehaviour(go, b);
            }
            b = InferBehaviour(go);
            QuitProtectScreenMode();
            return b;
        }

        public void SetLayoutMode(LayoutModeTag m)
        {
            ModeName = m;
            switch (m)
            {
                case LayoutModeTag.UniformExpand:
                    {
                        Mode = new LayoutModeUniformExpand();
                        break;
                    }
                case LayoutModeTag.CrossLine:
                    {
                        Mode = new LayoutModeCrossLine();
                        break;
                    }
            }
        }

        public void SetLayoutMode(LayoutMode m)
        {
            Mode = m;
            if (m is LayoutModeUniformExpand)
            {
                ModeName = LayoutModeTag.UniformExpand;
            }
            else if (m is LayoutModeCrossLine)
            {
                ModeName = LayoutModeTag.CrossLine;
            }
        }

        public LayoutMode GetLayoutMode()
        {
            return Mode;
        }

        public void ToAll(OnEnum onEnum, bool useIgnoreList)
        {
            ToAll(onEnum, useIgnoreList, gameObject);
        }
        private void ToAll(OnEnum onEnum, bool useIgnoreList, GameObject go)
        {
            if (IsLegal(go))
            {
                Transform t = go.transform;
                for (int i = 0; i < t.childCount; i++)
                {
                    var child = t.GetChild(i).gameObject;
                    if (!useIgnoreList || !IsIgnore(child))
                        onEnum(child);
                    ToAll(onEnum, useIgnoreList, child);
                }
            }
        }
        private Vector2 _saved_sizeDelta = Vector2.zero;
        private Vector3 _saved_localPosition = Vector3.zero;

        public void EnterProtectScreenMode()
        {
            if (_saved_sizeDelta == Vector2.zero)
            {
                var t = gameObject.GetComponent<RectTransform>();
                _saved_localPosition = t.localPosition;
                _saved_sizeDelta = t.sizeDelta;
                t.localPosition = new Vector3(ReferenceResolution.x / 2, ReferenceResolution.y / 2, 0);
                t.sizeDelta = ReferenceResolution;
            }
        }

        public void QuitProtectScreenMode()
        {
            if (_saved_sizeDelta != Vector2.zero)
            {
                var t = gameObject.GetComponent<RectTransform>();
                t.localPosition = _saved_localPosition;
                t.sizeDelta = _saved_sizeDelta;
                _saved_sizeDelta = Vector2.zero;
                _saved_localPosition = Vector3.zero;
            }
        }

        void OnValidate()
        {
            SetLayoutMode(ModeName);
        }

        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}

