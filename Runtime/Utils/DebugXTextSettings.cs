using UnityEngine;

namespace DCFApixels
{
    public struct DebugXTextSettings
    {
        public const TextAnchor DEFAULT_TEXT_ANCHOR = TextAnchor.MiddleLeft;
        public const int DEFAULT_FONT_SIZE = 16;
        public static readonly DebugXTextSettings Default = new DebugXTextSettings(DEFAULT_FONT_SIZE, DEFAULT_TEXT_ANCHOR, default);

        public int FontSize;
        public TextAnchor TextAnchor;
        public Color BackgroundColor;
        public bool IsHasBackground
        {
            get { return BackgroundColor.a > 0; }
        }
        public DebugXTextSettings(int fontSize, TextAnchor textAnchor, Color backgroundColor)
        {
            FontSize = fontSize;
            TextAnchor = textAnchor;
            BackgroundColor = backgroundColor;
        }

        public DebugXTextSettings SetSize(int fontSize)
        {
            FontSize = fontSize;
            return this;
        }
        public DebugXTextSettings SetAnchor(TextAnchor textAnchor)
        {
            TextAnchor = textAnchor;
            return this;
        }
        public DebugXTextSettings SetBackground(Color backgroundColor)
        {
            BackgroundColor = backgroundColor;
            return this;
        }
    }
}