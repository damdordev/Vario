using System;
using UnityEngine;

namespace Damdor.Vario
{
    /// <summary>
    /// Base class representing a variable within the storage system.
    /// </summary>
    [Serializable]
    public abstract class VarioVariable
    {
        /// <summary>
        /// Gets the data type stored by this variable.
        /// </summary>
        public abstract Type Type { get; }
        
        /// <summary>
        /// Gets or sets the unique name identifying the variable.
        /// </summary>
        public abstract string Name { get; set; }
    }

    /// <summary>
    /// A generic base class for variables that store a specific type of value.
    /// </summary>
    /// <typeparam name="T">The type of the value being stored.</typeparam>
    [Serializable]
    public abstract class TypedVarioVariable<T> : VarioVariable
    {
        public override Type Type => typeof(T);
        
        /// <summary>
        /// Gets or sets the value of the variable.
        /// </summary>
        public abstract T Value { get; set; }
    }

    /// <summary>
    /// A generic base class for variables of serializable types.
    /// </summary>
    /// <typeparam name="T">The type of the value being stored.</typeparam>
    [Serializable]
    public class VarioVariable<T> : TypedVarioVariable<T>
    {
        public override Type Type => typeof(T);

        public override T Value
        {
            get => value;
            set => this.value = value;
        }
        
        public override string Name
        {
            get => name;
            set => name = value;
        }

        [SerializeField] private T value;
        [SerializeField] private string name;
    }
    
    /// <summary>
    /// A generic base class for variables of non-serializable types. The class stores a variable with <c>TSerializable</c>
    /// type and exposes it with <c>T</c>
    /// </summary>
    /// <remarks>
    /// To use variable of that type you have to register conversion via <c>VariableStorageSettings.RegisterConverter</c>
    /// </remarks>
    /// <typeparam name="T">The type of the value being exposed</typeparam>
    /// <typeparam name="TSerialize">The type of the value being stored</typeparam>
    [Serializable]
    public abstract class VarioVariable<T, TSerialize> : TypedVarioVariable<T>
    {
        public override T Value
        {
            get => VarioSettings.FromSerializable<T, TSerialize>(value);
            set => this.value = VarioSettings.ToSerializable<T, TSerialize>(value);
        }
        
        public override string Name
        {
            get => name;
            set => name = value;
        }

        [SerializeField] private TSerialize value;
        [SerializeField] private string name;
    }
    
}