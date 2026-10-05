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
        
        /// <summary>
        /// Creates an instance of the variable with the same name and value as the current instance.
        /// </summary>
        /// <returns>
        /// A new <see cref="VarioVariable"/> instance that is a clone of the current instance.
        /// </returns>
        public abstract VarioVariable Clone();
        
        internal abstract void Reset();
        internal abstract void TryCopyFrom(VarioVariable variable);
    }

    /// <summary>
    /// A generic base class for variables of serializable types.
    /// </summary>
    /// <typeparam name="T">The type of the value being stored.</typeparam>
    [Serializable]
    public class VarioVariable<T> : VarioVariable
    {
        public override Type Type => typeof(T);

        public T Value
        {
            get => value;
            set => this.value = value;
        }
        
        public override string Name
        {
            get => name;
            set => name = value;
        }

        public override VarioVariable Clone()
        {
            var variable = VarioPooling.PopVariable<T>();
            variable.Name = Name;
            variable.Value = Value;
            return variable;
        }

        /// <summary>
        /// Copies the name and value from the specified variable to the current instance.
        /// </summary>
        /// <param name="other">The variable from which to copy the name and value.</param>
        public void CopyFrom(VarioVariable<T> other)
        {
            TryCopyFrom(other);
        }

        internal override void Reset()
        {
            Name = null;
            Value = default;
        }
        
        internal override void TryCopyFrom(VarioVariable variable)
        {
            if (variable is VarioVariable<T> typedVariable)
            {
                Value = typedVariable.Value;   
            }
        }

        [SerializeField] private T value;
        [SerializeField] private string name;
    }
    
}