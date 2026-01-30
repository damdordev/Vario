using System;
using UnityEngine;

namespace Damdor.VariableStorage
{
    [Serializable]
    public abstract class Variable
    {
        public abstract Type Type { get; }

        public string Name
        {
            get => name;
            set => name = value;
        }

        [SerializeField] private string name;
    }

    [Serializable]
    public class Variable<T> : Variable
    {
        public override Type Type => typeof(T);

        public T Value
        {
            get => value;
            set => this.value = value;
        }

        [SerializeField] private T value;
    }
}