using System;
using System.Collections.Generic;
using Damdor.Foundation;
using UnityEditor;
using UnityEngine;

namespace Damdor.Vario
{
    /// <summary>
    /// Provides global configuration and registry settings for the Variable Storage system.
    /// </summary>
    public class VarioSettings : AssetPostprocessor
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
        
        public static TypedVarioVariable<T> Create<T>()
        {
            EnsureInit();
            if (valueToVariableType.TryGetValue(typeof(T), out var type))
            {
                return (TypedVarioVariable<T>)Activator.CreateInstance(type);
            }
            return null;
        }

        public static string GetVariableName(Type type)
        {
            if (!variableTypeToName.TryGetValue(type, out var name)) name = "";
            return name;
        }

        private static void EnsureInit()
        {
            if (init) return;

            supportedTypes.Clear();
            globalStorages.Clear();
            valueToVariableType.Clear();
            variableTypeToName.Clear();
            
            var settingsAssets = Resources.LoadAll<TextAsset>("vario_settings");
            for (var i = 0; i < settingsAssets.Length; ++i)
            {
                var json = settingsAssets[i].text;
                var settingsData = (Dictionary<string, object>) Json.Deserialize(json);
                Resources.UnloadAsset(settingsAssets[i]);

                if (settingsData != null && settingsData.TryGetValue("types", out var types))
                {
                    ProcessTypesFromSettingsData((Dictionary<string, object>) types);
                }
                if (settingsData != null && settingsData.TryGetValue("globalStorages", out var globalStorages))
                {
                    ProcessGlobalStoragesFromSettingsData((List<object>) globalStorages);
                }
            }

            init = true;
        }
        

        private static void ProcessTypesFromSettingsData(Dictionary<string, object> types)
        {
            foreach (var pair in types)
            {
                var type = ReflectionHelper.FindType((string) pair.Value);
                if (type != null) RegisterVariableType(type, pair.Key);
            }
        }
        
        private static void ProcessGlobalStoragesFromSettingsData(List<object> storages)
        {
            foreach (string storageName in storages)
            {
                var storage = Resources.Load<VarioGlobalStorage>(storageName);
                if(storage != null) globalStorages.Add(storage.Storage);
            }
        }  
        
        private static void RegisterVariableType(Type type, string name)
        {
            if(supportedTypes.Contains(type)) return;
            supportedTypes.Add(type);
            variableTypeToName[type] = name;
 
            var variable = (VarioVariable) Activator.CreateInstance(type);
            valueToVariableType[variable.Type] = type;
        }

        void OnPreprocessAsset()
        {
            if (assetImporter.assetPath.EndsWith("vario_settings.json"))
            {
                init = false;
            }
        }
        
    }
}