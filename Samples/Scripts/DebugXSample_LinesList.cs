using System;
using UnityEngine;

namespace DCFApixels.DebugXCore.Samples
{
    [SelectionBase]
    public class DebugXSample_LinesList : MonoBehaviour
    {
        public Color Color;
        public Transform[] Points;
        public bool IsStrip;

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

        private Vector3[] _convertedPoints = new Vector3[32];
        private void Draw()
        {
            if(Points.Length > _convertedPoints.Length)
            {
                _convertedPoints = new Vector3[Points.Length << 1];
            }
            for (int i = 0; i < Points.Length; i++)
            {
                _convertedPoints[i] = Points[i] == null ? Vector3.zero : Points[i].position;
            }
            if (IsStrip)
            {
                DebugX.Draw(Color).LineStrip(_convertedPoints, Points.Length);
            }
            else
            {
                DebugX.Draw(Color).Lines(_convertedPoints, Points.Length);
            }
        }
    }
}
