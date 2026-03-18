using System;
using System.Collections.Generic;
using UnityEngine;

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

        /// <summary>
        /// Registers a new variable type to be recognized by the storage system.
        /// </summary>
        /// <remarks>
        /// The list is used only for creating a proper editor therefore new types should be created in editor script
        /// in response from compilation.
        /// </remarks>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// public class VariableStorageIntegration
        /// {
        ///    [UnityEditor.Callbacks.DidReloadScripts]
        ///    private static void OnScriptsReloaded()
        ///    {
        ///        VariableStorageSettings.RegisterVariableType<MyCustomVariableType>();
        ///    }
        ///
        /// }
        /// ]]>
        /// </code>
        /// </example>
        /// <typeparam name="T">The specific class deriving from <see cref="VarioVariable"/> to register.</typeparam>
        public static void RegisterVariableType<T>() where T : VarioVariable
        {
            EnsureInit();
            DoRegisterVariableType<T>();
        }

        /// <summary>
        /// Registers a custom <see cref="VarioStorage"/> instance as a global storage.
        /// </summary>
        /// <param name="globalStorage">The variable storage instance to register globally.</param>
        public static void RegisterGlobalStorage(VarioStorage globalStorage)
        {
            EnsureInit();
            globalStorages.Add(globalStorage);
        }

        /// <summary>
        /// Registers a predefined default global storage based on the provided enum value.
        /// </summary>
        /// <param name="storage">The type of default global storage to register (e.g., Easing).</param>
        public static void RegisterGlobalStorage(VarioDefaultStorage storage)
        {
            switch (storage)
            {
                case VarioDefaultStorage.Easing:
                    RegisterGlobalStorage(
                        Resources.Load<VarioGlobalStorage>("Vario_DefaultEasing").Storage
                    );
                break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(storage), storage, null);
            }
            
        }
        
        /// <summary>
        /// Clears all custom registrations and restores the system to its original default state. This method should
        /// be invoked before your game restarts if you register any custom types
        /// </summary>
        public static void ResetToInitialSettings()
        {
            ResetSupportedTypes();
            globalStorages.Clear();
            init = true;
        }

        private static void ResetSupportedTypes()
        {
            supportedTypes.Clear();
            valueToVariableType.Clear();

            DoRegisterVariableType<BoolVarioVariable>();
            DoRegisterVariableType<IntVarioVariable>();
            DoRegisterVariableType<FloatVarioVariable>();
            DoRegisterVariableType<StringVarioVariable>();
            DoRegisterVariableType<ColorVarioVariable>();
            DoRegisterVariableType<Vector2VarioVariable>();
            DoRegisterVariableType<Vector3VarioVariable>();
            DoRegisterVariableType<GameObjectVarioVariable>();
            DoRegisterVariableType<RectVarioVariable>();
            DoRegisterVariableType<AnimationCurveVarioVariable>();
            DoRegisterVariableType<AudioClipVarioVariable>();
            DoRegisterVariableType<MaterialVarioVariable>();
            DoRegisterVariableType<ShaderVarioVariable>();
            DoRegisterVariableType<MeshVarioVariable>();
            DoRegisterVariableType<TextureVarioVariable>();
            DoRegisterVariableType<SpriteVarioVariable>();
            DoRegisterVariableType<TextAssetVarioVariable>();
            DoRegisterVariableType<LayerMaskVarioVariable>();
            DoRegisterVariableType<TransformVarioVariable>();
            DoRegisterVariableType<RectTransformVarioVariable>();
            DoRegisterVariableType<CanvasGroupVarioVariable>();
            DoRegisterVariableType<GraphicVarioVariable>();
            DoRegisterVariableType<ImageVarioVariable>();
            
#if TEXT_MESH_PRO
            DoRegisterVariableType<TMPTextVarioVariable>();
#endif
        }
        
        internal static TypedVarioVariable<T> Create<T>()
        {
            EnsureInit();
            if (valueToVariableType.TryGetValue(typeof(T), out var type))
            {
                return (TypedVarioVariable<T>)Activator.CreateInstance(type);
            }
            return null;
        }
        
        private static void DoRegisterVariableType<T>() where T : VarioVariable
        {
            if(supportedTypes.Contains(typeof(T))) return;
            supportedTypes.Add(typeof(T));

            var variable = Activator.CreateInstance<T>();
            valueToVariableType[variable.Type] = typeof(T);
        }

        private static void EnsureInit()
        {
            if (init) return;
            ResetToInitialSettings();
        }
        
    }
}