//#undef DEBUG
using DCFApixels.DebugXCore;
using DCFApixels.DebugXCore.Internal;
using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Rendering;

namespace DCFApixels
{
    using static DebugXConsts;
    using IN = System.Runtime.CompilerServices.MethodImplAttribute;

    public static unsafe partial class DebugX
    {
        public readonly partial struct DrawHandler
        {
            #region Lambda //TODO
            //[IN(LINE)]
            //public DrawHandler Lambda(Action<(Color color, float duration)> drawCallback)
            //{
            //    return Gizmo(new LambdaGizmo(drawCallback));
            //}
            //private readonly struct LambdaGizmo : IGizmo<LambdaGizmo>
            //{
            //    public readonly Action<(Color color, float duration)> Action;
            //    [IN(LINE)] public LambdaGizmo(Action<(Color color, float duration)> action) { Action = action; }
            //    public IGizmoRenderer<LambdaGizmo> RegisterNewRenderer() => new Renderer();
            //    private class Renderer : IGizmoRenderer<LambdaGizmo>
            //    {
            //        public int ExecuteOrder => 0;
            //        public bool IsStaticRender => false;
            //        public void Prepare(Camera camera, GizmosList<LambdaGizmo> list) { }
            //        public void Render(Camera camera, GizmosList<LambdaGizmo> list, CommandBuffer cb)
            //        {
            //            foreach (var item in list)
            //            {
            //                item.Value.Action((item.Color, item.Timer));
            //            }
            //        }
            //    }
            //}
            #endregion

            #region Mesh //TODO потестить
            [IN(LINE)]
            public DrawHandler Mesh<TMat>(Mesh mesh, Vector3 position, Quaternion rotation, Vector3 size)
                where TMat : struct, IStaticMaterial
            {
                return Gizmo(new MeshGizmo<TMat>(mesh, position, rotation, size));
            }
            [IN(LINE)]
            public DrawHandler Mesh(Mesh mesh, Vector3 position, Quaternion rotation, Vector3 size)
            {
                return Gizmo(new MeshGizmo<LitMat>(mesh, position, rotation, size));
            }
            [IN(LINE)]
            public DrawHandler UnlitMesh(Mesh mesh, Vector3 position, Quaternion rotation, Vector3 size)
            {
                return Gizmo(new MeshGizmo<UnlitMat>(mesh, position, rotation, size));
            }
            [IN(LINE)]
            public DrawHandler WireMesh(Mesh mesh, Vector3 position, Quaternion rotation, Vector3 size)
            {
                return Gizmo(new MeshGizmo<WireMat>(mesh, position, rotation, size));
            }

            private readonly struct MeshGizmoLayout
            {
                public readonly Mesh Mesh;
                public readonly Quaternion Rotation;
                public readonly Vector3 Position;
                public readonly Vector3 Size;
                public MeshGizmoLayout(Mesh mesh, Vector3 position, Quaternion rotation, Vector3 size)
                {
                    Mesh = mesh;
                    Rotation = rotation;
                    Position = position;
                    Size = size;
                }
            }
            private readonly struct MeshGizmo<TMat> : IGizmo<MeshGizmo<TMat>>
                where TMat : struct, IStaticMaterial
            {
                public readonly Mesh Mesh;
                public readonly Quaternion Rotation;
                public readonly Vector3 Position;
                public readonly Vector3 Size;
                [IN(LINE)]
                public MeshGizmo(Mesh mesh, Vector3 position, Quaternion rotation, Vector3 size)
                {
                    Mesh = mesh;
                    Position = position;
                    Rotation = rotation;
                    Size = size;
                }
                public IGizmoRenderer<MeshGizmo<TMat>> RegisterNewRenderer() { return new Renderer(); }

                private class Renderer : MeshRendererBase, IGizmoRenderer<MeshGizmo<TMat>>
                {
                    public Renderer() : base(default(TMat)) { }
                    public void Prepare(Camera camera, GizmosList<MeshGizmo<TMat>> list)
                    {
                        Prepare(list);
                    }
                    public void Render(Camera camera, GizmosList<MeshGizmo<TMat>> list, CommandBuffer cb)
                    {
                        Render(cb);
                    }
                }
            }
            #endregion

            #region InstancingMesh
            [IN(LINE)]
            public DrawHandler Mesh<TMesh, TMat>(Vector3 position, Quaternion rotation, Vector3 size)
                where TMesh : struct, IStaticMesh
                where TMat : struct, IStaticMaterial
            {
                return Gizmo(new InstancingMeshGizmo<TMesh, TMat>(position, rotation, size));
            }
            [IN(LINE)]
            public DrawHandler Mesh<TMesh>(Vector3 position, Quaternion rotation, Vector3 size)
                where TMesh : struct, IStaticMesh
            {
                return Gizmo(new InstancingMeshGizmo<TMesh, LitMat>(position, rotation, size));
            }
            [IN(LINE)]
            public DrawHandler UnlitMesh<TMesh>(Vector3 position, Quaternion rotation, Vector3 size)
                where TMesh : struct, IStaticMesh
            {
                return Gizmo(new InstancingMeshGizmo<TMesh, UnlitMat>(position, rotation, size));
            }
            [IN(LINE)]
            public DrawHandler WireMesh<TMesh>(Vector3 position, Quaternion rotation, Vector3 size)
                where TMesh : struct, IStaticMesh
            {
                return Gizmo(new InstancingMeshGizmo<TMesh, WireMat>(position, rotation, size));
            }

            private readonly struct InstancingMeshGizmoLayout
            {
                public readonly Quaternion Rotation;
                public readonly Vector3 Position;
                public readonly Vector3 Size;
                public InstancingMeshGizmoLayout(Vector3 position, Quaternion rotation, Vector3 size)
                {
                    Rotation = rotation;
                    Position = position;
                    Size = size;
                }
            }
            private readonly struct InstancingMeshGizmo<TMesh, TMat> : IGizmo<InstancingMeshGizmo<TMesh, TMat>>
                where TMesh : struct, IStaticMesh
                where TMat : struct, IStaticMaterial
            {
                public readonly Quaternion Rotation;
                public readonly Vector3 Position;
                public readonly Vector3 Size;
                [IN(LINE)]
                public InstancingMeshGizmo(Vector3 position, Quaternion rotation, Vector3 size)
                {
                    Rotation = rotation;
                    Position = position;
                    Size = size;
                }
                public IGizmoRenderer<InstancingMeshGizmo<TMesh, TMat>> RegisterNewRenderer() { return new Renderer(); }
                private class Renderer : InstancingMeshRendererBase, IGizmoRenderer<InstancingMeshGizmo<TMesh, TMat>>
                {
                    public Renderer() : base(default(TMesh), default(TMat)) { }
                    public void Prepare(Camera camera, GizmosList<InstancingMeshGizmo<TMesh, TMat>> list)
                    {
                        Prepare(list);
                    }
                    public void Render(Camera camera, GizmosList<InstancingMeshGizmo<TMesh, TMat>> list, CommandBuffer cb)
                    {
                        Render(cb);
                    }
                }
            }
            #endregion

            #region Line
            [IN(LINE)]
            public DrawHandler Line<TMat>(Vector3 start, Vector3 end)
                where TMat : struct, IStaticMaterial
            {
                return Gizmo(new WireLineGizmo<TMat>(start, end));
            }
            [IN(LINE)]
            public DrawHandler Line(Vector3 start, Vector3 end)
            {
                return Gizmo(new WireLineGizmo<LineMat>(start, end));
            }
            private readonly struct WireLineGizmo<TMat> : IGizmo<WireLineGizmo<TMat>>
                where TMat : struct, IStaticMaterial
            {
                public readonly Vector3 Start;
                public readonly Vector3 End;
                [IN(LINE)]
                public WireLineGizmo(Vector3 start, Vector3 end)
                {
                    Start = start;
                    End = end;
                }
                public IGizmoRenderer<WireLineGizmo<TMat>> RegisterNewRenderer() { return new Renderer(); }
                private class Renderer : WireLineRendererBase, IGizmoRenderer<WireLineGizmo<TMat>>
                {
                    public Renderer() : base(default(TMat)) { }
                    public void Prepare(Camera camera, GizmosList<WireLineGizmo<TMat>> list)
                    {
                        Prepare(list);
                    }
                    public void Render(Camera camera, GizmosList<WireLineGizmo<TMat>> list, CommandBuffer cb)
                    {
                        Render(cb);
                    }
                }
            }
            #endregion

            // Base Renderers

            #region MeshRendererBase
            private class MeshRendererBase
            {
                private readonly struct GizmoData
                {
                    public readonly Mesh Mesh;
                    public readonly Quaternion Rotation;
                    public readonly Vector3 Position;
                    public readonly Vector3 Size;
                }
                private readonly struct UnmanagedGizmoData
                {
                    public readonly void* RawMesh;
                    public readonly Quaternion Rotation;
                    public readonly Vector3 Position;
                    public readonly Vector3 Size;
                }
                private struct PrepareJob : IJobParallelFor
                {
                    [NativeDisableUnsafePtrRestriction]
                    public Gizmo<UnmanagedGizmoData>* Items;
                    [NativeDisableUnsafePtrRestriction]
                    public Matrix4x4* ResultMatrices;
                    [NativeDisableUnsafePtrRestriction]
                    public Vector4* ResultColors;
                    public void Execute(int index)
                    {
                        ref readonly var item = ref Items[index];
                        //if (item.IsSwaped == 0) { return; }
                        ResultMatrices[index] = Matrix4x4.TRS(item.Value.Position, item.Value.Rotation, item.Value.Size);
                        ResultColors[index] = item.Color;
                    }
                }

                private readonly IStaticMaterial _material;

                private int _buffersLength = 0;
                private PinnedArray<Matrix4x4> _matrices;
                private PinnedArray<Vector4> _colors;
                private PinnedArray<Gizmo<UnmanagedGizmoData>> _gizmos;

                private readonly MaterialPropertyBlock _materialPropertyBlock;

                private JobHandle _jobHandle;
                private int _prepareCount = 0;

                public virtual int ExecuteOrder => _material.GetExecuteOrder();
                public virtual bool IsStaticRender => true;

                public MeshRendererBase(IStaticMaterial material)
                {
                    _materialPropertyBlock = new MaterialPropertyBlock();
                    _material = material;
                    _matrices = PinnedArray<Matrix4x4>.Pin(DummyArray<Matrix4x4>.Get());
                    _colors = PinnedArray<Vector4>.Pin(DummyArray<Vector4>.Get());
                }
                public void Prepare(GizmosList rawList)
                {
                    var list = rawList.As<GizmoData>();
                    var items = list.Items;
                    var count = list.Count;
                    _prepareCount = count;

                    if (_buffersLength < count)
                    {
                        if (_matrices.Array != null)
                        {
                            _matrices.Dispose();
                            _colors.Dispose();
                        }
                        _matrices = PinnedArray<Matrix4x4>.Pin(new Matrix4x4[DebugXUtility.NextPow2(count)]);
                        _colors = PinnedArray<Color>.Pin(new Color[DebugXUtility.NextPow2(count)]).As<Vector4>();
                        _buffersLength = count;
                    }
                    if (ReferenceEquals(_gizmos.Array, items) == false)
                    {
                        if (_gizmos.Array != null)
                        {
                            _gizmos.Dispose();
                        }
                        var itemsUnmanaged = UnsafeUtility.As<Gizmo<GizmoData>[], Gizmo<UnmanagedGizmoData>[]>(ref items);
                        _gizmos = PinnedArray<Gizmo<UnmanagedGizmoData>>.Pin(itemsUnmanaged);
                    }

                    var job = new PrepareJob
                    {
                        Items = _gizmos.Ptr,
                        ResultMatrices = _matrices.Ptr,
                        ResultColors = _colors.Ptr,
                    };
                    _jobHandle = job.Schedule(count, 64);
                }
                public void Render(CommandBuffer cb)
                {
                    Material material = _material.GetMaterial_SupportCumputeShaders();
                    var items = new GizmosList<UnmanagedGizmoData>(_gizmos.Array, _prepareCount).As<GizmoData>().Items;
                    _materialPropertyBlock.Clear();
                    _jobHandle.Complete();

                    for (int i = 0; i < _prepareCount; i++)
                    {
                        ref readonly var item = ref items[i];
                        _materialPropertyBlock.SetColor(ColorPropertyID, item.Color);
                        cb.DrawMesh(item.Value.Mesh, _matrices.Ptr[i], material, 0, -1, _materialPropertyBlock);
                    }
                }
            }
            #endregion

            #region InstancingMeshRendererBase
            private class InstancingMeshRendererBase
            {
                private readonly struct GizmoData
                {
                    public readonly Quaternion Rotation;
                    public readonly Vector3 Position;
                    public readonly Vector3 Size;
                }
                private struct PrepareJob : IJobParallelFor
                {
                    [NativeDisableUnsafePtrRestriction]
                    public Gizmo<GizmoData>* Items;
                    [NativeDisableUnsafePtrRestriction]
                    public Matrix4x4* ResultMatrices;
                    [NativeDisableUnsafePtrRestriction]
                    public Vector4* ResultColors;
                    public void Execute(int index)
                    {
                        ref readonly var item = ref Items[index];
                        //if (item.IsSwaped == 0) { return; }
                        ResultMatrices[index] = Matrix4x4.TRS(item.Value.Position, item.Value.Rotation, item.Value.Size);
                        ResultColors[index] = item.Color;
                    }
                }

                private readonly IStaticMesh _mesh;
                private readonly IStaticMaterial _material;

                private readonly MaterialPropertyBlock _materialPropertyBlock;

                private int _buffersLength = 0;
                private PinnedArray<Matrix4x4> _matrices;
                private PinnedArray<Vector4> _colors;
                private PinnedArray<Gizmo<GizmoData>> _gizmos;

                private JobHandle _jobHandle;
                private int _prepareCount = 0;

                public InstancingMeshRendererBase(IStaticMesh mesh, IStaticMaterial material)
                {
                    _mesh = mesh;
                    _material = material;
                    _materialPropertyBlock = new MaterialPropertyBlock();

                    _matrices = PinnedArray<Matrix4x4>.Pin(DummyArray<Matrix4x4>.Get());
                    _colors = PinnedArray<Vector4>.Pin(DummyArray<Vector4>.Get());
                }
                public virtual int ExecuteOrder => _material.GetExecuteOrder();
                public virtual bool IsStaticRender => true;
                protected void Prepare(GizmosList rawList)
                {
                    var list = rawList.As<GizmoData>();
                    _prepareCount = list.Count;
                    var items = list.Items;
                    var count = list.Count;

                    if (_buffersLength < count)
                    {
                        _matrices.Dispose();
                        _colors.Dispose();
                        _matrices = PinnedArray<Matrix4x4>.Pin(new Matrix4x4[DebugXUtility.NextPow2(count)]);
                        _colors = PinnedArray<Vector4>.Pin(new Vector4[DebugXUtility.NextPow2(count)]);
                        _buffersLength = count;
                    }
                    if (ReferenceEquals(_gizmos.Array, items) == false)
                    {
                        if (_gizmos.Array != null)
                        {
                            _gizmos.Dispose();
                        }
                        _gizmos = PinnedArray<Gizmo<GizmoData>>.Pin(items);
                    }

                    var job = new PrepareJob
                    {
                        Items = _gizmos.Ptr,
                        ResultMatrices = _matrices.Ptr,
                        ResultColors = _colors.Ptr,
                    };
                    _jobHandle = job.Schedule(count, 64);
                }
                protected void Render(CommandBuffer cb)
                {
                    Material material = _material.GetMaterial_SupportCumputeShaders();
                    Mesh mesh = _mesh.GetMesh();
                    _materialPropertyBlock.Clear();
                    _jobHandle.Complete();
                    if (IsSupportsComputeShaders)
                    {
                        _materialPropertyBlock.SetVectorArray(ColorPropertyID, _colors.Array);
                        cb.DrawMeshInstanced(mesh, 0, material, -1, _matrices.Array, _prepareCount, _materialPropertyBlock);
                    }
                    else
                    {
                        for (int i = 0; i < _prepareCount; i++)
                        {
                            _materialPropertyBlock.SetColor(ColorPropertyID, _colors.Ptr[i]);
                            cb.DrawMesh(mesh, _matrices.Ptr[i], material, 0, -1, _materialPropertyBlock);
                        }
                    }
                }
            }
            #endregion

            #region LineRendererBase
            private class WireLineRendererBase
            {
                private readonly struct GizmoData
                {
                    public readonly Vector3 Start;
                    public readonly Vector3 End;
                }
                private struct DrawData
                {
                    public Matrix4x4 Matrix;
                    public Color Color;
                }
                private struct PrepareJob : IJobParallelFor
                {
                    [NativeDisableUnsafePtrRestriction]
                    public Gizmo<GizmoData>* Items;
                    [NativeDisableUnsafePtrRestriction]
                    public DrawData* ResultData;
                    public void Execute(int index)
                    {
                        ref readonly var item = ref Items[index];

                        Vector3 halfDiff = new Vector3(
                            (item.Value.End.x - item.Value.Start.x) * 0.5f,
                            (item.Value.End.y - item.Value.Start.y) * 0.5f,
                            (item.Value.End.z - item.Value.Start.z) * 0.5f);

                        (ResultData + index)->Matrix = Matrix4x4.TRS(item.Value.Start + halfDiff, Quaternion.identity, halfDiff);
                        (ResultData + index)->Color = item.Color;
                    }
                }
                private readonly IStaticMesh _mesh = default(WireLineMesh);
                private readonly IStaticMaterial _material;
                private readonly MaterialPropertyBlock _materialPropertyBlock;
                private GraphicsBuffer _graphicsBuffer;

                private int _buffersLength = 0;
                private PinnedArray<DrawData> _drawDatas;
                private PinnedArray<Gizmo<GizmoData>> _gizmos;

                private JobHandle _jobHandle;
                private int _prepareCount = 0;

                public WireLineRendererBase(IStaticMaterial material)
                {
                    _material = material;
                    _materialPropertyBlock = new MaterialPropertyBlock();
                    _drawDatas = PinnedArray<DrawData>.Pin(DummyArray<DrawData>.Get());
                }
                public virtual int ExecuteOrder => _material.GetExecuteOrder();
                public virtual bool IsStaticRender => true;
                public void Prepare(GizmosList rawList)
                {
                    var list = rawList.As<GizmoData>();
                    var items = list.Items;
                    var count = list.Count;
                    _prepareCount = count;

                    if (_buffersLength < count)
                    {
                        int capacity = DebugXUtility.NextPow2(count);
                        _drawDatas.Dispose();
                        _drawDatas = PinnedArray<DrawData>.Pin(new DrawData[capacity]);
                        AllocateGraphicsBuffer(capacity);
                        _buffersLength = count;
                    }
                    if (ReferenceEquals(_gizmos.Array, items) == false)
                    {
                        _gizmos.Dispose();
                        _gizmos = PinnedArray<Gizmo<GizmoData>>.Pin(items);
                    }

                    var job = new PrepareJob
                    {
                        Items = _gizmos.Ptr,
                        ResultData = _drawDatas.Ptr,
                    };
                    _jobHandle = job.Schedule(count, 64);
                }


                private readonly static int _BufferPropertyID = Shader.PropertyToID("_DataBuffer");
                public void Render(CommandBuffer cb)
                {
                    Mesh mesh = _mesh.GetMesh();
                    if (IsSupportsComputeShaders)
                    {
                        Material material = _material.GetMaterial_SupportCumputeShaders();
                        _jobHandle.Complete();
                        _graphicsBuffer.SetData(_drawDatas.Array);
                        cb.DrawMeshInstancedProcedural(mesh, 0, material, -1, _prepareCount, _materialPropertyBlock);
                    }
                    else
                    {
                        Material material = _material.GetMaterial_Default();
                        _jobHandle.Complete();
                        for (int i = 0; i < _prepareCount; i++)
                        {
                            _materialPropertyBlock.SetColor(ColorPropertyID, _drawDatas.Ptr[i].Color);
                            cb.DrawMesh(mesh, _drawDatas.Ptr[i].Matrix, material, 0, -1, _materialPropertyBlock);
                        }
                    }
                }
                private void AllocateGraphicsBuffer(int capacity)
                {
                    _graphicsBuffer?.Dispose();
                    _materialPropertyBlock.Clear();
                    _graphicsBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, capacity, Marshal.SizeOf<DrawData>());
                    _materialPropertyBlock.SetBuffer(_BufferPropertyID, _graphicsBuffer);
                }
            }
            #endregion
        }
    }
}