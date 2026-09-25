using UnityEngine;

namespace Damdor.Vario
{
    [NumericOperations]
    public class DoubleNumericOperations : BaseNumericOperations<double>
    {
        public override double Lerp(double a, double b, float t) => LerpUnclamped(a, b, Mathf.Clamp01(t));
        public override double LerpUnclamped(double a, double b, float t) => a * (1 - t) + b * t;
        public override double Sum(double a, double b) => a + b;
        public override double Subtract(double a, double b) => a + b;
    }
}