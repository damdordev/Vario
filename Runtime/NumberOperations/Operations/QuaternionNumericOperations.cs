using UnityEngine;

namespace Damdor.Vario
{
    [NumericOperations]
    public class QuaternionNumericOperations : BaseNumericOperations<Quaternion>
    {
        public override Quaternion Lerp(Quaternion a, Quaternion b, float t) => Quaternion.Lerp(a, b, t);
        public override Quaternion LerpUnclamped(Quaternion a, Quaternion b, float t) => Quaternion.LerpUnclamped(a, b, t);
        public override Quaternion Sum(Quaternion a, Quaternion b) => a * b;
        public override Quaternion Subtract(Quaternion a, Quaternion b) => a * Quaternion.Inverse(b);
    }
}