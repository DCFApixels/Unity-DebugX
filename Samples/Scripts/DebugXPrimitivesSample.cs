using DCFApixels.DebugXCore;
using UnityEngine;

namespace DCFApixels
{
    public class DebugXPrimitivesSample : MonoBehaviour
    {
        [Header("Base")]
        public Transform[] transforms;
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


        private void Update()
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

                i++; DebugX.Draw(time, GetColor(i)).Sphere(transforms[i].position, size.x * 0.5f);
                i++; DebugX.Draw(time, GetColor(i)).WireSphere(transforms[i].position, size.x * 0.5f);
                i++; DebugX.Draw(time, GetColor(i)).Circle(transforms[i].position, rotation, size.x * 0.5f);
                i++; DebugX.Draw(time, GetColor(i)).WireCircle(transforms[i].position, rotation, size.x * 0.5f);

                i++; DebugX.Draw(time, GetColor(i)).BillboardCircle(transforms[i].position, size.x * 0.5f);


                i++; DebugX.Draw(time, GetColor(i)).Cube(transforms[i].position, rotation, size);
                i++; DebugX.Draw(time, GetColor(i)).WireCube(transforms[i].position, rotation, size);
                i++; DebugX.Draw(time, GetColor(i)).Quad(transforms[i].position, rotation, size);
                i++; DebugX.Draw(time, GetColor(i)).WireQuad(transforms[i].position, rotation, size);

                i++; DebugX.Draw(time, GetColor(i)).Cross(transforms[i].position, size.x);


                i++; DebugX.Draw(time, GetColor(i)).CubePoints(transforms[i].position, rotation, size);
                i++; DebugX.Draw(time, GetColor(i)).CubeGrid(transforms[i].position, rotation, size, new Vector3Int(2, 3, 4));
                i++; DebugX.Draw(time, GetColor(i)).QuadPoints(transforms[i].position, rotation, size);
                i++; DebugX.Draw(time, GetColor(i)).QuadGrid(transforms[i].position, rotation, size, new Vector2Int(2, 3));

                i++; DebugX.Draw(time, GetColor(i)).Mesh<SphereMesh, WireMat>(transforms[i].position, Quaternion.identity, new Vector3(size.x, size.x, size.x) * 0.5f);


                i++; DebugX.Draw(time, GetColor(i)).Capsule(transforms[i].position, rotation, size.x / 2f, size.y);
                i++; DebugX.Draw(time, GetColor(i)).WireCapsule(transforms[i].position, rotation, size.x / 2f, size.y);
                i++; DebugX.Draw(time, GetColor(i)).FlatCapsule(transforms[i].position, rotation, size.x / 2f, size.y);
                i++; DebugX.Draw(time, GetColor(i)).WireFlatCapsule(transforms[i].position, rotation, size.x / 2f, size.y);

                i++; DebugX.Draw(time, GetColor(i)).Text(transforms[i].position, "Hello World");


                i++; DebugX.Draw(time, GetColor(i)).Dot(transforms[i].position);
                i++; DebugX.Draw(time, GetColor(i)).WireDot(transforms[i].position);
                i++; DebugX.Draw(time, GetColor(i)).DotCross(transforms[i].position);
                i++; DebugX.Draw(time, GetColor(i)).DotQuad(transforms[i].position);
                i++; DebugX.Draw(time, GetColor(i)).DotDiamond(transforms[i].position);
            }
        }
    }
}