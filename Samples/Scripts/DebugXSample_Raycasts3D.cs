using DCFApixels.DebugXCore.Samples.Internal;
using UnityEngine;

namespace DCFApixels.DebugXCore.Samples
{
    [SelectionBase]
    public class DebugXSample_Raycasts3D : MonoBehaviour
    {
        public Gradient Gradient;
        public float GradientMultiplier = 5;
        public Transform[] Points;
        public Transform WarrningPoint;


#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Draw();
        }
#else
        private void Update()
        {
            Draw();
        }
#endif

        private void Draw()
        {
#if DEBUGX_ENABLE_PHYSICS3D
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
#else
            DebugX.Draw(GetColor(WarrningPoint).Inverse()).Text(WarrningPoint.position, "Add \"DEBUGX_ENABLE_PHYSICS3D\" define", DebugXTextSettings.WorldSpaceScale.SetSize(22).SetAnchor(TextAnchor.MiddleCenter));
#endif
        }
        private Color GetColor(Transform pos1)
        {
            return Gradient.Evaluate(pos1, GradientMultiplier);
        }
    }
}
