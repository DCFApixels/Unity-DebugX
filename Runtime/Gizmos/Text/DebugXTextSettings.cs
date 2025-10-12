#if DISABLE_DEBUG
#undef DEBUG
#endif
using UnityEngine;

namespace DCFApixels
{
    /// <summary>
    /// All additional settings for text rendering are stored here.
    /// </summary>
    public readonly struct DebugXTextSettings
    {
        public const TextAnchor DEFAULT_TEXT_ANCHOR = TextAnchor.MiddleLeft;
        public const int DEFAULT_FONT_SIZE = 16;

        public const float SCREEN_SPACE_SCALE_FACTOR = 0f;
        public const float WORLD_SPACE_SCALE_FACTOR = 1f;

        public static readonly DebugXTextSettings Default = new DebugXTextSettings(DEFAULT_FONT_SIZE, DEFAULT_TEXT_ANCHOR, default, 0);
        public static readonly DebugXTextSettings WorldSpaceScale = Default.SetWorldSpaceScaleFactor();

        /// <summary>
        /// Font size. Default is <see cref="DEFAULT_FONT_SIZE" />.
        /// </summary>
        public readonly int FontSize;

        /// <summary>
        /// Text alignment. Default is <see cref="DEFAULT_TEXT_ANCHOR" />.
        /// </summary>
        public readonly TextAnchor TextAnchor;

        public readonly Color BackgroundColor;
        public readonly float WorldSpaceScaleFactor;

        // ReSharper disable once UnusedMember.Global
        public bool IsHasBackground => BackgroundColor.a > 0;

        public DebugXTextSettings(int fontSize, TextAnchor textAnchor, Color backgroundColor, float worldSpaceScaleFactor)
        {
            FontSize = fontSize;
            TextAnchor = textAnchor;
            BackgroundColor = backgroundColor;
            WorldSpaceScaleFactor = worldSpaceScaleFactor;
        }

        /// <summary>
        /// Set font size. Default is <see cref="DEFAULT_FONT_SIZE" />.
        /// </summary>
        public DebugXTextSettings SetSize(int fontSize)
        {
            return new DebugXTextSettings(fontSize, TextAnchor, BackgroundColor, WorldSpaceScaleFactor);
        }

        /// <summary>
        /// Sets text alignment. Default is <see cref="DEFAULT_TEXT_ANCHOR" />.
        /// </summary>
        public DebugXTextSettings SetAnchor(TextAnchor textAnchor)
        {
            return new DebugXTextSettings(FontSize, textAnchor, BackgroundColor, WorldSpaceScaleFactor);
        }

        /// <summary>
        /// Sets background image color behind text. Ignored if transparent.
        /// </summary>
        public DebugXTextSettings SetBackground(Color backgroundColor)
        {
            return new DebugXTextSettings(FontSize, TextAnchor, backgroundColor, WorldSpaceScaleFactor);
        }

        /// <summary>
        /// Synchronizes the text scale in screen space. The text will remain the same size on the screen.
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        public DebugXTextSettings SetScreenSpaceScaleFactor()
        {
            return SetCustomSpaceScaleFactor(SCREEN_SPACE_SCALE_FACTOR);
        }

        /// <summary>
        /// Synchronizes the text scale in world space. The text will remain the same size on the scene.
        /// </summary>
        public DebugXTextSettings SetWorldSpaceScaleFactor()
        {
            return SetCustomSpaceScaleFactor(WORLD_SPACE_SCALE_FACTOR);
        }

        /// <summary>
        /// Allows you to control the text scale depending on the camera zoom.
        /// </summary>
        /// <param name="factor">
        /// <br />
        /// 0 - screen space<br />
        /// 1 - world space<br />
        /// Values in between [0.00 - 1.00] blend these spaces together.
        /// </param>
        public DebugXTextSettings SetCustomSpaceScaleFactor(float factor)
        {
            return new DebugXTextSettings(FontSize, TextAnchor, BackgroundColor, factor);
        }
    }
}