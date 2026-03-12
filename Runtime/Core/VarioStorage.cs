using System;
using System.Collections.Generic;
using UnityEngine;

namespace Damdor.Vario
{
    /// <summary>
    /// Manages a collection of variables and provides methods for retrieval, removal and updates.
    /// </summary>
    [Serializable]
    public class VarioStorage
    {
        /// <summary>
        /// Gets the list of all variables registered in this storage.
        /// </summary>
        public List<VarioVariable> Variables => variables;
        
        [SerializeReference] private List<VarioVariable> variables = new();
        
        /// <summary>
        /// Retrieves a typed variable instance.
        /// </summary>
        /// <typeparam name="T">The type of the variable to find.</typeparam>
        /// <param name="name">The name of the variable.</param>
        /// <returns>The <see cref="TypedVarioVariable{T}"/> if it exists and matches the type; otherwise, <c>null</c>.</returns>
        public TypedVarioVariable<T> GetVariable<T>(string name)
        {
            foreach (var variable in variables)
            {
                if (variable is TypedVarioVariable<T> typedVariable && variable.Name == name) return typedVariable;
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
        /// <param name="varioVariable">The <see cref="VarioVariable"/> instance to add.</param>
        public void AddVariable(VarioVariable varioVariable)
        {
            variables.Add(varioVariable);
        }

    }
}