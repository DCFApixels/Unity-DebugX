//#undef DEBUG
using DCFApixels.DebugXCore;
using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace DCFApixels
{
    using IN = System.Runtime.CompilerServices.MethodImplAttribute;

    public unsafe static partial class DebugX
    {
        public readonly partial struct DrawHandler
        {
            #region BillboardCircle
            [IN(LINE)] public DrawHandler BillboardCircle(Vector3 position, float radius) => Mesh<CircleMesh, BillboardMat>(position, Quaternion.identity, new Vector3(radius, radius, radius));
            #endregion

            #region Cross
            [IN(LINE)] public DrawHandler Cross(Vector3 position, float size) => Mesh<DotCrossMesh, BillboardMat>(position, Quaternion.identity, new Vector3(size, size, size));
            #endregion

            #region DotCross
            [IN(LINE)] public DrawHandler DotCross(Vector3 position) => Mesh<DotCrossMesh, DotMat>(position, Quaternion.identity, new Vector3(0.06f, 0.06f, 0.06f));
            #endregion


            #region Sphere
            [IN(LINE)] public DrawHandler Sphere(Vector3 position, float radius) => Mesh<SphereMesh, LitMat>(position, Quaternion.identity, new Vector3(radius, radius, radius));
            #endregion

            #region WireSphere
            [IN(LINE)]
            public DrawHandler WireSphere(Vector3 position, float radius)
            {
                Mesh<WireSphereMesh, UnlitMat>(position, Quaternion.identity, new Vector3(radius, radius, radius));
                Gizmo(new WireSphereGizmo(position, radius));
                return this;
            }
            private readonly struct WireSphereGizmo : IGizmo<WireSphereGizmo>
            {
                public readonly Vector3 Position;
                public readonly float Radius;
                [IN(LINE)]
                public WireSphereGizmo(Vector3 position, float radius)
                {
                    Position = position;
                    Radius = radius;
                }
                public IGizmoRenderer<WireSphereGizmo> RegisterNewRenderer() { return new Renderer(); }

                #region Renderer
                private class Renderer : InstancingMeshRendererBase, IGizmoRenderer<WireSphereGizmo>
                {
                    private Gizmo<InstancingMeshGizmoLayout>[] _buffer = Array.Empty<Gizmo<InstancingMeshGizmoLayout>>();
                    public Renderer() : base(default(WireCircleMesh), default(UnlitMat)) { }
                    public override bool IsStaticRender => false;
                    public void Prepare(Camera camera, GizmosList<WireSphereGizmo> list)
                    {
                        if (camera == null) { return; }

                        if (_buffer.Length < list.Count)
                        {
                            _buffer = new Gizmo<InstancingMeshGizmoLayout>[list.Items.Length];
                        }

                        if (camera.orthographic)
                        {
                            Quaternion cameraRotation = camera.transform.rotation;
                            for (int i = 0; i < list.Count; i++)
                            {
                                ref readonly var item = ref list[i];
                                _buffer[i] = new Gizmo<InstancingMeshGizmoLayout>(
                                    new InstancingMeshGizmoLayout(item.Value.Position, cameraRotation, new Vector3(item.Value.Radius, item.Value.Radius, item.Value.Radius)),
                                    item.Color, IMMEDIATE_DURATION);
                            }
                        }
                        else
                        {
                            Vector3 cameraPosition = camera.transform.position;
                            for (int i = 0; i < list.Count; i++)
                            {
                                ref readonly var item = ref list[i];
                                Vector3 vector = item.Value.Position - Matrix4x4.Inverse(Matrix4x4.identity).MultiplyPoint(cameraPosition);
                                float sqrMagnitude = vector.sqrMagnitude;
                                float sqrRadius = item.Value.Radius * item.Value.Radius;
                                float x = sqrRadius * sqrRadius / sqrMagnitude;
                                if (x / sqrRadius < 1f)
                                {
                                    float resultSize = FastSqrt(sqrRadius - x);

                                    _buffer[i] = new Gizmo<InstancingMeshGizmoLayout>(
                                        new InstancingMeshGizmoLayout(
                                            item.Value.Position - sqrRadius * vector / sqrMagnitude,
                                            Quaternion.LookRotation((cameraPosition - item.Value.Position).normalized),
                                            new Vector3(resultSize, resultSize, resultSize)),
                                        item.Color, IMMEDIATE_DURATION);
                                }
                            }
                        }

                        Prepare(new GizmosList<InstancingMeshGizmoLayout>(_buffer, list.Count));
                    }
                    public void Render(Camera camera, GizmosList<WireSphereGizmo> list, CommandBuffer cb)
                    {
                        if (camera == null) { return; }

                        Render(cb);
                    }
                }
                #endregion
            }
            #endregion

            #region Circle
            [IN(LINE)] public DrawHandler Circle(Vector3 position, Vector3 normal, float radius) => Mesh<CircleMesh, LitMat>(position, Quaternion.LookRotation(normal), new Vector3(radius, radius, radius));
            [IN(LINE)] public DrawHandler Circle(Vector3 position, Quaternion rotation, float radius) => Mesh<CircleMesh, LitMat>(position, rotation, new Vector3(radius, radius, radius));
            #endregion

            #region WireCircle
            [IN(LINE)] public DrawHandler WireCircle(Vector3 position, Vector3 normal, float radius) => WireCircle(position, Quaternion.LookRotation(normal), radius);
            [IN(LINE)] public DrawHandler WireCircle(Vector3 position, Quaternion rotation, float radius) => Mesh<WireCircleMesh, UnlitMat>(position, rotation, new Vector3(radius, radius, radius));
            #endregion

            #region Dot
            [IN(LINE)] public DrawHandler Dot(Vector3 position) => Mesh<DotMesh, DotMat>(position, Quaternion.identity, new Vector3(DOT_RADIUS, DOT_RADIUS, DOT_RADIUS));
            #endregion

            #region WireDot
            [IN(LINE)] public DrawHandler WireDot(Vector3 position) => Mesh<WireCircleMesh, DotMat>(position, Quaternion.identity, new Vector3(DOT_RADIUS / 2, DOT_RADIUS / 2, DOT_RADIUS / 2));
            #endregion

            #region Capsule
            [IN(LINE)]
            public DrawHandler Capsule(Vector3 position, Quaternion rotation, float radius, float height)
            {
                radius = Mathf.Max(0, radius);
                height -= radius * 2f;
                height = Mathf.Max(0, height);
                var halfHeigth = height * 0.5f;
                var normal = rotation * Vector3.up;

                Mesh<CapsuleHeadMesh, LitMat>(position + normal * halfHeigth, rotation, new Vector3(radius, radius, radius));
                Mesh<CapsuleBodyMesh, LitMat>(position, rotation, new Vector3(radius, height, radius));
                Mesh<CapsuleHeadMesh, LitMat>(position - normal * halfHeigth, rotation * Quaternion.Euler(0, 0, 180), new Vector3(radius, radius, radius));
                return this;
            }
            #endregion

            #region WireCapsule
            public DrawHandler WireCapsule(Vector3 point1, Vector3 point2, float radius)
            {
                //TODO посмотреть поворот
                return WireCapsule((point1 + point2) * 0.5f, Quaternion.LookRotation(point2 - point1), radius, Vector3.Distance(point1, point2));
            }
            public DrawHandler WireCapsule(Vector3 position, Quaternion rotation, float radius, float height)
            {
                WireFlatCapsule(position, rotation, radius, height);
                WireFlatCapsule(position, rotation * Quaternion.Euler(0, 90, 0), radius, height);

                radius = Mathf.Max(0, radius);
                height -= radius * 2f;
                height = Mathf.Max(0, height);
                var halfHeigth = height * 0.5f;
                var normalUp = rotation * Vector3.up;

                var center = position;

                Vector3 Start, End;
                Start = center - normalUp * halfHeigth;
                End = center + normalUp * halfHeigth;

                WireCircle(Start, rotation * Quaternion.Euler(90, 0, 0), radius);
                WireCircle(End, rotation * Quaternion.Euler(90, 0, 0), radius);
                return this;
            }
            #endregion

            #region FlatCapsule
            [IN(LINE)]
            public DrawHandler FlatCapsule(Vector3 position, Quaternion rotation, float radius, float height)
            {
                radius = Mathf.Max(0, radius);
                height -= radius * 2f;
                height = Mathf.Max(0, height);
                var halfHeigth = height * 0.5f;
                var normal = rotation * Vector3.up;

                Mesh<FlatCapsuleHeadMesh, LitMat>(position + normal * halfHeigth, rotation, new Vector3(radius, radius, radius));
                Mesh<FlatCapsuleBodyMesh, LitMat>(position, rotation, new Vector3(radius, height, radius));
                Mesh<FlatCapsuleHeadMesh, LitMat>(position - normal * halfHeigth, rotation * Quaternion.Euler(0, 0, 180), new Vector3(radius, radius, radius));
                return this;
            }
            #endregion

            #region WireFlatCapsule
            private static readonly Quaternion Rot180 = Quaternion.Euler(0, 0, 180);
            [IN(LINE)]
            public DrawHandler WireFlatCapsule(Vector3 position, Quaternion rotation, float radius, float height)
            {
                height -= radius * 2f;

                var normalForward = rotation * Vector3.forward;
                var normalUp = rotation * Vector3.up;
                var halfHeigth = height * 0.5f;

                Vector3 from = Vector3.Cross(normalForward, normalUp).normalized;
                Vector3 start = position - normalUp * halfHeigth;
                Vector3 end = position + normalUp * halfHeigth;

                Mesh<WireArcMesh, UnlitMat>(end, rotation, new Vector3(radius, radius, radius));
                Mesh<WireArcMesh, UnlitMat>(start, rotation * Rot180, new Vector3(radius, radius, radius));

                Vector3 perpendicular = from * radius;

                Vector3* lines = stackalloc Vector3[]
                {
                    start - perpendicular,
                    end - perpendicular,
                    start + perpendicular,
                    end + perpendicular,
                };

                Line(lines[0], lines[1]);
                Line(lines[2], lines[3]);

                return this;
            }
            #endregion

            #region Cube
            //[IN(LINE)] public void Cube(Vector3 position, float size) => Cube(position, Quaternion.identity, new Vector3(size, size, size));
            //[IN(LINE)] public void Cube(Vector3 position, Vector3 size) => Cube(position, Quaternion.identity, size);
            [IN(LINE)] public DrawHandler Cube(Vector3 position, Quaternion rotation, float size) => Mesh<CubeMesh, LitMat>(position, rotation, new Vector3(size, size, size));
            [IN(LINE)] public DrawHandler Cube(Vector3 position, Quaternion rotation, Vector3 size) => Mesh<CubeMesh, LitMat>(position, rotation, size);
            #endregion

            #region WireCube
            //[IN(LINE)] public void WireCube(Vector3 position, float size) => WireCube(position, Quaternion.identity, new Vector3(size, size, size));
            //[IN(LINE)] public void WireCube(Vector3 position, Vector3 size) => WireCube(position, Quaternion.identity, size);
            [IN(LINE)] public DrawHandler WireCube(Vector3 position, Quaternion rotation, float size) => WireCube(position, rotation, new Vector3(size, size, size));
            [IN(LINE)] public DrawHandler WireCube(Vector3 position, Quaternion rotation, Vector3 size) => Mesh<WireCubeMesh, UnlitMat>(position, rotation, size);
            #endregion

            #region CubePoints
            [IN(LINE)] public DrawHandler CubePoints(Vector3 position, Quaternion rotation, float size) => CubePoints(position, rotation, new Vector3(size, size, size));
            [IN(LINE)]
            public DrawHandler CubePoints(Vector3 position, Quaternion rotation, Vector3 size)
            {
                Vector3 halfSize = size / 2f;

                Vector3* vertices = stackalloc Vector3[]
                {
                    new Vector3(-halfSize.x, -halfSize.y, -halfSize.z), // 0
                    new Vector3(halfSize.x, -halfSize.y, -halfSize.z),  // 1
                    new Vector3(halfSize.x, -halfSize.y, halfSize.z),   // 2
                    new Vector3(-halfSize.x, -halfSize.y, halfSize.z),  // 3
                    new Vector3(-halfSize.x, halfSize.y, -halfSize.z),  // 4
                    new Vector3(halfSize.x, halfSize.y, -halfSize.z),   // 5
                    new Vector3(halfSize.x, halfSize.y, halfSize.z),    // 6
                    new Vector3(-halfSize.x, halfSize.y, halfSize.z),   // 7
                };

                for (int i = 0; i < 8; i++)
                {
                    Dot(rotation * vertices[i] + position);
                }

                return this;
            }
            #endregion

            #region CubeGrid
            [IN(LINE)] public DrawHandler CubeGrid(Vector3 position, Quaternion rotation, float size, Vector3Int cells) => CubeGrid(position, rotation, new Vector3(size, size, size), cells);
            [IN(LINE)]
            public unsafe DrawHandler CubeGrid(Vector3 position, Quaternion rotation, Vector3 size, Vector3Int cells)
            {
                Vector3 halfSize = size / 2f;

                Vector3* vertices = stackalloc Vector3[]
                {
                    new Vector3(-halfSize.x, -halfSize.y, -halfSize.z), // 0
                    new Vector3(halfSize.x, -halfSize.y, -halfSize.z),  // 1
                    new Vector3(halfSize.x, -halfSize.y, halfSize.z),   // 2
                    new Vector3(-halfSize.x, -halfSize.y, halfSize.z),  // 3
                    new Vector3(-halfSize.x, halfSize.y, -halfSize.z),  // 4
                    new Vector3(halfSize.x, halfSize.y, -halfSize.z),   // 5
                    new Vector3(halfSize.x, halfSize.y, halfSize.z),    // 6
                    new Vector3(-halfSize.x, halfSize.y, halfSize.z),   // 7
                };

                for (int i = 0; i < 8; i++)
                {
                    vertices[i] = rotation * vertices[i] + position;
                }

                Vector3 up = rotation * Vector3.up * (size.y / cells.y);
                for (int i = 0; i <= cells.y; i++)
                {
                    Vector3 pos = up * i;
                    Line(vertices[0] + pos, vertices[1] + pos);
                    Line(vertices[1] + pos, vertices[2] + pos);
                    Line(vertices[2] + pos, vertices[3] + pos);
                    Line(vertices[3] + pos, vertices[0] + pos);
                }
                Vector3 right = rotation * Vector3.right * (size.x / cells.x);
                for (int i = 0; i <= cells.x; i++)
                {
                    Vector3 pos = right * i;
                    Line(vertices[0] + pos, vertices[3] + pos);
                    Line(vertices[3] + pos, vertices[7] + pos);
                    Line(vertices[4] + pos, vertices[0] + pos);
                    Line(vertices[7] + pos, vertices[4] + pos);
                }
                Vector3 forward = rotation * Vector3.forward * (size.z / cells.z);
                for (int i = 0; i <= cells.z; i++)
                {
                    Vector3 pos = forward * i;
                    Line(vertices[4] + pos, vertices[5] + pos);
                    Line(vertices[5] + pos, vertices[1] + pos);
                    Line(vertices[1] + pos, vertices[0] + pos);
                    Line(vertices[0] + pos, vertices[4] + pos);
                }

                return this;
            }
            #endregion

            #region Quad
            //[IN(LINE)] public DrawHandler Quad(Vector3 position, Vector3 normal, float size) => Mesh(Meshes.Quad, position, Quaternion.LookRotation(normal), new Vector3(size, size, size));
            //[IN(LINE)] public DrawHandler Quad(Vector3 position, Vector3 normal, Vector2 size) => Mesh(Meshes.Quad, position, Quaternion.LookRotation(normal), size); 
            [IN(LINE)] public DrawHandler Quad(Vector3 position, Quaternion rotation, float size) => Mesh<QuadMesh, LitMat>(position, rotation, new Vector3(size, size, size));
            [IN(LINE)] public DrawHandler Quad(Vector3 position, Quaternion rotation, Vector2 size) => Mesh<QuadMesh, LitMat>(position, rotation, size);
            #endregion

            #region WireQuad
            //[IN(LINE)] public DrawHandler WireQuad(Vector3 position, Vector3 normal, float size) => WireQuad(position, Quaternion.LookRotation(normal), new Vector2(size, size));
            //[IN(LINE)] public DrawHandler WireQuad(Vector3 position, Vector3 normal, Vector2 size) => WireQuad(position, Quaternion.LookRotation(normal), size);
            [IN(LINE)] public DrawHandler WireQuad(Vector3 position, Quaternion rotation, float size) => WireQuad(position, rotation, new Vector2(size, size));
            [IN(LINE)] public DrawHandler WireQuad(Vector3 position, Quaternion rotation, Vector2 size) => Mesh<WireCubeMesh, UnlitMat>(position, rotation, size);
            #endregion

            #region QuadPoints
            [IN(LINE)] public DrawHandler QuadPoints(Vector3 position, Quaternion rotation, float size) => QuadPoints(position, rotation, new Vector2(size, size));
            [IN(LINE)]
            public DrawHandler QuadPoints(Vector3 position, Quaternion rotation, Vector2 size)
            {
                const int PointsCount = 4;
                Vector2 halfSize = size * 0.5f;
                Vector3* vertices = stackalloc Vector3[PointsCount]
                {
                    new Vector3(-halfSize.x, -halfSize.y, 0), // 0
                    new Vector3(halfSize.x, -halfSize.y, 0f), // 1
                    new Vector3(halfSize.x, halfSize.y, 0f),  // 2
                    new Vector3(-halfSize.x, halfSize.y, 0f), // 3
                };

                for (int i = 0; i < PointsCount; i++)
                {
                    Dot(rotation * vertices[i] + position);
                }

                return this;
            }
            #endregion

            #region QuadGrid
            [IN(LINE)] public DrawHandler QuadGrid(Vector3 position, Quaternion rotation, float size, Vector2Int сells) => QuadGrid(position, rotation, new Vector2(size, size), сells);
            [IN(LINE)]
            public DrawHandler QuadGrid(Vector3 position, Quaternion rotation, Vector2 size, Vector2Int сells)
            {
                Vector2 halfSize = size * 0.5f;
                Vector3 Start, End;

                Start = rotation * new Vector3(-halfSize.x, -halfSize.y, 0) + position;
                End = rotation * new Vector3(-halfSize.x, halfSize.y, 0f) + position;
                Vector3 right = rotation * (Vector3.right * size / сells.x);
                for (int i = 0; i <= сells.x; i++)
                {
                    Line(Start, End);
                    Start += right;
                    End += right;
                }

                Start = rotation * new Vector3(-halfSize.x, -halfSize.y, 0) + position;
                End = rotation * new Vector3(halfSize.x, -halfSize.y, 0f) + position;
                Vector3 up = rotation * (Vector3.up * size / сells.y);
                for (int i = 0; i <= сells.y; i++)
                {
                    Line(Start, End);
                    Start += up;
                    End += up;
                }

                return this;
            }
            #endregion

            #region DotQuad
            [IN(LINE)] public DrawHandler DotQuad(Vector3 position) => Mesh<DotQuadMesh, DotMat>(position, Quaternion.identity, new Vector3(DOT_RADIUS, DOT_RADIUS, DOT_RADIUS));
            #endregion

            #region DotDiamond
            [IN(LINE)] public DrawHandler DotDiamond(Vector3 position) => Mesh<DotDiamondMesh, DotMat>(position, Quaternion.identity, new Vector3(DOT_RADIUS * 0.9f, DOT_RADIUS * 0.9f, DOT_RADIUS * 0.9f));
            #endregion
        }
    }
}