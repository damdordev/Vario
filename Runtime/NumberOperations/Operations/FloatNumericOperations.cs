using UnityEngine;

namespace Damdor.Vario
{
    [NumericOperations]
    public class FloatNumericOperations : BaseNumericOperations<float>
    {
        public override float Lerp(float a, float b, float t) => Mathf.Lerp(a, b, t);
        public override float LerpUnclamped(float a, float b, float t) => Mathf.LerpUnclamped(a, b, t);
        public override float Sum(float a, float b) => a + b;
        public override float Subtract(float a, float b) => a - b;
    }
}