using System;
using System.Collections.Generic;

namespace Damdor.Vario
{
    /// <summary>
    /// Provides global configuration and registry settings for the Variable Storage system.
    /// </summary>
    public static class VarioSettings
    {
        /// <summary>
        /// Gets a list of all <see cref="Type"/> objects currently supported and registered in the system.
        /// If you need to add new supported type then use method <c>RegisterVariableType</c> 
        /// </summary>
        /// <value>A collection of types that can be handled by the storage system.</value>
        public static IReadOnlyList<Type> SupportedTypes
        {
            get
            {
                EnsureInit();
                return supportedTypes;
            }
        }

        /// <summary>
        /// Gets a list of all registered global variable storages.
        /// These storages are accessible globally and can be used for application-wide variables.
        /// </summary>
        /// <value>A read-only collection of registered global <see cref="VarioStorage"/> instances.</value>
        public static IReadOnlyList<VarioStorage> GlobalStorages
        {
            get
            {
                EnsureInit();
                return globalStorages;
            }
        }

        private static bool init;
        
        private static readonly List<Type> supportedTypes = new();
        private static readonly List<VarioStorage> globalStorages = new();
        private static readonly Dictionary<Type, Type> valueToVariableType = new();
        private static readonly Dictionary<Type, string> variableTypeToName = new();
        
        public static VarioVariable<T> Create<T>()
        {
            EnsureInit();
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

        private static void EnsureInit()
        {
            if (init) return;
            
            Reset();
            VarioSettingsLoader.Load();
            init = true;
        }

        internal static void Reset()
        {
            init = false;
            supportedTypes.Clear();
            globalStorages.Clear();
            valueToVariableType.Clear();
            variableTypeToName.Clear();
        }
        
        internal static void RegisterVariableType(Type type, string name)
        {
            if(supportedTypes.Contains(type)) return;
            supportedTypes.Add(type);
            variableTypeToName[type] = name;
 
            var variable = (VarioVariable) Activator.CreateInstance(type);
            valueToVariableType[variable.Type] = type;
        }

        internal static void RegisterGlobalStorage(VarioStorage storage)
        {
            globalStorages.Add(storage);
        }
        
    }
}