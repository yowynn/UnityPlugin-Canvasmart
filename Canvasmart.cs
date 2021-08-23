using System;
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
        public delegate void OnEnumGameObject(GameObject go);
        public delegate void OnEnumLayoutMode(LayoutModeTag m);
        public enum LayoutModeTag
        {
            UniformTension,
            CentralAxis,
            CentralSector,
        }

        private Dictionary<LayoutModeTag, LayoutMode> ModeMap;

        [SerializeField] private int m_test;
        private void InitLayoutMode()
        {
            ModeMap = new Dictionary<LayoutModeTag, LayoutMode>();
            EnumLayoutMode(m =>
            {
                switch (m)
                {
                    case LayoutModeTag.UniformTension:
                        {
                            var mode = new UniformTensionMode();
                            ModeMap.Add(m, mode);
                            break;
                        }
                    case LayoutModeTag.CentralAxis:
                        {
                            var mode = new CentralAxisMode();
                            ModeMap.Add(m, mode);
                            break;
                        }
                    case LayoutModeTag.CentralSector:
                        {
                            var mode = new CentralSectorMode();
                            mode.SectorSize = SectorSize;
                            ModeMap.Add(m, mode);
                            break;
                        }
                    default:
                        {
                            throw new Exception("未定义对应模式");
                        }
                }
            });
        }

        [Tooltip("自动布局模式")]
        public LayoutModeTag ModeName = LayoutModeTag.UniformTension;
        protected LayoutMode Mode;
        [Tooltip("UI的参考分辨率")]
        public Vector2 ReferenceResolution = new Vector2(2048, 1151);
        [Tooltip("忽略物件列表")]
        public List<GameObject> IgnoredList = new List<GameObject>();
        [Tooltip("中宫定位模式的中宫比例")]
        public Vector2 SectorSize = new Vector2(1f / 3, 1f / 3);
        public bool IsLegal(GameObject go, bool canvasIllegal = false)
        {
            if(go == gameObject)
            {
                return !canvasIllegal;
            }
            else
            {
                return Mode.IsLayoutNode(go) && IsInDirectControl(go);
            }
        }

        public bool IsInDirectControl(GameObject go)
        {
            var t = go.transform.parent;
            while(t != null)
            {
                var c = t.GetComponent<Canvasmart>();
                if (c != null)
                    return c == this;
                t = t.parent;
            }
            return false;
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
            if (ModeMap == null)
                InitLayoutMode();
            if (ModeMap.TryGetValue(m, out Mode))
                ModeName = m;
            else
                throw new Exception("Mode not find");
        }

        public LayoutMode GetLayoutMode()
        {
            if(Mode == null)
                SetLayoutMode(ModeName);
            return Mode;
        }

        public LayoutMode GetLayoutMode(LayoutModeTag m)
        {
            if (ModeMap == null)
                InitLayoutMode();
            LayoutMode mode;
            ModeMap.TryGetValue(m, out mode);
            return mode;
        }

        public void ToAll(OnEnumGameObject onEnum, bool useIgnoreList)
        {
            ToAll(onEnum, useIgnoreList, gameObject);
        }
        private void ToAll(OnEnumGameObject onEnum, bool useIgnoreList, GameObject go)
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

        public static void EnumLayoutMode(OnEnumLayoutMode cb)
        {
            foreach (LayoutModeTag m in Enum.GetValues(typeof(LayoutModeTag)))
                cb(m);
        }

        void Awake()
        {
            InitLayoutMode();
        }
#if UNITY_EDITOR

        void OnValidate()
        {
            SetLayoutMode(ModeName);
            SectorSize.x = Mathf.Min(1, Mathf.Max(0, SectorSize.x));
            SectorSize.y = Mathf.Min(1, Mathf.Max(0, SectorSize.y));
            (GetLayoutMode(LayoutModeTag.CentralSector) as CentralSectorMode).SectorSize = SectorSize;
        }
        void Reset()
        {
            SectorSize = new Vector2(1f / 3, 1f / 3);
        }
#endif
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}

