using UnityEngine;

namespace DCFApixels.DebugXCore.Internal
{
    internal static class ColorExtensions
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
    }
}
