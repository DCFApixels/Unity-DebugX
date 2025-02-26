using UnityEngine;

namespace DCFApixels.DebugXCore.Samples
{
    [SelectionBase]
    public class DebugXSample_Primitives3D : MonoBehaviour
    {
        public Gradient Gradient;
        public float GradientMultiplier = 5;
        public Transform[] Points;

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
            int i = -1;
            const float RADIUS_M = 0.5f;

            i++; DebugX.Draw(GetColor(Points[i])).Cube(Points[i].position, Points[i].rotation, Points[i].localScale);
            i++; DebugX.Draw(GetColor(Points[i])).WireCube(Points[i].position, Points[i].rotation, Points[i].localScale);
            i++; DebugX.Draw(GetColor(Points[i])).CubeGrid(Points[i].position, Points[i].rotation, Points[i].localScale, Vector3Int.one * 3);
            i++; DebugX.Draw(GetColor(Points[i])).CubePoints(Points[i].position, Points[i].rotation, Points[i].localScale);

            i++; DebugX.Draw(GetColor(Points[i])).Cylinder(Points[i].position, Points[i].rotation, Points[i].localScale.x * RADIUS_M, Points[i].localScale.y);
            i++; DebugX.Draw(GetColor(Points[i])).WireCylinder(Points[i].position, Points[i].rotation, Points[i].localScale.x * RADIUS_M, Points[i].localScale.y);
            i++; DebugX.Draw(GetColor(Points[i])).Cone(Points[i].position, Points[i].rotation, Points[i].localScale.x * RADIUS_M, Points[i].localScale.y);
            i++; DebugX.Draw(GetColor(Points[i])).WireCone(Points[i].position, Points[i].rotation, Points[i].localScale.x * RADIUS_M, Points[i].localScale.y);

            i++; DebugX.Draw(GetColor(Points[i])).Sphere(Points[i].position, Points[i].localScale.x * RADIUS_M);
            i++; DebugX.Draw(GetColor(Points[i])).WireSphere(Points[i].position, Points[i].localScale.x * RADIUS_M);
            i++; DebugX.Draw(GetColor(Points[i])).Capsule(Points[i].position, Points[i].rotation, Points[i].localScale.x * RADIUS_M, Points[i].localScale.y);
            i++; DebugX.Draw(GetColor(Points[i])).WireCapsule(Points[i].position, Points[i].rotation, Points[i].localScale.x * RADIUS_M, Points[i].localScale.y);
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
