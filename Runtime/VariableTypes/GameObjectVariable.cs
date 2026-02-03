using System;
using UnityEngine;

namespace Damdor.VariableStorage
{
    [Serializable]
    [VariableTypeName("gameObject")]
    public class GameObjectVariable : Variable<GameObject> { }
}