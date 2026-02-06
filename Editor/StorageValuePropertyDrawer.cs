using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Damdor.VariableStorage.Editor
{
    [CustomPropertyDrawer(typeof(StorageValue<>))]
    [CustomPropertyDrawer(typeof(StorageValue<,>))]
    public class StorageValuePropertyDrawer : PropertyDrawer
    {
        private const float SourceSize = 0.4f;
        private const float Spacing = 20f;
        
        private SerializedProperty sourceProperty;
        private SerializedProperty nameProperty;
        private SerializedProperty valueProperty;
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            RetrieveProperties(property);
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
            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.BeginChangeCheck();

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label, EditorStyles.label);
            RetrieveProperties(property);
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
                            new StringDropdown(variables, newChoice =>
                            {
                                nameProperty.stringValue = newChoice;
                                property.serializedObject.ApplyModifiedProperties();
                            }).Show(rect);
                        }
                    }
                    break;
            }

            if (EditorGUI.EndChangeCheck())
            {
                property.serializedObject.ApplyModifiedProperties();
            }
            
        }

        private void RetrieveProperties(SerializedProperty property)
        {
            sourceProperty = property.FindPropertyRelative("source");
            nameProperty = property.FindPropertyRelative("name");
            valueProperty = property.FindPropertyRelative("value");
        }

        private string[] GetVariables(SerializedProperty property)
        {
            var storageSource = property.serializedObject.targetObject as IVariableStorageSource;
            if (storageSource == null) return null;
            var storage =  storageSource.Storage;
            if (storage == null) return null;
            var type = property.GetPropertyType().GetGenericArguments()[0];
            return storage.Variables.Where(t => t.Type == type).Select(v => v.Name).ToArray();
        }

    }
}