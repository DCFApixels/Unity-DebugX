using UnityEngine;

namespace DCFApixels.DebugXCore.Samples
{
    [SelectionBase]
    public class DebugXSample_Raycasts2D : MonoBehaviour
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
#if DEBUGX_ENABLE_PHYSICS2D
            int i = 0;
            const float RADIUS_M = 0.5f;

            Transform point;
            Ray ray;
            RaycastHit2D hit;

            point = Points[i++];
            ray = new Ray(point.position, point.forward);
            hit = Physics2D.Raycast(ray.origin, ray.direction, float.PositiveInfinity, -1);
            DebugX.Draw(GetColor(point)).Raycast2D(ray, hit);

            point = Points[i++];
            ray = new Ray(point.position, point.forward);
            hit = Physics2D.CircleCast(ray.origin, point.localScale.x * RADIUS_M, ray.direction, float.PositiveInfinity, int.MaxValue);
            DebugX.Draw(GetColor(point)).CircleCast2D(ray, point.localScale.x * RADIUS_M, hit);

            point = Points[i++];
            ray = new Ray(point.position, point.forward);
            hit = Physics2D.BoxCast(ray.origin, point.localScale, point.eulerAngles.z, ray.direction, float.PositiveInfinity, int.MaxValue);
            DebugX.Draw(GetColor(point)).BoxCast2D(ray, point.eulerAngles.z, point.localScale, hit);

            point = Points[i++];
            ray = new Ray(point.position, point.forward);
            hit = Physics2D.CapsuleCast(ray.origin, point.localScale, CapsuleDirection2D.Vertical, point.eulerAngles.z, ray.direction, float.PositiveInfinity, int.MaxValue);
            DebugX.Draw(GetColor(point)).CapsuleCast2D(ray, point.eulerAngles.z, point.localScale, CapsuleDirection2D.Vertical, hit);
#else
            DebugX.Draw(Inverse(GetColor(WarrningPoint))).Text(WarrningPoint.position, "Add \"DEBUGX_ENABLE_PHYSICS2D\" define", DebugXTextSettings.WorldSpaceScale.SetSize(22).SetAnchor(TextAnchor.MiddleCenter));
#endif
        }
        private Color GetColor(Transform pos1)
        {
            Vector3 pos = pos1.localPosition;
            pos /= GradientMultiplier == 0 ? 1 : GradientMultiplier;
            pos += Vector3.one * 0.5f;
            float t = pos.x + pos.z;
            t /= 2f;
            return Gradient.Evaluate(Mathf.Clamp01(t));
        }
        private Color Inverse(Color c)
        {
            var a = c.a;
            c = Color.white - c;
            c.a = a;
            return c;
        }
    }
}
