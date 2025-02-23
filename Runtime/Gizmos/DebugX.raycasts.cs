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

                    DotDiamond(hit.point);
                    RayArrow(hit.point, hit.normal);
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

                    DotDiamond(hit.point);
                    WireSphere(end, radius);
                    RayArrow(hit.point, hit.normal);

                    Setup(Color.SetAlpha(ShadowAlphaMultiplier)).Line(origin, end);
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

                    DotDiamond(hit.point);
                    WireCube(end, rotation, size * 2f);
                    RayArrow(hit.point, hit.normal);


                    Setup(Color.SetAlpha(ShadowAlphaMultiplier)).Line(origin, end);
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

                    DotDiamond(hit.point);
                    WireCapsule(end, rotation, radius, height);
                    RayArrow(hit.point, hit.normal);


                    Setup(Color.SetAlpha(ShadowAlphaMultiplier)).Line(origin, end);
                }
                return this;
            }
            #endregion


            #region Raycast2D
            [IN(LINE)] public DrawHandler Raycast2D(Ray ray, RaycastHit2D hit) => Raycast2D(ray.origin, ray.direction, hit);
            [IN(LINE)]
            public DrawHandler Raycast2D(Vector2 origin, Vector2 direction, RaycastHit2D hit)
            {
                if (hit.collider == null)
                {
                    RayFade(origin, direction * 3f);
                }
                else
                {
                    Line(origin, origin + direction * hit.distance);

                    DotDiamond(hit.point);
                    RayArrow(hit.point, hit.normal);
                }
                return this;
            }
            #endregion

            #region CircleCast2D
            private static readonly Vector3 Normal2D = Vector3.forward;
            [IN(LINE)] public DrawHandler CircleCast2D(Ray ray, float radius, RaycastHit2D hit) => CircleCast2D(ray.origin, ray.direction, radius, hit);
            [IN(LINE)]
            public DrawHandler CircleCast2D(Vector2 origin, Vector2 direction, float radius, RaycastHit2D hit)
            {
                WireCircle(origin, Normal2D, radius);
                if (hit.collider == null)
                {
                    RayFade(origin, direction * 3f);
                }
                else
                {
                    Vector2 end = origin + direction * hit.distance;

                    //WidthOutLine(origin, end, radius * 2f);

                    DotDiamond(hit.point);
                    WireCircle(end, Normal2D, radius);
                    RayArrow(hit.point, hit.normal);

                    //Setup(Color.SetAlpha(ShadowAlphaMultiplier)).
                        Line(origin, end);
                }
                return this;
            }
            #endregion

            #region BoxCast2D
            [IN(LINE)] public DrawHandler BoxCast2D(Ray ray, float angle, Vector3 size, RaycastHit2D hit) => BoxCast2D(ray.origin, ray.direction, angle, size, hit);
            [IN(LINE)]
            public DrawHandler BoxCast2D(Vector2 origin, Vector2 direction, float angle, Vector3 size, RaycastHit2D hit)
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
                    Vector3 end = origin + direction * hit.distance;

                    //WidthOutLine(origin, end, size.x * 2f);

                    DotDiamond(hit.point);
                    WireQuad(end, rotation, size * 2f);
                    RayArrow(hit.point, hit.normal);


                    //Setup(Color.SetAlpha(ShadowAlphaMultiplier)).
                        Line(origin, end);
                }
                return this;
            }
            #endregion

            #region CapsuleCast2D
            [IN(LINE)] public DrawHandler CapsuleCast2D(Ray ray, float angle, Vector2 size, CapsuleDirection2D capsuleDirection, RaycastHit2D hit) => CapsuleCast2D(ray.origin, ray.direction, angle, size, capsuleDirection, hit);
            [IN(LINE)]
            public DrawHandler CapsuleCast2D(Vector2 origin, Vector2 direction, float angle, Vector2 size, CapsuleDirection2D capsuleDirection, RaycastHit2D hit)
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
                    Vector3 end = origin + direction * hit.distance;
            
                    //WidthOutLine(origin, end, radius * 2f);
            
                    DotDiamond(hit.point);
                    WireFlatCapsule(end, rotation, radius, height);
                    RayArrow(hit.point, hit.normal);
            
            
                    //Setup(Color.SetAlpha(ShadowAlphaMultiplier)).
                        Line(origin, end);
                }
                return this;
            }
            #endregion
        }
    }
}