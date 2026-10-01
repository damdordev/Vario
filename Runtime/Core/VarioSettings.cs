using System;
using System.Collections.Generic;
using UnityEngine;

namespace Damdor.Vario
{
    /// <summary>
    /// Provides global configuration and registry settings for the Variable Storage system.
    /// </summary>
    public static partial class VarioSettings
    {
        /// <summary>
        /// Gets a list of all <see cref="Type"/> objects currently supported and registered in the system.
        /// If you need to add new supported type then use method <c>RegisterVariableType</c> 
        /// </summary>
        /// <value>A collection of types that can be handled by the storage system.</value>
        public static IReadOnlyList<Type> SupportedTypes => supportedTypes;

        /// <summary>
        /// Gets a list of all registered global variable storages.
        /// These storages are accessible globally and can be used for application-wide variables.
        /// </summary>
        /// <value>A read-only collection of registered global <see cref="VarioStorage"/> instances.</value>
        public static IReadOnlyList<VarioStorage> GlobalStorages
        {
            get
            {
                EnsureGlobalStoragesInit();
                return globalStorages;
            }
        }

        /// <summary>
        /// Gets or sets the maximum number of variables of each type to be stored in the pool.
        /// When the pool size is reduced, excess variables are discarded.
        /// </summary>
        public static int VariablePoolSize
        {
            get => VarioPooling.VariablePoolSize;
            set => VarioPooling.VariablePoolSize = value;
        }
        
        /// <summary>
        /// Gets or sets the maximum number of storages to be stored in the pool.
        /// When the pool size is reduced, excess storages are discarded.
        /// </summary>
        public static int StoragePoolSize
        {
            get => VarioPooling.StoragePoolSize;
            set => VarioPooling.StoragePoolSize = value;
        }
        
        private static readonly List<Type> supportedTypes = new();
        private static readonly Dictionary<Type, Type> valueToVariableType = new();
        private static readonly Dictionary<Type, string> variableTypeToName = new();
        
        private static bool globalStoragesInit;
        private static readonly List<VarioStorage> globalStorages = new();

        private static readonly Dictionary<Type, INumericOperations> algorithms = new();
        
        public static VarioVariable<T> Create<T>()
        {
            if (valueToVariableType.TryGetValue(typeof(T), out var type))
            {
                return (VarioVariable<T>)Activator.CreateInstance(type);
            }
            return null;
        }

        public static string GetVariableName(Type type)
        {
            var name = variableTypeToName.GetValueOrDefault(type, "");
            return name;
        }
        
        /// <summary>
        /// Gets the registered numeric operations algorithm for the specified type.
        /// </summary>
        /// <typeparam name="T">The type to get operations for.</typeparam>
        /// <returns>The operations algorithm if found; otherwise, null.</returns>
        public static INumericOperations<T> GetNumericOperations<T>() =>
            algorithms.TryGetValue(typeof(T), out var result)
                ? (INumericOperations<T>)result 
                : null;

        public static void RegisterNumericOperations(INumericOperations numericOperations)
        {
            algorithms[numericOperations.NumberType] = numericOperations;
        }
        
        private static void EnsureGlobalStoragesInit()
        {
            if (globalStoragesInit) return;

            globalStorages.Clear();
            var library = Resources.Load<VarioGlobalStorageLibrary>("VarioGlobalStorages");
            if (library != null)
            {
                foreach (var globalStorage in library.GlobalStorages)
                {
                    if(globalStorage == null || globalStorage.Storage == null) continue;
                    globalStorages.Add(globalStorage.Storage);
                }
            }
            
            globalStoragesInit = true;
        }

        internal static void ResetGlobalStoragesInit()
        {
            globalStoragesInit = false;
        }
        
        
        public static void RegisterVariableType(Type type, string name)
        {
            if(supportedTypes.Contains(type)) return;
            supportedTypes.Add(type);
            variableTypeToName[type] = name;
 
            var variable = (VarioVariable) Activator.CreateInstance(type);
            valueToVariableType[variable.Type] = type;
        }
        
    }
}