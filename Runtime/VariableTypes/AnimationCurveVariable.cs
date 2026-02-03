using System;
using UnityEngine;

namespace Damdor.VariableStorage
{
    [Serializable]
    [VariableTypeName("curve")]
    public class AnimationCurveVariable : Variable<AnimationCurve> { }
}