using System;
using System.Collections.Generic;

namespace DCFApixels.DebugXCore.Internal
{
    internal interface IGizmoTypeCodeAddCallback
    {
        void OnAddGizmoTypeCode<T>() where T : IGizmo<T>;
    }
    internal static class GizmoTypeCode<T> where T : IGizmo<T>
    {
        public static readonly int ID = GizmoTypeCode.NewID();
    }
    internal static class GizmoTypeCode
    {
        private static int _increment = 0;
        private static readonly object _lock = new object();
        private static List<IGizmoTypeCodeAddCallback> _listeners = new List<IGizmoTypeCodeAddCallback>();
        public static int TypesCount
        {
            get { return _increment; }
        }
        public static int NewID()
        {
            lock (_lock)
            {
                _increment++;
                OnAddNewID(_increment);
                return _increment - 1;
            }
        }
        public static event Action<int> OnAddNewID = delegate { };
        public static void AddListener(IGizmoTypeCodeAddCallback listener)
        {
            _listeners.Add(listener);
        }
        public static void RemoveListener(IGizmoTypeCodeAddCallback listener)
        {
            _listeners.Remove(listener);
        }
    }
}