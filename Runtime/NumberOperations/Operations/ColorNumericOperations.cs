using UnityEngine;

namespace Damdor.Vario
{
    [NumericOperations]
    public class ColorNumericOperations : BaseNumericOperations<Color>
    {
        public override Color Lerp(Color a, Color b, float t) => Color.Lerp(a, b, t);
        public override Color LerpUnclamped(Color a, Color b, float t) => Color.LerpUnclamped(a, b, t);
        public override Color Sum(Color a, Color b) => a + b;
        public override Color Subtract(Color a, Color b) => a - b;
    }
}