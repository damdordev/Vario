using System.Collections.Generic;
using Damdor.Foundation;
using UnityEditor;
using UnityEngine;

namespace Damdor.Vario
{
    internal class VarioSettingsLoader : AssetPostprocessor
    {

        public static void Load()
        {
            var settingsAssets = Resources.LoadAll<TextAsset>("vario_settings");
            foreach (var t in settingsAssets)
            {
                var json = t.text;
                var settingsData = (Dictionary<string, object>)Json.Deserialize(json);
                Resources.UnloadAsset(t);

                if (settingsData != null && settingsData.TryGetValue("types", out var types))
                {
                    ProcessTypesFromSettingsData((Dictionary<string, object>)types);
                }

                if (settingsData != null && settingsData.TryGetValue("globalStorages", out var storages))
                {
                    ProcessGlobalStoragesFromSettingsData((List<object>)storages);
                }
            }
        }

        private static void ProcessTypesFromSettingsData(Dictionary<string, object> types)
        {
            foreach (var pair in types)
            {
                var type = ReflectionHelper.FindType((string)pair.Value);
                if (type != null) VarioSettings.RegisterVariableType(type, pair.Key);
            }
        }

        private static void ProcessGlobalStoragesFromSettingsData(List<object> storages)
        {
            foreach (string storageName in storages)
            {
                var storage = Resources.Load<VarioGlobalStorage>(storageName);
                if (storage != null) VarioSettings.RegisterGlobalStorage(storage.Storage);
            }
        }
        
        void OnPreprocessAsset()
        {
            if (assetImporter.assetPath.EndsWith("vario_settings.json"))
            {
                VarioSettings.Reset();
            }
        }

    }
}