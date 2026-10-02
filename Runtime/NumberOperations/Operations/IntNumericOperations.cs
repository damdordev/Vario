using UnityEngine;

namespace Damdor.Vario
{
    [NumericOperations]
    public class IntNumericOperations : BaseNumericOperations<int>
    {
        public override int Lerp(int a, int b, float t) => (int) Mathf.Lerp(a, b, t);
        public override int LerpUnclamped(int a, int b, float t) => (int) Mathf.LerpUnclamped(a, b, t);
        public override int Sum(int a, int b) => a + b;
        public override int Subtract(int a, int b) => a - b;
    }
}