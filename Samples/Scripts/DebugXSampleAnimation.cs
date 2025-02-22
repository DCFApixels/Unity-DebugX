using UnityEngine;

namespace DCFApixels
{
    public class DebugXSampleAnimation : MonoBehaviour
    {
        [Header("Animation")]
        public bool isAnimated = false;
        public float radius = 5f;
        public float moveSpeed = 30f;
        public float rotateSpeed = 30f;
        private float _angle = 0f;


        private void Update()
        {
            if (isAnimated)
            {
                _angle += moveSpeed * Time.deltaTime;

                float radians = _angle * Mathf.Deg2Rad;

                float x = Mathf.Cos(radians) * radius;
                float z = Mathf.Sin(radians) * radius;

                transform.localPosition = new Vector3(x, transform.localPosition.y, z);

                transform.Rotate(new Vector3(0, rotateSpeed * Time.deltaTime, 0));
            }
            else
            {
                transform.localPosition = default;
            }
        }
    }
}