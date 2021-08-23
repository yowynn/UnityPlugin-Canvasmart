using UnityEngine;
using System;

namespace Canvasmart
{
    [Flags]
    public enum AnchorPrefab
    {
        Left        = 0b_0000_1000,
        Center      = 0b_0000_0100,
        Right       = 0b_0000_0010,
        Xstretch    = 0b_0000_0001,
        XMask       = 0b_0000_1111,
        Bottom      = 0b_1000_0000,
        Middle      = 0b_0100_0000,
        Top         = 0b_0010_0000,
        Ystretch    = 0b_0001_0000,
        YMask       = 0b_1111_0000,
    }
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

        public static bool Vector2Equals(Vector2 a, Vector2 b)
        {
            return FloatEquals(a.x, b.x) && FloatEquals(a.y, b.y);
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
