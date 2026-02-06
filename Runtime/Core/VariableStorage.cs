using System;
using System.Collections.Generic;
using UnityEngine;

namespace Damdor.VariableStorage
{
    /// <summary>
    /// Manages a collection of variables and provides methods for evaluation, retrieval, and updates.
    /// </summary>
    [Serializable]
    public class VariableStorage
    {
        /// <summary>
        /// Gets the list of all variables registered in this storage.
        /// </summary>
        public List<Variable> Variables => variables;
        
        [SerializeReference] private List<Variable> variables = new();

        /// <summary>
        /// Evaluates the value of a variable by name.
        /// </summary>
        /// <typeparam name="T">The exactly type of the variable value.</typeparam>
        /// <param name="name">The unique identifier of the variable.</param>
        /// <param name="defaultValue">The value to return if the variable is not found.</param>
        /// <returns>The value of the variable if found; otherwise, <paramref name="defaultValue"/>.</returns>
        public T Evaluate<T>(string name, T defaultValue = default) => Evaluate<T>(name, null, defaultValue);
        
        /// <summary>
        /// Evaluates the value of a variable by name, optionally searching a parent storage.
        /// </summary>
        /// <typeparam name="T">The expected type of the variable value.</typeparam>
        /// <param name="name">The unique identifier of the variable.</param>
        /// <param name="parent">An optional fallback storage to search if the variable isn't found locally.</param>
        /// <param name="defaultValue">The value to return if the variable is not found in either storage.</param>
        /// <returns>The found value or the provided <paramref name="defaultValue"/>.</returns>
        public T Evaluate<T>(string name, VariableStorage parent, T defaultValue = default)
        {
            if(TryEvaluate<T>(variables, name, out var result)) return result;
            if (parent != null && TryEvaluate(parent.variables, name, out result)) return result;
            return defaultValue;
        }

        /// <summary>
        /// Evaluates a <see cref="StorageValue{T}"/> to retrieve its final value based on its source.
        /// </summary>
        /// <typeparam name="T">The type of the value to evaluate.</typeparam>
        /// <param name="value">The storage value definition containing the source, name, or literal value.</param>
        /// <param name="defaultValue">The value to return if the variable lookup fails.</param>
        /// <returns>
        /// If <see cref="StorageValue{T}.Source"/> is <c>Storage</c>, returns the value from storage; 
        /// otherwise, returns the value <see cref="StorageValue{T}.Value"/>.
        /// </returns>
        public T Evaluate<T>(StorageValue<T> value, T defaultValue = default) => Evaluate<T>(value, null, defaultValue);

        /// <summary>
        /// Evaluates a <see cref="StorageValue{T}"/> to retrieve its final value based on its source.
        /// </summary>
        /// <typeparam name="T">The type of the value to evaluate.</typeparam>
        /// <param name="value">The storage value definition containing the source, name, or literal value.</param>
        /// <param name="parent">An optional parent storage for hierarchical lookups.</param>
        /// <param name="defaultValue">The value to return if the variable lookup fails.</param>
        /// <returns>
        /// If <see cref="StorageValue{T}.Source"/> is <c>Storage</c>, returns the value from storage; 
        /// otherwise, returns the value <see cref="StorageValue{T}.Value"/>.
        /// </returns>
        public T Evaluate<T>(StorageValue<T> value, VariableStorage parent, T defaultValue = default) => value.Source switch
        {
            ValueSource.Raw => value.Value,
            ValueSource.Storage => Evaluate(value.Name, parent, defaultValue),
            _ => throw new ArgumentOutOfRangeException($"Unknown value source: {typeof(T).Name} ")
        };
        
        /// <summary>
        /// Evaluates a <see cref="StorageValue{T, TSerializable}"/> to retrieve its final value based on its source.
        /// </summary>
        /// <typeparam name="T">The type of the value to evaluate.</typeparam>
        /// <typeparam name="TSerializable">The type of the value used to serialize a raw value.</typeparam>
        /// <param name="value">The storage value definition containing the source, name, or literal value.</param>
        /// <param name="defaultValue">The value to return if the variable lookup fails.</param>
        /// <returns>
        /// If <see cref="StorageValue{T, TSerializable}.Source"/> is <c>Storage</c>, returns the value from storage; 
        /// otherwise, returns the value <see cref="StorageValue{T, TSerializable}.Value"/>.
        /// </returns>
        public T Evaluate<T, TSerializable>(StorageValue<T, TSerializable> value, T defaultValue = default)
            => Evaluate(value, null, defaultValue);
        
        /// <summary>
        /// Evaluates a <see cref="StorageValue{T, TSerializable}"/> to retrieve its final value based on its source.
        /// </summary>
        /// <typeparam name="T">The type of the value to evaluate.</typeparam>
        /// <typeparam name="TSerializable">The type of the value used to serialize a raw value.</typeparam>
        /// <param name="value">The storage value definition containing the source, name, or literal value.</param>
        /// <param name="parent">An optional parent storage for hierarchical lookups.</param>
        /// <param name="defaultValue">The value to return if the variable lookup fails.</param>
        /// <returns>
        /// If <see cref="StorageValue{T, TSerializable}.Source"/> is <c>Storage</c>, returns the value from storage; 
        /// otherwise, returns the value <see cref="StorageValue{T, TSerializable}.Value"/>.
        /// </returns>
        public T Evaluate<T, TSerializable>(StorageValue<T, TSerializable> value, VariableStorage parent, T defaultValue = default) => value.Source switch
        {
            ValueSource.Raw => value.Value,
            ValueSource.Storage => Evaluate(value.Name, parent, defaultValue),
            _ => throw new ArgumentOutOfRangeException($"Unknown value source: {typeof(T).Name} ")
        };
        
        /// <summary>
        /// Retrieves a typed variable instance.
        /// </summary>
        /// <typeparam name="T">The type of the variable to find.</typeparam>
        /// <param name="name">The name of the variable.</param>
        /// <returns>The <see cref="TypedVariable{T}"/> if it exists and matches the type; otherwise, <c>null</c>.</returns>
        public TypedVariable<T> GetVariable<T>(string name)
        {
            foreach (var variable in variables)
            {
                if (variable is TypedVariable<T> typedVariable && variable.Name == name) return typedVariable;
            }

            return null;
        }
        
        /// <summary>
        /// Checks if a variable of a specific type exists in the storage.
        /// </summary>
        /// <typeparam name="T">The type to check for.</typeparam>
        /// <param name="name">The name of the variable.</param>
        /// <returns><c>true</c> if the variable exists and matches the type <typeparamref name="T"/>; otherwise, <c>false</c>.</returns>
        public bool HasVariable<T>(string name)
        {
            return GetVariable<T>(name) != null;
        }

        /// <summary>
        /// Updates the value of an existing variable.
        /// </summary>
        /// <remarks>It's up to developer to check if variable of the same type and name exist. If yes then use method <c>AddVariable</c></remarks> 
        /// <typeparam name="T">The type of the variable value.</typeparam>
        /// <param name="name">The name of the variable to update.</param>
        /// <param name="value">The new value to assign.</param>
        /// <returns><c>true</c> if the variable was found and updated; <c>false</c> if the variable does not exist or type mismatch.</returns>
        public bool UpdateVariable<T>(string name, T value)
        {
            var variable = GetVariable<T>(name);
            if(variable != null)  variable.Value = value;
            return variable != null;
        }

        /// <summary>
        /// Adds a new variable to the storage collection if it is not already present.
        /// </summary>
        /// <remarks>It's up to developer to check if variable of the same type and name exist. If yes then use method <c>UpdateVariable</c></remarks>
        /// <param name="variable">The <see cref="Variable"/> instance to add.</param>
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