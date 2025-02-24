#undef DEBUG

#if DEBUG
#define DEV_MODE
#endif
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections.LowLevel.Unsafe;
using static DCFApixels.DebugX;

namespace DCFApixels.DebugXCore.Internal
{
    using IN = System.Runtime.CompilerServices.MethodImplAttribute;
    internal interface IStructListElement<T>
    {
        void OnSwap(ref T element);
    }
    [System.Diagnostics.DebuggerDisplay("Count: {Count}")]
    internal struct StructList
    {
        internal Array _items;
        internal int _count;
        internal readonly bool _isUseArrayPool;
        public int Count
        {
            [IN(LINE)]
            get { return _count; }
        }
        private StructList(Array items, int count, bool isUseArrayPool)
        {
            _items = items;
            _count = count;
            _isUseArrayPool = isUseArrayPool;
        }
        public static StructList ConvertFrom<T>(StructList<T> list)
        {
            return new StructList(list._items, list._count, list._isUseArrayPool);
        }
    }
    [System.Diagnostics.DebuggerDisplay("Count: {Count}")]
    internal struct StructList<T> : IDisposable
    {
        //private struct Dummy : IStructListElement<T>
        //{
        //    public void OnSwap(ref T element) { }
        //}
        //private static IStructListElement<T> _internal = default(T) as IStructListElement<T> ?? default(Dummy);
        internal T[] _items;
        internal int _count;
        internal readonly bool _isUseArrayPool;
        private static readonly bool _isUnmanaged = UnsafeUtility.IsUnmanaged<T>();

        public bool IsCreated
        {
            [IN(LINE)]
            get { return _items != null; }
        }
        public int Count
        {
            [IN(LINE)]
            get { return _count; }
        }
        public int Capacity
        {
            [IN(LINE)]
            get { return _items.Length; }
            set
            {
                UpSize(value);
            }
        }
        public T this[int index]
        {
            [IN(LINE)]
            get
            {
#if DEV_MODE
                    if (index < 0 || index >= _count) { new ArgumentOutOfRangeException(); }
#endif
                return _items[index];
            }
            [IN(LINE)]
            set
            {
#if DEV_MODE
                    if (index < 0 || index >= _count) { new ArgumentOutOfRangeException(); }
#endif
                _items[index] = value;
            }
        }

        [IN(LINE)]
        public StructList(int minCapacity, bool useArrayPool = false)
        {
            minCapacity = NextPow2(minCapacity);
            if (useArrayPool)
            {
                _items = ArrayPool<T>.Shared.Rent(minCapacity);
            }
            else
            {
                _items = new T[minCapacity];
            }
            _count = 0;
            _isUseArrayPool = useArrayPool;
        }
        private StructList(StructList list)
        {
            _items = (T[])list._items;
            _count = list._count;
            _isUseArrayPool = list._isUseArrayPool;
        }

        [IN(LINE)]
        public void Add(T item)
        {
            UpSize(_count + 1);
            //_internal.OnSwap(ref item);
            _items[_count++] = item;
        }
        [IN(LINE)]
        public void AddRange(ReadOnlySpan<T> items)
        {
            UpSize(_count + items.Length);
            for (int i = 0; i < items.Length; i++)
            {
                //_internal.OnSwap(ref item);
                _items[_count++] = items[i];
            }
        }
        public void UpSize(int newMinSize)
        {
            if (newMinSize <= _items.Length) { return; }
            newMinSize = NextPow2(newMinSize);
            if (newMinSize <= _items.Length) { return; }
            if (_isUseArrayPool)
            {
                var newItems = ArrayPool<T>.Shared.Rent(newMinSize);
                for (int i = 0, iMax = _count; i < iMax; i++)
                {
                    newItems[i] = _items[i];
                }
                ArrayPool<T>.Shared.Return(_items, !_isUnmanaged);
                _items = newItems;
            }
            else
            {
                Array.Resize(ref _items, newMinSize);
            }
        }
        [IN(LINE)]
        public int IndexOf(T item)
        {
            return Array.IndexOf(_items, item, 0, _count);
        }
        [IN(LINE)]
        public void SwapAt(int idnex1, int idnex2)
        {
            T tmp = _items[idnex1];
            _items[idnex1] = _items[idnex2];
            _items[idnex2] = tmp;
            //_internal.OnSwap(ref _items[idnex1]);
            //_internal.OnSwap(ref _items[idnex2]);
        }
        [IN(LINE)]
        public void FastRemoveAt(int index)
        {
#if DEV_MODE
                if (index < 0 || index >= _count) { new ArgumentOutOfRangeException(); }
#endif
            _items[index] = _items[--_count];
            //_internal.OnSwap(ref _items[index]);
        }
        [IN(LINE)]
        public void RemoveAt(int index)
        {
#if DEV_MODE
                if (index < 0 || index >= _count) { new ArgumentOutOfRangeException(); }
#endif
            _items[index] = _items[--_count];
            //_internal.OnSwap(ref _items[index]);
            _items[_count] = default;
        }
        [IN(LINE)]
        public void RemoveAtWithOrder(int index)
        {
#if DEV_MODE
                if (index < 0 || index >= _count) { new ArgumentOutOfRangeException(); }
#endif
            for (int i = index; i < _count; i++)
            {
                _items[i] = _items[i + 1];
                //_internal.OnSwap(ref _items[i]);
            }
        }
        [IN(LINE)]
        public bool Remove(T item)
        {
            int index = IndexOf(item);
            if (index >= 0)
            {
                RemoveAt(index);
                return true;
            }
            return false;
        }
        [IN(LINE)]
        public bool RemoveWithOrder(T item)
        {
            int index = IndexOf(item);
            if (index >= 0)
            {
                RemoveAtWithOrder(index);
                return true;
            }
            return false;
        }
        [IN(LINE)]
        public void FastClear()
        {
            _count = 0;
        }
        [IN(LINE)]
        public void Clear()
        {
            for (int i = 0; i < _count; i++)
            {
                _items[i] = default;
            }
            _count = 0;
        }
        [IN(LINE)]
        public ReadOnlySpan<T>.Enumerator GetEnumerator()
        {
            return new ReadOnlySpan<T>(_items, 0, _count).GetEnumerator();
        }
        [IN(LINE)]
        public Span<T> AsSpan()
        {
            return new Span<T>(_items, 0, _count);
        }
        [IN(LINE)]
        public ReadOnlySpan<T> AsReadOnlySpan()
        {
            return new ReadOnlySpan<T>(_items, 0, _count);
        }
        [IN(LINE)]
        public IEnumerable<T> ToEnumerable()
        {
            return _items.Take(_count);
        }
        [IN(LINE)]
        private static int NextPow2(int v)
        {
            v--;
            v |= v >> 1;
            v |= v >> 2;
            v |= v >> 4;
            v |= v >> 8;
            v |= v >> 16;
            return ++v;
        }

        public void Dispose()
        {
            if (_isUseArrayPool)
            {
                ArrayPool<T>.Shared.Return(_items);
            }
            _items = null;
            _count = 0;
        }

        public static implicit operator StructList(StructList<T> a) => StructList.ConvertFrom(a);
        public static explicit operator StructList<T>(StructList a) => new StructList<T>(a);
    }
}