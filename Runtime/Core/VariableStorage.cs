using System;
using System.Collections.Generic;
using UnityEngine;

namespace Damdor.VariableStorage
{
    [Serializable]
    public class VariableStorage
    {
        public static List<Type> SupportedTypes => supportedTypes;

        public List<Variable> Variables => variables;
        [SerializeReference] private List<Variable> variables = new();

        private static List<Type> supportedTypes = new()
        {
            typeof(BoolVariable),
            typeof(IntVariable),
            typeof(FloatVariable),
            typeof(StringVariable),
            typeof(ColorVariable),
            typeof(Vector2Variable),
            typeof(Vector3Variable),
            typeof(GameObjectVariable),
            typeof(RectVariable),
            typeof(AnimationCurveVariable),
            typeof(AudioClipVariable),
            typeof(MaterialVariable),
            typeof(ShaderVariable),
            typeof(MeshVariable),
            typeof(TextureVariable),
            typeof(SpriteVariable),
            typeof(TextAssetVariable),
            typeof(LayerMaskVariable),

#if DAMDOR_FOUNDATION
            typeof(DateTimeVariable),
            typeof(TimeSpanVariable),
#endif

        };

    }
}