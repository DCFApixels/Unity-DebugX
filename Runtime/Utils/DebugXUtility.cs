using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DCFApixels.DebugXCore
{
    using IN = MethodImplAttribute;
    public static class DebugXUtility
    {
        private const MethodImplOptions LINE = MethodImplOptions.AggressiveInlining;
        public static T LoadStaticData<T>(T instance, string path)
        {
            object obj = instance;
            var type = obj.GetType();
            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var prefab = Resources.Load<GameObject>(path);

            if (prefab == null)
            {
                Debug.LogError($"{path} not found.");
                return (T)obj;
            }

            if (fields.Count() <= 0)
            {
                Debug.LogError($"{typeof(T).Name} no fields.");
                return (T)obj;
            }

            foreach (var field in fields.Where(o => o.FieldType == typeof(Mesh)))
            {
                var child = prefab.transform.Find(field.Name);
                var meshFilter = child.GetComponent<MeshFilter>();
                if (meshFilter != null)
                {
                    field.SetValue(obj, meshFilter.sharedMesh);
                }
                else
                { 
                    Debug.LogWarning(field.Name + " not found.");
                }
            }
            foreach (var field in fields.Where(o => o.FieldType == typeof(Material)))
            {
                var child = prefab.transform.Find(field.Name);
                var meshFilter = child.GetComponent<Renderer>();
                if (meshFilter != null)
                {
                    field.SetValue(obj, meshFilter.sharedMaterial);
                }
                else
                {
                    Debug.LogWarning(field.Name + " not found.");
                }
            }

            return (T)obj;
        }
        public static string GetGenericTypeName(Type type, int maxDepth, bool isFull)
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
                string paramTypeName = GetGenericTypeName(typeParameters[i], maxDepth - 1, false);
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
