using UnityEngine;

namespace Damdor.Vario
{
    public static class Vector3Extension
    {
        public static Vector3 Modify(this Vector3 v, Vector3 other, Vector3AxisFilter filter)
        {
            if (filter.X) v.x = other.x;
            if (filter.Y) v.y = other.y;
            if (filter.Z) v.z = other.z;
            return v;
        }
    }
}