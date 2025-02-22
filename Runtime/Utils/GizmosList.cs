#undef DEBUG

#if DEBUG
#define DEV_MODE
#endif
using System;
using System.Diagnostics;
using Unity.Collections.LowLevel.Unsafe;
using static DCFApixels.DebugX;

namespace DCFApixels.DebugXCore
{
    using IN = System.Runtime.CompilerServices.MethodImplAttribute;

    [DebuggerDisplay("Count:{Count}")]
    public readonly ref struct GizmosList
    {
        public readonly Array Items;
        public readonly int Count;
        public GizmosList(Array items, int count)
        {
            Items = items;
            Count = count;
        }
        public static GizmosList ConvertFrom<T>(GizmosList<T> list)
        {
            return new GizmosList(list.Items, list.Count);
        }
        public GizmosList<T> As<T>()
        {
            var items = Items;
            return new GizmosList<T>(UnsafeUtility.As<Array, Gizmo<T>[]>(ref items), Count);
        }
        internal static GizmosList<T> From<T>(GizmoInternal<T>[] items, int count) where T : IGizmo<T>
        {
            return new GizmosList<T>(UnsafeUtility.As<GizmoInternal<T>[], Gizmo<T>[]>(ref items), count);
        }
    }
    [DebuggerDisplay("Count:{Count}")]
    public readonly ref struct GizmosList<T>
    {
        public readonly Gizmo<T>[] Items;
        public readonly int Count;
        [IN(LINE)]
        public GizmosList(Gizmo<T>[] items, int count)
        {
            Items = items;
            Count = count;
        }
        [IN(LINE)]
        public GizmosList(GizmosList list)
        {
            Items = (Gizmo<T>[])list.Items;
            Count = list.Count;
        }
        public ref Gizmo<T> this[int index]
        {
            [IN(LINE)]
            get { return ref Items[index]; }
        }
        public GizmosList<TOther> As<TOther>()
        {
            var items = Items;
            return new GizmosList<TOther>(UnsafeUtility.As<Gizmo<T>[], Gizmo<TOther>[]>(ref items), Count);
        }
        [IN(LINE)] public Enumerator GetEnumerator() { return new Enumerator(Items, Count); }
        public struct Enumerator
        {
            public readonly Gizmo<T>[] Items;
            public readonly int Count;
            private int _index;
            [IN(LINE)]
            public Enumerator(Gizmo<T>[] items, int count)
            {
                Items = items;
                Count = count;
                _index = -1;
            }
            public ref Gizmo<T> Current
            {
                [IN(LINE)]
                get => ref Items[_index];
            }
            [IN(LINE)] public bool MoveNext() { return ++_index < Count; }
        }

        public static implicit operator GizmosList(GizmosList<T> a) => GizmosList.ConvertFrom(a);
        public static explicit operator GizmosList<T>(GizmosList a) => new GizmosList<T>(a);
    }
}