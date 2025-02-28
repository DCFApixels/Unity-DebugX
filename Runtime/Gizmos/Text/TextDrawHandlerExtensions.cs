using System.Runtime.CompilerServices;
using UnityEngine;

namespace DCFApixels {
    using IN = MethodImplAttribute;
    using DrawHandler = DebugX.DrawHandler;
    
    public static class TextDrawHandlerExtensions {
        private const MethodImplOptions LINE = DebugX.LINE;
        
        [IN(LINE)] public static DrawHandler Text(this DrawHandler drawHandler, Vector3 position, object text) =>
            drawHandler.Gizmo(new TextGizmo(position, text, DebugXTextSettings.Default));

        [IN(LINE)] public static DrawHandler Text(this DrawHandler drawHandler, Vector3 position, object text, DebugXTextSettings settings) => 
            drawHandler.Gizmo(new TextGizmo(position, text, settings));
    }
}