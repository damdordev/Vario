using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Damdor.VariableStorage
{
    [Serializable]
    public class VariableStorage
    {
        public static IEnumerable<Type> SupportedTypes => supportedTypes.Values;

        [SerializeReference] private List<Variable> variables = new()
        {
            new IntVariable { Name = "intv", Value = 5 },
            new FloatVariable { Name = "floatv", Value = 10.1f },
        };
        
        public bool Contains<T>(string name)
            => variables.Any(v => v.Name == name && v.Type == typeof(T));

        public T Get<T>(string name)
        {
            foreach (var variable in variables)
            {
                if(variable.Name == name && variable is TypedVariable<T> variableT) return  variableT.Value;
            }

            return default;
        }

        public void Set<T>(string name, T value)
        {
            foreach (var variable in variables)
            {
                if (variable.Name == name && variable is TypedVariable<T> variableT)
                {
                    variableT.Value = value;
                    return;
                }
            }

            if (!supportedTypes.TryGetValue(typeof(T), out var variableType)) return;
            
            var newVariable = (TypedVariable<T>) Activator.CreateInstance(variableType);
            newVariable.Name = name;
            newVariable.Value = value;
            variables.Add(newVariable);
        }

        public void Remove<T>(string name)
        {
            variables.RemoveAll(v => v.Name == name && v.Type == typeof(T));
        }

        public bool Rename<T>(string oldName, string newName)
        {
            if (Contains<T>(newName)) return false;
            
            foreach (var variable in variables)
            {
                if (variable.Type != typeof(T) || variable.Name != oldName) continue;
                Remove<T>(newName);
                variable.Name = newName;
                return true;
            }

            return false;
        }

        public IEnumerable<VariableMetadata> AllVariables =>
            variables.Select(v => new VariableMetadata(v.Name, v.Type));

        private static Dictionary<Type, Type> supportedTypes = new()
        {
            { typeof(bool), typeof(BoolVariable) },
            { typeof(int), typeof(IntVariable) },
            { typeof(float), typeof(FloatVariable) },
            { typeof(string), typeof(StringVariable) },
            { typeof(Color), typeof(ColorVariable) },
            { typeof(Vector2), typeof(Vector2Variable) },
            { typeof(Vector3), typeof(Vector3Variable) },
            { typeof(GameObject), typeof(GameObjectVariable) },
            { typeof(Rect), typeof(RectVariable) },
            { typeof(AnimationCurve),  typeof(AnimationCurveVariable) },
            { typeof(AudioClip), typeof(AudioClipVariable) },
            { typeof(Material), typeof(MaterialVariable) },
            { typeof(Shader), typeof(ShaderVariable) },
            { typeof(Mesh), typeof(MeshVariable) },
            { typeof(Texture), typeof(TextureVariable) },
            { typeof(Sprite), typeof(SpriteVariable) },
            { typeof(TextAsset), typeof(TextAssetVariable) },
            { typeof(LayerMask), typeof(LayerMaskVariable) },
            
            #if DAMDOR_FOUNDATION
            { typeof(DateTime), typeof(DateTimeVariable) },
            { typeof(TimeSpan), typeof(TimeSpanVariable) },
            #endif
            
        };

        public static void RegisterVariableType<TVariable, T>() where TVariable : Variable<T>
        {
            supportedTypes[typeof(T)] = typeof(TVariable);
        }
        
    }
}