using System;
using UnityEngine;

namespace Damdor.Vario
{
    [NumericOperations]
    public class TimeSpanNumericOperations : BaseNumericOperations<TimeSpan>
    {
        public override TimeSpan Lerp(TimeSpan a, TimeSpan b, float t)
            => LerpUnclamped(a, b, Mathf.Clamp01(t));

        public override TimeSpan LerpUnclamped(TimeSpan a, TimeSpan b, float t)
        {
            var ticks = (long)(a.Ticks * (1 - t) + b.Ticks * t);
            return new TimeSpan(ticks);
        }

        public override TimeSpan Sum(TimeSpan a, TimeSpan b) => a + b;
        public override TimeSpan Subtract(TimeSpan a, TimeSpan b) => a - b;
    }
}