using UnityEngine;

namespace Canvasmart
{
    public static class RectTransformUtil
    {
        public static void AutoSize(RectTransform rt, bool onChildren = false)
        {
            if (rt == null)
                return;
            var rect = rt.rect;
            var pos = rt.localPosition;
            float xMin = rect.xMin;
            float yMin = rect.yMin;
            float xMax = rect.xMax;
            float yMax = rect.yMax;
            foreach (var child in rt)
            {
                var rt0 = child as RectTransform;
                if (rt0 != null)
                {
                    if (onChildren)
                        AutoSize(rt0, true);
                    var rect0 = rt0.rect;
                    var pos0 = rt0.localPosition;
                    xMin = Mathf.Min(xMin, rect0.xMin + pos0.x);
                    yMin = Mathf.Min(yMin, rect0.yMin + pos0.y);
                    xMax = Mathf.Max(xMax, rect0.xMax + pos0.x);
                    yMax = Mathf.Max(yMax, rect0.yMax + pos0.y);
                }
            }
            var deltaWidth = (xMax - xMin) - rect.width;
            var deltaHeight = (yMax - yMin) - rect.height;
            if (deltaWidth != 0 || deltaHeight != 0)
            {
                rt.sizeDelta += new Vector2(deltaWidth, deltaHeight);
                rt.pivot = new Vector2(xMin / (xMin - xMax), yMin / (yMin - yMax));
                var newrect = rt.rect;
                foreach (var child in rt)
                {
                    var rt0 = child as RectTransform;
                    if (rt0 != null)
                    {
                        var anchorMin = rect.min + rect.size * rt0.anchorMin;
                        rt0.anchorMin = (anchorMin - newrect.min) / newrect.size;
                        var anchorMax = rect.min + rect.size * rt0.anchorMax;
                        rt0.anchorMax = (anchorMax - newrect.min) / newrect.size;
                    }
                }
            }
        }
    }
}
