using System.Linq;
using System.Reflection;
using UnityEngine;

namespace DCFApixels.DebugXCore.Internal
{
    internal static class MeshesListLoader
    {
        public static T Load<T>(T instance, string path)
        {
            object obj = instance;
            var type = obj.GetType();
            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Where(o => o.FieldType == typeof(Mesh));
            var prefab = Resources.Load<GameObject>(path);

            if (prefab == null)
            {
                Debug.LogError($"{path} not found");
                return (T)obj;
            }

            if (fields.Count() <= 0)
            {
                Debug.LogError($"{typeof(T).Name} no fields");
                return (T)obj;
            }

            foreach (var field in fields)
            {
                //if (prefab == null) { continue; }
                var child = prefab.transform.Find(field.Name);
                var meshFilter = child.GetComponent<MeshFilter>();
                if (meshFilter != null)
                {
                    field.SetValue(obj, meshFilter.sharedMesh);
                }
                else
                {
                    Debug.LogWarning(field.Name + " not found");
                }
            }

            return (T)obj;
        }
    }
}