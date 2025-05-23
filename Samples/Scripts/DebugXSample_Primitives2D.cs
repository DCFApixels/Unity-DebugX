using DCFApixels.DebugXCore.Samples.Internal;
using UnityEngine;

namespace DCFApixels.DebugXCore.Samples
{
    [SelectionBase]
    public class DebugXSample_Primitives2D : MonoBehaviour
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

            i++; DebugX.Draw(GetColor(Points[i])).Quad(Points[i].position, Points[i].rotation, Points[i].localScale);
            i++; DebugX.Draw(GetColor(Points[i])).WireQuad(Points[i].position, Points[i].rotation, Points[i].localScale);
            i++; DebugX.Draw(GetColor(Points[i])).QuadGrid(Points[i].position, Points[i].rotation, Points[i].localScale, Vector2Int.one * 3);
            i++; DebugX.Draw(GetColor(Points[i])).QuadPoints(Points[i].position, Points[i].rotation, Points[i].localScale);

            i++;
            i++; DebugX.Draw(GetColor(Points[i])).Triangle(Points[i].position, Points[i].rotation, Points[i].localScale);
            i++; DebugX.Draw(GetColor(Points[i])).WireTriangle(Points[i].position, Points[i].rotation, Points[i].localScale);
            i++;

            i++; DebugX.Draw(GetColor(Points[i])).Circle(Points[i].position, Points[i].rotation, Points[i].localScale.x * RADIUS_M);
            i++; DebugX.Draw(GetColor(Points[i])).WireCircle(Points[i].position, Points[i].rotation, Points[i].localScale.x * RADIUS_M);
            i++; DebugX.Draw(GetColor(Points[i])).FlatCapsule(Points[i].position, Points[i].rotation, Points[i].localScale.x * RADIUS_M, Points[i].localScale.y);
            i++; DebugX.Draw(GetColor(Points[i])).WireFlatCapsule(Points[i].position, Points[i].rotation, Points[i].localScale.x * RADIUS_M, Points[i].localScale.y);
        }
        private Color GetColor(Transform pos1)
        {
            return Gradient.Evaluate(pos1, GradientMultiplier);
        }
    }
}
