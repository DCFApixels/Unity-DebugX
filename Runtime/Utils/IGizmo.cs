using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace DCFApixels.DebugXCore
{
    public interface IGizmo<T> where T : IGizmo<T>
    {
        IGizmoRenderer<T> RegisterNewRenderer();
    }
    public interface IGizmoRenderer<T> where T : IGizmo<T>
    {
        int ExecuteOrder { get; }
        //Статик рендер означает что в рамках одного контекста для всех камер используется одинаковый набор команд в CommandBuffer
        //Поэтому Prepare и Render можно вызвать один раз, а не по разу на каждую камеру
        bool IsStaticRender { get; }
        void Prepare(Camera camera, GizmosList<T> list);
        void Render(Camera camera, GizmosList<T> list, CommandBuffer cb);
    }
    public interface IGizmoRenderer_PostRender<T> : IGizmoRenderer<T> where T : IGizmo<T>
    {
        void PostRender(Camera camera, GizmosList<T> list);
    }


    public readonly struct RenderContext : IEquatable<RenderContext>
    {
        public readonly Camera Camera;
        public bool IsStatic
        {
            get { return Camera == null || Camera.name == "SceneCamera"; }
        }
        public RenderContext(Camera camera)
        {
            Camera = camera;
        }
        public bool Equals(RenderContext other)
        {
            return Camera == other.Camera;
        }
        public override bool Equals(object obj)
        {
            return obj is RenderContext && Equals((RenderContext)obj);
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(Camera);
        }
    }
}