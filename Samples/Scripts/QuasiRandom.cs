using System;
using System.ComponentModel;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using q32 = System.UInt32;
using q64 = System.UInt64;

namespace DCFApixels
{
    using IN = MethodImplAttribute;
    /// <summary>Quasi Random. Use new additive recursive R-sequence.</summary>
    [Serializable]
    [DataContract]
    internal partial struct QuasiRandom : IEquatable<QuasiRandom>, IFormattable
    {
        private const MethodImplOptions LINE = MethodImplOptions.AggressiveInlining;

        #region Consts
        private const uint UINT_HIBIT = 0x8000_0000;

        private const q32 Q32_MAX = q32.MaxValue;
        private const q64 Q64_MAX = q64.MaxValue;

        private const decimal G1 = 1.6180339887498948482045868383m;
        private const q32 X1_Q32 = (q32)(1m / G1 * Q32_MAX) + 1;
        private const q64 X1_Q64 = (q64)(1m / G1 * Q64_MAX) + 1;

        private const decimal G2 = 1.3247179572447460259609088563m;
        private const q32 X2_Q32 = (q32)(1m / G2 * Q32_MAX) + 1;
        private const q32 Y2_Q32 = (q32)(1m / (G2 * G2) * Q32_MAX) + 1;
        private const q64 X2_Q64 = (q64)(1m / G2 * Q64_MAX) + 1;
        private const q64 Y2_Q64 = (q64)(1m / (G2 * G2) * Q64_MAX) + 1;

        private const decimal G3 = 1.2207440846057594753616853503m;
        private const q32 X3_Q32 = (q32)(1m / G3 * Q32_MAX) + 1;
        private const q32 Y3_Q32 = (q32)(1m / (G3 * G3) * Q32_MAX) + 1;
        private const q32 Z3_Q32 = (q32)(1m / (G3 * G3 * G3) * Q32_MAX) + 1;
        private const q64 X3_Q64 = (q64)(1m / G3 * Q64_MAX) + 1;
        private const q64 Y3_Q64 = (q64)(1m / (G3 * G3) * Q64_MAX) + 1;
        private const q64 Z3_Q64 = (q64)(1m / (G3 * G3 * G3) * Q64_MAX) + 1;

        private const decimal G4 = 1.1673039782614186842560459007m;
        private const q32 X4_Q32 = (q32)(1m / G4 * Q32_MAX) + 1;
        private const q32 Y4_Q32 = (q32)(1m / (G4 * G4) * Q32_MAX) + 1;
        private const q32 Z4_Q32 = (q32)(1m / (G4 * G4 * G4) * Q32_MAX) + 1;
        private const q32 W4_Q32 = (q32)(1m / (G4 * G4 * G4 * G4) * Q32_MAX) + 1;
        private const q64 X4_Q64 = (q64)(1m / G4 * Q64_MAX) + 1;
        private const q64 Y4_Q64 = (q64)(1m / (G4 * G4) * Q64_MAX) + 1;
        private const q64 Z4_Q64 = (q64)(1m / (G4 * G4 * G4) * Q64_MAX) + 1;
        private const q64 W4_Q64 = (q64)(1m / (G4 * G4 * G4 * G4) * Q64_MAX) + 1;
        #endregion

        #region Math
        /*----------------------------------------------------------------------------------------------------------------*/

        [IN(LINE)] private readonly uint GetX1_Q32(uint state) => X1_Q32 * state;
        [IN(LINE)] private readonly ulong GetX1_Q64(uint state) => X1_Q64 * state;

        [IN(LINE)] private readonly float GetX1_F(uint state) => Q32ToFloat(GetX1_Q32(state));
        [IN(LINE)] private readonly double GetX1_D(uint state) => Q64ToDouble(GetX1_Q64(state));

        /*----------------------------------------------------------------------------------------------------------------*/

        [IN(LINE)] private readonly uint GetX2_Q32(uint state) => X2_Q32 * state;
        [IN(LINE)] private readonly uint GetY2_Q32(uint state) => Y2_Q32 * state;

        [IN(LINE)] private readonly ulong GetX2_Q64(uint state) => X2_Q64 * state;
        [IN(LINE)] private readonly ulong GetY2_Q64(uint state) => Y2_Q64 * state;

        [IN(LINE)] private readonly float GetX2_F(uint state) => Q32ToFloat(GetX2_Q32(state));
        [IN(LINE)] private readonly float GetY2_F(uint state) => Q32ToFloat(GetY2_Q32(state));

        [IN(LINE)] private readonly double GetX2_D(uint state) => Q64ToDouble(GetX2_Q64(state));
        [IN(LINE)] private readonly double GetY2_D(uint state) => Q64ToDouble(GetY2_Q64(state));

        /*----------------------------------------------------------------------------------------------------------------*/

        [IN(LINE)] private readonly uint GetX3_Q32(uint state) => X3_Q32 * state;
        [IN(LINE)] private readonly uint GetY3_Q32(uint state) => Y3_Q32 * state;
        [IN(LINE)] private readonly uint GetZ3_Q32(uint state) => Z3_Q32 * state;

        [IN(LINE)] private readonly ulong GetX3_Q64(uint state) => X3_Q64 * state;
        [IN(LINE)] private readonly ulong GetY3_Q64(uint state) => Y3_Q64 * state;
        [IN(LINE)] private readonly ulong GetZ3_Q64(uint state) => Z3_Q64 * state;

        [IN(LINE)] private readonly float GetX3_F(uint state) => Q32ToFloat(GetX3_Q32(state));
        [IN(LINE)] private readonly float GetY3_F(uint state) => Q32ToFloat(GetY3_Q32(state));
        [IN(LINE)] private readonly float GetZ3_F(uint state) => Q32ToFloat(GetZ3_Q32(state));

        [IN(LINE)] private readonly double GetX3_D(uint state) => Q64ToDouble(GetX3_Q64(state));
        [IN(LINE)] private readonly double GetY3_D(uint state) => Q64ToDouble(GetY3_Q64(state));
        [IN(LINE)] private readonly double GetZ3_D(uint state) => Q64ToDouble(GetZ3_Q64(state));

        /*----------------------------------------------------------------------------------------------------------------*/

        [IN(LINE)] private readonly uint GetX4_Q32(uint state) => X4_Q32 * state;
        [IN(LINE)] private readonly uint GetY4_Q32(uint state) => Y4_Q32 * state;
        [IN(LINE)] private readonly uint GetZ4_Q32(uint state) => Z4_Q32 * state;
        [IN(LINE)] private readonly uint GetW4_Q32(uint state) => W4_Q32 * state;

        [IN(LINE)] private readonly ulong GetX4_Q64(uint state) => X4_Q64 * state;
        [IN(LINE)] private readonly ulong GetY4_Q64(uint state) => Y4_Q64 * state;
        [IN(LINE)] private readonly ulong GetZ4_Q64(uint state) => Z4_Q64 * state;
        [IN(LINE)] private readonly ulong GetW4_Q64(uint state) => W4_Q64 * state;

        [IN(LINE)] private readonly float GetX4_F(uint state) => Q32ToFloat(GetX4_Q32(state));
        [IN(LINE)] private readonly float GetY4_F(uint state) => Q32ToFloat(GetY4_Q32(state));
        [IN(LINE)] private readonly float GetZ4_F(uint state) => Q32ToFloat(GetZ4_Q32(state));
        [IN(LINE)] private readonly float GetW4_F(uint state) => Q32ToFloat(GetW4_Q32(state));

        [IN(LINE)] private readonly double GetX4_D(uint state) => Q64ToDouble(GetX4_Q64(state));
        [IN(LINE)] private readonly double GetY4_D(uint state) => Q64ToDouble(GetY4_Q64(state));
        [IN(LINE)] private readonly double GetZ4_D(uint state) => Q64ToDouble(GetZ4_Q64(state));
        [IN(LINE)] private readonly double GetW4_D(uint state) => Q64ToDouble(GetW4_Q64(state));

        /*----------------------------------------------------------------------------------------------------------------*/

        [IN(LINE)] private static unsafe float AsFloat(uint value) => *(float*)&value;
        [IN(LINE)] private static unsafe double AsDouble(ulong value) => *(double*)&value;

        [IN(LINE)] private static float Q32ToFloat(uint value) => AsFloat((value >> 9) | 0x3F80_0000) - 1f;
        [IN(LINE)] private static double Q64ToDouble(ulong value) => AsDouble((value >> 12) | 0x7FF0_0000_0000_0000) - 1d;

        [IN(LINE)] private static int Compresse32(uint value, int range) => (int)((value * (ulong)range) >> 32);
        [IN(LINE)] private static int Compresse32(uint value, ulong range) => (int)((value * range) >> 32);
        [IN(LINE)] private static int Compresse32(uint value, int min, ulong range) => (int)((value * range) >> 32) + min;

        [IN(LINE)] private static uint CompresseU32(uint value, uint range) => (uint)((value * (ulong)range) >> 32);
        [IN(LINE)] private static uint CompresseU32(uint value, ulong range) => (uint)((value * range) >> 32);
        [IN(LINE)] private static uint CompresseU32(uint value, uint min, ulong range) => (uint)((value * range) >> 32) + min;

        [IN(LINE)] private static long Compresse64(ulong value, long range) => (long)CompresseU64(value, (ulong)range);
        [IN(LINE)] private static long Compresse64(ulong value, ulong range) => (long)CompresseU64(value, range);
        [IN(LINE)] private static long Compresse64(ulong value, long min, ulong range) => Compresse64(value, range) + min;

        [IN(LINE)] private static ulong CompresseU64(ulong value, ulong min, ulong range) => CompresseU64(value, range) + min;

        [IN(LINE)]
        private unsafe static ulong CompresseU64(ulong value, ulong range)
        {
            const int N = 2;
            const int M = 4;

            if (range <= uint.MaxValue && value <= uint.MaxValue)
            {
                return CompresseU32((uint)value, (uint)range);
            }


            uint* v_bits = (uint*)&value;
            uint* r_bits = (uint*)&range;

            uint* mulInts = stackalloc uint[M];

            for (int i = 0; i < N; i++)
            {
                int index = i;
                ulong remainder = 0;

                for (int j = 0; j < N; j++)
                {
                    uint yi = r_bits[j];
                    remainder = remainder + (ulong)v_bits[i] * yi + mulInts[index];
                    mulInts[index++] = (uint)remainder;
                    remainder = remainder >> 32;
                }

                while (remainder != 0)
                {
                    remainder += mulInts[index];
                    mulInts[index++] = (uint)remainder;
                    remainder = remainder >> 32;
                }
            }

            return (((ulong)mulInts[3]) << 32) + mulInts[2];
        }

        /*----------------------------------------------------------------------------------------------------------------*/
        #endregion

#if UNITY_5_3_OR_NEWER
        [UnityEngine.SerializeField]
#endif
        [DataMember]
        private uint _state;

        #region Constructors
        [IN(LINE)]
        public QuasiRandom(int seed)
        {
            _state = (uint)seed;
        }
        [IN(LINE)]
        public QuasiRandom(uint seed)
        {
            _state = seed;
        }
        public static QuasiRandom AutoSeed()
        {
            uint seed = (uint)DateTime.Now.Ticks;
            seed ^= seed << 13;
            seed ^= seed >> 17;
            seed ^= seed << 5;
            return new QuasiRandom(seed);
        }
        #endregion


        #region Bool
        [IN(LINE)] public bool NextBool() => (GetX1_Q32(++_state) & UINT_HIBIT) > 0;

        [IN(LINE)]
        public void NextBool2(out bool x, out bool y)
        {
            x = (GetX2_Q32(++_state) & UINT_HIBIT) > 0;
            y = (GetY2_Q32(_state) & UINT_HIBIT) > 0;
        }
        [IN(LINE)]
        public void NextBool3(out bool x, out bool y, out bool z)
        {
            x = (GetX3_Q32(++_state) & UINT_HIBIT) > 0;
            y = (GetY3_Q32(_state) & UINT_HIBIT) > 0;
            z = (GetZ3_Q32(_state) & UINT_HIBIT) > 0;
        }
        [IN(LINE)]
        public void NextBool4(out bool x, out bool y, out bool z, out bool w)
        {
            x = (GetX4_Q32(++_state) & UINT_HIBIT) > 0;
            y = (GetY4_Q32(_state) & UINT_HIBIT) > 0;
            z = (GetZ4_Q32(_state) & UINT_HIBIT) > 0;
            w = (GetW4_Q32(_state) & UINT_HIBIT) > 0;
        }

        [IN(LINE)]
        public Bool2 NextBool2()
        {
            return new Bool2(
                (GetX2_Q32(++_state) & UINT_HIBIT) > 0,
                (GetY2_Q32(_state) & UINT_HIBIT) > 0);
        }
        [IN(LINE)]
        public Bool3 NextBool3()
        {
            return new Bool3(
                (GetX3_Q32(++_state) & UINT_HIBIT) > 0,
                (GetY3_Q32(_state) & UINT_HIBIT) > 0,
                (GetZ3_Q32(_state) & UINT_HIBIT) > 0);
        }
        [IN(LINE)]
        public Bool4 NextBool4()
        {
            return new Bool4(
                (GetX4_Q32(++_state) & UINT_HIBIT) > 0,
                (GetY4_Q32(_state) & UINT_HIBIT) > 0,
                (GetZ4_Q32(_state) & UINT_HIBIT) > 0,
                (GetW4_Q32(_state) & UINT_HIBIT) > 0);
        }
        #endregion

        #region Int
        [IN(LINE)] public int NextInt() => (int)(GetX1_Q32(++_state) >> 1);
        [IN(LINE)] public int NextInt(int max) => Compresse32(GetX1_Q32(++_state), max);
        [IN(LINE)] public int NextInt(int min, int max) => Compresse32(GetX1_Q32(++_state), min, (ulong)(max - min));

        [IN(LINE)]
        public void NextInt2(out int x, out int y)
        {
            x = (int)(GetX2_Q32(++_state) >> 1);
            y = (int)(GetY2_Q32(_state) >> 1);
        }
        [IN(LINE)]
        public void NextInt2(int max, out int x, out int y)
        {
            x = Compresse32(GetX2_Q32(++_state), max);
            y = Compresse32(GetY2_Q32(_state), max);
        }
        [IN(LINE)]
        public void NextInt2(int min, int max, out int x, out int y)
        {
            ulong range = (ulong)(max - min);
            x = Compresse32(GetX2_Q32(++_state), min, range);
            y = Compresse32(GetY2_Q32(_state), min, range);
        }

        [IN(LINE)]
        public void NextInt3(out int x, out int y, out int z)
        {
            x = (int)(GetX3_Q32(++_state) >> 1);
            y = (int)(GetY3_Q32(_state) >> 1);
            z = (int)(GetZ3_Q32(_state) >> 1);
        }
        [IN(LINE)]
        public void NextInt3(int max, out int x, out int y, out int z)
        {
            x = Compresse32(GetX3_Q32(++_state), max);
            y = Compresse32(GetY3_Q32(_state), max);
            z = Compresse32(GetZ3_Q32(_state), max);
        }
        [IN(LINE)]
        public void NextInt3(int min, int max, out int x, out int y, out int z)
        {
            ulong range = (ulong)(max - min);
            x = Compresse32(GetX3_Q32(++_state), min, range);
            y = Compresse32(GetY3_Q32(_state), min, range);
            z = Compresse32(GetZ3_Q32(_state), min, range);
        }

        [IN(LINE)]
        public void NextInt4(out int x, out int y, out int z, out int w)
        {
            x = (int)(GetX4_Q32(++_state) >> 1);
            y = (int)(GetY4_Q32(_state) >> 1);
            z = (int)(GetZ4_Q32(_state) >> 1);
            w = (int)(GetW4_Q32(_state) >> 1);
        }
        [IN(LINE)]
        public void NextInt4(int max, out int x, out int y, out int z, out int w)
        {
            x = Compresse32(GetX4_Q32(++_state), max);
            y = Compresse32(GetY4_Q32(_state), max);
            z = Compresse32(GetZ4_Q32(_state), max);
            w = Compresse32(GetW4_Q32(_state), max);
        }
        [IN(LINE)]
        public void NextInt4(int min, int max, out int x, out int y, out int z, out int w)
        {
            ulong range = (ulong)(max - min);
            x = Compresse32(GetX4_Q32(++_state), min, range);
            y = Compresse32(GetY4_Q32(_state), min, range);
            z = Compresse32(GetZ4_Q32(_state), min, range);
            w = Compresse32(GetW4_Q32(_state), min, range);
        }

        [IN(LINE)]
        public Int2 NextInt2()
        {
            return new Int2(
                (int)(GetX2_Q32(++_state) >> 1),
                (int)(GetY2_Q32(_state) >> 1));
        }
        [IN(LINE)]
        public Int2 NextInt2(int max)
        {
            return new Int2(
                Compresse32(GetX2_Q32(++_state), max),
                Compresse32(GetY2_Q32(_state), max));
        }
        [IN(LINE)]
        public Int2 NextInt2(int min, int max)
        {
            ulong range = (ulong)(max - min);
            return new Int2(
                Compresse32(GetX2_Q32(++_state), min, range),
                Compresse32(GetY2_Q32(_state), min, range));
        }

        [IN(LINE)]
        public Int3 NextInt3()
        {
            return new Int3(
                (int)(GetX3_Q32(++_state) >> 1),
                (int)(GetY3_Q32(_state) >> 1),
                (int)(GetZ3_Q32(_state) >> 1));
        }
        [IN(LINE)]
        public Int3 NextInt3(int max)
        {
            return new Int3(
                Compresse32(GetX3_Q32(++_state), max),
                Compresse32(GetY3_Q32(_state), max),
                Compresse32(GetZ3_Q32(_state), max));
        }
        [IN(LINE)]
        public Int3 NextInt3(int min, int max)
        {
            ulong range = (ulong)(max - min);
            return new Int3(
                Compresse32(GetX3_Q32(++_state), min, range),
                Compresse32(GetY3_Q32(_state), min, range),
                Compresse32(GetZ3_Q32(_state), min, range));
        }

        [IN(LINE)]
        public Int4 NextInt4()
        {
            return new Int4(
                (int)(GetX4_Q32(++_state) >> 1),
                (int)(GetY4_Q32(_state) >> 1),
                (int)(GetZ4_Q32(_state) >> 1),
                (int)(GetW4_Q32(_state) >> 1));
        }
        [IN(LINE)]
        public Int4 NextInt4(int max)
        {
            return new Int4(
                Compresse32(GetX4_Q32(++_state), max),
                Compresse32(GetY4_Q32(_state), max),
                Compresse32(GetZ4_Q32(_state), max),
                Compresse32(GetW4_Q32(_state), max));
        }
        [IN(LINE)]
        public Int4 NextInt4(int min, int max)
        {
            ulong range = (ulong)(max - min);
            return new Int4(
                Compresse32(GetX4_Q32(++_state), min, range),
                Compresse32(GetY4_Q32(_state), min, range),
                Compresse32(GetZ4_Q32(_state), min, range),
                Compresse32(GetW4_Q32(_state), min, range));
        }
        #endregion

        #region UInt
        [IN(LINE)] public uint NextUInt() => GetX1_Q32(++_state);
        [IN(LINE)] public uint NextUInt(uint max) => CompresseU32(GetX1_Q32(++_state), max);
        [IN(LINE)] public uint NextUInt(uint min, uint max) => CompresseU32(GetX1_Q32(++_state), min, max - min);

        [IN(LINE)]
        public void NextUInt2(out uint x, out uint y)
        {
            x = GetX2_Q32(++_state);
            y = GetY2_Q32(_state);
        }
        [IN(LINE)]
        public void NextUInt2(uint max, out uint x, out uint y)
        {
            x = CompresseU32(GetX2_Q32(++_state), max);
            y = CompresseU32(GetY2_Q32(_state), max);
        }
        [IN(LINE)]
        public void NextUInt2(uint min, uint max, out uint x, out uint y)
        {
            ulong range = max - min;
            x = CompresseU32(GetX2_Q32(++_state), min, range);
            y = CompresseU32(GetY2_Q32(_state), min, range);
        }

        [IN(LINE)]
        public void NextUInt3(out uint x, out uint y, out uint z)
        {
            x = GetX3_Q32(++_state);
            y = GetY3_Q32(_state);
            z = GetZ3_Q32(_state);
        }
        [IN(LINE)]
        public void NextUInt3(uint max, out uint x, out uint y, out uint z)
        {
            x = CompresseU32(GetX3_Q32(++_state), max);
            y = CompresseU32(GetY3_Q32(_state), max);
            z = CompresseU32(GetZ3_Q32(_state), max);
        }
        [IN(LINE)]
        public void NextUInt3(uint min, uint max, out uint x, out uint y, out uint z)
        {
            ulong range = max - min;
            x = CompresseU32(GetX3_Q32(++_state), min, range);
            y = CompresseU32(GetY3_Q32(_state), min, range);
            z = CompresseU32(GetZ3_Q32(_state), min, range);
        }

        [IN(LINE)]
        public void NextUInt4(out uint x, out uint y, out uint z, out uint w)
        {
            x = GetX4_Q32(++_state);
            y = GetY4_Q32(_state);
            z = GetZ4_Q32(_state);
            w = GetW4_Q32(_state);
        }
        [IN(LINE)]
        public void NextUInt4(uint max, out uint x, out uint y, out uint z, out uint w)
        {
            x = CompresseU32(GetX4_Q32(++_state), max);
            y = CompresseU32(GetY4_Q32(_state), max);
            z = CompresseU32(GetZ4_Q32(_state), max);
            w = CompresseU32(GetW4_Q32(_state), max);
        }
        [IN(LINE)]
        public void NextUInt4(uint min, uint max, out uint x, out uint y, out uint z, out uint w)
        {
            ulong range = (ulong)(max - min);
            x = CompresseU32(GetX4_Q32(++_state), min, range);
            y = CompresseU32(GetY4_Q32(_state), min, range);
            z = CompresseU32(GetZ4_Q32(_state), min, range);
            w = CompresseU32(GetW4_Q32(_state), min, range);
        }

        [IN(LINE)]
        public UInt2 NextUInt2()
        {
            return new UInt2(
                GetX2_Q32(++_state),
                GetY2_Q32(_state));
        }
        [IN(LINE)]
        public UInt2 NextUInt2(uint max)
        {
            return new UInt2(
                CompresseU32(GetX2_Q32(++_state), max),
                CompresseU32(GetY2_Q32(_state), max));
        }
        [IN(LINE)]
        public UInt2 NextUInt2(uint min, uint max)
        {
            ulong range = max - min;
            return new UInt2(
                CompresseU32(GetX2_Q32(++_state), min, range),
                CompresseU32(GetY2_Q32(_state), min, range));
        }

        [IN(LINE)]
        public UInt3 NextUInt3()
        {
            return new UInt3(
                GetX3_Q32(++_state),
                GetY3_Q32(_state),
                GetZ3_Q32(_state));
        }
        [IN(LINE)]
        public UInt3 NextUInt3(uint max)
        {
            return new UInt3(
                CompresseU32(GetX3_Q32(++_state), max),
                CompresseU32(GetY3_Q32(_state), max),
                CompresseU32(GetZ3_Q32(_state), max));
        }
        [IN(LINE)]
        public UInt3 NextUInt3(uint min, uint max)
        {
            ulong range = max - min;
            return new UInt3(
                CompresseU32(GetX3_Q32(++_state), min, range),
                CompresseU32(GetY3_Q32(_state), min, range),
                CompresseU32(GetZ3_Q32(_state), min, range));
        }

        [IN(LINE)]
        public UInt4 NextUInt4()
        {
            return new UInt4(
                GetX4_Q32(++_state),
                GetY4_Q32(_state),
                GetZ4_Q32(_state),
                GetW4_Q32(_state));
        }
        [IN(LINE)]
        public UInt4 NextUInt4(uint max)
        {
            return new UInt4(
                CompresseU32(GetX4_Q32(++_state), max),
                CompresseU32(GetY4_Q32(_state), max),
                CompresseU32(GetZ4_Q32(_state), max),
                CompresseU32(GetW4_Q32(_state), max));
        }
        [IN(LINE)]
        public UInt4 NextUInt4(uint min, uint max)
        {
            ulong range = max - min;
            return new UInt4(
                CompresseU32(GetX4_Q32(++_state), min, range),
                CompresseU32(GetY4_Q32(_state), min, range),
                CompresseU32(GetZ4_Q32(_state), min, range),
                CompresseU32(GetW4_Q32(_state), min, range));
        }
        #endregion

        #region Long
        [IN(LINE)] public long NextLong() => (long)(GetX1_Q64(++_state) >> 1);
        [IN(LINE)] public long NextLong(long max) => Compresse64(GetX1_Q64(++_state), max);
        [IN(LINE)] public long NextLong(long min, long max) => Compresse64(GetX1_Q64(++_state), min, (ulong)(max - min));

        [IN(LINE)]
        public void NextLong2(out long x, out long y)
        {
            x = (long)(GetX2_Q64(++_state) >> 1);
            y = (long)(GetY2_Q64(_state) >> 1);
        }
        //[IN(LINE)]
        public void NextLong2(long max, out long x, out long y)
        {
            x = Compresse64(GetX2_Q64(++_state), max);
            y = Compresse64(GetY2_Q64(_state), max);
        }
        //[IN(LINE)]
        public void NextLong2(long min, long max, out long x, out long y)
        {
            ulong range = (ulong)(max - min);
            x = Compresse64(GetX2_Q64(++_state), min, range);
            y = Compresse64(GetY2_Q64(_state), min, range);
        }

        [IN(LINE)]
        public void NextLong3(out long x, out long y, out long z)
        {
            x = (long)(GetX3_Q64(++_state) >> 1);
            y = (long)(GetY3_Q64(_state) >> 1);
            z = (long)(GetZ3_Q64(_state) >> 1);
        }
        //[IN(LINE)]
        public void NextLong3(long max, out long x, out long y, out long z)
        {
            x = Compresse64(GetX3_Q64(++_state), max);
            y = Compresse64(GetY3_Q64(_state), max);
            z = Compresse64(GetZ3_Q64(_state), max);
        }
        //[IN(LINE)]
        public void NextLong3(long min, long max, out long x, out long y, out long z)
        {
            ulong range = (ulong)(max - min);
            x = Compresse64(GetX3_Q64(++_state), min, range);
            y = Compresse64(GetY3_Q64(_state), min, range);
            z = Compresse64(GetZ3_Q64(_state), min, range);
        }

        [IN(LINE)]
        public void NextLong4(out long x, out long y, out long z, out long w)
        {
            x = (long)(GetX4_Q64(++_state) >> 1);
            y = (long)(GetY4_Q64(_state) >> 1);
            z = (long)(GetZ4_Q64(_state) >> 1);
            w = (long)(GetW4_Q64(_state) >> 1);
        }
        //[IN(LINE)]
        public void NextLong4(long max, out long x, out long y, out long z, out long w)
        {
            x = Compresse64(GetX4_Q64(++_state), max);
            y = Compresse64(GetY4_Q64(_state), max);
            z = Compresse64(GetZ4_Q64(_state), max);
            w = Compresse64(GetW4_Q64(_state), max);
        }
        //[IN(LINE)]
        public void NextLong4(long min, long max, out long x, out long y, out long z, out long w)
        {
            ulong range = (ulong)(max - min);
            x = Compresse64(GetX4_Q64(++_state), min, range);
            y = Compresse64(GetY4_Q64(_state), min, range);
            z = Compresse64(GetZ4_Q64(_state), min, range);
            w = Compresse64(GetW4_Q64(_state), min, range);
        }

        [IN(LINE)]
        public Long2 NextLong2()
        {
            return new Long2(
                (long)(GetX2_Q64(++_state) >> 1),
                (long)(GetY2_Q64(_state) >> 1));
        }
        //[IN(LINE)]
        public Long2 NextLong2(long max)
        {
            return new Long2(
                Compresse64(GetX2_Q64(++_state), max),
                Compresse64(GetY2_Q64(_state), max));
        }
        //[IN(LINE)]
        public Long2 NextLong2(long min, long max)
        {
            ulong range = (ulong)(max - min);
            return new Long2(
                Compresse64(GetX2_Q64(++_state), min, range),
                Compresse64(GetY2_Q64(_state), min, range));
        }

        [IN(LINE)]
        public Long3 NextLong3()
        {
            return new Long3(
                (long)(GetX3_Q64(++_state) >> 1),
                (long)(GetY3_Q64(_state) >> 1),
                (long)(GetZ3_Q64(_state) >> 1));
        }
        //[IN(LINE)]
        public Long3 NextLong3(long max)
        {
            return new Long3(
                Compresse64(GetX3_Q64(++_state), max),
                Compresse64(GetY3_Q64(_state), max),
                Compresse64(GetZ3_Q64(_state), max));
        }
        //[IN(LINE)]
        public Long3 NextLong3(long min, long max)
        {
            ulong range = (ulong)(max - min);
            return new Long3(
                Compresse64(GetX3_Q64(++_state), min, range),
                Compresse64(GetY3_Q64(_state), min, range),
                Compresse64(GetZ3_Q64(_state), min, range));
        }

        [IN(LINE)]
        public Long4 NextLong4()
        {
            return new Long4(
                (long)(GetX4_Q64(++_state) >> 1),
                (long)(GetY4_Q64(_state) >> 1),
                (long)(GetZ4_Q64(_state) >> 1),
                (long)(GetW4_Q64(_state) >> 1));
        }
        //[IN(LINE)]
        public Long4 NextLong4(long max)
        {
            return new Long4(
                Compresse64(GetX4_Q64(++_state), max),
                Compresse64(GetY4_Q64(_state), max),
                Compresse64(GetZ4_Q64(_state), max),
                Compresse64(GetW4_Q64(_state), max));
        }
        //[IN(LINE)]
        public Long4 NextLong4(long min, long max)
        {
            ulong range = (ulong)(max - min);
            return new Long4(
                Compresse64(GetX4_Q64(++_state), min, range),
                Compresse64(GetY4_Q64(_state), min, range),
                Compresse64(GetZ4_Q64(_state), min, range),
                Compresse64(GetW4_Q64(_state), min, range));
        }
        #endregion

        #region ULong
        [IN(LINE)] public ulong NextULong() => GetX1_Q64(++_state);
        [IN(LINE)] public ulong NextULong(ulong max) => CompresseU64(GetX1_Q64(++_state), max);
        [IN(LINE)] public ulong NextULong(ulong min, ulong max) => CompresseU64(GetX1_Q64(++_state), min, max - min);

        [IN(LINE)]
        public void NextULong2(out ulong x, out ulong y)
        {
            x = GetX2_Q64(++_state);
            y = GetY2_Q64(_state);
        }
        //[IN(LINE)]
        public void NextLong2(ulong max, out ulong x, out ulong y)
        {
            x = CompresseU64(GetX2_Q64(++_state), max);
            y = CompresseU64(GetY2_Q64(_state), max);
        }
        //[IN(LINE)]
        public void NextLong2(ulong min, ulong max, out ulong x, out ulong y)
        {
            ulong range = max - min;
            x = CompresseU64(GetX2_Q64(++_state), min, range);
            y = CompresseU64(GetY2_Q64(_state), min, range);
        }

        [IN(LINE)]
        public void NextULong3(out ulong x, out ulong y, out ulong z)
        {
            x = GetX3_Q64(++_state);
            y = GetY3_Q64(_state);
            z = GetZ3_Q64(_state);
        }
        //[IN(LINE)]
        public void NextLong3(ulong max, out ulong x, out ulong y, out ulong z)
        {
            x = CompresseU64(GetX3_Q64(++_state), max);
            y = CompresseU64(GetY3_Q64(_state), max);
            z = CompresseU64(GetZ3_Q64(_state), max);
        }
        //[IN(LINE)]
        public void NextLong3(ulong min, ulong max, out ulong x, out ulong y, out ulong z)
        {
            ulong range = max - min;
            x = CompresseU64(GetX3_Q64(++_state), min, range);
            y = CompresseU64(GetY3_Q64(_state), min, range);
            z = CompresseU64(GetZ3_Q64(_state), min, range);
        }

        [IN(LINE)]
        public void NextULong4(out ulong x, out ulong y, out ulong z, out ulong w)
        {
            x = GetX4_Q64(++_state);
            y = GetY4_Q64(_state);
            z = GetZ4_Q64(_state);
            w = GetW4_Q64(_state);
        }
        //[IN(LINE)]
        public void NextULong4(ulong max, out ulong x, out ulong y, out ulong z, out ulong w)
        {
            x = CompresseU64(GetX4_Q64(++_state), max);
            y = CompresseU64(GetY4_Q64(_state), max);
            z = CompresseU64(GetZ4_Q64(_state), max);
            w = CompresseU64(GetW4_Q64(_state), max);
        }
        //[IN(LINE)]
        public void NextULong4(ulong min, ulong max, out ulong x, out ulong y, out ulong z, out ulong w)
        {
            ulong range = max - min;
            x = CompresseU64(GetX4_Q64(++_state), min, range);
            y = CompresseU64(GetY4_Q64(_state), min, range);
            z = CompresseU64(GetZ4_Q64(_state), min, range);
            w = CompresseU64(GetW4_Q64(_state), min, range);
        }

        [IN(LINE)]
        public ULong2 NextULong2()
        {
            return new ULong2(
                GetX2_Q64(++_state),
                GetY2_Q64(_state));
        }
        //[IN(LINE)]
        public ULong2 NextULong2(ulong max)
        {
            return new ULong2(
                CompresseU64(GetX2_Q64(++_state), max),
                CompresseU64(GetY2_Q64(_state), max));
        }
        //[IN(LINE)]
        public ULong2 NextULong2(ulong min, ulong max)
        {
            ulong range = (ulong)(max - min);
            return new ULong2(
                CompresseU64(GetX2_Q64(++_state), min, range),
                CompresseU64(GetY2_Q64(_state), min, range));
        }

        [IN(LINE)]
        public ULong3 NextULong3()
        {
            return new ULong3(
                GetX3_Q64(++_state),
                GetY3_Q64(_state),
                GetZ3_Q64(_state));
        }
        //[IN(LINE)]
        public ULong3 NextULong3(ulong max)
        {
            return new ULong3(
                CompresseU64(GetX3_Q64(++_state), max),
                CompresseU64(GetY3_Q64(_state), max),
                CompresseU64(GetZ3_Q64(_state), max));
        }
        //[IN(LINE)]
        public ULong3 NextULong3(ulong min, ulong max)
        {
            ulong range = (ulong)(max - min);
            return new ULong3(
                CompresseU64(GetX3_Q64(++_state), min, range),
                CompresseU64(GetY3_Q64(_state), min, range),
                CompresseU64(GetZ3_Q64(_state), min, range));
        }

        [IN(LINE)]
        public ULong4 NextULong4()
        {
            return new ULong4(
                GetX4_Q64(++_state),
                GetY4_Q64(_state),
                GetZ4_Q64(_state),
                GetW4_Q64(_state));
        }
        //[IN(LINE)]
        public ULong4 NextULong4(ulong max)
        {
            return new ULong4(
                CompresseU64(GetX4_Q64(++_state), max),
                CompresseU64(GetY4_Q64(_state), max),
                CompresseU64(GetZ4_Q64(_state), max),
                CompresseU64(GetW4_Q64(_state), max));
        }
        //[IN(LINE)]
        public ULong4 NextULong4(ulong min, ulong max)
        {
            ulong range = (ulong)(max - min);
            return new ULong4(
                CompresseU64(GetX4_Q64(++_state), min, range),
                CompresseU64(GetY4_Q64(_state), min, range),
                CompresseU64(GetZ4_Q64(_state), min, range),
                CompresseU64(GetW4_Q64(_state), min, range));
        }
        #endregion

        #region Float
        [IN(LINE)] public float NextFloat() => GetX1_F(++_state);

        [IN(LINE)]
        public void NextFloat2(out float x, out float y)
        {
            x = GetX2_F(++_state);
            y = GetY2_F(_state);
        }
        [IN(LINE)]
        public void NextFloat3(out float x, out float y, out float z)
        {
            x = GetX3_F(++_state);
            y = GetY3_F(_state);
            z = GetZ3_F(_state);
        }
        [IN(LINE)]
        public void NextFloat4(out float x, out float y, out float z, out float w)
        {
            x = GetX4_F(++_state);
            y = GetY4_F(_state);
            z = GetZ4_F(_state);
            w = GetW4_F(_state);
        }

        [IN(LINE)]
        public Float2 NextFloat2()
        {
            return new Float2(
                GetX2_F(++_state),
                GetY2_F(_state));
        }
        [IN(LINE)]
        public Float3 NextFloat3()
        {
            return new Float3(
                GetX3_F(++_state),
                GetY3_F(_state),
                GetZ3_F(_state));
        }
        [IN(LINE)]
        public Float4 NextFloat4()
        {
            return new Float4(
                GetX4_F(++_state),
                GetY4_F(_state),
                GetZ4_F(_state),
                GetW4_F(_state));
        }
        #endregion

        #region Double
        [IN(LINE)]
        public double NextDouble() => GetX1_D(++_state);

        [IN(LINE)]
        public void NextDouble2(out double x, out double y)
        {
            x = GetX2_D(++_state);
            y = GetY2_D(_state);
        }
        [IN(LINE)]
        public void NextDouble3(out double x, out double y, out double z)
        {
            x = GetX3_D(++_state);
            y = GetY3_D(_state);
            z = GetZ3_D(_state);
        }
        [IN(LINE)]
        public void NextDouble4(out double x, out double y, out double z, out double w)
        {
            x = GetX4_D(++_state);
            y = GetY4_D(_state);
            z = GetZ4_D(_state);
            w = GetW4_D(_state);
        }

        [IN(LINE)]
        public Double2 NextDouble2()
        {
            return new Double2(
                GetX2_D(++_state),
                GetY2_D(_state));
        }
        [IN(LINE)]
        public Double3 NextDouble3()
        {
            return new Double3(
                GetX3_D(++_state),
                GetY3_D(_state),
                GetZ3_D(_state));
        }
        [IN(LINE)]
        public Double4 NextDouble4()
        {
            return new Double4(
                GetX4_D(++_state),
                GetY4_D(_state),
                GetZ4_D(_state),
                GetW4_D(_state));
        }
        #endregion


        #region State
        public uint GetState() => _state;
        public uint SetState(int state) => _state = (uint)state;
        public uint SetState(uint state) => _state = state;
        #endregion

        #region Other
        [IN(LINE)]
        public override bool Equals(object o) => o is QuasiRandom target && Equals(target);
        [IN(LINE)]
        public bool Equals(QuasiRandom a) => a._state == _state;

        [IN(LINE)]
        public override int GetHashCode() => (int)_state;

        [IN(LINE)]
        public override string ToString() => $"{nameof(QuasiRandom)}({_state})";
        [IN(LINE)]
        public string ToString(string format, IFormatProvider formatProvider)
        {
            return $"{nameof(QuasiRandom)}({_state.ToString(format, formatProvider)})";
        }

        [IN(LINE)]
        public static bool operator ==(QuasiRandom a, QuasiRandom b) => a._state == b._state;
        [IN(LINE)]
        public static bool operator !=(QuasiRandom a, QuasiRandom b) => a._state != b._state;
        #endregion

        #region Vectors
        [EditorBrowsable(EditorBrowsableState.Never)]
        public partial struct Bool2
        {
            public bool X, Y;
            [IN(LINE)] public Bool2(bool x, bool y) { X = x; Y = y; }
            [IN(LINE)] public void Deconstruct(out bool x, out bool y) { x = X; y = Y; }
            [IN(LINE)] public static explicit operator Vector2(Bool2 a) => new Vector2(a.X ? 1 : 0, a.Y ? 1 : 0);
            [IN(LINE)] public static explicit operator Vector3(Bool2 a) => new Vector3(a.X ? 1 : 0, a.Y ? 1 : 0, 0);
            [IN(LINE)] public static explicit operator Vector4(Bool2 a) => new Vector4(a.X ? 1 : 0, a.Y ? 1 : 0, 0, 0);
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public partial struct Bool3
        {
            public bool X, Y, Z;
            [IN(LINE)] public Bool3(bool x, bool y, bool z) { X = x; Y = y; Z = z; }
            [IN(LINE)] public void Deconstruct(out bool x, out bool y, out bool z) { x = X; y = Y; z = Z; }
            [IN(LINE)] public static explicit operator Vector3(Bool3 a) => new Vector3(a.X ? 1 : 0, a.Y ? 1 : 0, a.Z ? 1 : 0);
            [IN(LINE)] public static explicit operator Vector4(Bool3 a) => new Vector4(a.X ? 1 : 0, a.Y ? 1 : 0, a.Z ? 1 : 0, 0);
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public partial struct Bool4
        {
            public bool X, Y, Z, W;
            [IN(LINE)] public Bool4(bool x, bool y, bool z, bool w) { X = x; Y = y; Z = z; W = w; }
            [IN(LINE)] public void Deconstruct(out bool x, out bool y, out bool z, out bool w) { x = X; y = Y; z = Z; w = W; }
            [IN(LINE)] public static explicit operator Vector4(Bool4 a) => new Vector4(a.X ? 1f : 0f, a.Y ? 1f : 0f, a.Z ? 1f : 0f, a.W ? 1f : 0f);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public partial struct Float2
        {
            public float X, Y;
            [IN(LINE)] public Float2(float x, float y) { X = x; Y = y; }
            [IN(LINE)] public void Deconstruct(out float x, out float y) { x = X; y = Y; }
            [IN(LINE)] public static implicit operator Vector2(Float2 a) => new Vector2(a.X, a.Y);
            [IN(LINE)] public static implicit operator Vector3(Float2 a) => new Vector3(a.X, a.Y, 0);
            [IN(LINE)] public static implicit operator Vector4(Float2 a) => new Vector4(a.X, a.Y, 0, 0);
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public partial struct Float3
        {
            public float X, Y, Z;
            [IN(LINE)] public Float3(float x, float y, float z) { X = x; Y = y; Z = z; }
            [IN(LINE)] public void Deconstruct(out float x, out float y, out float z) { x = X; y = Y; z = Z; }
            [IN(LINE)] public static implicit operator Vector3(Float3 a) => new Vector3(a.X, a.Y, a.Z);
            [IN(LINE)] public static implicit operator Vector4(Float3 a) => new Vector4(a.X, a.Y, a.Z, 0);
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public partial struct Float4
        {
            public float X, Y, Z, W;
            [IN(LINE)] public Float4(float x, float y, float z, float w) { X = x; Y = y; Z = z; W = w; }
            [IN(LINE)] public void Deconstruct(out float x, out float y, out float z, out float w) { x = X; y = Y; z = Z; w = W; }
            [IN(LINE)] public static implicit operator Vector4(Float4 a) => new Vector4(a.X, a.Y, a.Z, a.W);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public partial struct Double2
        {
            public double X, Y;
            [IN(LINE)] public Double2(double x, double y) { X = x; Y = y; }
            [IN(LINE)] public void Deconstruct(out double x, out double y) { x = X; y = Y; }
            [IN(LINE)] public static explicit operator Vector2(Double2 a) => new Vector2((float)a.X, (float)a.Y);
            [IN(LINE)] public static explicit operator Vector3(Double2 a) => new Vector3((float)a.X, (float)a.Y, 0);
            [IN(LINE)] public static explicit operator Vector4(Double2 a) => new Vector4((float)a.X, (float)a.Y, 0, 0);
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public partial struct Double3
        {
            public double X, Y, Z;
            [IN(LINE)] public Double3(double x, double y, double z) { X = x; Y = y; Z = z; }
            [IN(LINE)] public void Deconstruct(out double x, out double y, out double z) { x = X; y = Y; z = Z; }
            [IN(LINE)] public static explicit operator Vector3(Double3 a) => new Vector3((float)a.X, (float)a.Y, (float)a.Z);
            [IN(LINE)] public static explicit operator Vector4(Double3 a) => new Vector4((float)a.X, (float)a.Y, (float)a.Z, 0);
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public partial struct Double4
        {
            public double X, Y, Z, W;
            [IN(LINE)] public Double4(double x, double y, double z, double w) { X = x; Y = y; Z = z; W = w; }
            [IN(LINE)] public void Deconstruct(out double x, out double y, out double z, out double w) { x = X; y = Y; z = Z; w = W; }
            [IN(LINE)] public static explicit operator Vector4(Double4 a) => new Vector4((float)a.X, (float)a.Y, (float)a.Z, (float)a.W);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public partial struct Int2
        {
            public int X, Y;
            [IN(LINE)] public Int2(int x, int y) { X = x; Y = y; }
            [IN(LINE)] public void Deconstruct(out int x, out int y) { x = X; y = Y; }
            [IN(LINE)] public static implicit operator Vector2(Int2 a) => new Vector2(a.X, a.Y);
            [IN(LINE)] public static implicit operator Vector3(Int2 a) => new Vector3(a.X, a.Y, 0);
            [IN(LINE)] public static implicit operator Vector4(Int2 a) => new Vector4(a.X, a.Y, 0, 0);
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public partial struct Int3
        {
            public int X, Y, Z;
            [IN(LINE)] public Int3(int x, int y, int z) { X = x; Y = y; Z = z; }
            [IN(LINE)] public void Deconstruct(out int x, out int y, out int z) { x = X; y = Y; z = Z; }
            [IN(LINE)] public static implicit operator Vector3(Int3 a) => new Vector3(a.X, a.Y, a.Z);
            [IN(LINE)] public static implicit operator Vector4(Int3 a) => new Vector4(a.X, a.Y, a.Z, 0);
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public partial struct Int4
        {
            public int X, Y, Z, W;
            [IN(LINE)] public Int4(int x, int y, int z, int w) { X = x; Y = y; Z = z; W = w; }
            [IN(LINE)] public void Deconstruct(out int x, out int y, out int z, out int w) { x = X; y = Y; z = Z; w = W; }
            [IN(LINE)] public static implicit operator Vector4(Int4 a) => new Vector4(a.X, a.Y, a.Z, a.W);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public partial struct UInt2
        {
            public uint X, Y;
            [IN(LINE)] public UInt2(uint x, uint y) { X = x; Y = y; }
            [IN(LINE)] public void Deconstruct(out uint x, out uint y) { x = X; y = Y; }
            [IN(LINE)] public static implicit operator Vector2(UInt2 a) => new Vector2(a.X, a.Y);
            [IN(LINE)] public static implicit operator Vector3(UInt2 a) => new Vector3(a.X, a.Y, 0);
            [IN(LINE)] public static implicit operator Vector4(UInt2 a) => new Vector4(a.X, a.Y, 0, 0);
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public partial struct UInt3
        {
            public uint X, Y, Z;
            [IN(LINE)] public UInt3(uint x, uint y, uint z) { X = x; Y = y; Z = z; }
            [IN(LINE)] public void Deconstruct(out uint x, out uint y, out uint z) { x = X; y = Y; z = Z; }
            [IN(LINE)] public static implicit operator Vector3(UInt3 a) => new Vector3(a.X, a.Y, a.Z);
            [IN(LINE)] public static implicit operator Vector4(UInt3 a) => new Vector4(a.X, a.Y, a.Z, 0);
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public partial struct UInt4
        {
            public uint X, Y, Z, W;
            [IN(LINE)] public UInt4(uint x, uint y, uint z, uint w) { X = x; Y = y; Z = z; W = w; }
            [IN(LINE)] public void Deconstruct(out uint x, out uint y, out uint z, out uint w) { x = X; y = Y; z = Z; w = W; }
            [IN(LINE)] public static implicit operator Vector4(UInt4 a) => new Vector4(a.X, a.Y, a.Z, a.W);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public partial struct Long2
        {
            public long X, Y;
            [IN(LINE)] public Long2(long x, long y) { X = x; Y = y; }
            [IN(LINE)] public void Deconstruct(out long x, out long y) { x = X; y = Y; }
            [IN(LINE)] public static implicit operator Vector2(Long2 a) => new Vector2(a.X, a.Y);
            [IN(LINE)] public static implicit operator Vector3(Long2 a) => new Vector3(a.X, a.Y, 0);
            [IN(LINE)] public static implicit operator Vector4(Long2 a) => new Vector4(a.X, a.Y, 0, 0);
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public partial struct Long3
        {
            public long X, Y, Z;
            [IN(LINE)] public Long3(long x, long y, long z) { X = x; Y = y; Z = z; }
            [IN(LINE)] public void Deconstruct(out long x, out long y, out long z) { x = X; y = Y; z = Z; }
            [IN(LINE)] public static implicit operator Vector3(Long3 a) => new Vector3(a.X, a.Y, a.Z);
            [IN(LINE)] public static implicit operator Vector4(Long3 a) => new Vector4(a.X, a.Y, a.Z, 0);
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public partial struct Long4
        {
            public long X, Y, Z, W;
            [IN(LINE)] public Long4(long x, long y, long z, long w) { X = x; Y = y; Z = z; W = w; }
            [IN(LINE)] public void Deconstruct(out long x, out long y, out long z, out long w) { x = X; y = Y; z = Z; w = W; }
            [IN(LINE)] public static implicit operator Vector4(Long4 a) => new Vector4(a.X, a.Y, a.Z, a.W);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public partial struct ULong2
        {
            public ulong X, Y;
            [IN(LINE)] public ULong2(ulong x, ulong y) { X = x; Y = y; }
            [IN(LINE)] public void Deconstruct(out ulong x, out ulong y) { x = X; y = Y; }
            [IN(LINE)] public static implicit operator Vector2(ULong2 a) => new Vector2(a.X, a.Y);
            [IN(LINE)] public static implicit operator Vector3(ULong2 a) => new Vector3(a.X, a.Y, 0);
            [IN(LINE)] public static implicit operator Vector4(ULong2 a) => new Vector4(a.X, a.Y, 0, 0);
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public partial struct ULong3
        {
            public ulong X, Y, Z;
            [IN(LINE)] public ULong3(ulong x, ulong y, ulong z) { X = x; Y = y; Z = z; }
            [IN(LINE)] public void Deconstruct(out ulong x, out ulong y, out ulong z) { x = X; y = Y; z = Z; }
            [IN(LINE)] public static implicit operator Vector3(ULong3 a) => new Vector3(a.X, a.Y, a.Z);
            [IN(LINE)] public static implicit operator Vector4(ULong3 a) => new Vector4(a.X, a.Y, a.Z, 0);
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public partial struct ULong4
        {
            public ulong X, Y, Z, W;
            [IN(LINE)] public ULong4(ulong x, ulong y, ulong z, ulong w) { X = x; Y = y; Z = z; W = w; }
            [IN(LINE)] public void Deconstruct(out ulong x, out ulong y, out ulong z, out ulong w) { x = X; y = Y; z = Z; w = W; }
            [IN(LINE)] public static implicit operator Vector4(ULong4 a) => new Vector4(a.X, a.Y, a.Z, a.W);
        }
        #endregion
    }
}