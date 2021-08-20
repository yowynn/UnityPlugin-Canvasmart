using UnityEngine;

namespace Canvasmart
{
    public enum LayoutBehaviour
    {
        Custom = 0,                                             // 自定义
        Normal = 1,                                             // 正常
        StretchVertical = 2,                                    // 垂直拉伸
        StretchHorizontal = 3,                                  // 水平拉伸
        StretchAll = 4,                                         // 垂直水平拉伸
        Illegal = -1,                                           // 非法
    }
    public interface ILayoutMode
    {
        string Name{ get; }
        LayoutBehaviour InferBehaviour(GameObject go);
        void SetBehaviour(GameObject go, LayoutBehaviour b);
    }

    public abstract class LayoutMode : ILayoutMode
    {
        public static float Precision => 1e-6f;
        public abstract string Name { get; }

        public abstract LayoutBehaviour InferBehaviour(GameObject go);

        public abstract void SetBehaviour(GameObject go, LayoutBehaviour b);

        public static bool FloatEquals(float a, float b)
        {
            return Mathf.Abs(a - b) < Precision;
        }
        public static Rect FloatPositiveRect(Rect rect)
        {
            if (rect.width <= .0f)
            {
                rect.width = Precision;
            }
            if (rect.height <= .0f)
            {
                rect.height = Precision;
            }
            return rect;
        }

        public LayoutBehaviour InferAutoBehaviour(GameObject go)
        {
            LayoutBehaviour b = LayoutBehaviour.Illegal;
            if (IsLayoutNode(go))
            {
                var rt = go.GetComponent<RectTransform>();
                var area = (rt.parent as RectTransform).rect;
                var pos = rt.localPosition;
                if (rt.anchorMin.x == rt.anchorMax.x)
                {
                    if (rt.anchorMin.y == rt.anchorMax.y)
                    {
                        // centralize - centralize
                        b = LayoutBehaviour.Normal;
                    }
                    else
                    {
                        // centralize - stretch
                        b = LayoutBehaviour.StretchVertical;
                    }
                }
                else
                {
                    if (rt.anchorMin.y == rt.anchorMax.y)
                    {
                        // stretch - centralize
                        b = LayoutBehaviour.StretchHorizontal;
                    }
                    else
                    {
                        // stretch - stretch
                        b = LayoutBehaviour.StretchAll;
                    }
                }
            }
            return b;
        }

        public bool IsLayoutNode(GameObject go)
        {
            var rt = go.GetComponent<RectTransform>();
            if(rt != null)
            {
                var prt = rt.parent;
                if (prt is RectTransform)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
