using UnityEngine;

namespace Damdor.Foundation
{
    public static class TransformExtension
    {
        
        public static string GetFullPath(this Transform transform)
        {
            var path = transform.name;
            var t = transform.transform.parent;
            while(t != null)
            {
                path = t.name + "/" + path;
                t = t.parent;
            }
            return path;
        }
        
    }
}