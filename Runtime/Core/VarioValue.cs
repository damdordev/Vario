using System;
using UnityEngine;

namespace Damdor.Vario
{
    public enum ValueSource
    {
        Raw,
        Storage
    }
    
    /// <summary>
    /// A wrapper class used to define how a value should be retrieved.
    /// </summary>
    /// <typeparam name="T">The exact type of the value.</typeparam>
    [Serializable]
    public struct VarioValue<T>
    {
        /// <summary>
        /// Gets or sets the source mode for this value
        /// </summary>
        /// <value>A <see cref="ValueSource"/> enum value determining how to interpret the data.</value>
        public ValueSource Source
        {
            get => source;
            set => source = value;
        }

        /// <summary>
        /// Gets or sets the name/key used to look up the value in a <see cref="VarioStorage"/>.
        /// </summary>
        /// <value>The string identifier of the variable; ignored if <see cref="Source"/> is a <c>Raw</c>.</value>
        public string Name
        {
            get => name;
            set => name = value;
        }

        /// <summary>
        /// Gets or sets a literal/constant value.
        /// </summary>
        /// <value>The raw value of type <typeparamref name="T"/> used when <see cref="Source"/> is set to <c>Raw</c>.</value>
        public T Value
        {
            get => value is UnityEngine.Object unityObject && unityObject == null ? default : value;
            set  => this.value = value;
        }

        /// <summary>
        /// Evaluates the value based on its source. Method falls back to global storages if not found
        /// in <c>storage</c>
        /// If source is <c>Raw</c>, returns <see cref="Value"/>.
        /// If source is <c>Storage</c>, attempts to retrieve the value from the provided <paramref name="storage"/>.
        /// </summary>
        /// <param name="storage">The storage to look up the value in if source is <c>Storage</c>. Can be null</param>
        /// <param name="defaultValue">The value to return if the variable is not found in storage.</param>
        /// <returns>The evaluated value.</returns>
        public T Evaluate(VarioStorage storage, T defaultValue = default)
            => VarioInternalHelper.Evaluate(storage, this, true, defaultValue);

        /// <summary>
        /// Evaluates the value based on its source. Method ignores global storages if not found
        /// in <c>storage</c>
        /// If source is <c>Raw</c>, returns <see cref="Value"/>.
        /// If source is <c>Storage</c>, attempts to retrieve the value from the provided <paramref name="storage"/>.
        /// </summary>
        /// <param name="storage">The storage to look up the value in if source is <c>Storage</c>. Can be null</param>
        /// <param name="defaultValue">The value to return if the variable is not found in storage.</param>
        /// <returns>The evaluated value.</returns>
        public T EvaluateLocal(VarioStorage storage, T defaultValue = default)
            => VarioInternalHelper.Evaluate(storage, this, false, defaultValue);
        
        /// <summary>
        /// Creates a <see cref="VarioValue{T}"/> initialized with a raw value.
        /// </summary>
        /// <param name="value">The raw value to store.</param>
        /// <returns>A new instance configured for <c>ValueSource.Raw</c>.</returns>
        public static VarioValue<T> Raw(T value) => new()
        {
            Source = ValueSource.Raw,
            Value = value
        };
        
        /// <summary>
        /// Creates a <see cref="VarioValue{T}"/> initialized to reference a variable in storage.
        /// </summary>
        /// <param name="name">The name of the variable to reference.</param>
        /// <returns>A new instance configured for <c>ValueSource.Storage</c>.</returns>
        public static VarioValue<T> FromStorage(string name) => new()
        {
            Source = ValueSource.Storage,
            Name = name
        };
        
        public static implicit operator VarioValue<T>(T value) => new() { source = ValueSource.Raw, Value = value };
        
        [SerializeField] private ValueSource source;
        [SerializeField] private string name;
        [SerializeField] private T value;
    }
    
}