using System;
using System.Collections.Generic;
using UnityEngine;

namespace Damdor.VariableStorage
{
    [Serializable]
    public class VariableStorage
    {
        public List<Variable> Variables => variables;
        [SerializeReference] private List<Variable> variables = new();

        public T Evaluate<T>(string name, T defaultValue = default) => Evaluate<T>(name, null, defaultValue);
        
        public T Evaluate<T>(string name, VariableStorage parent, T defaultValue = default)
        {
            if(TryEvaluate<T>(variables, name, out var result)) return result;
            if (parent != null && TryEvaluate(parent.variables, name, out result)) return result;
            return defaultValue;
        }

        public T Evaluate<T>(StorageValue<T> value, T defaultValue = default) => Evaluate<T>(value, null, defaultValue);

        public T Evaluate<T>(StorageValue<T> value, VariableStorage parent, T defaultValue = default) => value.Source switch
        {
            ValueSource.Raw => value.Value,
            ValueSource.Storage => Evaluate(value.Name, parent, defaultValue),
            _ => throw new ArgumentOutOfRangeException($"Unknown value source: {typeof(T).Name} ")
        };
        
        public T Evaluate<T, TSerializable>(StorageValue<T, TSerializable> value, T defaultValue = default)
            => Evaluate(value, null, defaultValue);
        
        public T Evaluate<T, TSerializable>(StorageValue<T, TSerializable> value, VariableStorage parent, T defaultValue = default) => value.Source switch
        {
            ValueSource.Raw => value.Value,
            ValueSource.Storage => Evaluate(value.Name, parent, defaultValue),
            _ => throw new ArgumentOutOfRangeException($"Unknown value source: {typeof(T).Name} ")
        };

        public TypedVariable<T> GetVariable<T>(string name)
        {
            foreach (var variable in variables)
            {
                if (variable is TypedVariable<T> typedVariable && variable.Name == name) return typedVariable;
            }

            return null;
        }
        
        public bool HasVariable<T>(string name)
        {
            return GetVariable<T>(name) != null;
        }

        public bool UpdateVariable<T>(string name, T value)
        {
            var variable = GetVariable<T>(name);
            if(variable != null)  variable.Value = value;
            return variable != null;
        }

        public void AddVariable(Variable variable)
        {
            variables.Add(variable);
        }

        private bool TryEvaluate<T>(List<Variable> variables, string name, out T value)
        {
            foreach (var variable in variables)
            {
                if (variable.Name == name && variable.Type == typeof(T) && variable is TypedVariable<T> typedVariable)
                {
                    value = typedVariable.Value;
                    return true;
                }
            }

            value = default;
            return false;
        }

    }
}