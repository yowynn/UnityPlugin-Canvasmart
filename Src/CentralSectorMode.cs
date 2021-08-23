using UnityEngine;

namespace Canvasmart
{

    public class CentralSectorMode : LayoutMode
    {
        public override string Name => "中宫定位";

        public Vector2 SectorSize = new Vector2(1f / 3, 1f / 3);

        public override LayoutBehaviour InferBehaviour(GameObject go)
        {
            LayoutBehaviour b = LayoutBehaviour.Illegal;
            if (IsLayoutNode(go))
            {
                b = LayoutBehaviour.Custom;

                var ap = InferAnchorPrefab(go);
                var anchor = GetAnchorRect(ap);
                var rt = go.GetComponent<RectTransform>();
                if (Vector2Equals(rt.anchorMin, anchor.min) && Vector2Equals(rt.anchorMax, anchor.max))
                {
                    var apx = ap & AnchorPrefab.XMask;
                    var apy = ap & AnchorPrefab.YMask;
                    if (apx == AnchorPrefab.Xstretch)
                    {
                        if (apy == AnchorPrefab.Ystretch)
                            b = LayoutBehaviour.StretchAll;
                        else
                            b = LayoutBehaviour.StretchHorizontal;
                    }
                    else
                    {
                        if (apy == AnchorPrefab.Ystretch)
                            b = LayoutBehaviour.StretchVertical;
                        else
                            b = LayoutBehaviour.Normal;
                    }
                }
            }
            return b;
        }

        public override void SetBehaviour(GameObject go, LayoutBehaviour b)
        {
            if (IsLayoutNode(go))
            {
                var ap = InferAnchorPrefab(go);
                var anchor = GetAnchorRect(ap);
                var rt = go.GetComponent<RectTransform>();
                var area = FloatPositiveRect((rt.parent as RectTransform).rect);
                var rect = FloatPositiveRect(rt.rect);
                var pos = rt.localPosition;
                switch (b)
                {
                    case LayoutBehaviour.Normal:
                        {
                            // centralize - centralize
                            rt.anchorMin = anchor.min;
                            rt.anchorMax = anchor.max;
                            rt.sizeDelta = rect.size;
                            rt.localPosition = pos;
                            break;
                        }
                    case LayoutBehaviour.StretchVertical:
                        {
                            // centralize - stretch
                            rt.anchorMin = anchor.min;
                            rt.anchorMax = anchor.max;
                            rt.sizeDelta = new Vector2(rect.width, rect.height - area.height);
                            rt.localPosition = pos;
                            break;
                        }
                    case LayoutBehaviour.StretchHorizontal:
                        {
                            // stretch - centralize
                            rt.anchorMin = anchor.min;
                            rt.anchorMax = anchor.max;
                            rt.sizeDelta = new Vector2(rect.width - area.width, rect.height);
                            rt.localPosition = pos;
                            break;
                        }
                    case LayoutBehaviour.StretchAll:
                        {
                            // stretch - stretch
                            rt.anchorMin = anchor.min;
                            rt.anchorMax = anchor.max;
                            rt.sizeDelta = new Vector2(rect.width - area.width, rect.height - area.height);
                            rt.localPosition = pos;
                            break;
                        }
                    default:
                        break;
                }
            }
        }

        public AnchorPrefab InferAnchorPrefab(GameObject go)
        {
            AnchorPrefab ap = (AnchorPrefab)0;
            var rt = go.GetComponent<RectTransform>();
            var area = (rt.parent as RectTransform).rect;
            var deltaSize = (Vector2.one - SectorSize) / 2;

            var pos = rt.localPosition;
            var rect = new Rect(area.position + area.size * deltaSize, area.size * SectorSize);

            if (rt.anchorMin.x != rt.anchorMax.x)
                ap |= AnchorPrefab.Xstretch;
            else if (pos.x < rect.xMin)
                ap |= AnchorPrefab.Left;
            else if (pos.x > rect.xMax)
                ap |= AnchorPrefab.Right;
            else
                ap |= AnchorPrefab.Center;
            if (rt.anchorMin.y != rt.anchorMax.y)
                ap |= AnchorPrefab.Ystretch;
            else if (pos.y < rect.yMin)
                ap |= AnchorPrefab.Bottom;
            else if (pos.y > rect.yMax)
                ap |= AnchorPrefab.Top;
            else
                ap |= AnchorPrefab.Middle;
            return ap;
        }

        public Rect GetAnchorRect(AnchorPrefab ap)
        {
            Rect r = new Rect();
            float x1, x2, y1, y2;
            switch (ap & AnchorPrefab.XMask)
            {
                case AnchorPrefab.Left: x1 = 0f; x2 = 0f; break;
                case AnchorPrefab.Center: x1 = 0.5f; x2 = 0.5f; break;
                case AnchorPrefab.Right: x1 = 1f; x2 = 1f; break;
                case AnchorPrefab.Xstretch: x1 = 0f; x2 = 1f; break;
                default: throw new System.Exception("?");
            }
            switch (ap & AnchorPrefab.YMask)
            {
                case AnchorPrefab.Bottom: y1 = 0f; y2 = 0f; break;
                case AnchorPrefab.Middle: y1 = 0.5f; y2 = 0.5f; break;
                case AnchorPrefab.Top: y1 = 1f; y2 = 1f; break;
                case AnchorPrefab.Ystretch: y1 = 0f; y2 = 1f; break;
                default: throw new System.Exception("?");
            }
            r.xMin = x1;
            r.xMax = x2;
            r.yMin = y1;
            r.yMax = y2;
            return r;
        }
    }

}
