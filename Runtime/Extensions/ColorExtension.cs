using UnityEngine;
// ReSharper disable UnusedMember.Global

namespace Damdor.Vario
{
    public static class ColorExtension
    {
        public static Color Modify(this Color v, Color other, ColorChannelFilter filter)
        {
            if (filter.R) v.r = other.r;
            if (filter.G) v.g = other.g;
            if (filter.B) v.b = other.b;
            if (filter.A) v.a = other.a;
            return v;
        }
        
        public static Color Modify(this Color v, Color other, ColorHSVFilter filter)
        {
            if (filter.H && filter.S && filter.V)
            {
                var simpleResult = other;
                simpleResult.a = filter.A ? other.a : v.a;
                return simpleResult;
            }
            
            float h, s, vValue;
            Color.RGBToHSV(v, out h, out s, out vValue);
            
            float otherH, otherS, otherV;
            Color.RGBToHSV(other, out otherH, out otherS, out otherV);

            h = filter.H ? otherH : h;
            s = filter.S ? otherS : s;
            vValue = filter.V ? otherV : vValue;
            
            var result = Color.HSVToRGB(h, s, vValue);
            result.a = filter.A ? other.a : v.a;
            
            return result;
        }
    }
}