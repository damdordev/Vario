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
        public IReadOnlyList<VarioVariable> Variables => variables;
        
        [SerializeReference] private List<VarioVariable> variables = new();

        /// <summary>
        /// Retrieves the value of a variable by name.
        /// </summary>
        /// <typeparam name="T">The type of the variable to retrieve.</typeparam>
        /// <param name="name">The name of the variable.</param>
        /// <param name="defaultValue">The value to return if the variable is not found.</param>
        /// <returns>The value of the variable, or the default value if not found.</returns>
        public T Get<T>(string name, T defaultValue = default)
        {
            var variable = GetVariableImpl<T>(name);
            return variable == null ? defaultValue : variable.Value;
        }
        
        /// <summary>
        /// Checks if a variable with the specified name and type exists in the storage.
        /// </summary>
        /// <typeparam name="T">The type of the variable to check for.</typeparam>
        /// <param name="name">The name of the variable.</param>
        /// <returns>True if the variable exists, otherwise false.</returns>
        public bool Contains<T>(string name)
        {
            return GetVariableImpl<T>(name) != null;
        }
        
        /// <summary>
        /// Updates the value of a variable. If the variable does not exist, it will be created.
        /// </summary>
        /// <typeparam name="T">The type of the variable.</typeparam>
        /// <param name="name">The name of the variable to update or create.</param>
        /// <param name="value">The new value to set.</param>
        public void Update<T>(string name, T value)
        {
            var variable = GetVariableImpl<T>(name);
            if (variable == null)
            {
                variable = VarioSettings.Create<T>();
                variable.Name = name;
                variables.Add(variable);
            }

            variable.Value = value;
        }

        /// <summary>
        /// Removes a variable from the storage.
        /// </summary>
        /// <typeparam name="T">The type of the variable to remove.</typeparam>
        /// <param name="name">The name of the variable to remove.</param>
        /// <returns>True if the variable was found and removed, otherwise false.</returns>
        public bool Remove<T>(string name)
        {
            for (var i = 0; i < variables.Count; i++)
            {
                if (variables[i] is not VarioVariable<T> || variables[i].Name != name) continue;
                variables.RemoveAt(i);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Creates a clone of the storage utilizing object pooling.
        /// </summary>
        /// <returns>A cloned instance of the VarioStorage.</returns>
        public VarioStorage Clone()
        {
            var storage = VarioPooling.PopStorage();
            foreach (var variable in variables)
            {
                storage.variables.Add(variable.Clone());
            }

            return storage;
        }

        /// <summary>
        /// Creates a new instance of VarioStorage from the object pool.
        /// </summary>
        /// <returns>A VarioStorage instance.</returns>
        public static VarioStorage Create()
            => VarioPooling.PopStorage();

        /// <summary>
        /// Clears the storage and releases it along with its variables back to the object pool.
        /// </summary>
        /// <remarks>
        /// Note: This method should be executed only for cloned VarioStorage instances.
        /// </remarks>
        public void Release()
        {
            VarioPooling.ReleaseStorage(this);
        }

        internal void Reset()
        {
            foreach (var variable in variables) VarioPooling.ReleaseVariable(variable);
            variables.Clear();
        }
        
        private VarioVariable<T> GetVariableImpl<T>(string name)
        {
            foreach (var variable in variables)
            {
                if (variable is VarioVariable<T> typedVariable && variable.Name == name) return typedVariable;
            }

            return null;
        }

    }
}