using System;

namespace Damdor.Vario
{
    /// <summary>
    /// Base abstract class providing the standard implementation of the non-generic interface
    /// for numeric operations of a specific type.
    /// </summary>
    /// <typeparam name="T">The numeric type.</typeparam>
    public abstract class BaseNumericOperations<T> : INumericOperations<T>
    {
        /// <inheritdoc />
        public Type NumberType => typeof(T);
        
        /// <inheritdoc />
        public abstract T Lerp(T a, T b, float t);
        
        /// <inheritdoc />
        public abstract T LerpUnclamped(T a, T b, float t);
        
        /// <inheritdoc />
        public abstract T Sum(T a, T b);
        
        /// <inheritdoc />
        public abstract T Subtract(T a, T b);
    }
}