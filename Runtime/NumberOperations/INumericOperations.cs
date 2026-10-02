using System;
// ReSharper disable UnusedMember.Global

namespace Damdor.Vario
{
    /// <summary>
    /// Base interface for numeric operations to identify the target number type.
    /// </summary>
    public interface INumericOperations
    {
        /// <summary>
        /// Gets the type of the number this operation set applies to.
        /// </summary>
        Type NumberType { get; } 
    }
    
    /// <summary>
    /// Defines standard numeric operations for a specific type.
    /// </summary>
    /// <typeparam name="T">The numeric type (e.g., float, Vector3).</typeparam>
    public interface INumericOperations<T> : INumericOperations
    {
        /// <summary>
        /// Linearly interpolates between a and b by t, clamped to the range [0, 1].
        /// </summary>
        T Lerp(T a, T b, float t);

        /// <summary>
        /// Linearly interpolates between a and b by t, without clamping t.
        /// </summary>
        T LerpUnclamped(T a, T b, float t);

        /// <summary>
        /// Calculates the sum of a and b.
        /// </summary>
        T Sum(T a, T b);

        /// <summary>
        /// Subtracts b from a.
        /// </summary>
        T Subtract(T a, T b);
    }
}