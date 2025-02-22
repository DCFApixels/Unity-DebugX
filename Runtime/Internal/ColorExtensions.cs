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
    }
}
