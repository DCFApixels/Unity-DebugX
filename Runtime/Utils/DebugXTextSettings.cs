using UnityEngine;

namespace DCFApixels
{
    public readonly struct DebugXTextSettings
    {
        public const TextAnchor DEFAULT_TEXT_ANCHOR = TextAnchor.MiddleLeft;
        public const int DEFAULT_FONT_SIZE = 16;
        public static readonly DebugXTextSettings Default = new DebugXTextSettings(DEFAULT_FONT_SIZE, DEFAULT_TEXT_ANCHOR, default, 0);
        public static readonly DebugXTextSettings WorldSpaceScale = Default.SetWorldSpaceScaleFactor(1f);

        public readonly int FontSize;
        public readonly TextAnchor TextAnchor;
        public readonly Color BackgroundColor;
        public readonly float WorldSpaceScaleFactor;
        public bool IsHasBackground
        {
            get { return BackgroundColor.a > 0; }
        }
        public DebugXTextSettings(int fontSize, TextAnchor textAnchor, Color backgroundColor, float worldSpaceScaleFactor)
        {
            FontSize = fontSize;
            TextAnchor = textAnchor;
            BackgroundColor = backgroundColor;
            WorldSpaceScaleFactor = worldSpaceScaleFactor;
        }

        public DebugXTextSettings SetSize(int fontSize)
        {
            return new DebugXTextSettings(fontSize, TextAnchor, BackgroundColor, WorldSpaceScaleFactor);
        }
        public DebugXTextSettings SetAnchor(TextAnchor textAnchor)
        {
            return new DebugXTextSettings(FontSize, textAnchor, BackgroundColor, WorldSpaceScaleFactor);
        }
        public DebugXTextSettings SetBackground(Color backgroundColor)
        {
            return new DebugXTextSettings(FontSize, TextAnchor, backgroundColor, WorldSpaceScaleFactor);
        }
        public DebugXTextSettings SetWorldSpaceScaleFactor(float factor)
        {
            return new DebugXTextSettings(FontSize, TextAnchor, BackgroundColor, factor);
        }
    }
}