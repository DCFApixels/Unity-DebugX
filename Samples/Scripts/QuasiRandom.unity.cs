#if UNITY_5_3_OR_NEWER
using System.Runtime.CompilerServices;
using UnityEngine;

namespace DCFApixels
{
    using IN = MethodImplAttribute;

    internal partial struct QuasiRandom
    {
        #region Vectors
        public partial struct Bool2
        {
            [IN(LINE)] public static explicit operator Vector2(Bool2 a) => new Vector2(a.X ? 1f : 0f, a.Y ? 1f : 0f);
            [IN(LINE)] public static explicit operator Vector3(Bool2 a) => new Vector3(a.X ? 1f : 0f, a.Y ? 1f : 0f);
            [IN(LINE)] public static explicit operator Vector4(Bool2 a) => new Vector4(a.X ? 1f : 0f, a.Y ? 1f : 0f);
            [IN(LINE)] public static explicit operator Vector2Int(Bool2 a) => new Vector2Int(a.X ? 1 : 0, a.Y ? 1 : 0);
            [IN(LINE)] public static explicit operator Vector3Int(Bool2 a) => new Vector3Int(a.X ? 1 : 0, a.Y ? 1 : 0);
        }
        public partial struct Bool3
        {
            [IN(LINE)] public static explicit operator Vector3(Bool3 a) => new Vector3(a.X ? 1f : 0f, a.Y ? 1f : 0f, a.Z ? 1f : 0f);
            [IN(LINE)] public static explicit operator Vector4(Bool3 a) => new Vector4(a.X ? 1f : 0f, a.Y ? 1f : 0f, a.Z ? 1f : 0f);
            [IN(LINE)] public static explicit operator Vector3Int(Bool3 a) => new Vector3Int(a.X ? 1 : 0, a.Y ? 1 : 0, a.Z ? 1 : 0);
        }
        public partial struct Bool4
        {
            [IN(LINE)] public static explicit operator Vector4(Bool4 a) => new Vector4(a.X ? 1f : 0f, a.Y ? 1f : 0f, a.Z ? 1f : 0f, a.W ? 1f : 0f);
        }

        public partial struct Float2
        {
            [IN(LINE)] public static implicit operator Vector2(Float2 a) => new Vector2(a.X, a.Y);
            [IN(LINE)] public static implicit operator Vector3(Float2 a) => new Vector3(a.X, a.Y);
            [IN(LINE)] public static implicit operator Vector4(Float2 a) => new Vector4(a.X, a.Y);
            [IN(LINE)] public static explicit operator Vector2Int(Float2 a) => new Vector2Int((int)a.X, (int)a.Y);
            [IN(LINE)] public static explicit operator Vector3Int(Float2 a) => new Vector3Int((int)a.X, (int)a.Y);
        }
        public partial struct Float3
        {
            [IN(LINE)] public static implicit operator Vector3(Float3 a) => new Vector3(a.X, a.Y, a.Z);
            [IN(LINE)] public static implicit operator Vector4(Float3 a) => new Vector4(a.X, a.Y, a.Z);
            [IN(LINE)] public static explicit operator Vector3Int(Float3 a) => new Vector3Int((int)a.X, (int)a.Y, (int)a.Z);
        }
        public partial struct Float4
        {
            [IN(LINE)] public static implicit operator Vector4(Float4 a) => new Vector4(a.X, a.Y, a.Z, a.W);
        }

        public partial struct Double2
        {
            [IN(LINE)] public static explicit operator Vector2(Double2 a) => new Vector2((float)a.X, (float)a.Y);
            [IN(LINE)] public static explicit operator Vector3(Double2 a) => new Vector3((float)a.X, (float)a.Y);
            [IN(LINE)] public static explicit operator Vector4(Double2 a) => new Vector4((float)a.X, (float)a.Y);
            [IN(LINE)] public static explicit operator Vector2Int(Double2 a) => new Vector2Int((int)a.X, (int)a.Y);
            [IN(LINE)] public static explicit operator Vector3Int(Double2 a) => new Vector3Int((int)a.X, (int)a.Y);
        }
        public partial struct Double3
        {
            [IN(LINE)] public static explicit operator Vector3(Double3 a) => new Vector3((float)a.X, (float)a.Y, (float)a.Z);
            [IN(LINE)] public static explicit operator Vector4(Double3 a) => new Vector4((float)a.X, (float)a.Y, (float)a.Z);
            [IN(LINE)] public static explicit operator Vector3Int(Double3 a) => new Vector3Int((int)a.X, (int)a.Y, (int)a.Z);
        }
        public partial struct Double4
        {
            [IN(LINE)] public static explicit operator Vector4(Double4 a) => new Vector4((float)a.X, (float)a.Y, (float)a.Z, (float)a.W);
        }

        public partial struct Int2
        {
            [IN(LINE)] public static explicit operator Vector2(Int2 a) => new Vector2(a.X, a.Y);
            [IN(LINE)] public static explicit operator Vector3(Int2 a) => new Vector3(a.X, a.Y);
            [IN(LINE)] public static explicit operator Vector4(Int2 a) => new Vector4(a.X, a.Y);
            [IN(LINE)] public static implicit operator Vector2Int(Int2 a) => new Vector2Int(a.X, a.Y);
            [IN(LINE)] public static implicit operator Vector3Int(Int2 a) => new Vector3Int(a.X, a.Y);
        }
        public partial struct Int3
        {
            [IN(LINE)] public static explicit operator Vector3(Int3 a) => new Vector3(a.X, a.Y, a.Z);
            [IN(LINE)] public static explicit operator Vector4(Int3 a) => new Vector4(a.X, a.Y, a.Z);
            [IN(LINE)] public static implicit operator Vector3Int(Int3 a) => new Vector3Int(a.X, a.Y, a.Z);
        }
        public partial struct Int4
        {
            [IN(LINE)] public static implicit operator Vector4(Int4 a) => new Vector4(a.X, a.Y, a.Z, a.W);
        }

        public partial struct UInt2
        {
            [IN(LINE)] public static explicit operator Vector2(UInt2 a) => new Vector2(a.X, a.Y);
            [IN(LINE)] public static explicit operator Vector3(UInt2 a) => new Vector3(a.X, a.Y);
            [IN(LINE)] public static explicit operator Vector4(UInt2 a) => new Vector4(a.X, a.Y);
            [IN(LINE)] public static explicit operator Vector2Int(UInt2 a) => new Vector2Int((int)a.X, (int)a.Y);
            [IN(LINE)] public static explicit operator Vector3Int(UInt2 a) => new Vector3Int((int)a.X, (int)a.Y);
        }
        public partial struct UInt3
        {
            [IN(LINE)] public static explicit operator Vector3(UInt3 a) => new Vector3(a.X, a.Y, a.Z);
            [IN(LINE)] public static explicit operator Vector4(UInt3 a) => new Vector4(a.X, a.Y, a.Z);
            [IN(LINE)] public static explicit operator Vector3Int(UInt3 a) => new Vector3Int((int)a.X, (int)a.Y, (int)a.Z);
        }
        public partial struct UInt4
        {
            [IN(LINE)] public static implicit operator Vector4(UInt4 a) => new Vector4(a.X, a.Y, a.Z, a.W);
        }

        public partial struct Long2
        {
            [IN(LINE)] public static explicit operator Vector2(Long2 a) => new Vector2(a.X, a.Y);
            [IN(LINE)] public static explicit operator Vector3(Long2 a) => new Vector3(a.X, a.Y);
            [IN(LINE)] public static explicit operator Vector4(Long2 a) => new Vector4(a.X, a.Y);
            [IN(LINE)] public static explicit operator Vector2Int(Long2 a) => new Vector2Int((int)a.X, (int)a.Y);
            [IN(LINE)] public static explicit operator Vector3Int(Long2 a) => new Vector3Int((int)a.X, (int)a.Y);
        }
        public partial struct Long3
        {
            [IN(LINE)] public static explicit operator Vector3(Long3 a) => new Vector3(a.X, a.Y, a.Z);
            [IN(LINE)] public static explicit operator Vector4(Long3 a) => new Vector4(a.X, a.Y, a.Z);
            [IN(LINE)] public static explicit operator Vector3Int(Long3 a) => new Vector3Int((int)a.X, (int)a.Y, (int)a.Z);
        }
        public partial struct Long4
        {
            [IN(LINE)] public static explicit operator Vector4(Long4 a) => new Vector4(a.X, a.Y, a.Z, a.W);
        }

        public partial struct ULong2
        {
            [IN(LINE)] public static explicit operator Vector2(ULong2 a) => new Vector2(a.X, a.Y);
            [IN(LINE)] public static explicit operator Vector3(ULong2 a) => new Vector3(a.X, a.Y);
            [IN(LINE)] public static explicit operator Vector4(ULong2 a) => new Vector4(a.X, a.Y);
            [IN(LINE)] public static explicit operator Vector2Int(ULong2 a) => new Vector2Int((int)a.X, (int)a.Y);
            [IN(LINE)] public static explicit operator Vector3Int(ULong2 a) => new Vector3Int((int)a.X, (int)a.Y);
        }
        public partial struct ULong3
        {
            [IN(LINE)] public static explicit operator Vector3(ULong3 a) => new Vector3(a.X, a.Y, a.Z);
            [IN(LINE)] public static explicit operator Vector4(ULong3 a) => new Vector4(a.X, a.Y, a.Z);
            [IN(LINE)] public static explicit operator Vector3Int(ULong3 a) => new Vector3Int((int)a.X, (int)a.Y, (int)a.Z);
        }
        public partial struct ULong4
        {
            [IN(LINE)] public static explicit operator Vector4(ULong4 a) => new Vector4(a.X, a.Y, a.Z, a.W);
        }
        #endregion
    }
}
#endif