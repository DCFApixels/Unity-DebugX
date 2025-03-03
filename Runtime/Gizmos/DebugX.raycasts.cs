//#undef DEBUG
using DCFApixels.DebugXCore.Internal;
using UnityEngine;

namespace DCFApixels
{
    using IN = System.Runtime.CompilerServices.MethodImplAttribute;

    public static partial class DebugX
    {
        private const float ShadowAlphaMultiplier = 0.3f;
        public readonly partial struct DrawHandler
        {
            #region RaycastHit
            [IN(LINE)]
            public DrawHandler RaycastHit(RaycastHit hit)
            {
                if (hit.collider != null)
                {
                    DotDiamond(hit.point);
                    RayArrow(hit.point, hit.normal);
                }
                return this;
            }
            [IN(LINE)]
            public DrawHandler RaycastHit(RaycastHit2D hit)
            {
                if (hit.collider != null)
                {
                    DotDiamond(hit.point);
                    RayArrow(hit.point, hit.normal);
                }
                return this;
            }
            #endregion


            #region Raycast
            [IN(LINE)] public DrawHandler Raycast(Ray ray, RaycastHit hit) => Raycast(ray.origin, ray.direction, hit);
            [IN(LINE)]
            public DrawHandler Raycast(Vector3 origin, Vector3 direction, RaycastHit hit)
            {
                if (hit.collider == null)
                {
                    RayFade(origin, direction * 3f);
                }
                else
                {
                    Line(origin, origin + direction * hit.distance);
                    RaycastHit(hit);
                }
                return this;
            }
            #endregion

            #region SphereCast
            [IN(LINE)] public DrawHandler SphereCast(Ray ray, float radius, RaycastHit hit) => SphereCast(ray.origin, ray.direction, radius, hit);
            [IN(LINE)]
            public DrawHandler SphereCast(Vector3 origin, Vector3 direction, float radius, RaycastHit hit)
            {
                WireSphere(origin, radius);
                if (hit.collider == null)
                {
                    RayFade(origin, direction * 3f);
                }
                else
                {
                    Vector3 end = origin + direction * hit.distance;

                    WidthOutLine(origin, end, radius * 2f);
                    Setup(Color.SetAlpha(ShadowAlphaMultiplier)).Line(origin, end);
                    WireSphere(end, radius);

                    RaycastHit(hit);
                }
                return this;
            }
            #endregion

            #region BoxCast
            [IN(LINE)] public DrawHandler BoxCast(Ray ray, Quaternion rotation, Vector3 size, RaycastHit hit) => BoxCast(ray.origin, ray.direction, rotation, size, hit);
            [IN(LINE)]
            public DrawHandler BoxCast(Vector3 origin, Vector3 direction, Quaternion rotation, Vector3 size, RaycastHit hit)
            {
                WireCube(origin, rotation, size * 2f);
                if (hit.collider == null)
                {
                    RayFade(origin, direction * 3f);
                }
                else
                {
                    Vector3 end = origin + direction * hit.distance;

                    WidthOutLine(origin, end, size.x * 2f);
                    Setup(Color.SetAlpha(ShadowAlphaMultiplier)).Line(origin, end);
                    WireCube(end, rotation, size * 2f);

                    RaycastHit(hit);
                }
                return this;
            }
            #endregion

            #region CapsuleCast
            [IN(LINE)]
            public DrawHandler CapsuleCast(Vector3 point1, Vector3 point2, Vector3 direction, float radius, RaycastHit hit)
            {
                Vector3 center = (point1 + point2) * 0.5f;
                Quaternion rotation = Quaternion.LookRotation(point2 - point1, Vector3.up);
                rotation = rotation * Quaternion.Euler(90, 0, 0);
                CapsuleCast(center, direction, rotation, radius, Vector3.Distance(point1, point2) + radius * 2f, hit);
                return this;
            }
            [IN(LINE)]
            public DrawHandler CapsuleCast(Ray ray, Vector3 size, Quaternion rotation, float radius, float height, RaycastHit hit)
            {
                CapsuleCast(ray.origin, ray.direction, rotation, radius, height, hit);
                return this;
            }
            [IN(LINE)]
            public DrawHandler CapsuleCast(Vector3 origin, Vector3 direction, Quaternion rotation, float radius, float height, RaycastHit hit)
            {
                WireCapsule(origin, rotation, radius, height);
                if (hit.collider == null)
                {
                    RayFade(origin, direction * 3f);
                }
                else
                {
                    Vector3 end = origin + direction * hit.distance;

                    WidthOutLine(origin, end, radius * 2f);
                    Setup(Color.SetAlpha(ShadowAlphaMultiplier)).Line(origin, end);
                    WireCapsule(end, rotation, radius, height);

                    RaycastHit(hit);
                }
                return this;
            }
            #endregion


            #region Raycast2D
            [IN(LINE)] public DrawHandler Raycast2D(Ray ray, RaycastHit2D hit) => Raycast2D(ray.origin, ray.direction, hit);
            [IN(LINE)]
            public DrawHandler Raycast2D(Vector3 origin, Vector2 direction, RaycastHit2D hit)
            {
                if (hit.collider == null)
                {
                    RayFade(origin, direction * 3f);
                }
                else
                {
                    Line(origin, origin + (Vector3)direction * hit.distance);

                    RaycastHit(hit);
                }
                return this;
            }
            #endregion

            #region CircleCast2D
            private static readonly Vector3 Normal2D = Vector3.forward;
            [IN(LINE)] public DrawHandler CircleCast2D(Ray ray, float radius, RaycastHit2D hit) => CircleCast2D(ray.origin, ray.direction, radius, hit);
            [IN(LINE)]
            public DrawHandler CircleCast2D(Vector3 origin, Vector2 direction, float radius, RaycastHit2D hit)
            {
                WireCircle(origin, Normal2D, radius);
                if (hit.collider == null)
                {
                    RayFade(origin, direction * 3f);
                }
                else
                {
                    Vector3 end = origin + (Vector3)direction * hit.distance;

                    Line(origin, end);
                    WireCircle(end, Normal2D, radius);

                    RaycastHit(hit);
                }
                return this;
            }
            #endregion

            #region BoxCast2D
            [IN(LINE)] public DrawHandler BoxCast2D(Ray ray, float angle, Vector3 size, RaycastHit2D hit) => BoxCast2D(ray.origin, ray.direction, angle, size, hit);
            [IN(LINE)]
            public DrawHandler BoxCast2D(Vector3 origin, Vector2 direction, float angle, Vector3 size, RaycastHit2D hit)
            {
                size *= 0.5f;
                Quaternion rotation = Quaternion.Euler(0, 0, angle);
                WireQuad(origin, rotation, size * 2f);
                if (hit.collider == null)
                {
                    RayFade(origin, direction * 3f);
                }
                else
                {
                    Vector3 end = origin + (Vector3)direction * hit.distance;

                    Line(origin, end);
                    WireQuad(end, rotation, size * 2f);

                    RaycastHit(hit);
                }
                return this;
            }
            #endregion

            #region CapsuleCast2D
            [IN(LINE)] public DrawHandler CapsuleCast2D(Ray ray, float angle, Vector2 size, CapsuleDirection2D capsuleDirection, RaycastHit2D hit) => CapsuleCast2D(ray.origin, ray.direction, angle, size, capsuleDirection, hit);
            [IN(LINE)]
            public DrawHandler CapsuleCast2D(Vector3 origin, Vector2 direction, float angle, Vector2 size, CapsuleDirection2D capsuleDirection, RaycastHit2D hit)
            {
                var rotation = Quaternion.Euler(0, 0, angle);
                var height = (capsuleDirection == CapsuleDirection2D.Vertical ? size.y : size.x);
                var radius = (capsuleDirection == CapsuleDirection2D.Vertical ? size.x : size.y) * 0.5f;
                WireFlatCapsule(origin, rotation, radius, height);
                if (hit.collider == null)
                {
                    RayFade(origin, direction * 3f);
                }
                else
                {
                    Vector3 end = origin + (Vector3)direction * hit.distance;

                    Line(origin, end);
                    WireFlatCapsule(end, rotation, radius, height);

                    RaycastHit(hit);
                }
                return this;
            }
            #endregion
        }
    }
}