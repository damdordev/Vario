namespace Damdor.Vario
{
    [NumericOperations]
    public class BoolNumericOperations : BaseNumericOperations<bool>
    {
        public override bool Lerp(bool a, bool b, float t) => t < 0.5f ? a : b;
        public override bool LerpUnclamped(bool a, bool b, float t) => t < 0.5f ? a : b;
        public override bool Sum(bool a, bool b) => a || b;
        public override bool Subtract(bool a, bool b) => a && !b;
    }
}