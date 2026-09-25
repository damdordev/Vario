using UnityEngine;

namespace Damdor.Vario
{
    [NumericOperations]
    public class Vector3NumericOperations : BaseNumericOperations<Vector3>
    {
        public override Vector3 Lerp(Vector3 a, Vector3 b, float t) => Vector3.Lerp(a, b, t);
        public override Vector3 LerpUnclamped(Vector3 a, Vector3 b, float t) => Vector3.LerpUnclamped(a, b, t);
        public override Vector3 Sum(Vector3 a, Vector3 b) => a + b;
        public override Vector3 Subtract(Vector3 a, Vector3 b) => a - b;
    }
}