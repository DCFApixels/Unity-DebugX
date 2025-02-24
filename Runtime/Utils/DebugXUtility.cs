using System;
using UnityEngine;
using static DCFApixels.DebugX;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DCFApixels.DebugXCore
{
    using IN = System.Runtime.CompilerServices.MethodImplAttribute;
    public static class DebugXUtility
    {
        public static string GetGenericTypeName_Internal(Type type, int maxDepth, bool isFull)
        {
#if DEBUG || !REFLECTION_DISABLED //в дебажных утилитах REFLECTION_DISABLED только в релизном билде работает
            string typeName = isFull ? type.FullName : type.Name;
            if (!type.IsGenericType || maxDepth == 0)
            {
                return typeName;
            }
            int genericInfoIndex = typeName.LastIndexOf('`');
            if (genericInfoIndex > 0)
            {
                typeName = typeName.Remove(genericInfoIndex);
            }

            string genericParams = "";
            Type[] typeParameters = type.GetGenericArguments();
            for (int i = 0; i < typeParameters.Length; ++i)
            {
                //чтобы строка не была слишком длинной, используются сокращенные имена для типов аргументов
                string paramTypeName = GetGenericTypeName_Internal(typeParameters[i], maxDepth - 1, false);
                genericParams += (i == 0 ? paramTypeName : $", {paramTypeName}");
            }
            return $"{typeName}<{genericParams}>";
#else
            Debug.LogWarning($"Reflection is not available, the {nameof(GetGenericTypeName_Internal)} method does not work.");
            return isFull ? type.FullName : type.Name;
#endif
        }

        [IN(LINE)]
        public static float FastMagnitude(Vector3 v)
        {
            return FastSqrt(v.x * v.x + v.y * v.y + v.z * v.z);
        }
        [IN(LINE)]
        public static unsafe float FastSqrt(float number)
        {
            long i;
            float x2, y;
            const float threehalfs = 1.5F;

            x2 = number * 0.5F;
            y = number;
            i = *(long*)&y;                         // evil floating point bit level hacking
            i = 0x5f3759df - (i >> 1);              // what the fuck?
            y = *(float*)&i;
            y = y * (threehalfs - (x2 * y * y));    // 1st iteration
            //y  = y * ( threehalfs - ( x2 * y * y ) );   // 2nd iteration, this can be removed

            return 1 / y;
        }
        [IN(LINE)]
        public static int NextPow2(int v)
        {
            v--;
            v |= v >> 1;
            v |= v >> 2;
            v |= v >> 4;
            v |= v >> 8;
            v |= v >> 16;
            return ++v;
        }
        [IN(LINE)]
        public static bool IsGizmosRender()
        {
            bool result = true;
#if UNITY_EDITOR
            result = Handles.ShouldRenderGizmos();
#endif
            return result;
        }
    }
}
