using System;
using UnityEngine;

namespace Damdor.Vario
{
    [Serializable]
    [VarioVariable("AnimationCurve")]
    public class AnimationCurveVarioVariable : VarioVariable<AnimationCurve>
    {
        public override VarioVariable Clone()
        {
            var variable = VarioPooling.PopVariable<AnimationCurve>();
            variable.Name = Name;
            variable.Value = new AnimationCurve(Value.keys);
            return variable;
        }

        internal override void TryCopyFrom(VarioVariable variable)
        {
            if (variable is AnimationCurveVarioVariable typedVariable)
            {
                Value = new AnimationCurve(typedVariable.Value.keys);
            }
        }
    }
}