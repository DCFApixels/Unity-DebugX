using System;

namespace DCFApixels.DebugXCore
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class DebugXDrawGizmoAttribute : Attribute
    {
        public Type Type;
        public DebugXDrawGizmoAttribute(Type type)
        {
            Type = type;
        }
    }
}