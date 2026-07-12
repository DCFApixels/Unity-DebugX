#undef DEBUG
using System;
using System.Runtime.InteropServices;
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
    internal readonly struct PinnedArrayHandle : IDisposable
    {
#if UNITY_6000_6_OR_NEWER
        public readonly GCHandle Handle;
        private PinnedArrayHandle(GCHandle handle)
        {
            Handle = handle;
        }
        public static IntPtr Pin(Array array, out PinnedArrayHandle handle)
        {
            var h = GCHandle.Alloc(array, GCHandleType.Pinned);
            handle = new PinnedArrayHandle(h);
            return h.AddrOfPinnedObject();
        }
        public void Dispose()
        {
            Handle.Free();
        }
#else
        public readonly ulong Handle;
        private PinnedArrayHandle(ulong handle)
        {
            Handle = handle;
        }
        public unsafe static IntPtr Pin(Array array, out PinnedArrayHandle handle)
        {
            var ptr = UnsafeUtility.PinGCArrayAndGetDataAddress(array, out ulong rawHandle);
            handle = new PinnedArrayHandle(rawHandle);
            return (IntPtr)ptr;
        }
        public void Dispose()
        {
            UnsafeUtility.ReleaseGCObject(Handle);
        }
#endif
    }
    internal unsafe readonly struct PinnedArray<T> : IDisposable where T : unmanaged
    {
        public readonly T[] Array;
        public readonly T* Ptr;
        public readonly PinnedArrayHandle Handle;
        public PinnedArray(T[] array, T* ptr, PinnedArrayHandle handle)
        {
            Array = array;
            Ptr = ptr;
            Handle = handle;
        }
        public static PinnedArray<T> Pin(T[] array)
        {
            var ptr = PinnedArrayHandle.Pin(array, out var handle);
            return new PinnedArray<T>(array, (T*)ptr, handle);
        }
        public void Dispose()
        {
            if (Ptr != null)
            {
                Handle.Dispose();
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