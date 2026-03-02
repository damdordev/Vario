using UnityEngine;

namespace Damdor.VariableStorage
{
    [CreateAssetMenu(fileName = "global", menuName = "Damdor/Variable storage/Global variable storage")]
    public class GlobalVariableStorage : ScriptableObject
    {
        public VariableStorage Storage => storage;
        
        [SerializeField] private VariableStorage storage;
    }
}