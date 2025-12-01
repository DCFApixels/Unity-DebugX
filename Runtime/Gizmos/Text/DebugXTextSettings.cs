#if DISABLE_DEBUG
#undef DEBUG
#endif
using System;
using UnityEngine;

namespace DCFApixels
{
    /// <summary>
    /// All additional settings for text rendering are stored here.
    /// </summary>
    public struct DebugXTextSettings
    {
        public const TextAnchor DEFAULT_TEXT_ANCHOR = TextAnchor.MiddleLeft;
        public const int DEFAULT_FONT_SIZE = 16;
        public const float SCREEN_SPACE_SCALE_BLEND = 0f;
        public const float WORLD_SPACE_SCALE_BLEND = 1f;

        public static readonly DebugXTextSettings ScreenSpace = new DebugXTextSettings(DEFAULT_FONT_SIZE, DEFAULT_TEXT_ANCHOR, Color.clear, 0);
        public static readonly DebugXTextSettings WorldSpace = ScreenSpace.InWorldSpace();

        /// <summary> Font size. Default is <see cref="DEFAULT_FONT_SIZE" />. </summary>
        public int FontSize;
        /// <summary> Text alignment. Default is <see cref="DEFAULT_TEXT_ANCHOR" />. </summary>
        public TextAnchor TextAnchor;
        public Color BackgroundColor;
        public float WorldSpaceBlendMultiplier;

        // ReSharper disable once UnusedMember.Global
        public bool IsHasBackground { get { return BackgroundColor.a > 0; } }
        public DebugXTextSettings(int fontSize, TextAnchor textAnchor, Color backgroundColor, float worldSpaceBlendMultiplier)
        {
            FontSize = fontSize;
            TextAnchor = textAnchor;
            BackgroundColor = backgroundColor;
            WorldSpaceBlendMultiplier = worldSpaceBlendMultiplier;
        }

        /// <summary>
        /// Set font size. Default is <see cref="DEFAULT_FONT_SIZE" />.
        /// </summary>
        public DebugXTextSettings Size(int fontSize)
        {
            return new DebugXTextSettings(fontSize, TextAnchor, BackgroundColor, WorldSpaceBlendMultiplier);
        }

        /// <summary>
        /// Sets text alignment. Default is <see cref="DEFAULT_TEXT_ANCHOR" />.
        /// </summary>
        public DebugXTextSettings Anchor(TextAnchor textAnchor)
        {
            return new DebugXTextSettings(FontSize, textAnchor, BackgroundColor, WorldSpaceBlendMultiplier);
        }

        /// <summary>
        /// Sets background image color behind text. Ignored if transparent.
        /// </summary>
        public DebugXTextSettings Background(Color backgroundColor)
        {
            return new DebugXTextSettings(FontSize, TextAnchor, backgroundColor, WorldSpaceBlendMultiplier);
        }

        /// <summary>
        /// Synchronizes the text scale in screen space. The text will remain the same size on the screen.
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        public DebugXTextSettings InScreenSpace()
        {
            return ScreenWorldSpaceBlend(SCREEN_SPACE_SCALE_BLEND);
        }

        /// <summary>
        /// Synchronizes the text scale in world space. The text will remain the same size on the scene.
        /// </summary>
        public DebugXTextSettings InWorldSpace()
        {
            return ScreenWorldSpaceBlend(WORLD_SPACE_SCALE_BLEND);
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
        public DebugXTextSettings ScreenWorldSpaceBlend(float t)
        {
            return new DebugXTextSettings(FontSize, TextAnchor, BackgroundColor, t);
        }

        #region Obsolete
        [Obsolete("Use " + nameof(WorldSpaceBlendMultiplier))]
        public float WorldSpaceScaleFactor
        {
            get => WorldSpaceBlendMultiplier;
            set => WorldSpaceBlendMultiplier = value;
        }

        [Obsolete("Use " + nameof(ScreenSpace))]
        public static readonly DebugXTextSettings Default = new DebugXTextSettings(DEFAULT_FONT_SIZE, DEFAULT_TEXT_ANCHOR, default, 0);
        [Obsolete("Use " + nameof(WorldSpace))]
        public static readonly DebugXTextSettings WorldSpaceScale = Default.InWorldSpace();

        [Obsolete("Use " + nameof(SCREEN_SPACE_SCALE_BLEND))]
        public const float SCREEN_SPACE_SCALE_FACTOR = 0f;
        [Obsolete("Use " + nameof(WORLD_SPACE_SCALE_BLEND))]
        public const float WORLD_SPACE_SCALE_FACTOR = 1f;

        [Obsolete("Use " + nameof(Size))]
        public DebugXTextSettings SetSize(int fontSize)
        {
            return new DebugXTextSettings(fontSize, TextAnchor, BackgroundColor, WorldSpaceBlendMultiplier);
        }
        [Obsolete("Use " + nameof(Anchor))]
        public DebugXTextSettings SetAnchor(TextAnchor textAnchor)
        {
            return new DebugXTextSettings(FontSize, textAnchor, BackgroundColor, WorldSpaceBlendMultiplier);
        }
        [Obsolete("Use " + nameof(Background))]
        public DebugXTextSettings SetBackground(Color backgroundColor)
        {
            return new DebugXTextSettings(FontSize, TextAnchor, backgroundColor, WorldSpaceBlendMultiplier);
        }
        [Obsolete("Use " + nameof(InScreenSpace))]
        public DebugXTextSettings SetScreenSpaceScaleFactor()
        {
            return SetCustomSpaceScaleFactor(SCREEN_SPACE_SCALE_BLEND);
        }
        [Obsolete("Use " + nameof(InWorldSpace))]
        public DebugXTextSettings SetWorldSpaceScaleFactor()
        {
            return SetCustomSpaceScaleFactor(WORLD_SPACE_SCALE_BLEND);
        }
        [Obsolete("Use " + nameof(ScreenWorldSpaceBlend))]
        public DebugXTextSettings SetCustomSpaceScaleFactor(float factor)
        {
            return new DebugXTextSettings(FontSize, TextAnchor, BackgroundColor, factor);
        }
        #endregion
    }
}