using UnityEngine;

namespace DCFApixels.DebugXCore.Samples.Internal
{
    internal static class Utils
    {
        public static Color Evaluate(this Gradient gradient, Transform transform, float m)
        {
            Vector3 pos = transform.localPosition;
            pos /= m == 0 ? 1 : m;
            pos += Vector3.one * 0.5f;
            float t = pos.x + pos.z;
            t /= 2f;
            return gradient.Evaluate(Mathf.Clamp01(t));
        }
        public static Color Evaluate(this Gradient gradient, Transform transform0, Transform transform1, float m)
        {
            Vector3 pos = (transform0.localPosition + transform1.localPosition) * 0.5f;
            pos /= m == 0 ? 1 : m;
            pos += Vector3.one * 0.5f;
            float t = pos.x + pos.z;
            t /= 3f;
            return gradient.Evaluate(Mathf.Clamp01(t));
        }
        public static Color Inverse(this Color c)
        {
            var a = c.a;
            c = Color.white - c;
            c.a = a;
            return c;
        }
    }
}
