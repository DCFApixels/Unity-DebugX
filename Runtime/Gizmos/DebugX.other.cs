#if DISABLE_DEBUGX
#undef DEBUG
#undef UNITY_EDITOR
#endif
#if UNITY_EDITOR
using UnityEditor;
#endif
using DCFApixels.DebugXCore;
using UnityEngine;
using UnityEngine.Rendering;

namespace DCFApixels
{
    public static class OtherGizmosExtensions
    {
        public static DebugX.DrawHandler WireArc(this DebugX.DrawHandler self, Vector3 center, Vector3 normal, Vector3 from, float angle, float radius)
        {
            return self.Gizmo(new WireArcGizmos(center, normal, from, angle, radius));
        }
        public static DebugX.DrawHandler Bounds(this DebugX.DrawHandler self, Renderer renderer)
        {
            var bounds = renderer.bounds;
            return self.WireCube(bounds.center, Quaternion.identity, bounds.size);
        }
        public static DebugX.DrawHandler Frustum(this DebugX.DrawHandler self,
            Vector3 center,
            Quaternion rotation,
            float fov,
            float farClipPlane,
            float nearClipPlane,
            float aspect)
        {
            // Строим матрицу проекции
            float tanHalfFov = Mathf.Tan(fov * 0.5f * Mathf.Deg2Rad);
            float halfHeight = nearClipPlane * tanHalfFov;
            float halfWidth = halfHeight * aspect;
            Matrix4x4 proj = Matrix4x4.Frustum(-halfWidth, halfWidth, -halfHeight, halfHeight, nearClipPlane, farClipPlane);
            Matrix4x4 invProj = proj.inverse;

            Matrix4x4 flipZ = Matrix4x4.Scale(new Vector3(1, 1, -1));
            Matrix4x4 scaleToNDC = Matrix4x4.Scale(Vector3.one * 2f);
            Matrix4x4 localToWorld = Matrix4x4.TRS(center, rotation, Vector3.one);
            Matrix4x4 finalMatrix = localToWorld * flipZ * invProj * scaleToNDC;

            self.Mesh<WireCubeMesh, GeometryUnlitMat>(finalMatrix);
            return self;
        }
        public static DebugX.DrawHandler Frustum(this DebugX.DrawHandler self, Camera camera)
        {
            Matrix4x4 viewProj = camera.projectionMatrix * camera.worldToCameraMatrix;
            Matrix4x4 invVP = viewProj.inverse;
            Matrix4x4 scale = Matrix4x4.Scale(Vector3.one * 2f);
            Matrix4x4 finalMatrix = invVP * scale;

            self.Mesh<WireCubeMesh, GeometryUnlitMat>(finalMatrix);
            return self;
        }

#if DEBUGX_ENABLE_PHYSICS3D
        public static DebugX.DrawHandler Bounds(this DebugX.DrawHandler self, Collider collider)
        {
            var bounds = collider.bounds;
            return self.WireCube(bounds.center, Quaternion.identity, bounds.size);
        }
        public static DebugX.DrawHandler Collider(this DebugX.DrawHandler self, BoxCollider collider)
        {
            var scale = collider.transform.lossyScale;
            scale.x = Mathf.Abs(scale.x);
            scale.y = Mathf.Abs(scale.y);
            scale.z = Mathf.Abs(scale.z);
            return self.WireCube(collider.transform.TransformPoint(collider.center), collider.transform.rotation, Vector3.Scale(collider.size, scale));
        }
        public static DebugX.DrawHandler Collider(this DebugX.DrawHandler self, SphereCollider collider)
        {
            var scale = collider.transform.lossyScale;
            scale.x = Mathf.Abs(scale.x);
            scale.y = Mathf.Abs(scale.y);
            scale.z = Mathf.Abs(scale.z);
            float radius = collider.radius * Mathf.Max(scale.x, Mathf.Max(scale.y, scale.z));
            return self.WireSphere(collider.transform.TransformPoint(collider.center), radius);
        }
        public static DebugX.DrawHandler Collider(this DebugX.DrawHandler self, CapsuleCollider collider)
        {
            var scale = collider.transform.lossyScale;
            scale.x = Mathf.Abs(scale.x);
            scale.y = Mathf.Abs(scale.y);
            scale.z = Mathf.Abs(scale.z);
            float radius = collider.radius * Mathf.Max(scale.x, scale.z);
            float height = Mathf.Max(collider.height * scale.y, radius * 2f);
            return self.WireCapsule(collider.transform.TransformPoint(collider.center), collider.transform.rotation, radius, height);
        }
        public static DebugX.DrawHandler Collider(this DebugX.DrawHandler self, CharacterController collider)
        {
            return self.WireCapsule(collider.transform.TransformPoint(collider.center), Quaternion.identity, collider.radius, collider.height);
        }
        public static DebugX.DrawHandler Collider(this DebugX.DrawHandler self, MeshCollider collider)
        {
            var transform = collider.transform;
            return self.WireMesh(collider.sharedMesh, transform.position, transform.rotation, transform.localScale);
        }
#endif

#if DEBUGX_ENABLE_PHYSICS2D
        public static DebugX.DrawHandler Bounds(this DebugX.DrawHandler self, Collider2D collider)
        {
            var bounds = collider.bounds;
            return self.WireCube(bounds.center, Quaternion.identity, bounds.size);
        }
        public static DebugX.DrawHandler Collider(this DebugX.DrawHandler self, BoxCollider2D collider)
        {
            return self.WireQuad(collider.transform.TransformPoint(collider.offset), collider.transform.rotation, collider.size);
        }
        public static DebugX.DrawHandler Collider(this DebugX.DrawHandler self, CircleCollider2D collider)
        {
            return self.WireCircle(collider.transform.TransformPoint(collider.offset), collider.transform.rotation, collider.radius);
        }
        public static DebugX.DrawHandler Collider(this DebugX.DrawHandler self, CapsuleCollider2D collider)
        {
            float radius = collider.size.x * 0.5f;
            float height = Mathf.Max(collider.size.y, collider.size.x);
            return self.WireCapsule(collider.transform.TransformPoint(collider.offset), collider.transform.rotation, radius, height);
        }
#endif
    }
}

namespace DCFApixels.DebugXCore
{
    using static DebugX;
    using IN = System.Runtime.CompilerServices.MethodImplAttribute;
    public readonly struct WireArcGizmos : IGizmo<WireArcGizmos>
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
            Normal = normal.SafeNormalized();
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
