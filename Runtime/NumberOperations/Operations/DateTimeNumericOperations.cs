using System;
using UnityEngine;

namespace Damdor.Vario
{
    [NumericOperations]
    public class DateTimeNumericOperations : BaseNumericOperations<DateTime>
    {
        public override DateTime Lerp(DateTime a, DateTime b, float t)
            => LerpUnclamped(a, b, Mathf.Clamp01(t));

        public override DateTime LerpUnclamped(DateTime a, DateTime b, float t)
        {
            var delta = (b.Ticks - a.Ticks) * (double)t;
            var ticks = (long)Math.Clamp(a.Ticks + delta, DateTime.MinValue.Ticks, DateTime.MaxValue.Ticks);
            return new DateTime(ticks);
        }

        public override DateTime Sum(DateTime a, DateTime b) => a.AddTicks(b.Ticks);
        public override DateTime Subtract(DateTime a, DateTime b) => a.AddTicks(-b.Ticks);
    }
}