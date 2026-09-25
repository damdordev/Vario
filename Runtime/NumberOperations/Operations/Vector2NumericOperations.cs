using UnityEngine;

namespace Damdor.Vario
{
    [NumericOperations]
    public class Vector2NumericOperations : BaseNumericOperations<Vector2>
    {
        public override Vector2 Lerp(Vector2 a, Vector2 b, float t) => Vector2.Lerp(a, b, t);
        public override Vector2 LerpUnclamped(Vector2 a, Vector2 b, float t) => Vector2.LerpUnclamped(a, b, t);
        public override Vector2 Sum(Vector2 a, Vector2 b) => a + b;
        public override Vector2 Subtract(Vector2 a, Vector2 b) => a - b;
    }
}