using UnityEngine;

namespace DCFApixels
{
    public class DebugXSample_Raycasts : MonoBehaviour
    {
        public Gradient Gradient;
        public float GradientMultiplier = 5;
        public Transform[] Points;

        private void OnDrawGizmos()
        {
            int i = 0;
            const float RADIUS_M = 0.5f;

            Transform point;
            Ray ray;
            RaycastHit hit;

            point = Points[i++];
            ray = new Ray(point.position, point.forward);
            Physics.Raycast(ray, out hit, float.PositiveInfinity, int.MaxValue, QueryTriggerInteraction.UseGlobal);
            DebugX.Draw(GetColor(point)).Raycast(ray, hit);

            point = Points[i++];
            ray = new Ray(point.position, point.forward);
            Physics.SphereCast(ray.origin, point.localScale.x * RADIUS_M, ray.direction, out hit, float.PositiveInfinity, int.MaxValue, QueryTriggerInteraction.UseGlobal);
            DebugX.Draw(GetColor(point)).SphereCast(ray, point.localScale.x * RADIUS_M, hit);

            point = Points[i++];
            ray = new Ray(point.position, point.forward);
            Physics.BoxCast(ray.origin, point.localScale * RADIUS_M, ray.direction, out hit, point.rotation, float.PositiveInfinity, int.MaxValue, QueryTriggerInteraction.UseGlobal);
            DebugX.Draw(GetColor(point)).BoxCast(ray, point.rotation, point.localScale * RADIUS_M, hit);

            point = Points[i++];
            ray = new Ray(point.position, point.forward);
            Vector3 point1 = ray.origin + point.up * point.localScale.y * RADIUS_M * 0.5f;
            Vector3 point2 = ray.origin + point.up * point.localScale.y * RADIUS_M * -0.5f;
            Physics.CapsuleCast(point1, point2, point.localScale.x * RADIUS_M, ray.direction, out hit, float.PositiveInfinity, int.MaxValue, QueryTriggerInteraction.UseGlobal);
            DebugX.Draw(GetColor(point)).CapsuleCast(point1, point2, ray.direction, point.localScale.x * RADIUS_M, hit);
        }
        private Color GetColor(Transform pos1)
        {
            Vector3 pos = pos1.localPosition;
            pos /= GradientMultiplier == 0 ? 1 : GradientMultiplier;
            pos += Vector3.one * 0.5f;
            float t = pos.x + pos.y + pos.z;
            t /= 3f;
            return Gradient.Evaluate(Mathf.Clamp01(t));
        }
    }
}
