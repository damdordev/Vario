using System;
using UnityEngine;

namespace Damdor.Vario
{
    [Serializable]
    [VarioVariable("AnimationCurve")]
    public class AnimationCurveVarioVariable : VarioVariable<AnimationCurve>
    {
        internal override VarioVariable Clone()
        {
            var variable = VarioPooling.PopVariable<AnimationCurve>();
            variable.Name = Name;
            variable.Value = new AnimationCurve(Value.keys);
            return variable;
        }
    }
}