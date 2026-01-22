#if DISABLE_DEBUG
#undef DEBUG
#endif
#if DEBUG
#define DEV_MODE
#endif
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using DCFApixels.DebugXCore;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;
using Unity.Collections.LowLevel.Unsafe;
using DCFApixels.DebugXCore.Internal;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DCFApixels
{
    using static DebugXConsts;
    using IN = System.Runtime.CompilerServices.MethodImplAttribute;

    public static class DebugXEvents
    {
        public delegate void OnDrawGizmoHandler(Camera camera);
        private static event OnDrawGizmoHandler _onDrawGizmo = delegate { };
        public static event OnDrawGizmoHandler OnDrawGizmo
        {
            add
            {
                _onDrawGizmo -= value;
                _onDrawGizmo += value;
            }
            remove
            {
                _onDrawGizmo -= value;
            }
        }

        private static void CleanupEvent(ref OnDrawGizmoHandler eventToClean)
        {
            var invocationList = eventToClean.GetInvocationList();

            var validDelegates = invocationList
                .Where(d => d.Target as UnityEngine.Object == null || d.Target as UnityEngine.Object != null)
                .Where(d => !(d.Target is UnityEngine.Object targetObj) || targetObj != null)
                .ToArray();

            if (validDelegates.Length != invocationList.Length)
            {
                eventToClean = null;
                foreach (var delegateItem in validDelegates)
                {
                    eventToClean += (OnDrawGizmoHandler)delegateItem;
                }
            }
        }

        internal static void CleanupOnDrawGizmo()
        {
            CleanupEvent(ref _onDrawGizmo);
        }
        internal static void InvokeOnDrawGizmo(Camera camera)
        {
            _onDrawGizmo(camera);
        }
    }

    public static unsafe partial class DebugX
    {
        private static DebugXPauseState _pauseState = DebugXPauseState.Unpaused;
        private static bool _isCameraContext = false;

        private static double _lastUnityTime;
        private static float _deltaTime = 0;

        private static ulong _editorTicks = 0;
        private static ulong _lastEditorToRenderTicks = 1000;
        private static ulong _renderTicks = 100;
        private static ulong _timeTicks = 0;

#if UNITY_EDITOR
        private static readonly Unity.Profiling.ProfilerMarker _onDrawGizmoCalllback = new Unity.Profiling.ProfilerMarker($"{nameof(DebugX)}.{nameof(DebugXEvents.OnDrawGizmo)}");
#endif

        public static ulong RenderTicks
        {
            get { return _renderTicks; }
        }
        public static ulong TimeTicks
        {
            get { return _timeTicks; }
        }

        #region Other
        public static void ClearAllGizmos()
        {
            RenderContextController.ClearAllGizmos();
        }
        private static void SetCameraContext()
        {
            _isCameraContext = true;
        }
        private static void SetGameSceneContext()
        {
            _isCameraContext = false;
        }
#if UNITY_EDITOR
        private static void EditorApplication_pauseStateChanged(PauseState obj)
        {
            _pauseState = obj == PauseState.Paused ? DebugXPauseState.Paused : DebugXPauseState.PreUnpaused;
        }
#endif
        #endregion

        #region ctor
        static DebugX()
        {
            InitGlobals();

            if (IsSRP)
            {
                RenderPipelineManager.beginCameraRendering -= OnPreRender_SRP;
                RenderPipelineManager.beginCameraRendering += OnPreRender_SRP;

                RenderPipelineManager.endCameraRendering -= OnPostRender_SRP;
                RenderPipelineManager.endCameraRendering += OnPostRender_SRP;
            }
            else
            {
                Camera.onPreRender -= OnPreRender_BRP;
                Camera.onPreRender += OnPreRender_BRP;

                Camera.onPostRender -= OnPostRender_BRP;
                Camera.onPostRender += OnPostRender_BRP;
            }

            var curentLoop = PlayerLoop.GetCurrentPlayerLoop();
            var systemsList = curentLoop.subSystemList;
            for (int i = 0; i < systemsList.Length; i++)
            {
                ref var system = ref systemsList[i];
                if (system.type == typeof(EarlyUpdate))
                {
                    system.updateDelegate -= PreUpdateCallback;
                    system.updateDelegate += PreUpdateCallback;
                }
                if (system.type == typeof(PostLateUpdate))
                {
                    system.updateDelegate -= PreRenderCallback;
                    system.updateDelegate += PreRenderCallback;
                }
                //if (system.type == typeof(TriggerEndOfFrameCallbacks))
                //{
                //    system.updateDelegate -= OnTriggerEndOfFrameCallbacks;
                //    system.updateDelegate += OnTriggerEndOfFrameCallbacks;
                //}
            }
            curentLoop.subSystemList = systemsList;
            PlayerLoop.SetPlayerLoop(curentLoop);

            //Application.onBeforeRender -= Application_onBeforeRender;
            //Application.onBeforeRender += Application_onBeforeRender;


#if UNITY_EDITOR
            EditorApplication.pauseStateChanged -= EditorApplication_pauseStateChanged;
            EditorApplication.pauseStateChanged += EditorApplication_pauseStateChanged;
            EditorApplication.update -= EditorApplication_update;
            EditorApplication.update += EditorApplication_update;
#endif
        }
        #endregion

        #region Draw/DrawHandler
        private static float GetCurrentDefaultDuration()
        {
            if (GetCurrentContextController().Context.Camera == null)
            {
                return DEFAULT_DURATION;
            }
            else
            {
                return IMMEDIATE_DURATION;
            }
        }
        public static DrawHandler Draw()
        {
            return new DrawHandler(GetCurrentDefaultDuration(), DEFAULT_COLOR);
        }
        public static DrawHandler Draw(float duration)
        {
            return new DrawHandler(duration, DEFAULT_COLOR);
        }
        public static DrawHandler Draw(Color color)
        {
            return new DrawHandler(GetCurrentDefaultDuration(), color);
        }
        public static DrawHandler Draw((Color color, float alphaMultiplier) color)
        {
            return new DrawHandler(GetCurrentDefaultDuration(), color.ToColor());
        }
        public static DrawHandler Draw(float duration, Color color)
        {
            return new DrawHandler(duration, color);
        }
        public static DrawHandler Draw(float duration, (Color color, float alphaMultiplier) color)
        {
            return new DrawHandler(duration, color.ToColor());
        }
        //public static DrawHandler Draw(float r, float g, float b, float a = 1f) //TODO

        public readonly partial struct DrawHandler
        {
            public readonly Color Color;
            public readonly float Duration;
            //private readonly RenderContextController ContextController;
            //public Camera Camera
            //{
            //    get { return ContextController.Context.Camera; }
            //}
            [IN(LINE)]
            public DrawHandler(float time, Color color)
            {
                Color = color;
                Duration = time;
                //ContextController = GetCurrenRenderContextController();
            }
            [IN(LINE)] public DrawHandler Setup(float duration, Color color) => new DrawHandler(duration, color);
            [IN(LINE)] public DrawHandler Setup(float duration, (Color color, float alphaMultiplier) color) => new DrawHandler(duration, color.ToColor());
            [IN(LINE)] public DrawHandler Setup(float duration) => new DrawHandler(duration, Color);
            [IN(LINE)] public DrawHandler Setup(Color color) => new DrawHandler(Duration, color);
            [IN(LINE)] public DrawHandler Setup((Color color, float alphaMultiplier) color) => new DrawHandler(Duration, color.ToColor());
            //[IN(LINE)]
            //private DrawHandler(float time, Color color, RenderContextController contextController)
            //{
            //    Color = color;
            //    Duration = time;
            //    ContextController = contextController;
            //}
            //[IN(LINE)] public DrawHandler Setup(float duration, Color color) => new DrawHandler(duration, color, ContextController);
            //[IN(LINE)] public DrawHandler Setup(float duration) => new DrawHandler(duration, Color, ContextController);
            //[IN(LINE)] public DrawHandler Setup(Color color) => new DrawHandler(Duration, color, ContextController);
            //[IN(LINE)] public DrawHandler Setup(Camera camera) => new DrawHandler(Duration, Color, ContextController.Context.Camera != camera ? RenderContextController.GetController(new RenderContext(camera)) : ContextController);
        }
        #endregion

        #region Gizmo data
        internal struct GizmoInternal<T> where T : IGizmo<T>
        {
            public readonly T Value;
            public readonly Color Color;
            public float Timer;
            //public int IsSwaped;
            [IN(LINE)]
            public GizmoInternal(T value, float timer, Color color)
            {
                Value = value;
                Timer = timer;
                Color = color;
                //IsSwaped = -1;
            }
        }
        public readonly struct Gizmo<T>
        {
            public readonly T Value;
            public readonly Color Color;
            public readonly float Timer;
            public Gizmo(T value, Color color, float timer)
            {
                Value = value;
                Color = color;
                Timer = timer;
            }
            //public readonly int IsSwaped;
            public override string ToString()
            {
                return $"{Color} {Timer}";
            }
        }
        #endregion


        #region Render Callbacks
        private static void OnPreRender_SRP(ScriptableRenderContext context, Camera camera)
        {
            PreRender_General(camera);
        }
        private static void OnPostRender_SRP(ScriptableRenderContext context, Camera camera)
        {
            PostRender_General(CommandBufferExecutorSRP.GetInstance(context), camera);
            context.Submit();
        }
        private static void OnPreRender_BRP(Camera camera)
        {
            PreRender_General(camera);
        }
        private static void OnPostRender_BRP(Camera camera)
        {
            PostRender_General(CommandBufferExecutorBRP.GetInstance(), camera);
        }

        private static void PreUpdateCallback()
        {
            _editorTicks++;
            _currentCamera = null;

            if (_lastUnityTime < Time.unscaledTimeAsDouble)
            {
                _timeTicks++;
                if (_pauseState == DebugXPauseState.Unpaused)
                {
                    _deltaTime = Time.unscaledDeltaTime * _timeScaleCache;
                }
                else
                {
                    const float Min = 1f / 200f;
                    const float Max = 1f / 20f;
                    if (_deltaTime < Min || _deltaTime > Max)
                    {
                        _deltaTime = Max;
                    }
                }
                for (int i = 0, iMax = GizmosBuffer.All.Count; i < iMax; i++)
                {
                    GizmosBuffer.All[i].UpdateTimer(_deltaTime);
                }
            }
            _lastUnityTime = Time.unscaledTimeAsDouble;
            if (_pauseState == DebugXPauseState.PreUnpaused)
            {
                _pauseState = DebugXPauseState.Unpaused;
            }
            SetGameSceneContext();
        }
        private static void PreRenderCallback()
        {
            RenderContextController.ClearCommandBuffers();
            SetCameraContext();
            _currentCamera = null;
        }


        private static void EditorApplication_update()
        {
            _editorTicks++;
        }

        private static void PreRender_General(Camera camera)
        {
            if (camera == null) { return; }
            _currentCamera = camera;
        }

        private static void PostRender_General(ICommandBufferExecutor cbExecutor, Camera camera)
        {
            if (_lastEditorToRenderTicks != _editorTicks)
            {
                _renderTicks++;
                _lastEditorToRenderTicks = _editorTicks;


                DebugXEvents.CleanupOnDrawGizmo();
            }

            if (DebugXUtility.IsGizmosRender())
            {
#if UNITY_EDITOR
                using (_onDrawGizmoCalllback.Auto())
#endif
                {
                    DebugXEvents.InvokeOnDrawGizmo(camera);
                }


                RenderContextController.StaicContextController.Prepare();
                RenderContextController.StaicContextController.Render(cbExecutor);
                cbExecutor.Submit();
                RenderContextController.StaicContextController.PostRender();
                RenderContextController.StaicContextController.RunEnd();
            }

            if (camera == null) { return; }
            _currentCamera = camera;
            RenderContextController contextController = RenderContextController.GetController(new RenderContext(camera));
            contextController.Prepare();
            contextController.Render(cbExecutor);
            cbExecutor.Submit();
            contextController.PostRender();
            contextController.RunEnd();
        }
        #endregion


        #region Gizmo Method
        private static Camera _currentCamera;
        private static Camera GetCurrentCamera()
        {
            return _currentCamera == null ? Camera.current : _currentCamera;
        }
        private static RenderContextController _currenRenderContextControler = RenderContextController.GetController(new RenderContext(null));
        public readonly partial struct DrawHandler
        {
            [IN(LINE)]
            public DrawHandler Gizmo<T>(T value) where T : IGizmo<T>
            {
#if UNITY_EDITOR || !DEBUGX_DISABLE_INBUILD
                GetCurrentContextController().Add(value, Duration, Color);
#endif
                return this;
            }
            [IN(LINE)]
            public DrawHandler Gizmos<T>(ReadOnlySpan<T> values) where T : IGizmo<T>
            {
#if UNITY_EDITOR || !DEBUGX_DISABLE_INBUILD
                GetCurrentContextController().AddRange(values, Duration, Color);
#endif
                return this;
            }
        }
        private static RenderContextController GetCurrentContextController()
        {
            //RenderContextController controller;
            //if (ContextController == null)
            //{
            //    controller = DrawHandler.GetCurrentContextController();
            //}
            //else
            //{
            //    controller = ContextController;
            //}
            //return controller;


            if (_isCameraContext)
            {
                if (_currenRenderContextControler.Context.Camera != GetCurrentCamera())
                {
                    _currenRenderContextControler = RenderContextController.GetController(new RenderContext(Camera.current));
                }
            }
            else
            {
                _currenRenderContextControler = RenderContextController.StaicContextController;
            }
            return _currenRenderContextControler;
        }
        #endregion


        #region RenderContextControler
        private class RenderContextController
        {
            private static readonly Dictionary<RenderContext, RenderContextController> _allControllers = new Dictionary<RenderContext, RenderContextController>();
            private static readonly RenderContextController _staicContextController;
            private static readonly CommandBuffer _staticContextBuffer;
            static RenderContextController()
            {
                _staticContextBuffer = new CommandBuffer();
                _staticContextBuffer.name = "DebugX_StaticContextRender";
                //_staicContextController = new RenderContextControler(new RenderContext(null), _staticContextBuffer);

                _staicContextController = GetController(new RenderContext(null));
            }
            public static IEnumerable<RenderContextController> AllConteollers
            {
                get { return _allControllers.Values; }
            }
            public static RenderContextController StaicContextController
            {
                get { return _staicContextController; }
            }
            public static RenderContextController GetController(RenderContext context)
            {
                if (_allControllers.TryGetValue(context, out RenderContextController result) == false)
                {
                    CommandBuffer cb;
                    if (context.Camera == null)
                    {
                        cb = _staticContextBuffer;
                    }
                    else
                    {
                        cb = new CommandBuffer();
                        cb.name = "DebugX_CameraContextRender";
                    }
                    result = new RenderContextController(context, cb);
                    //TODO тут может быть утечка
                    _allControllers.Add(context, result);
                }
                return result;
            }
            public static void ClearCommandBuffers()
            {
                foreach (var item in _allControllers)
                {
                    item.Value.commandBuffer.Clear();
                }
            }
            public static void UpdateControllersList()
            {

            }
            public static void ClearAllGizmos()
            {
                foreach (var controller in AllConteollers)
                {
                    foreach (var buffer in controller._buffers)
                    {
                        buffer.Clear();
                    }
                }
            }

            public readonly RenderContext Context;
            private readonly CommandBuffer commandBuffer;
            private GizmosBuffer[] _buffersMap = Array.Empty<GizmosBuffer>();
            private readonly List<GizmosBuffer> _buffers = new List<GizmosBuffer>();

            private readonly Unity.Profiling.ProfilerMarker _cameraMarker;

            //private long _version;
            //private long _lastVersion;
            //private void IncrementVersion()
            //{
            //    System.Threading.Interlocked.Increment(ref _version);
            //}

            private RenderContextController(RenderContext context, CommandBuffer cb)
            {
                Context = context;
                commandBuffer = cb;

                _cameraMarker = new Unity.Profiling.ProfilerMarker($"{Context.Camera}");

                _buffersMap = new GizmosBuffer[GizmoTypeCode.TypesCount];
                //TODO тут может быть утечка
                GizmoTypeCode.OnAddNewID += TypeCode_OnAddNewID;
            }
            private void TypeCode_OnAddNewID(int count)
            {
                Array.Resize(ref _buffersMap, DebugXUtility.NextPow2(count));
            }


            [IN(LINE)]
            public void Add<T>(T value, float time, Color color) where T : IGizmo<T>
            {
                GetBuffer<T>().Add(value, time, color);
            }
            [IN(LINE)]
            public void AddRange<T>(ReadOnlySpan<T> values, float time, Color color) where T : IGizmo<T>
            {
                GetBuffer<T>().AddRange(values, time, color);
            }
            [IN(LINE)]
            private GizmosBuffer<T> GetBuffer<T>() where T : IGizmo<T>
            {
                int id = GizmoTypeCode<T>.ID;
                if (_buffersMap[id] == null)
                {
                    var result = new GizmosBuffer<T>(Context, commandBuffer);
                    _buffersMap[id] = result;
                    _buffers.Add(result);
                    _buffers.Sort((a, b) => a.ExecuteOrder - b.ExecuteOrder);
                    return result;
                }
                else
                {
                    return UnsafeUtility.As<GizmosBuffer, GizmosBuffer<T>>(ref _buffersMap[id]);
                }
            }

            //[IN(LINE)]
            //public void UpdateTimer(float deltaTime)
            //{
            //    int removed = 0;
            //    for (int i = 0, iMax = _buffers.Count; i < iMax; i++)
            //    {
            //        removed |= _buffers[i].UpdateTimer(deltaTime);
            //    }
            //    //if(removed > 0)
            //    //{
            //    //    IncrementVersion();
            //    //}
            //}

            [IN(LINE)]
            public void Prepare()
            {
#if UNITY_EDITOR
                using (_cameraMarker.Auto())
#endif
                {
                    for (int i = 0, iMax = _buffers.Count; i < iMax; i++)
                    {
                        _buffers[i].Prepare();
                    }
                }
            }

            [IN(LINE)]
            public void Render(ICommandBufferExecutor cbExecutor)
            {
#if UNITY_EDITOR
                using (_cameraMarker.Auto())
#endif
                {
                    for (int i = 0, iMax = _buffers.Count; i < iMax; i++)
                    {
                        _buffers[i].Render(cbExecutor);
                    }
                }
            }


            [IN(LINE)]
            public void PostRender()
            {
#if UNITY_EDITOR
                using (_cameraMarker.Auto())
#endif
                {
                    for (int i = 0, iMax = _buffers.Count; i < iMax; i++)
                    {
                        _buffers[i].PostRender();
                    }
                }
            }

            public void RunEnd()
            {
                int removed = 0;
                for (int i = 0, iMax = _buffers.Count; i < iMax; i++)
                {
                    removed |= _buffers[i].RunEnd();
                }
                //if (removed > 0)
                //{
                //    IncrementVersion();
                //}
            }
        }
        #endregion

        #region GizmosBuffer
        private abstract class GizmosBuffer
        {
            public readonly static List<GizmosBuffer> All = new List<GizmosBuffer>();

            public abstract int ExecuteOrder { get; }
            public abstract int UpdateTimer(float deltaTime);
            public abstract void Prepare();
            public abstract void Render(ICommandBufferExecutor cbExecutor);
            public abstract void PostRender();
            public abstract int RunEnd();
            public abstract void Clear();
        }
        private class GizmosBuffer<T> : GizmosBuffer where T : IGizmo<T>
        {
            private static int _lastAvailablePoolMemory;
            private static int _bufferMaxSize;
            private static readonly int _elementSize = RuntimeHelpers.IsReferenceOrContainsReferences<T>() ? sizeof(IntPtr) : Marshal.SizeOf<T>();
            private static void CheckAvailablePoolMemory()
            {
                if(_bufferMaxSize != 0 && _lastAvailablePoolMemory == DebugX.AvailablePoolMemory)
                {
                    return;
                }
                if(DebugX.AvailablePoolMemory < _elementSize * 10)
                {
                    _bufferMaxSize = DebugX.AvailablePoolMemory * 1024 / _elementSize;
                }
                else
                {
                    _bufferMaxSize = DebugX.AvailablePoolMemory / _elementSize * 1024;
                }
                _lastAvailablePoolMemory = DebugX.AvailablePoolMemory;
            }
            private class DummyRenderer : IGizmoRenderer<T>
            {
                public int ExecuteOrder { get { return 0; } }
                public bool IsStaticRender { get { return true; } }
                public void Prepare(Camera camera, GizmosList<T> list) { }
                public void Render(Camera camera, GizmosList<T> list, CommandBuffer cb) { }
            }
            private static readonly bool _isUnmanaged = UnsafeUtility.IsUnmanaged(typeof(T));

            private StructList<GizmoInternal<T>> _gizmos = new StructList<GizmoInternal<T>>(32);
            private readonly string _debugName;

            private readonly RenderContext _context;
            private readonly CommandBuffer _staticCommandBuffer;
            //private readonly CommandBuffer _dynamicCommandBuffer;

            private readonly IGizmoRenderer<T> _renderer;
            private readonly IGizmoRenderer_PostRender<T> _rendererUnityGizmos;
            private readonly bool _isStatic;

#if DEV_MODE
            private static readonly Unity.Profiling.ProfilerMarker _timerMarker = new Unity.Profiling.ProfilerMarker($"{DebugXUtility.GetGenericTypeName(typeof(T), 3, false)}.{nameof(UpdateTimer)}");
            private static readonly Unity.Profiling.ProfilerMarker _prepareMarker = new Unity.Profiling.ProfilerMarker($"{DebugXUtility.GetGenericTypeName(typeof(T), 3, false)}.{nameof(Prepare)}");
            private static readonly Unity.Profiling.ProfilerMarker _renderMarker = new Unity.Profiling.ProfilerMarker($"{DebugXUtility.GetGenericTypeName(typeof(T), 3, false)}.{nameof(Render)}");
#endif

            public override int ExecuteOrder { get { return _renderer.ExecuteOrder; } }


            public GizmosBuffer(RenderContext context, CommandBuffer cb)
            {
                Type type = typeof(T);
                _context = context;
                _staticCommandBuffer = cb;
                _debugName = DebugXUtility.GetGenericTypeName(type, 3, false);
                //_dynamicCommandBuffer = new CommandBuffer();
                //_dynamicCommandBuffer.name = _debugName;

                _renderer = default(T).RegisterNewRenderer();
                if (_renderer == null)
                {
                    _renderer = new DummyRenderer();
                }
                _isStatic = _renderer.IsStaticRender;
                _rendererUnityGizmos = _renderer as IGizmoRenderer_PostRender<T>;

                All.Add(this);
                All.Sort((a, b) => a.ExecuteOrder - b.ExecuteOrder);

            }
            [IN(LINE)]
            public void Add(T value, float time, Color color)
            {
                CheckAvailablePoolMemory();
                if ((_gizmos.Count + 1) >= _bufferMaxSize) { return; }

                _gizmos.Add(new GizmoInternal<T>(value, time, color));
            }
            [IN(LINE)]
            public void AddRange(ReadOnlySpan<T> values, float time, Color color)
            {
                CheckAvailablePoolMemory();
                if ((_gizmos.Count + values.Length) >= _bufferMaxSize) { return; }

                _gizmos.UpSize(_gizmos._count + values.Length);
                for (int i = 0; i < values.Length; i++)
                {
                    _gizmos.Add(new GizmoInternal<T>(values[i], time, color));
                }
            }
            

            public sealed override int UpdateTimer(float deltaTime)
            {
                _staticCommandBuffer.Clear();
                int removeCount = 0;
#if DEV_MODE
                using (_timerMarker.Auto())
#endif
                {
                    for (int i = 0; i < _gizmos.Count; i++)
                    {
                        ref var item = ref _gizmos._items[i];
                        if (item.Timer <= 0)
                        {
                            _gizmos.FastRemoveAt(i);
                            i--;
                            removeCount++;
                            continue;
                        }
                        item.Timer -= deltaTime;
                    }
                }
                return removeCount;
            }

            public sealed override int RunEnd()
            {
                int removeCount = 0;
#if DEV_MODE
                using (_timerMarker.Auto())
#endif
                {
                    int lastCount = _gizmos.Count;
                    for (int i = 0; i < _gizmos.Count; i++)
                    {
                        ref var item = ref _gizmos._items[i];
                        if (item.Timer < 0)
                        {
                            _gizmos.FastRemoveAt(i);
                            i--;
                            removeCount++;
                            continue;
                        }
                    }
                    //микрооптимизация, удаляются елементы без зануления, и тут зануляются только managed типы
                    if (_isUnmanaged == false)
                    {
                        for (int i = _gizmos.Count; i < lastCount; i++)
                        {
                            _gizmos._items[i] = default;
                        }
                    }
                }
                return removeCount;
            }


            private ulong _prepareLastRenderTicks = 0;
            public override void Prepare()
            {
                if (_gizmos.Count <= 0) { return; }
#if DEV_MODE
                using (_prepareMarker.Auto())
#endif
                {
                    if (_isStatic == false || _prepareLastRenderTicks != _renderTicks)
                    {
                        _prepareLastRenderTicks = _renderTicks;
                        GizmosList<T> list = GizmosList.From(_gizmos._items, _gizmos._count);
                        try
                        {
                            _renderer.Prepare(GetCurrentCamera(), list);
                        }
                        catch (Exception e) { throw new Exception($"[{_debugName}] [Prepare] ", e); }
                    }
                }
            }
            private ulong _renderLastRenderTicks = 0;
            public override void Render(ICommandBufferExecutor cbExecutor)
            {
                if (_gizmos.Count <= 0) { return; }
#if DEV_MODE
                using (_renderMarker.Auto())
#endif
                {
                    //if (_isStatic == false || _renderLastRenderTicks != _renderTicks)
                    {
                        _renderLastRenderTicks = _renderTicks;
                        GizmosList<T> list = GizmosList.From(_gizmos._items, _gizmos._count);
                        try
                        {
                            _staticCommandBuffer.Clear();
                            _renderer.Render(GetCurrentCamera(), list, _staticCommandBuffer);
                        }
                        catch (Exception e) { throw new Exception($"[{_debugName}] [Render] ", e); }
                    }
                    cbExecutor.Execute(_staticCommandBuffer);
                }
            }
            public override void PostRender()
            {
                if (_rendererUnityGizmos == null) { return; }
                //Debug.Log(_gizmos._count);
                if (_gizmos.Count <= 0) { return; }
#if DEV_MODE
                using (_renderMarker.Auto())
#endif
                {
                    GizmosList<T> list = GizmosList.From(_gizmos._items, _gizmos._count);
                    try
                    {
                        _rendererUnityGizmos.PostRender(GetCurrentCamera(), list);
                    }
                    catch (Exception e) { throw new Exception($"[{_debugName}] [Render] ", e); }
                }
            }

            public override void Clear()
            {
                if (_isUnmanaged)
                {
                    _gizmos.FastClear();
                }
                else
                {
                    _gizmos.Clear();
                }
            }
        }
        #endregion
    }
}