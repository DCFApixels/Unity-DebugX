#undef DEBUG

using System;
using Unity.Collections.LowLevel.Unsafe;

namespace DCFApixels.DebugXCore.Internal
{
    internal static class DummyArray<T>
    {
        private readonly static T[] _array = new T[2];
        public static T[] Get()
        {
            return _array;
        }
    }
    internal unsafe readonly struct PinnedArray<T> : IDisposable where T : unmanaged
    {
        public readonly T[] Array;
        public readonly T* Ptr;
        public readonly ulong Handle;
        public PinnedArray(T[] array, T* ptr, ulong handle)
        {
            Array = array;
            Ptr = ptr;
            Handle = handle;
        }
        public static PinnedArray<T> Pin(T[] array)
        {
            return new PinnedArray<T>(array, (T*)UnsafeUtility.PinGCArrayAndGetDataAddress(array, out ulong handle), handle);
        }
        public void Dispose()
        {
            if (Ptr != null)
            {
                UnsafeUtility.ReleaseGCObject(Handle);
            }
        }
        public PinnedArray<U> As<U>() where U : unmanaged
        {
            T[] array = Array;
            U[] newArray = UnsafeUtility.As<T[], U[]>(ref array);
            return new PinnedArray<U>(newArray, (U*)Ptr, Handle);
        }
    }
}