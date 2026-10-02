using System.Collections.Generic;
using UnityEngine;

namespace Damdor.Vario
{
    [CreateAssetMenu(fileName = "VarioGlobalStorages", menuName = "Damdor/Vario/Global storage library")]
    public class VarioGlobalStorageLibrary : ScriptableObject
    {
        public IReadOnlyList<VarioGlobalStorage> GlobalStorages => globalStorages;
        
        [SerializeField] private List<VarioGlobalStorage> globalStorages;

        private void Awake()
        {
            VarioSettings.ResetGlobalStoragesInit();
        }

        private void OnDestroy()
        {
            VarioSettings.ResetGlobalStoragesInit();
        }
        
        private void OnValidate()
        {
            VarioSettings.ResetGlobalStoragesInit();
        }
        
    }
}