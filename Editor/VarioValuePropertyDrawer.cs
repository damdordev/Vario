using System;
using System.Linq;
using Damdor.Foundation.Editor;
using UnityEditor;
using UnityEngine;

namespace Damdor.Vario.Editor
{
    [CustomPropertyDrawer(typeof(VarioValue<>))]
    [CustomPropertyDrawer(typeof(VarioValue<,>))]
    public class VarioValuePropertyDrawer : PropertyDrawer
    {
        private const float SourceSize = 0.4f;
        private const float Spacing = 20f;
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var sourceProperty = property.FindPropertyRelative("source");
            var valueProperty = property.FindPropertyRelative("value");
            
            var source = (ValueSource) sourceProperty.enumValueIndex;
            return source switch
            {
                ValueSource.Raw => Mathf.Max(EditorGUIUtility.singleLineHeight, EditorGUI.GetPropertyHeight(valueProperty, true)),
                ValueSource.Storage => EditorGUIUtility.singleLineHeight,
                _ => throw new ArgumentOutOfRangeException($"Wrong value source: {source}")
            };
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var sourceProperty = property.FindPropertyRelative("source");
            var nameProperty = property.FindPropertyRelative("name");
            var valueProperty = property.FindPropertyRelative("value");
            
            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.BeginChangeCheck();

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label, EditorStyles.label);
            var sourceLength = SourceSize * position.width;
            
            EditorGUI.PropertyField(
                new Rect(position.x, position.y, sourceLength, EditorGUIUtility.singleLineHeight),
                sourceProperty,
                GUIContent.none
            );

            var source = (ValueSource) sourceProperty.enumValueIndex;
            switch (source)
            {
                case ValueSource.Raw:
                    EditorGUI.PropertyField(
                        new Rect(position.x + sourceLength + Spacing, position.y, position.width - sourceLength - Spacing, position.height),
                        valueProperty,
                        GUIContent.none,
                        true
                    );
                    break;
                case ValueSource.Storage:
                    var variables = GetVariables(property);
                    if (variables == null)
                    {
                        EditorGUI.PropertyField(
                            new Rect(
                                position.x + sourceLength + Spacing,
                                position.y,
                                position.width - sourceLength - Spacing,
                                position.height
                            ),
                            nameProperty,
                            GUIContent.none,
                            true
                        );
                    }
                    else
                    {
                        var rect = new Rect(
                            position.x + sourceLength + Spacing,
                            position.y,
                            position.width - sourceLength - Spacing,
                            position.height
                        );
                        if (GUI.Button(rect, nameProperty.stringValue, EditorStyles.popup))
                        {
                            new HierarchicalDropdown<string>(variables, v => v, newChoice =>
                            {
                                nameProperty.stringValue = newChoice;
                                property.serializedObject.ApplyModifiedProperties();
                            }).Show(rect);
                        }
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Wrong value source: {source}");
            }

            if (EditorGUI.EndChangeCheck())
            {
                property.serializedObject.ApplyModifiedProperties();
            }
            
        }

        private static string[] GetVariables(SerializedProperty property)
        {
            var storageSource = property.serializedObject.targetObject as IVarioStorageSource;
            var storage = storageSource?.Storage;
            if (storage == null) return null;
            var type = SerializedPropertyHelper.GetPropertyType(property).GetGenericArguments()[0];
            var variables = storage.Variables.Where(t => t.Type == type).Select(v => v.Name).ToList();

            foreach (var globaStorage in VarioSettings.GlobalStorages)
            {
                variables.AddRange(globaStorage.Variables.Where(t => t.Type == type).Select(v => v.Name));
            }
            
            return variables.Distinct().OrderBy(v => v).ToArray();
        }

    }
}