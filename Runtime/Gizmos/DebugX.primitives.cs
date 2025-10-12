#if DISABLE_DEBUG
#undef DEBUG
#endif
using DCFApixels.DebugXCore;
using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace DCFApixels
{
    using static DebugXConsts;
    using IN = System.Runtime.CompilerServices.MethodImplAttribute;
    public unsafe static partial class DebugX
    {
        public readonly partial struct DrawHandler
        {
            #region BillboardCircle
            [IN(LINE)]
            public DrawHandler BillboardCircle(Vector3 position, float radius)
            {
                return Mesh<CircleMesh, BillboardMat>(position, Quaternion.identity, new Vector3(radius, radius, radius));
            }
            #endregion

            #region BillboardCross
            [IN(LINE)]
            [Obsolete("Use BillboardCross(position, size)")]
            public DrawHandler Cross(Vector3 position, float size)
            {
                return Mesh<DotCrossMesh, BillboardMat>(position, Quaternion.identity, new Vector3(size, size, size));
            }
            [IN(LINE)]
            public DrawHandler BillboardCross(Vector3 position, float size)
            {
                return Mesh<DotCrossMesh, BillboardMat>(position, Quaternion.identity, new Vector3(size, size, size));
            }
            #endregion


            #region Sphere
            [IN(LINE)]
            public DrawHandler Sphere<TMat>(Vector3 position, float radius)
                where TMat : struct, IStaticMaterial
            {
                return Mesh<SphereMesh, TMat>(position, Quaternion.identity, new Vector3(radius, radius, radius));
            }
            [IN(LINE)]
            public DrawHandler Sphere(Vector3 position, float radius)
            {
                return Mesh<SphereMesh>(position, Quaternion.identity, new Vector3(radius, radius, radius));
            }
            #endregion

            #region WireSphere
            [IN(LINE)]
            public DrawHandler WireSphere(Vector3 position, float radius)
            {
                Mesh<WireSphereMesh, GeometryUnlitMat>(position, Quaternion.identity, new Vector3(radius, radius, radius));
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
                    public Renderer() : base(default(WireCircleMesh), default(GeometryUnlitMat)) { }
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
                                    float resultSize = DebugXUtility.FastSqrt(sqrRadius - x);

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
            [IN(LINE)]
            public DrawHandler Circle<TMat>(Vector3 position, Vector3 normal, float radius)
                where TMat : struct, IStaticMaterial
            {
                return Mesh<CircleMesh, TMat>(position, Quaternion.LookRotation(normal.CheckNormalOrDefault()), new Vector3(radius, radius, radius));
            }
            [IN(LINE)]
            public DrawHandler Circle<TMat>(Vector3 position, Quaternion rotation, float radius)
                where TMat : struct, IStaticMaterial
            {
                return Mesh<CircleMesh, TMat>(position, rotation, new Vector3(radius, radius, radius));
            }
            [IN(LINE)]
            public DrawHandler Circle(Vector3 position, Vector3 normal, float radius)
            {
                return Mesh<CircleMesh>(position, Quaternion.LookRotation(normal.CheckNormalOrDefault()), new Vector3(radius, radius, radius));
            }
            [IN(LINE)]
            public DrawHandler Circle(Vector3 position, Quaternion rotation, float radius)
            {
                return Mesh<CircleMesh>(position, rotation, new Vector3(radius, radius, radius));
            }
            #endregion

            #region WireCircle
            [IN(LINE)] public DrawHandler WireCircle(Vector3 position, Vector3 normal, float radius) => WireCircle(position, Quaternion.LookRotation(normal), radius);
            [IN(LINE)] public DrawHandler WireCircle(Vector3 position, Quaternion rotation, float radius) => Mesh<WireCircleMesh, GeometryUnlitMat>(position, rotation, new Vector3(radius, radius, radius));
            #endregion


            #region Cylinder
            [IN(LINE)]
            public DrawHandler Cylinder<TMat>(Vector3 position, Quaternion rotation, float radius, float height)
                where TMat : struct, IStaticMaterial
            {
                return Mesh<CylinderMesh, TMat>(position, rotation * Quaternion.Euler(-90, 0, 0), new Vector3(radius * 2f, radius * 2f, height));
            }
            [IN(LINE)]
            public DrawHandler Cylinder(Vector3 position, Quaternion rotation, float radius, float height)
            {
                return Mesh<CylinderMesh>(position, rotation * Quaternion.Euler(-90, 0, 0), new Vector3(radius * 2f, radius * 2f, height));
            }
            #endregion

            #region WireCylinder
            [IN(LINE)]
            public DrawHandler WireCylinder(Vector3 position, Quaternion rotation, float radius, float height)
            {
                var halfHeigth = height * 0.5f;
                var normalUp = rotation * Vector3.up;

                var center = position;

                Vector3 start, end;
                start = center - normalUp * halfHeigth;
                end = center + normalUp * halfHeigth;

                WireCircle(start, rotation * Quaternion.Euler(90, 0, 0), radius);
                WireCircle(end, rotation * Quaternion.Euler(90, 0, 0), radius);

                var normalForward = rotation * Vector3.forward;
                Vector3 from = Vector3.Cross(normalForward, normalUp).normalized;
                Vector3 perpendicular = from * radius;
                normalForward *= radius;

                Vector3* lines = stackalloc Vector3[]
                {
                    start - perpendicular,
                    end - perpendicular,
                    start + perpendicular,
                    end + perpendicular,
                    start - normalForward,
                    end - normalForward,
                    start + normalForward,
                    end + normalForward,
                };

                Line(lines[0], lines[1]);
                Line(lines[2], lines[3]);
                Line(lines[4], lines[5]);
                Line(lines[6], lines[7]);

                return this;
            }
            #endregion


            #region Cone
            [IN(LINE)]
            public DrawHandler Cone<TMat>(Vector3 position, Quaternion rotation, float radius, float height)
                where TMat : struct, IStaticMaterial
            {
                return Mesh<ConeMesh, TMat>(position, rotation * Quaternion.Euler(-90, 0, 0), new Vector3(radius * 2f, radius * 2f, height));
            }
            [IN(LINE)]
            public DrawHandler Cone(Vector3 position, Quaternion rotation, float radius, float height)
            {
                return Mesh<ConeMesh>(position, rotation * Quaternion.Euler(-90, 0, 0), new Vector3(radius * 2f, radius * 2f, height));
            }
            #endregion

            #region WireCone
            [IN(LINE)]
            public DrawHandler WireCone(Vector3 position, Quaternion rotation, float radius, float height)
            {
                var halfHeigth = height * 0.5f;
                var normalUp = rotation * Vector3.up;

                var center = position;

                Vector3 start, end;
                start = center - normalUp * halfHeigth;
                end = center + normalUp * halfHeigth;

                WireCircle(start, rotation * Quaternion.Euler(90, 0, 0), radius);

                var normalForward = rotation * Vector3.forward;
                Vector3 from = Vector3.Cross(normalForward, normalUp).normalized;
                Vector3 perpendicular = from * radius;
                normalForward *= radius;

                Vector3* lines = stackalloc Vector3[]
                {
                    start - perpendicular,
                    start + perpendicular,
                    start - normalForward,
                    start + normalForward,
                };

                Line(lines[0], end);
                Line(lines[1], end);
                Line(lines[2], end);
                Line(lines[3], end);

                return this;
            }
            #endregion


            #region Triangle
            [IN(LINE)]
            public DrawHandler Triangle<TMat>(Vector3 position, Quaternion rotation, Vector2 size)
                where TMat : struct, IStaticMaterial
            {
                return Mesh<TriangleMesh, TMat>(position, rotation, new Vector3(size.x, size.y, 1f));
            }
            [IN(LINE)]
            public DrawHandler Triangle(Vector3 position, Quaternion rotation, Vector2 size)
            {
                return Mesh<TriangleMesh>(position, rotation, new Vector3(size.x, size.y, 1f));
            }
            #endregion

            #region WireTriangle
            [IN(LINE)]
            public DrawHandler WireTriangle(Vector3 position, Quaternion rotation, Vector2 size)
            {
                Mesh<TriangleMesh, WireMat>(position, rotation, new Vector3(size.x, size.y, 1f));
                return this;
            }
            #endregion


            #region Capsule
            [IN(LINE)]
            public DrawHandler Capsule<TMat>(Vector3 position, Quaternion rotation, float radius, float height)
                where TMat : struct, IStaticMaterial
            {
                radius = Mathf.Max(0, radius);
                height -= radius * 2f;
                height = Mathf.Max(0, height);
                var halfHeigth = height * 0.5f;
                var normal = rotation * Vector3.up;

                Mesh<CapsuleHeadMesh, TMat>(position + normal * halfHeigth, rotation, new Vector3(radius, radius, radius));
                Mesh<CapsuleBodyMesh, TMat>(position, rotation, new Vector3(radius, height, radius));
                Mesh<CapsuleHeadMesh, TMat>(position - normal * halfHeigth, rotation * Quaternion.Euler(0, 0, 180), new Vector3(radius, radius, radius));
                return this;
            }
            [IN(LINE)]
            public DrawHandler Capsule(Vector3 position, Quaternion rotation, float radius, float height)
            {
                return Capsule<LitMat>(position, rotation, radius, height);
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
            public DrawHandler FlatCapsule<TMat>(Vector3 position, Quaternion rotation, float radius, float height)
                where TMat : struct, IStaticMaterial
            {
                radius = Mathf.Max(0, radius);
                height -= radius * 2f;
                height = Mathf.Max(0, height);
                var halfHeigth = height * 0.5f;
                var normal = rotation * Vector3.up;

                Mesh<FlatCapsuleHeadMesh, TMat>(position + normal * halfHeigth, rotation, new Vector3(radius, radius, radius));
                Mesh<FlatCapsuleBodyMesh, TMat>(position, rotation, new Vector3(radius, height, radius));
                Mesh<FlatCapsuleHeadMesh, TMat>(position - normal * halfHeigth, rotation * Quaternion.Euler(0, 0, 180), new Vector3(radius, radius, radius));
                return this;
            }
            [IN(LINE)]
            public DrawHandler FlatCapsule(Vector3 position, Quaternion rotation, float radius, float height)
            {
                return FlatCapsule<LitMat>(position, rotation, radius, height);
            }
            #endregion

            #region WireFlatCapsule
            private static readonly Quaternion Rot180 = Quaternion.Euler(0, 0, 180);
            [IN(LINE)]
            public DrawHandler WireFlatCapsule(Vector3 position, Quaternion rotation, float radius, float height)
            {
                height -= radius * 2f;

                var normalUp = rotation * Vector3.up;
                var halfHeigth = height * 0.5f;

                Vector3 start = position - normalUp * halfHeigth;
                Vector3 end = position + normalUp * halfHeigth;

                Mesh<WireArcMesh, GeometryUnlitMat>(end, rotation, new Vector3(radius, radius, radius));
                Mesh<WireArcMesh, GeometryUnlitMat>(start, rotation * Rot180, new Vector3(radius, radius, radius));

                var normalForward = rotation * Vector3.forward;
                Vector3 from = Vector3.Cross(normalForward, normalUp).normalized;
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
            [IN(LINE)]
            public DrawHandler Cube<TMat>(Vector3 position, Quaternion rotation, float size)
                where TMat : struct, IStaticMaterial
            {
                return Mesh<CubeMesh, TMat>(position, rotation, new Vector3(size, size, size));
            }
            [IN(LINE)]
            public DrawHandler Cube<TMat>(Vector3 position, Quaternion rotation, Vector3 size)
                where TMat : struct, IStaticMaterial
            {
                return Mesh<CubeMesh, TMat>(position, rotation, size);
            }
            [IN(LINE)]
            public DrawHandler Cube(Vector3 position, Quaternion rotation, float size)
            {
                return Mesh<CubeMesh>(position, rotation, new Vector3(size, size, size));
            }
            [IN(LINE)]
            public DrawHandler Cube(Vector3 position, Quaternion rotation, Vector3 size)
            {
                return Mesh<CubeMesh>(position, rotation, size);
            }
            #endregion

            #region WireCube
            //[IN(LINE)] public void WireCube(Vector3 position, float size) => WireCube(position, Quaternion.identity, new Vector3(size, size, size));
            //[IN(LINE)] public void WireCube(Vector3 position, Vector3 size) => WireCube(position, Quaternion.identity, size);
            [IN(LINE)] public DrawHandler WireCube(Vector3 position, Quaternion rotation, float size) => WireCube(position, rotation, new Vector3(size, size, size));
            [IN(LINE)] public DrawHandler WireCube(Vector3 position, Quaternion rotation, Vector3 size) => Mesh<WireCubeMesh, GeometryUnlitMat>(position, rotation, size);
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
                //float m, add;
                //int iMax;
                //
                //iMax = (cells.x + 1) / 2;
                //m = size.x / iMax;
                //add = (cells.x & 1) == 0 ? m * 0.5f : 0;
                //for (int i = 0; i < iMax; i++)
                //{
                //    WireCube(position, rotation, new Vector3(add + i * m, size.y, size.z));
                //}
                //
                //iMax = (cells.y + 1) / 2;
                //m = size.y / iMax;
                //add = (cells.y & 1) == 0 ? 0 : m * 0.5f;
                //for (int i = 0; i < iMax; i++)
                //{
                //    WireCube(position, rotation, new Vector3(size.x, add + i * m, size.z));
                //}
                //
                //iMax = (cells.z + 1) / 2;
                //m = size.z / iMax;
                //add = (cells.z & 1) == 0 ? 0 : m * 0.5f;
                //for (int i = 0; i < iMax; i++)
                //{
                //    WireCube(position, rotation, new Vector3(size.x, size.y, add + i * m));
                //}

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
            [IN(LINE)]
            public DrawHandler Quad<TMat>(Vector3 position, Quaternion rotation, float size)
                where TMat : struct, IStaticMaterial
            {
                return Mesh<QuadMesh, TMat>(position, rotation, new Vector3(size, size, 1f));
            }
            [IN(LINE)]
            public DrawHandler Quad<TMat>(Vector3 position, Quaternion rotation, Vector2 size)
                where TMat : struct, IStaticMaterial
            {
                return Mesh<QuadMesh, TMat>(position, rotation, new Vector3(size.x, size.y, 1f));
            }
            [IN(LINE)]
            public DrawHandler Quad(Vector3 position, Quaternion rotation, float size)
            {
                return Mesh<QuadMesh>(position, rotation, new Vector3(size, size, 1f));
            }
            [IN(LINE)]
            public DrawHandler Quad(Vector3 position, Quaternion rotation, Vector2 size)
            {
                return Mesh<QuadMesh>(position, rotation, new Vector3(size.x, size.y, 1f));
            }
            #endregion

            #region WireQuad
            //[IN(LINE)] public DrawHandler WireQuad(Vector3 position, Vector3 normal, float size) => WireQuad(position, Quaternion.LookRotation(normal), new Vector2(size, size));
            //[IN(LINE)] public DrawHandler WireQuad(Vector3 position, Vector3 normal, Vector2 size) => WireQuad(position, Quaternion.LookRotation(normal), size);
            [IN(LINE)] public DrawHandler WireQuad(Vector3 position, Quaternion rotation, float size) => WireQuad(position, rotation, new Vector2(size, size));
            [IN(LINE)] public DrawHandler WireQuad(Vector3 position, Quaternion rotation, Vector2 size) => Mesh<WireCubeMesh, GeometryUnlitMat>(position, rotation, size);
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
        }
    }
}