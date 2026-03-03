using UnityEngine;

namespace Damdor.Vario
{
    [CreateAssetMenu(fileName = "global", menuName = "Damdor/Vario/Global storage")]
    public class VarioGlobalStorage : ScriptableObject
    {
        public VarioStorage Storage => storage;
        
        [SerializeField] private VarioStorage storage;
    }
}