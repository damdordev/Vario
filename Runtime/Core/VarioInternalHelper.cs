using System;
using System.Collections.Generic;
#if DAMDOR_VARIO_UI_ELEMENTS
using UnityEngine.UIElements;
#endif

namespace Damdor.Vario
{
    internal static class VarioInternalHelper
    {
        
        public static T Evaluate<T>(VarioStorage mainStorage, VarioValue<T> value, bool useGlobalStorages, T defaultValue) => value.Source switch
        {
            ValueSource.Raw => value.Value,
            ValueSource.Storage => Evaluate(mainStorage, value.Name, useGlobalStorages, defaultValue),
            _ => throw new ArgumentOutOfRangeException($"Unknown value source: {value.Source} for type {typeof(T).Name} ")
        };
        
        private static T Evaluate<T>(VarioStorage mainStorage, string name, bool useGlobalStorages, T defaultValue)
        {
            if (mainStorage != null)
            {
                if (TryEvaluate<T>(mainStorage.Variables, name, out var result)) return result;
            }

            if (!useGlobalStorages) return defaultValue;

            foreach (var storage in VarioSettings.GlobalStorages)
            {
                if (storage != null && TryEvaluate<T>(storage.Variables, name, out var result)) return result;
            }

            return defaultValue;
        }
        
        private static bool TryEvaluate<T>(IReadOnlyList<VarioVariable> variables, string name, out T value)
        {
            foreach (var variable in variables)
            {
                if (variable == null || variable.Name != name || variable.Type != typeof(T) || variable is not VarioVariable<T> typedVariable) continue;
                value = typedVariable.Value;
                return true;
            }

            value = default;
            return false;
        }
        
#if DAMDOR_VARIO_UI_ELEMENTS
        public const string UiDocumentVariableName = "ui-document";
        public const string UiRootQueryVariableName = "ui-root-query";
        public const string UiRootVariableName = "ui-root";   
        
        public static VisualElement GetRoot(VarioStorage storage)
        {
            if (storage == null) throw new ArgumentException("Storage cannot be null");
            var document = storage.Get<UIDocument>(UiDocumentVariableName);
            if (!document) throw new ArgumentException($"{UiDocumentVariableName} not found in storage");
            var root = document.rootVisualElement;

            if (!storage.Contains<UiElementsQuery>(UiRootVariableName)) return root;
            var query = storage.Get<UiElementsQuery>(UiRootVariableName);
            root = root.Q<VisualElement>(query);

            return root;
        }
        
#endif
        
    }
}