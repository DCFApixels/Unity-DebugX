using UnityEngine;

namespace DCFApixels
{
    public class DebugXLinesSample : MonoBehaviour
    {
        [Header("Base")]
        public Transform[] startTransforms;
        public Transform[] endTransforms;
        public Gradient colors;
        public int colorStepsCount = 7;

        [Header("Draw")]
        public Vector3 rotationEuler;
        public Vector3 size = Vector3.one;
        public float time = 0;

        [Header("Animation")]
        public bool isAnimated = false;
        public float radius = 5f;
        public float speed = 30f;
        public float swithColorCooldown = 1f;
        private float _angle = 0f;
        private float _colorOffset = 0;

        [Header("Stress Test")]
        public int textCount = 1;

        private void LateUpdate()
        {
#if UNITY_EDITOR
            if (FrameDebugger.enabled) { return; }
#endif
            if (isAnimated)
            {
                _angle += speed * Time.deltaTime;

                float radians = _angle * Mathf.Deg2Rad;

                float x = Mathf.Cos(radians) * radius;
                float z = Mathf.Sin(radians) * radius;

                transform.localPosition = new Vector3(x, transform.localPosition.y, z);

                _colorOffset += Time.deltaTime * swithColorCooldown;
            }
            else
            {
                transform.localPosition = default;
                _colorOffset = 0;
            }

            Color GetColor(int i_)
            {
                return colors.Evaluate((_colorOffset + 1f * i_ / colorStepsCount) % 1f);
            }

            for (int h = 0; h < textCount; h++)
            {
                Quaternion rotation = Quaternion.Euler(rotationEuler);
                int i = -1;
                Vector3 start, end;


                const float LineMultiplier = 0.18f;
                i++;
                start = startTransforms[i].position;
                end = endTransforms[i].position;
                DebugX.Draw(time, GetColor(i)).Line(start, end);

                i++;
                start = startTransforms[i].position;
                end = endTransforms[i].position;
                DebugX.Draw(time, GetColor(i)).WidthLine(start, end, size.x * LineMultiplier);

                i++;
                start = startTransforms[i].position;
                end = endTransforms[i].position;
                DebugX.Draw(time, GetColor(i)).WidthOutLine(start, end, size.x * LineMultiplier);

                i++;
                start = startTransforms[i].position;
                end = endTransforms[i].position;
                DebugX.Draw(time, GetColor(i)).ZigzagLine(start, end, size.x * LineMultiplier);

                i++;
                i++;


                i++;
                start = startTransforms[i].position;
                end = endTransforms[i].position;
                DebugX.Draw(time, GetColor(i)).Line(start, end, DebugXLine.Arrow);

                i++;
                start = startTransforms[i].position;
                end = endTransforms[i].position;
                DebugX.Draw(time, GetColor(i)).Line(start, end, DebugXLine.Fade);

                i++;
                start = startTransforms[i].position;
                end = endTransforms[i].position;
                DebugX.Draw(time, GetColor(i)).Line(start, end, DebugXLine.Arrow, DebugXLine.Arrow);

                i++;
                start = startTransforms[i].position;
                end = endTransforms[i].position;
                DebugX.Draw(time, GetColor(i)).Line(start, end, DebugXLine.Fade, DebugXLine.Fade);

                i++;
                start = startTransforms[i].position;
                end = endTransforms[i].position;
                DebugX.Draw(time, GetColor(i)).Line(start, end, DebugXLine.Arrow, DebugXLine.Fade);
            }
        }
    }
}