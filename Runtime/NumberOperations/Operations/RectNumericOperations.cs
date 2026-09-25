using UnityEngine;

namespace Damdor.Vario
{
    [NumericOperations]
    public class RectNumericOperations : BaseNumericOperations<Rect>
    {
        public override Rect Lerp(Rect a, Rect b, float t)
        {
            return new Rect(
                Mathf.Lerp(a.x, b.x, t),
                Mathf.Lerp(a.y, b.y, t),
                Mathf.Lerp(a.width, b.width, t),
                Mathf.Lerp(a.height, b.height, t)
            );
        }

        public override Rect LerpUnclamped(Rect a, Rect b, float t)
        {
            return new Rect(
                Mathf.LerpUnclamped(a.x, b.x, t),
                Mathf.LerpUnclamped(a.y, b.y, t),
                Mathf.LerpUnclamped(a.width, b.width, t),
                Mathf.LerpUnclamped(a.height, b.height, t)
            );
        }

        public override Rect Sum(Rect a, Rect b)
        {
            return new Rect(a.x + b.x, a.y + b.y, a.width + b.width, a.height + b.height);
        }

        public override Rect Subtract(Rect a, Rect b)
        {
            return new Rect(a.x - b.x, a.y - b.y, a.width - b.width, a.height - b.height);
        }
    }
}