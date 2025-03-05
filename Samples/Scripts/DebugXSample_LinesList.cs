using UnityEngine;

namespace DCFApixels.DebugXCore.Samples
{
    [SelectionBase]
    public class DebugXSample_LinesList : MonoBehaviour
    {
        public Color Color;
        public Vector3[] Points = new Vector3[32];
        public float Frequency = 1;
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

        private void Draw()
        {
            if (Points == null) { return; }
            float sizeX = transform.localScale.x;
            float sizeY = transform.localScale.y * 0.5f;

            float start = transform.position.x - sizeX * 0.5f;
            float step = sizeX / Points.Length;
            for (int i = 0; i < Points.Length; i++)
            {
                float x = step * i;
                float y = Mathf.Sin(x * Frequency) * sizeY;
                Points[i] = new Vector3(start + x, transform.position.y + y, transform.position.z);
            }
            if (IsStrip)
            {
                DebugX.Draw(Color).LineStrip(Points, 0, Points.Length);
            }
            else
            {
                DebugX.Draw(Color).Lines(Points, 0, Points.Length);
            }
        }
    }
}
