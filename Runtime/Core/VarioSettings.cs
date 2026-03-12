using System;
using System.Collections.Generic;
using Damdor.Foundation;
using GluonGui.WorkspaceWindow.Views.WorkspaceExplorer.Explorer;
using UnityEditor.iOS;
using UnityEngine;
using UnityEngine.UI;

namespace Damdor.Vario
{
    /// <summary>
    /// Provides global configuration and registry settings for the Variable Storage system.
    /// </summary>
    public static class VarioSettings
    {
        private class ConverterData
        {
            public Type Type;
            public Type SerializableType;
            public Delegate ToSerializable;
            public Delegate FromSerializable;
        }
        
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
        private static readonly List<ConverterData> converters = new();
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
        /// Registers a pair of conversion delegates to handle transformations between a runtime type 
        /// and its serializable representation.
        /// </summary>
        /// <c>Converters are required both for creating a proper editor and in runtime</c>
        /// <typeparam name="T">The runtime data type used in game logic.</typeparam>
        /// <typeparam name="TSerializable">The data type used for serialization (e.g., a primitive or DTO).</typeparam>
        /// <param name="toSerializable">A delegate that converts the runtime type <typeparamref name="T"/> to <typeparamref name="TSerializable"/>.</param>
        /// <param name="fromSerializable">A delegate that converts the serializable type <typeparamref name="TSerializable"/> back to <typeparamref name="T"/>.</param>
        /// /// <example>
        /// <code>
        /// <![CDATA[
        /// public class VariableStorageIntegration
        /// {
        ///    // Unity cannot serialize this class. It can be a value from external library
        ///    public class MyCustomVariable
        ///    {
        ///        public int value;
        ///        // some big logic
        ///    }
        ///
        ///    [Serializable]
        ///    public class MyCustomSerializableVariable
        ///    {
        ///        public int value;
        ///    }
        /// 
        ///    [UnityEditor.Callbacks.DidReloadScripts]
        ///    private static void OnScriptsReloaded()
        ///    {
        ///        Init();
        ///    }
        ///
        ///    public void StartGame()
        ///    {
        ///        Init();
        ///    }
        ///
        ///    public void StopGame()
        ///    {
        ///        VariableStorageSettings.ResetToInitialSettings();
        ///    }
        ///
        ///    public void Init()
        ///    {
        ///    {
        ///        VariableStorageSettings.RegisterVariableType<MyCustomVariable>();
        ///        VariableStorageSettings.RegisterConverter<MyCustomVariable, MyCustomSerializableVariable>(Convert, Convert);
        ///    }
        ///
        ///    private MyCustomSerializableVariable Convert(MyCustomVariable x)
        ///        => new MyCustomSerializableVariable { Value = x.Value };
        ///
        ///    private MyCustomVariable Convert(MyCustomSerializableVariable x)
        ///        => new MyCustomVariable { Value = x.Value };        
        /// 
        /// }
        /// ]]>
        /// </code>
        /// </example>
        public static void RegisterConverter<T, TSerializable>(
            Converter<T, TSerializable> toSerializable,
            Converter<TSerializable, T> fromSerializable)
        {
            EnsureInit();
            DoRegisterConverter(toSerializable, fromSerializable);
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
            }
            
        }
        
        /// <summary>
        /// Clears all custom registrations and restores the system to its original default state. This method should
        /// be invoked before your game restarts if you register any custom types or converterss
        /// </summary>
        public static void ResetToInitialSettings()
        {
            ResetSupportedTypes();
            ResetConverters();
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

#if DAMDOR_FOUNDATION
            DoRegisterVariableType<DateTimeVarioVariable>();
            DoRegisterVariableType<TimeSpanVarioVariable>();
#endif
            
#if TEXT_MESH_PRO
            DoRegisterVariableType<TMPTextVarioVariable>();
#endif
        }
        
        private static void ResetConverters()
        {
            converters.Clear();
            
#if DAMDOR_FOUNDATION
            DoRegisterConverter<DateTime, SerializableDateTime>(x => x, x => x);
            DoRegisterConverter<TimeSpan, SerializableTimeSpan>(x => x, x => x);
#endif
        }

        public static TSerializable ToSerializable<T, TSerializable>(T value)
        {
            EnsureInit();
            var data = GetConverterData<T, TSerializable>();
            if (data == null) return default;

            var converter = (Converter<T, TSerializable>) data.ToSerializable;
            return converter(value);
        }
        
        public static T FromSerializable<T, TSerializable>(TSerializable serializableValue)
        {
            EnsureInit();
            var data = GetConverterData<T, TSerializable>();
            if (data == null) return default;

            var converter = (Converter<TSerializable, T>) data.FromSerializable;
            return converter(serializableValue);
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

        private static void DoRegisterConverter<T, TSerializable>(
            Converter<T, TSerializable> toSerializable,
            Converter<TSerializable, T> fromSerializable)
        {
            var data = GetConverterData<T, TSerializable>();
            if (data != null)
            {
                data.ToSerializable = toSerializable;
                data.FromSerializable = fromSerializable;
                return;
            }

            data = new ConverterData
            {
                Type = typeof(T),
                SerializableType = typeof(TSerializable),
                ToSerializable = toSerializable,
                FromSerializable = fromSerializable
            };
            converters.Add(data);
        }
        
        private static void DoRegisterVariableType<T>() where T : VarioVariable
        {
            if(supportedTypes.Contains(typeof(T))) return;
            supportedTypes.Add(typeof(T));

            var variable = Activator.CreateInstance<T>();
            valueToVariableType[variable.Type] = typeof(T);
        }
        
        private static ConverterData GetConverterData<T, TSerializable>()
        {
            var type = typeof(T);
            var serializableType = typeof(TSerializable);
            
            foreach (var data in converters)
            {
                if (data.Type == type && data.SerializableType == serializableType) return data;
            }

            return null;
        }

        private static void EnsureInit()
        {
            if (init) return;
            ResetToInitialSettings();
        }
        
    }
}