#if DISABLE_DEBUG
#undef DEBUG
#endif
#if UNITY_EDITOR
using UnityEditor;
#endif
using DCFApixels.DebugXCore;
using UnityEngine;
using UnityEngine.Rendering;

namespace DCFApixels
{
    public static class WireArcGizmosExtensions
    {
        public static DebugX.DrawHandler WireArc(this DebugX.DrawHandler self, Vector3 center, Vector3 normal, Vector3 from, float angle, float radius)
        {
            return self.Gizmo(new WireArcGizmos(center, normal, from, angle, radius));
        }
    }
}

namespace DCFApixels.DebugXCore
{
    using static DebugX;
    using IN = System.Runtime.CompilerServices.MethodImplAttribute;
    public struct WireArcGizmos : IGizmo<WireArcGizmos>
    {
        public readonly Vector3 Position;
        public readonly Vector3 Normal;
        public readonly Vector3 From;
        public readonly float Angle;
        public readonly float Radius;
        [IN(LINE)]
        public WireArcGizmos(Vector3 position, Vector3 normal, Vector3 from, float angle, float radius)
        {
            Position = position;
            Normal = normal.CheckNormalOrDefault();
            From = from;
            Angle = angle;
            Radius = radius;
        }
        public IGizmoRenderer<WireArcGizmos> RegisterNewRenderer()
        {
            return new Renderer();
        }

        private class Renderer : IGizmoRenderer<WireArcGizmos>
        {
            public int ExecuteOrder => default(WireMat).GetExecuteOrder();
            public bool IsStaticRender => false;
            public void Prepare(Camera camera, GizmosList<WireArcGizmos> list) { }
            public void Render(Camera camera, GizmosList<WireArcGizmos> list, CommandBuffer cb)
            {
#if UNITY_EDITOR
                Color handles_color = Handles.color;
                foreach (var gizmo in list)
                {
                    Handles.color = gizmo.Color;
                    Handles.DrawWireArc(gizmo.Value.Position, gizmo.Value.Normal, gizmo.Value.From, gizmo.Value.Angle, gizmo.Value.Radius);
                }
                Handles.color = handles_color;
#endif
            }
        }
    }

}
