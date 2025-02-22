using UnityEngine;

namespace DCFApixels
{
    public class DebugXRaycastsSample : MonoBehaviour
    {
        private const float Offaet = 30f;
        private const int Count = 4;
        private const float Radius = 6f;

        public Vector3 rotation = new Vector3(0, 45, 0);
        public Vector3 size = Vector3.one * 0.5f;
        public float time = 0f;

        private Transform[] _transforms;

        [Header("Stress Test")]
        public int textCount = 1;

        private void Start()
        {
            float addAngle = +360f / Count * Mathf.Deg2Rad;
            float angle = Offaet * Mathf.Deg2Rad;
            _transforms = new Transform[Count];
            for (int i = 0; i < Count; i++)
            {
                var transform = new GameObject().transform;
                _transforms[i] = transform;
                transform.SetParent(this.transform);

                angle += addAngle;
                transform.localPosition = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * Radius;
                transform.LookAt(this.transform.position);
            }
        }
        private void LateUpdate()
        {
#if UNITY_EDITOR
            if (FrameDebugger.enabled) { return; }
#endif
            Quaternion rotation = Quaternion.Euler(this.rotation);
            Transform transform;
            RaycastHit hit;

            for (int j = 0; j < textCount; j++)
            {
                int i = -1;

                transform = _transforms[++i];
                Physics.Raycast(transform.position, transform.forward, out hit);
                DebugX.Draw(time, Color.red).Raycast(transform.position, transform.forward, hit);

                transform = _transforms[++i];
                Physics.SphereCast(transform.position, size.x, transform.forward, out hit);
                DebugX.Draw(time, Color.yellow).SphereCast(transform.position, transform.forward, size.x, hit);

                transform = _transforms[++i];
                Physics.BoxCast(transform.position, size, transform.forward, out hit, rotation);
                DebugX.Draw(time, Color.green).BoxCast(transform.position, transform.forward, rotation, size, hit);

                transform = _transforms[++i];
                Vector3 point1, point2;
                point1 = transform.position + rotation * Vector3.down / 2f;
                point2 = transform.position + rotation * Vector3.up / 2f;

                Physics.CapsuleCast(point1, point2, size.x, transform.forward, out hit);
                DebugX.Draw(time, Color.blue).CapsuleCast(point1, point2, transform.forward, size.x, hit);
            }
        }
    }
}