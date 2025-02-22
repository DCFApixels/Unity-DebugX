using UnityEngine;

namespace DCFApixels
{
    public class DebugXSample_Primitives2D : MonoBehaviour
    {
        public Gradient Gradient;
        public float GradientMultiplier = 5;
        public Transform[] Points;

        private void OnDrawGizmos()
        {
            int i = -1;
            const float RADIUS_M = 0.5f;

            i++; DebugX.Draw(GetColor(Points[i])).Quad(Points[i].position, Points[i].rotation, Points[i].localScale);
            i++; DebugX.Draw(GetColor(Points[i])).WireQuad(Points[i].position, Points[i].rotation, Points[i].localScale);
            i++; DebugX.Draw(GetColor(Points[i])).QuadGrid(Points[i].position, Points[i].rotation, Points[i].localScale, Vector2Int.one * 3);
            i++; DebugX.Draw(GetColor(Points[i])).QuadPoints(Points[i].position, Points[i].rotation, Points[i].localScale);

            i++; DebugX.Draw(GetColor(Points[i])).Circle(Points[i].position, Points[i].rotation, Points[i].localScale.x * RADIUS_M);
            i++; DebugX.Draw(GetColor(Points[i])).WireCircle(Points[i].position, Points[i].rotation, Points[i].localScale.x * RADIUS_M);
            i++; DebugX.Draw(GetColor(Points[i])).FlatCapsule(Points[i].position, Points[i].rotation, Points[i].localScale.x * RADIUS_M, Points[i].localScale.y);
            i++; DebugX.Draw(GetColor(Points[i])).WireFlatCapsule(Points[i].position, Points[i].rotation, Points[i].localScale.x * RADIUS_M, Points[i].localScale.y);
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
