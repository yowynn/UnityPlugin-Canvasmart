using UnityEngine;

namespace Canvasmart
{

    public class LayoutModeCrossLine : LayoutMode
    {
        public override string Name => "轴线定位";

        public override LayoutBehaviour InferBehaviour(GameObject go)
        {
            LayoutBehaviour b = LayoutBehaviour.Illegal;
            if (IsLayoutNode(go))
            {
                b = LayoutBehaviour.Custom;
                var rt = go.GetComponent<RectTransform>();
                var area = (rt.parent as RectTransform).rect;
                var pos = rt.localPosition;
                var rect = rt.rect;

                var centerDelta = area.center - new Vector2(pos.x, pos.y);
                int xmode = rect.xMin < centerDelta.x ? (rect.xMax > centerDelta.x ? 0 : -1) : 1;
                int ymode = rect.yMin < centerDelta.y ? (rect.yMax > centerDelta.y ? 0 : -1) : 1;

                if (rt.anchorMin.x == rt.anchorMax.x)
                {
                    if (rt.anchorMin.y == rt.anchorMax.y)
                    {
                        // centralize - centralize
                        float px = xmode == 1 ? 1f : (xmode == -1 ? 0f : .5f);
                        float py = ymode == 1 ? 1f : (ymode == -1 ? 0f : .5f);
                        if (FloatEquals(px, rt.anchorMin.x) && FloatEquals(py, rt.anchorMin.y))
                        {
                            b = LayoutBehaviour.Normal;
                        }
                    }
                    else
                    {
                        // centralize - stretch
                        float px = xmode == 1 ? 1f : (xmode == -1 ? 0f : .5f);
                        float py1 = 0f;
                        float py2 = 1f;
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
                        float px1 = 0f;
                        float px2 = 1f;
                        float py = ymode == 1 ? 1f : (ymode == -1 ? 0f : .5f);
                        if (FloatEquals(px1, rt.anchorMin.x) && FloatEquals(px2, rt.anchorMax.x) && FloatEquals(py, rt.anchorMin.y))
                        {
                            b = LayoutBehaviour.StretchHorizontal;
                        }
                    }
                    else
                    {
                        // stretch - stretch
                        float px1 = 0f;
                        float px2 = 1f;
                        float py1 = 0f;
                        float py2 = 1f;
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

                var centerDelta = area.center - new Vector2(pos.x, pos.y);
                int xmode = rect.xMin < centerDelta.x ? (rect.xMax > centerDelta.x ? 0 : -1) : 1;
                int ymode = rect.yMin < centerDelta.y ? (rect.yMax > centerDelta.y ? 0 : -1) : 1;

                switch (b)
                {
                    case LayoutBehaviour.Normal:
                        {
                            // centralize - centralize
                            float px = xmode == 1 ? 1f : (xmode == -1 ? 0f : .5f);
                            float py = ymode == 1 ? 1f : (ymode == -1 ? 0f : .5f);
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
                            float px = xmode == 1 ? 1f : (xmode == -1 ? 0f : .5f);
                            float py1 = 0f;
                            float py2 = 1f;
                            var amin = rt.anchorMin;
                            amin.x = px;
                            amin.y = py1;
                            rt.anchorMin = amin;
                            var amax = rt.anchorMax;
                            amax.x = px;
                            amax.y = py2;
                            rt.anchorMax = amax;
                            rt.sizeDelta = new Vector2(rect.width, rect.height - area.height);
                            rt.localPosition = pos;
                            break;
                        }
                    case LayoutBehaviour.StretchHorizontal:
                        {
                            // stretch - centralize
                            float px1 = 0f;
                            float px2 = 1f;
                            float py = ymode == 1 ? 1f : (ymode == -1 ? 0f : .5f);
                            var amin = rt.anchorMin;
                            amin.x = px1;
                            amin.y = py;
                            rt.anchorMin = amin;
                            var amax = rt.anchorMax;
                            amax.x = px2;
                            amax.y = py;
                            rt.anchorMax = amax;
                            rt.sizeDelta = new Vector2(rect.width - area.width, rect.height);
                            rt.localPosition = pos;
                            break;
                        }
                    case LayoutBehaviour.StretchAll:
                        {
                            // stretch - stretch
                            float px1 = 0f;
                            float px2 = 1f;
                            float py1 = 0f;
                            float py2 = 1f;
                            var amin = rt.anchorMin;
                            amin.x = px1;
                            amin.y = py1;
                            rt.anchorMin = amin;
                            var amax = rt.anchorMax;
                            amax.x = px2;
                            amax.y = py2;
                            rt.anchorMax = amax;
                            rt.sizeDelta = new Vector2(rect.width - area.width, rect.height - area.height);
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
