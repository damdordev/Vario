using UnityEngine;

namespace Damdor.Vario
{
    public static class Vector4Extension
    {
        public static Vector4 Modify(this Vector4 v, Vector4 other, Vector4AxisFilter filter)
        {
            if (filter.X) v.x = other.x;
            if (filter.Y) v.y = other.y;
            if (filter.Z) v.z = other.z;
            if (filter.W) v.w = other.w;
            return v;
        }
    }
}