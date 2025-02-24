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

        private const string GLOBAL_TIME_SCALE_PREF_NAME = "DCFApixels.DebugX.TimeScale";
        private const string GLOBAL_DOT_SIZE_PREF_NAME = "DCFApixels.DebugX.DotSize";
        private const string GLOBAL_COLOR_PREF_NAME = "DCFApixels.DebugX.Color";

        private readonly static int GlobalDotSizePropertyID = Shader.PropertyToID("_DebugX_GlobalDotSize");
        private readonly static int GlobalColorPropertyID = Shader.PropertyToID("_DebugX_GlobalColor");
        internal readonly static int ColorPropertyID = Shader.PropertyToID("_Color");

        public const float IMMEDIATE_DURATION = -1;

        public readonly static bool IsSRP;
        public readonly static bool IsSupportsComputeShaders = SystemInfo.supportsComputeShaders;


        public const bool DISABLE_DEBUGX_INBUILD =
#if DISABLE_DEBUGX_INBUILD
            true;
#else
            false;
#endif

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