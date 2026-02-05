using System;
using System.Collections.Generic;
using Damdor.Foundation;

namespace Damdor.VariableStorage
{
    public static class VariableStorageSettings
    {
        private class ConverterData
        {
            public Type Type;
            public Type SerializableType;
            public Delegate ToSerializable;
            public Delegate FromSerializable;
        }

        public static List<Type> SupportedTypes
        {
            get
            {
                EnsureInit();
                return supportedTypes;
            }
        }

        private static bool init;
        
        private static readonly List<Type> supportedTypes = new();
        private static readonly List<ConverterData> converters = new();

        public static void RegisterVariableType<T>() where T : Variable
        {
            EnsureInit();
            if(supportedTypes.Contains(typeof(T))) return;
            supportedTypes.Add(typeof(T));
        }

        public static void RegisterConverter<T, TSerializable>(
            Converter<T, TSerializable> toSerializable,
            Converter<TSerializable, T> fromSerializable)
        {
            EnsureInit();
            DoRegisterConverter(toSerializable, fromSerializable);
        }

        public static void ResetToInitialSettings()
        {
            ResetSupportedTypes();
            ResetConverters();
            init = true;
        }

        private static void ResetSupportedTypes()
        {
            supportedTypes.Clear();

            supportedTypes.Add(typeof(BoolVariable));
            supportedTypes.Add(typeof(IntVariable));
            supportedTypes.Add(typeof(FloatVariable));
            supportedTypes.Add(typeof(StringVariable));
            supportedTypes.Add(typeof(ColorVariable));
            supportedTypes.Add(typeof(Vector2Variable));
            supportedTypes.Add(typeof(Vector3Variable));
            supportedTypes.Add(typeof(GameObjectVariable));
            supportedTypes.Add(typeof(RectVariable));
            supportedTypes.Add(typeof(AnimationCurveVariable));
            supportedTypes.Add(typeof(AudioClipVariable));
            supportedTypes.Add(typeof(MaterialVariable));
            supportedTypes.Add(typeof(ShaderVariable));
            supportedTypes.Add(typeof(MeshVariable));
            supportedTypes.Add(typeof(TextureVariable));
            supportedTypes.Add(typeof(SpriteVariable));
            supportedTypes.Add(typeof(TextAssetVariable));
            supportedTypes.Add(typeof(LayerMaskVariable));

#if DAMDOR_FOUNDATION
            supportedTypes.Add(typeof(DateTimeVariable));
            supportedTypes.Add(typeof(TimeSpanVariable));
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