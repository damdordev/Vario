using System;
using System.Collections.Generic;

namespace Damdor.Vario
{
    internal class VarioInternalHelper
    {
        
        public static T Evaluate<T>(VarioStorage mainStorage, string name, bool useGlobalStorages, T defaultValue)
        {
            if (mainStorage != null)
            {
                if (TryEvaluate<T>(mainStorage.Variables, name, out var result)) return result;
            }

            if (useGlobalStorages)
            {
                foreach (var storage in VarioSettings.GlobalStorages)
                {
                    if (storage != null && TryEvaluate<T>(storage.Variables, name, out var result)) return result;
                }
            }

            return defaultValue;
        }
        
        public static T Evaluate<T>(VarioStorage mainStorage, VarioValue<T> value, bool useGlobalStorages, T defaultValue) => value.Source switch
        {
            ValueSource.Raw => value.Value,
            ValueSource.Storage => Evaluate(mainStorage, value.Name, useGlobalStorages, defaultValue),
            _ => throw new ArgumentOutOfRangeException($"Unknown value source: {typeof(T).Name} ")
        };

        public static T Evaluate<T, TSerializable>(VarioStorage storage, VarioValue<T, TSerializable> value, bool useGlobalStorages, T defaultValue) => value.Source switch
        {
            ValueSource.Raw => value.Value,
            ValueSource.Storage => Evaluate(storage, value.Name, useGlobalStorages, defaultValue),
            _ => throw new ArgumentOutOfRangeException($"Unknown value source: {typeof(T).Name} ")
        };
        
        private static bool TryEvaluate<T>(List<VarioVariable> variables, string name, out T value)
        {
            foreach (var variable in variables)
            {
                if (variable.Name != name || variable.Type != typeof(T) || variable is not TypedVarioVariable<T> typedVariable) continue;
                value = typedVariable.Value;
                return true;
            }

            value = default;
            return false;
        }
        
    }
}