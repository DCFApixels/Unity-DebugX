using System.Linq;
using System.Reflection;
using UnityEngine;

namespace DCFApixels.DebugXCore
{
    public static class StaticDataLoader
    {
        public static T Load<T>(T instance, string path)
        {
            object obj = instance;
            var type = obj.GetType();
            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
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
                    Debug.LogWarning(field.Name + " not found");
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
                    Debug.LogWarning(field.Name + " not found");
                }
            }

            return (T)obj;
        }
    }
}