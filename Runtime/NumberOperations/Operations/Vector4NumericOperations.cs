using UnityEngine;

namespace Damdor.Vario
{
    [NumericOperations]
    public class Vector4NumericOperations : BaseNumericOperations<Vector4>
    {
        public override Vector4 Lerp(Vector4 a, Vector4 b, float t) => Vector4.Lerp(a, b, t);
        public override Vector4 LerpUnclamped(Vector4 a, Vector4 b, float t) => Vector4.LerpUnclamped(a, b, t);
        public override Vector4 Sum(Vector4 a, Vector4 b) => a + b;
        public override Vector4 Subtract(Vector4 a, Vector4 b) => a - b;
    }
}