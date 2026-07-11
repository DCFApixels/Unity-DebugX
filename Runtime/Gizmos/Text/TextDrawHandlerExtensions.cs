#if DISABLE_DEBUGX
#undef DEBUG
#undef UNITY_EDITOR
#endif
using System.Runtime.CompilerServices;
using UnityEngine;

namespace DCFApixels
{
    using DrawHandler = DebugX.DrawHandler;
    using IN = MethodImplAttribute;

    public static class TextDrawHandlerExtensions
    {
        private const MethodImplOptions LINE = DebugX.LINE;
#if DEBUG
        private static bool _singleWarningToggle = true;
#endif
        [IN(LINE)]
        public static DrawHandler Text<T>(this DrawHandler h, Vector3 position, T text) 
            where T : struct
        {
            return h.Text(position, text, DebugXTextSettings.ScreenSpace);
        }
        [IN(LINE)]
        public static DrawHandler Text(this DrawHandler h, Vector3 position, object text)
        {
            return h.Text(position, text, DebugXTextSettings.ScreenSpace);
        }
        [IN(LINE)]
        public static DrawHandler Text<T>(this DrawHandler h, Vector3 position, T text, DebugXTextSettings settings)
            where T : struct
        {
#if DEBUG
            return h.Text(position, (object)text, settings);
#endif
        }
        [IN(LINE)]
        public static DrawHandler Text(this DrawHandler h, Vector3 position, object text, DebugXTextSettings settings)
        {
            if (settings.Size <= float.Epsilon)
            {
#if DEBUG
                if (_singleWarningToggle)
                {
                    Debug.LogWarning("Text rendering requires FontSize > 0, otherwise the text will be invisible. To avoid invalid parameters, use DebugXTextSettings.Default instead of manual instantiation.");
                    _singleWarningToggle = false;
                }
#endif
                settings = settings.Size(DebugXTextSettings.DefaultFontSize);
            }
            return h.Gizmo(new TextGizmo(position, text, settings));
        }
    }
}