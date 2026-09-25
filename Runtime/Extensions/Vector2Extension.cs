using UnityEngine;

namespace Damdor.Vario
{
    public static class Vector2Extension
    {
        public static Vector2 Modify(this Vector2 v, Vector2 other, Vector2AxisFilter filter)
        {
            if (filter.X) v.x = other.x;
            if (filter.Y) v.y = other.y;
            return v;
        }
    }
}