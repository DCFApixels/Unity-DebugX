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
using UnityEngine.UIElements;

namespace DCFApixels
{
    public static class OtherGizmosExtensions
    {
        // 0 = start point, 1 = end point.
        private const float BONE_JOINT_POSITION = 0.35f;
        private const float BONE_RADIUS = 1f;

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

        private static float SoftSign(float a)
        {
            return a / (1 + Mathf.Abs(a));
        }
        public static DebugX.DrawHandler Bone(this DebugX.DrawHandler self, Transform start, Transform end, float radius = BONE_RADIUS)
        {
            if (start == null || end == null) { return self; }
            return Bone(self, start.position, end.position, start.rotation, radius);
        }

        public static DebugX.DrawHandler Bone(this DebugX.DrawHandler self, Vector3 start, Vector3 end, float radius = BONE_RADIUS)
        {
            return Bone(self, start, end, Quaternion.identity, radius);
        }
        public static DebugX.DrawHandler Bones(this DebugX.DrawHandler self, Transform root, float radius = BONE_RADIUS)
        {
            if (root == null) { return self; }
            self.WireDot(root.position);
            return BonesInternal(self, root, radius);
        }
        private static DebugX.DrawHandler BonesInternal(DebugX.DrawHandler self, Transform root, float radius)
        {
            for (int i = 0; i < root.childCount; i++)
            {
                Transform child = root.GetChild(i);
                self.Bone(root, child, radius);
                self.WireDot(child.position);
                BonesInternal(self, child, radius);
            }
            return self;
        }
        private static DebugX.DrawHandler Bone(DebugX.DrawHandler self, Vector3 start, Vector3 end, Quaternion rollRotation, float radius)
        {
            Vector3 delta = end - start;
            float length = delta.magnitude;
            radius = SoftSign(length * 0.06f) * radius;
            if (length <= float.Epsilon)
            {
                return self.DotDiamond(start);
            }

            //float jointRadius = radius > float.Epsilon ? radius : length * 0.12f;
            float jointRadius = radius;
            jointRadius = Mathf.Min(jointRadius, length * 0.45f);
            //float tipRadius = Mathf.Max(length * 0.002f, jointRadius * 0.02f);
            float tipRadius = 0f;

            float jointT = Mathf.Clamp01(BONE_JOINT_POSITION);
            Vector3 joint = Vector3.Lerp(start, end, jointT);

            self.Mesh<WireCubeMesh, GeometryUnlitMat>(CreateBonePartMatrix(start, joint, rollRotation, tipRadius, jointRadius));
            self.Mesh<WireCubeMesh, GeometryUnlitMat>(CreateBonePartMatrix(end, joint, rollRotation, tipRadius, jointRadius));
            return self;
        }
        private static Matrix4x4 CreateBonePartMatrix(Vector3 start, Vector3 end, Quaternion rollRotation, float startRadius, float endRadius)
        {
            Vector3 direction = end - start;
            float length = direction.magnitude;
            if (length <= float.Epsilon)
            {
                return Matrix4x4.TRS(start, Quaternion.identity, Vector3.zero);
            }
            Vector3 forward = direction / length;

            startRadius = Mathf.Max(startRadius, 0.0001f);
            endRadius = Mathf.Max(endRadius, 0.0001f);

            float ratio = endRadius / startRadius;
            float nearClipPlane = length / (ratio - 1f);
            float farClipPlane = nearClipPlane + length;

            Matrix4x4 proj = Matrix4x4.Frustum(-startRadius, startRadius, -startRadius, startRadius, nearClipPlane, farClipPlane);
            Matrix4x4 invProj = proj.inverse;

            Matrix4x4 flipZ = Matrix4x4.Scale(new Vector3(1, 1, -1));
            Matrix4x4 scaleToNDC = Matrix4x4.Scale(Vector3.one * 2f);
            Matrix4x4 localToWorld = Matrix4x4.TRS(start - forward * nearClipPlane, CreateBoneRotation(forward, rollRotation), Vector3.one);
            return localToWorld * flipZ * invProj * scaleToNDC;
        }
        private static Quaternion CreateBoneRotation(Vector3 forward, Quaternion rollRotation)
        {
            Vector3 up = Vector3.ProjectOnPlane(rollRotation * Vector3.up, forward);
            if (up.sqrMagnitude <= float.Epsilon)
            {
                up = Vector3.ProjectOnPlane(Vector3.up, forward);
            }
            if (up.sqrMagnitude <= float.Epsilon)
            {
                up = Vector3.ProjectOnPlane(Vector3.right, forward);
            }
            return Quaternion.LookRotation(forward, up.normalized).SafeQuaternion();
        }
        public static DebugX.DrawHandler Projection(this DebugX.DrawHandler self, Plane plane, Vector3 point, float circleRadius = 1f)
        {
            static float SoftSign(float a) { return a / (1f + Mathf.Abs(a)); }
            Vector3 closestPoint = plane.ClosestPointOnPlane(point);
            self.DotDiamond(point);
            self.Line(point, closestPoint);
            var dsq = (point - closestPoint).sqrMagnitude;
            float t = SoftSign(dsq * 0.06f);
            self = DebugX.Draw(self.Duration, (self.Color, 1f - t));
            self.Circle(closestPoint, plane.normal, Mathf.Lerp(circleRadius * 0.01f, circleRadius, t));
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
