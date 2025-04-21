using DCFApixels.DebugXCore.Samples.Internal;
using UnityEngine;

namespace DCFApixels.DebugXCore.Samples
{
    [SelectionBase]
    public class DebugXSample_Lines : MonoBehaviour
    {
        public Gradient Gradient;
        public float GradientMultiplier = 5;
        public float WidthMultiplier = 1f;
        public Transform[] StartLines;
        public Transform[] EndLines;

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

            i++; DebugX.Draw(GetColor(StartLines[i], EndLines[i])).Line(StartLines[i].position, EndLines[i].position);
            i++; DebugX.Draw(GetColor(StartLines[i], EndLines[i])).WidthLine(StartLines[i].position, EndLines[i].position, 0.2f * WidthMultiplier);
            i++; DebugX.Draw(GetColor(StartLines[i], EndLines[i])).WidthOutLine(StartLines[i].position, EndLines[i].position, 0.2f * WidthMultiplier);
            i++; DebugX.Draw(GetColor(StartLines[i], EndLines[i])).ZigzagLine(StartLines[i].position, EndLines[i].position, 0.2f * WidthMultiplier);

            i++; DebugX.Draw(GetColor(StartLines[i], EndLines[i])).LineFade(StartLines[i].position, EndLines[i].position);
            i++; DebugX.Draw(GetColor(StartLines[i], EndLines[i])).LineArrow(StartLines[i].position, EndLines[i].position);
            i++; DebugX.Draw(GetColor(StartLines[i], EndLines[i])).Line(StartLines[i].position, EndLines[i].position, DebugXLine.Arrow, DebugXLine.Arrow);
            i++; DebugX.Draw(GetColor(StartLines[i], EndLines[i])).Line(StartLines[i].position, EndLines[i].position, DebugXLine.Arrow, DebugXLine.Fade);
            i++; DebugX.Draw(GetColor(StartLines[i], EndLines[i])).Line(StartLines[i].position, EndLines[i].position, DebugXLine.Fade, DebugXLine.Fade);
        }
        private Color GetColor(Transform pos1, Transform pos2)
        {
            return Gradient.Evaluate(pos1, pos2, GradientMultiplier);
        }
    }
}
