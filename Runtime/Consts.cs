using System.Runtime.CompilerServices;
using UnityEngine;

namespace DCFApixels
{
    public static partial class DebugX
    {
        internal const MethodImplOptions LINE = MethodImplOptions.AggressiveInlining;

        private const float MinTime = 0f;
        private static readonly Color DefaultColor = Color.white;

        private const float DOT_SIZE = DOT_RADIUS * 2f;
        private const float DOT_RADIUS = 0.05f;
        private const float BackfaceAlphaMultiplier = 0.3f;

        internal readonly static int ColorPropertyID = Shader.PropertyToID("_Color");
        internal readonly static int GlobalDotSizePropertyID = Shader.PropertyToID("_DebugX_GlobalDotSize");
        internal readonly static int GlobalColorPropertyID = Shader.PropertyToID("_DebugX_GlobalColor");

        private readonly static bool IsSRP;
        private readonly static bool IsSupportsComputeShaders = SystemInfo.supportsComputeShaders;

        public const float IMMEDIATE_DURATION = -1;
    }

    public enum DebugXLine
    {
        Default,
        Arrow,
        Fade,
    }
}