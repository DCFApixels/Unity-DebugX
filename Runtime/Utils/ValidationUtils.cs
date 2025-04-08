using UnityEngine;

namespace DCFApixels.DebugXCore
{
    public static class ValidationUtils
    {
        public static Quaternion CheckQuaternionOrDefault(this Quaternion quaternion)
        {
            float sqrMagnitude = quaternion.x * quaternion.x + quaternion.y * quaternion.y + quaternion.z * quaternion.z + quaternion.w * quaternion.w;
            if (float.IsNaN(sqrMagnitude) || (sqrMagnitude < float.Epsilon))
            {
                return Quaternion.identity;
            }
            return quaternion;
        }

        public static Vector3 CheckNormalOrDefault(this Vector3 normal)
        {
            float sqrMagnitude = normal.sqrMagnitude;
            if (float.IsNaN(sqrMagnitude) || (sqrMagnitude < float.Epsilon))
            {
                return Vector3.forward;
            }
            return normal;
        }
    }
}