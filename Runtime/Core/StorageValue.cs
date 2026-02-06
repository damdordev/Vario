using System;
using UnityEngine;

namespace Damdor.VariableStorage
{
    public enum ValueSource
    {
        Raw,
        Storage
    }
    
    [Serializable]
    public class StorageValue<T>
    {
        public ValueSource Source
        {
            get => source;
            set => source = value;
        }

        public string Name
        {
            get => name;
            set => name = value;
        }

        public T Value
        {
            get => value;
            set  => this.value = value;
        }
        
        
        [SerializeField] private ValueSource source;
        [SerializeField] private string name;
        [SerializeField] private T value;
    }
    
    [Serializable]
    public struct StorageValue<T, TSerializable>
    {
        public ValueSource Source
        {
            get => source;
            set => source = value;
        }

        public string Name
        {
            get => name;
            set => name = value;
        }
        
        public TSerializable SerializableValue
        {
            get => value;
            set  => this.value = value;
        }
        
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