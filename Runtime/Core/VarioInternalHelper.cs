using System;
using System.Collections.Generic;

namespace Damdor.Vario
{
    internal class VarioInternalHelper
    {
        
        /// <summary>
        /// Evaluates the value of a variable by name, optionally searching a parent storage.
        /// </summary>
        /// <typeparam name="T">The expected type of the variable value.</typeparam>
        /// <param name="name">The unique identifier of the variable.</param>
        /// <param name="defaultValue">The value to return if the variable is not found in either storage.</param>
        /// <returns>The found value or the provided <paramref name="defaultValue"/>.</returns>
        public static T Evaluate<T>(VarioStorage mainStorage, string name, T defaultValue = default)
        {
            // ReSharper disable once DuplicatedSequentialIfBodies
            if (mainStorage != null)
            {
                if (TryEvaluate<T>(mainStorage.Variables, name, out var result)) return result;
            }

            foreach (var storage in VarioSettings.GlobalStorages)
            {
                if(storage != null && TryEvaluate<T>(storage.Variables, name, out var result)) return result;
            }
            return defaultValue;
        }
        
        /// <summary>
        /// Evaluates a <see cref="VarioValue{T}"/> to retrieve its final value based on its source.
        /// </summary>
        /// <typeparam name="T">The type of the value to evaluate.</typeparam>
        /// <param name="value">The storage value definition containing the source, name, or literal value.</param>
        /// <param name="defaultValue">The value to return if the variable lookup fails.</param>
        /// <returns>
        /// If <see cref="VarioValue{T}.Source"/> is <c>Storage</c>, returns the value from storage; 
        /// otherwise, returns the value <see cref="VarioValue{T}.Value"/>.
        /// </returns>
        public static T Evaluate<T>(VarioStorage mainStorage, VarioValue<T> value, T defaultValue = default) => value.Source switch
        {
            ValueSource.Raw => value.Value,
            ValueSource.Storage => Evaluate(mainStorage, value.Name, defaultValue),
            _ => throw new ArgumentOutOfRangeException($"Unknown value source: {typeof(T).Name} ")
        };

        /// <summary>
        /// Evaluates a <see cref="VarioValue{T,TSerializable}"/> to retrieve its final value based on its source.
        /// </summary>
        /// <typeparam name="T">The type of the value to evaluate.</typeparam>
        /// <typeparam name="TSerializable">The type of the value used to serialize a raw value.</typeparam>
        /// <param name="value">The storage value definition containing the source, name, or literal value.</param>
        /// <param name="defaultValue">The value to return if the variable lookup fails.</param>
        /// <returns>
        /// If <see cref="VarioValue{T,TSerializable}.Source"/> is <c>Storage</c>, returns the value from storage; 
        /// otherwise, returns the value <see cref="VarioValue{T,TSerializable}.Value"/>.
        /// </returns>
        public static T Evaluate<T, TSerializable>(VarioStorage storage, VarioValue<T, TSerializable> value, T defaultValue = default) => value.Source switch
        {
            ValueSource.Raw => value.Value,
            ValueSource.Storage => Evaluate(storage, value.Name, defaultValue),
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