using UnityEngine;

namespace DCFApixels.DebugXCore.Internal
{
    internal static class Extensions
    {
        public static Color SetAlpha(this Color self, float v)
        {
            self.a *= v;
            return self;
        }
        public static Color ToColor(ref this (Color color, float alpha) self)
        {
            self.color.a *= self.alpha;
            return self.color;
        }
        public static Vector3 Mult(this Vector3 a, Vector3 b)
        {
            a.x *= b.x;
            a.y *= b.y;
            a.z *= b.z;
            return a;
        }
        public static Vector2 Mult(this Vector2 a, Vector2 b)
        {
            a.x *= b.x;
            a.y *= b.y;
            return a;
        }
    }
}
