#if DISABLE_DEBUGX
#undef DEBUG
#undef UNITY_EDITOR
#endif
using UnityEngine;

namespace DCFApixels
{
    public struct DebugXTextSettings
    {
        public const TextAnchor DefaultTextAnchor = TextAnchor.MiddleLeft;
        public const int DefaultFontSize = 16;
        public const float ScreenSpaceScaleBlendMultiplier = 0f;
        public const float WorldSpaceScaleBlendMultiplier = 1f;

        public static readonly DebugXTextSettings ScreenSpace = new DebugXTextSettings(DefaultFontSize, DefaultTextAnchor, Color.clear, 0);
        public static readonly DebugXTextSettings WorldSpace = ScreenSpace.ToWorldScale();

        public int Size;
        public TextAnchor Anchor;
        public Color BackgroundColor;
        public float WorldToSpaceScaleBlend;

        public bool IsHasBackground { get { return BackgroundColor.a > 0; } }
        public DebugXTextSettings(int fontSize, TextAnchor textAnchor, Color backgroundColor, float worldSpaceBlendMultiplier)
        {
            Size = fontSize;
            Anchor = textAnchor;
            BackgroundColor = backgroundColor;
            WorldToSpaceScaleBlend = worldSpaceBlendMultiplier;
        }
    }

    public static class DebugXTextSettingsExtensions
    {
        public static DebugXTextSettings Size(this DebugXTextSettings settings, int size)
        {
            return new DebugXTextSettings(size, settings.Anchor, settings.BackgroundColor, settings.WorldToSpaceScaleBlend);
        }
        public static DebugXTextSettings Anchor(this DebugXTextSettings settings, TextAnchor anchor)
        {
            return new DebugXTextSettings(settings.Size, anchor, settings.BackgroundColor, settings.WorldToSpaceScaleBlend);
        }
        public static DebugXTextSettings BackgroundColor(this DebugXTextSettings settings, Color backgroundColor)
        {
            return new DebugXTextSettings(settings.Size, settings.Anchor, backgroundColor, settings.WorldToSpaceScaleBlend);
        }
        public static DebugXTextSettings ToScreenScale(this DebugXTextSettings settings)
        {
            return settings.WorldToSpaceScaleBlend(DebugXTextSettings.ScreenSpaceScaleBlendMultiplier);
        }
        public static DebugXTextSettings ToWorldScale(this DebugXTextSettings settings)
        {
            return settings.WorldToSpaceScaleBlend(DebugXTextSettings.WorldSpaceScaleBlendMultiplier);
        }
        public static DebugXTextSettings WorldToSpaceScaleBlend(this DebugXTextSettings settings, float t)
        {
            return new DebugXTextSettings(settings.Size, settings.Anchor, settings.BackgroundColor, t);
        }
    }
}