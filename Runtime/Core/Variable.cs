using System;
using UnityEngine;

namespace Damdor.VariableStorage
{
    [Serializable]
    public abstract class Variable
    {
        public abstract Type Type { get; }
        public abstract string Name { get; set; }
    }

    [Serializable]
    public abstract class TypedVariable<T> : Variable
    {
        public override Type Type => typeof(T);
        public abstract T Value { get; set; }
    }

    [Serializable]
    public class Variable<T> : TypedVariable<T>
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
    
    [Serializable]
    public abstract class Variable<T, TSerialize> : TypedVariable<T>
    {
        public override T Value
        {
            get => VariableStorageSettings.FromSerializable<T, TSerialize>(value);
            set => this.value = VariableStorageSettings.ToSerializable<T, TSerialize>(value);
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