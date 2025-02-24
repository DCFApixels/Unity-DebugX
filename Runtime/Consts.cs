using System.Runtime.CompilerServices;
using UnityEngine;

namespace DCFApixels
{
    public static partial class DebugX
    {
        internal const MethodImplOptions LINE = MethodImplOptions.AggressiveInlining;

        private const float DEFAULT_DURATION = 0f;
        private static readonly Color DEFAULT_COLOR = Color.white;

        private const float DOT_SIZE = DOT_RADIUS * 2f;
        private const float DOT_RADIUS = 0.05f;
        private const float BackfaceAlphaMultiplier = 0.3f;

        internal readonly static int ColorPropertyID = Shader.PropertyToID("_Color");
        internal readonly static int GlobalDotSizePropertyID = Shader.PropertyToID("_DebugX_GlobalDotSize");
        internal readonly static int GlobalColorPropertyID = Shader.PropertyToID("_DebugX_GlobalColor");

        private readonly static bool IsSRP;
        private readonly static bool IsSupportsComputeShaders = SystemInfo.supportsComputeShaders;

        public const float IMMEDIATE_DURATION = -1;

        private enum PauseStateX
        {
            Unpaused = 0,
            PreUnpaused = 1, //нужно чтобы отщелкунть паузу с задержкой в один тик
            Paused = 2,
        }
    }
    public enum DebugXLine
    {
        Default,
        Arrow,
        Fade,
    }
}