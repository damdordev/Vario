using System;
using UnityEngine;

namespace Damdor.VariableStorage
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
    public class StorageValue<T>
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
        /// Gets or sets the name/key used to look up the value in a <see cref="VariableStorage"/>.
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
    public struct StorageValue<T, TSerializable>
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
        /// Gets or sets the name/key used to look up the value in a <see cref="VariableStorage"/>.
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
            get => VariableStorageSettings.FromSerializable<T, TSerializable>(value);
            set => this.value = VariableStorageSettings.ToSerializable<T, TSerializable>(value);
        }
        
        [SerializeField] private ValueSource source;
        [SerializeField] private string name;
        [SerializeField] private TSerializable value;
    }
}