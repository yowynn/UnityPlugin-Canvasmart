using UnityEngine;

namespace Canvasmart
{

    public class LayoutModeUniformExpand : LayoutMode
    {
        public override string Name => "均匀扩展";

        public override LayoutBehaviour InferBehaviour(GameObject go)
        {
            LayoutBehaviour b = LayoutBehaviour.Illegal;
            if (IsLayoutNode(go))
            {
                b = LayoutBehaviour.Custom;

                var rt = go.GetComponent<RectTransform>();
                var area = (rt.parent as RectTransform).rect;
                var pos = rt.localPosition;
                if (rt.anchorMin.x == rt.anchorMax.x)
                {
                    if (rt.anchorMin.y == rt.anchorMax.y)
                    {
                        // centralize - centralize
                        float px = (pos.x - area.xMin) / area.width;
                        float py = (pos.y - area.yMin) / area.height;
                        if (FloatEquals(px, rt.anchorMin.x) && FloatEquals(py, rt.anchorMin.y))
                        {
                            b = LayoutBehaviour.Normal;
                        }
                    }
                    else
                    {
                        // centralize - stretch
                        float px = (pos.x - area.xMin) / area.width;
                        float py1 = (pos.y + rt.rect.yMin - area.yMin) / area.height;
                        float py2 = (pos.y + rt.rect.yMax - area.yMin) / area.height;
                        if (FloatEquals(px, rt.anchorMin.x) && FloatEquals(py1, rt.anchorMin.y) && FloatEquals(py2, rt.anchorMax.y))
                        {
                            b = LayoutBehaviour.StretchVertical;
                        }
                    }
                }
                else
                {
                    if (rt.anchorMin.y == rt.anchorMax.y)
                    {
                        // stretch - centralize
                        float px1 = (pos.x + rt.rect.xMin - area.xMin) / area.width;
                        float px2 = (pos.x + rt.rect.xMax - area.xMin) / area.width;
                        float py = (pos.y - area.yMin) / area.height;
                        if (FloatEquals(px1, rt.anchorMin.x) && FloatEquals(px2, rt.anchorMax.x) && FloatEquals(py, rt.anchorMin.y))
                        {
                            b = LayoutBehaviour.StretchHorizontal;
                        }
                    }
                    else
                    {
                        // stretch - stretch
                        float px1 = (pos.x + rt.rect.xMin - area.xMin) / area.width;
                        float px2 = (pos.x + rt.rect.xMax - area.xMin) / area.width;
                        float py1 = (pos.y + rt.rect.yMin - area.yMin) / area.height;
                        float py2 = (pos.y + rt.rect.yMax - area.yMin) / area.height;
                        if (FloatEquals(px1, rt.anchorMin.x) && FloatEquals(px2, rt.anchorMax.x) && FloatEquals(py1, rt.anchorMin.y) && FloatEquals(py2, rt.anchorMax.y))
                        {
                            b = LayoutBehaviour.StretchAll;
                        }
                    }
                }
            }
            return b;
        }

        public override void SetBehaviour(GameObject go, LayoutBehaviour b)
        {
            if (IsLayoutNode(go))
            {
                var rt = go.GetComponent<RectTransform>();
                var area = FloatPositiveRect((rt.parent as RectTransform).rect);
                var pos = rt.localPosition;
                var rect = FloatPositiveRect(rt.rect);
                switch (b)
                {
                    case LayoutBehaviour.Normal:
                        {
                            // centralize - centralize
                            float px = (pos.x - area.xMin) / area.width;
                            float py = (pos.y - area.yMin) / area.height;
                            var amin = rt.anchorMin;
                            amin.x = px;
                            amin.y = py;
                            rt.anchorMin = amin;
                            var amax = rt.anchorMax;
                            amax.x = px;
                            amax.y = py;
                            rt.anchorMax = amax;
                            rt.sizeDelta = rect.size;
                            rt.localPosition = pos;
                            break;
                        }
                    case LayoutBehaviour.StretchVertical:
                        {
                            // centralize - stretch
                            float px = (pos.x - area.xMin) / area.width;
                            float py1 = (pos.y + rect.yMin - area.yMin) / area.height;
                            float py2 = (pos.y + rect.yMax - area.yMin) / area.height;
                            var amin = rt.anchorMin;
                            amin.x = px;
                            amin.y = py1;
                            rt.anchorMin = amin;
                            var amax = rt.anchorMax;
                            amax.x = px;
                            amax.y = py2;
                            rt.anchorMax = amax;
                            rt.sizeDelta = new Vector2(rect.width, 0);
                            rt.localPosition = pos;
                            break;
                        }
                    case LayoutBehaviour.StretchHorizontal:
                        {
                            // stretch - centralize
                            float px1 = (pos.x + rect.xMin - area.xMin) / area.width;
                            float px2 = (pos.x + rect.xMax - area.xMin) / area.width;
                            float py = (pos.y - area.yMin) / area.height;
                            var amin = rt.anchorMin;
                            amin.x = px1;
                            amin.y = py;
                            rt.anchorMin = amin;
                            var amax = rt.anchorMax;
                            amax.x = px2;
                            amax.y = py;
                            rt.anchorMax = amax;
                            rt.sizeDelta = new Vector2(0, rect.height);
                            rt.localPosition = pos;
                            break;
                        }
                    case LayoutBehaviour.StretchAll:
                        {
                            // stretch - stretch
                            float px1 = (pos.x + rect.xMin - area.xMin) / area.width;
                            float px2 = (pos.x + rect.xMax - area.xMin) / area.width;
                            float py1 = (pos.y + rect.yMin - area.yMin) / area.height;
                            float py2 = (pos.y + rect.yMax - area.yMin) / area.height;
                            var amin = rt.anchorMin;
                            amin.x = px1;
                            amin.y = py1;
                            rt.anchorMin = amin;
                            var amax = rt.anchorMax;
                            amax.x = px2;
                            amax.y = py2;
                            rt.anchorMax = amax;
                            rt.sizeDelta = Vector2.zero;
                            rt.localPosition = pos;
                            break;
                        }
                    default:
                        break;
                }
            }
        }

    }

}
