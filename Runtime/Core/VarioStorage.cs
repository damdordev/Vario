using System;
using System.Collections.Generic;
using UnityEngine;

namespace Damdor.Vario
{
    /// <summary>
    /// Manages a collection of variables and provides methods for retrieval, removal and updates.
    /// </summary>
    [Serializable]
    public class VarioStorage : ISerializationCallbackReceiver
    {
        /// <summary>
        /// Gets the list of all variables registered in this storage.
        /// </summary>
        public IReadOnlyList<IReadonlyVarioVariable> Variables => variables;
        
        [SerializeReference] private List<VarioVariable> variables = new();
        private Dictionary<string, VarioVariable> lookupCache;
        private bool isReleased;

        /// <summary>
        /// Retrieves the value of a variable by name.
        /// </summary>
        /// <typeparam name="T">The type of the variable to retrieve.</typeparam>
        /// <param name="name">The name of the variable.</param>
        /// <param name="defaultValue">The value to return if the variable is not found.</param>
        /// <returns>The value of the variable, or the default value if not found.</returns>
        public T Get<T>(string name, T defaultValue = default)
        {
            AssertNameIsNotNull(name);
            var variable = GetVariableImpl<T>(name);
            return variable == null ? defaultValue : variable.Value;
        }
        
        /// <summary>
        /// Attempts to retrieve the value of a variable by name.
        /// </summary>
        /// <typeparam name="T">The type of the variable to retrieve.</typeparam>
        /// <param name="name">The name of the variable.</param>
        /// <param name="value">The variable value if found; otherwise, the default value.</param>
        /// <returns>True if the variable was found and is of type T; otherwise, false.</returns>
        public bool TryGet<T>(string name, out T value)
        {
            AssertNameIsNotNull(name);
            var variable = GetVariableImpl<T>(name);
            if (variable != null)
            {
                value = variable.Value;
                return true;
            }

            value = default;
            return false;
        }
        
        /// <summary>
        /// Checks if a variable with the specified name and type exists in the storage.
        /// </summary>
        /// <typeparam name="T">The type of the variable to check for.</typeparam>
        /// <param name="name">The name of the variable.</param>
        /// <returns>True if the variable exists, otherwise false.</returns>
        public bool Contains<T>(string name)
        {
            AssertNameIsNotNull(name);
            return GetVariableImpl<T>(name) != null;
        }
        
        /// <summary>
        /// Checks if a variable with the specified name exists in the storage.
        /// </summary>
        /// <param name="name">The name of the variable.</param>
        /// <returns>True if the variable exists, otherwise false.</returns>
        public bool Contains(string name)
        {
            AssertNameIsNotNull(name);
            return GetVariableImpl(name) != null;
        }

        /// <summary>
        /// Retrieves the type of a variable with the specified name.
        /// </summary>
        /// <param name="name">The name of the variable whose type is to be retrieved.</param>
        /// <returns>The type of the variable if found; otherwise, null.</returns>
        public Type GetType(string name)
        {
            AssertNameIsNotNull(name);
            return GetVariableImpl(name)?.Type;
        }
        
        /// <summary>
        /// Updates the value of a variable. If the variable does not exist, it will be created.
        /// </summary>
        /// <typeparam name="T">The type of the variable.</typeparam>
        /// <param name="name">The name of the variable to update or create.</param>
        /// <param name="value">The new value to set.</param>
        public void Update<T>(string name, T value)
        {
            AssertNameIsNotNull(name);
            var variable = GetVariableImpl(name);

            if (variable is not VarioVariable<T> typedVariable)
            {
                var newVariable = VarioPooling.PopVariable<T>();
                newVariable.Name = name;
                newVariable.Value = value;
                if(variable == null)
                {
                    variables.Add(newVariable);
                }
                else
                {
                    var index = variables.IndexOf(variable);
                    VarioPooling.ReleaseVariable(variable);
                    variables[index] = newVariable;
                }
                UpdateLookupCache(newVariable);
            } 
            else
            {
                typedVariable.Value = value;
            }
        }
        
        /// <summary>
        /// Updates the value of a variable by cloning passed variable.
        /// </summary>
        /// <param name="variable">Variable to clone</param>
        public void Update(IReadonlyVarioVariable variable)
        {
            EnsureLookupCache();
            if(variable == null) throw new ArgumentNullException(nameof(variable));
            AssertNameIsNotNull(variable.Name);

            var oldVariable = GetVariableImpl(variable.Name);
            if (variable == oldVariable) return;
            if (oldVariable == null)
            {
                var newVariable = variable.Clone();
                variables.Add(newVariable);
                UpdateLookupCache(newVariable);
            }
            else
            {
                if (oldVariable.Type == variable.Type)
                {
                    oldVariable.TryCopyFrom(variable);
                }
                else
                {             
                    var newVariable = variable.Clone();
                    variables[variables.IndexOf(oldVariable)] = newVariable;
                    VarioPooling.ReleaseVariable(oldVariable);
                    UpdateLookupCache(newVariable);
                }
            }

        }

        /// <summary>
        /// Removes a variable from the storage.
        /// </summary>
        /// <param name="name">The name of the variable to remove.</param>
        /// <returns>True if the variable was found and removed, otherwise false.</returns>
        public bool Remove(string name)
        {
            AssertNameIsNotNull(name);
            EnsureLookupCache();
            Remove(name, out var index);
            return index >= 0;
        }
        
        /// <summary>
        /// Creates a clone of the storage utilizing object pooling.
        /// </summary>
        /// <returns>A cloned instance of the VarioStorage.</returns>
        public VarioStorage Clone()
        {
            var storage = VarioPooling.PopStorage();
            storage.Clear();
            storage.EnsureLookupCache();
            foreach (var variable in variables)
            {
                if(variable == null || variable.Name == null) continue;
                var clone = variable.Clone();
                storage.variables.Add(clone);
                storage.lookupCache[clone.Name] = clone;
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
            if (isReleased) return;
            isReleased = true;
            VarioPooling.ReleaseStorage(this);
        }

        /// <summary>
        /// Clears all variables from the storage and resetting the internal collection.
        /// </summary>
        public void Clear()
        {
            variables.Clear();
            lookupCache?.Clear();
        }

        public void CopyFrom(VarioStorage other)
        {
            if (other == null || other == this) return;
            foreach (var variable in other.variables)
            {
                if(variable != null) Update(variable);
            }
        }

        internal void Reset()
        {
            foreach (var variable in variables) VarioPooling.ReleaseVariable(variable);
            Clear();
        }

        internal void OnGetFromPool()
        {
            isReleased = false;
        }
        
        private VarioVariable<T> GetVariableImpl<T>(string name)
        {
            var variable = GetVariableImpl(name);
            if (variable is VarioVariable<T> typedVariable) return typedVariable;

            return null;
        }
        
        private VarioVariable GetVariableImpl(string name)
        {
            EnsureLookupCache();
            lookupCache.TryGetValue(name, out var variable);
            return variable;
        }
        
        private void Remove(string name, out int index)
        {
            index = -1;
            AssertNameIsNotNull(name);

            var variable = GetVariableImpl(name);
            if(variable == null) return;
            
            index = variables.IndexOf(variable);
            if (index < 0) return;

            lookupCache.Remove(name);
            variables.RemoveAt(index);
            VarioPooling.ReleaseVariable(variable);
        }

        private void AssertNameIsNotNull(string name)
        {
            if(name == null) throw new ArgumentNullException(nameof(name));
        }
        
        private void UpdateLookupCache(VarioVariable variable)
        {
            EnsureLookupCache();
            lookupCache[variable.Name] = variable;
        }
        
        private void EnsureLookupCache()
        {
            if (lookupCache == null) RebuildLookupCache();
        }
        
        private void RebuildLookupCache()
        {
            lookupCache ??= new Dictionary<string, VarioVariable>();
            lookupCache.Clear();

            foreach (var variable in variables)
            {
                if (variable != null && variable.Name != null)
                {
                    lookupCache[variable.Name] = variable;
                }
            }
        }
        
        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            RebuildLookupCache();
        }
        
    }
}