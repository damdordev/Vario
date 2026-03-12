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
            get => value;
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
    
    /// <summary>
    /// A specialized wrapper for values that require a different format for serialization.
    /// </summary>
    /// <typeparam name="T">The runtime type used in logic.</typeparam>
    /// <typeparam name="TSerializable">The type used by Unity's serialization system.</typeparam>
    [Serializable]
    public struct VarioValue<T, TSerializable>
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
        /// Gets or sets the serializable representation of the value.
        /// </summary>
        /// <value>The data of type <typeparamref name="TSerializable"/> stored on disk or in scenes.</value>
        // ReSharper disable once UnusedMember.Global
        public TSerializable SerializableValue
        {
            get => value;
            set  => this.value = value;
        }
        
        /// <summary>
        /// Gets or sets a literal/constant value.
        /// </summary>
        /// <value>The raw value of type <typeparamref name="T"/> used when <see cref="Source"/> is set to <c>Raw</c>.</value>
        public T Value
        {
            get => VarioSettings.FromSerializable<T, TSerializable>(value);
            set => this.value = VarioSettings.ToSerializable<T, TSerializable>(value);
        }
        
        /// <summary>
        /// Evaluates the value based on its source. Method falls back to global storages if not found
        /// in <c>storage</c>
        /// If source is <c>Raw</c>, returns <see cref="Value"/>.
        /// If source is <c>Storage</c>, attempts to retrieve the value from the provided <paramref name="storage"/>.
        /// </summary>
        /// <param name="storage">The storage to look up the value in if source is <c>Storage</c>. Can be null.</param>
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
        /// Creates a <see cref="VarioValue{T, TSerializable}"/> initialized with a raw value.
        /// </summary>
        /// <param name="value">The raw value to store.</param>
        /// <returns>A new instance configured for <c>ValueSource.Raw</c>.</returns>
        public static VarioValue<T, TSerializable> Raw(T value) => new()
        {
            Source = ValueSource.Raw,
            Value = value
        };
        
        /// <summary>
        /// Creates a <see cref="VarioValue{T, TSerializable}"/> initialized to reference a variable in storage.
        /// </summary>
        /// <param name="name">The name of the variable to reference.</param>
        /// <returns>A new instance configured for <c>ValueSource.Storage</c>.</returns>
        public static VarioValue<T, TSerializable> FromStorage(string name) => new()
        {
            Source = ValueSource.Storage,
            Name = name
        };
        
        public static implicit operator VarioValue<T, TSerializable>(T value) => new() { source = ValueSource.Raw, Value = value };
        
        [SerializeField] private ValueSource source;
        [SerializeField] private string name;
        [SerializeField] private TSerializable value;
    }
}