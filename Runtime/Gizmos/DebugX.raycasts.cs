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

            #region Spherecast
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

            #region Spherecast
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

            #region Spherecast
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
        }
    }
}