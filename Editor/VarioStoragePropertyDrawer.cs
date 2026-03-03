using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Damdor.Foundation.Editor;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Damdor.Vario.Editor
{
    [CustomPropertyDrawer(typeof(VarioStorage))]
    public class VarioStoragePropertyDrawer : PropertyDrawer
    {
        private const float VariableTypeLength = 80f;
        private const float Spacing = 10f;

        private readonly Dictionary<string, ReorderableList> propertyPathToReorderableList = new();
        private readonly Dictionary<Type, string> variableTypeToName = new();
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var variablesProperty = property.FindPropertyRelative("variables");
            var list = GetReorderableList(variablesProperty);
            return list.GetHeight() +  EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var variablesProperty = property.FindPropertyRelative("variables");
            var list = GetReorderableList(variablesProperty);
            
            EditorGUI.BeginProperty(position, label, property);
            list.DoList(position);
            EditorGUI.EndProperty();
        }

        private ReorderableList GetReorderableList(SerializedProperty property)
        {
            var path = property.propertyPath;
            if(propertyPathToReorderableList.TryGetValue(path, out var reorderableList)) return reorderableList;

            reorderableList = new ReorderableList(
                property.serializedObject,
                property,
                true,
                true,
                true,
                true
            );
            
            reorderableList.drawHeaderCallback += rect =>
            {
                EditorGUI.LabelField(
                    rect,
                    property.displayName
                );
            };

            reorderableList.drawElementCallback += (rect, index, _, _) =>
            {
                var variable = (VarioVariable) property.GetArrayElementAtIndex(index).managedReferenceValue;

                var wasEnabled = GUI.enabled;
                GUI.enabled = false;
                EditorGUI.TextField(
                    new Rect(rect.x, rect.y, VariableTypeLength, EditorGUIUtility.singleLineHeight),
                    GetTypeName(variable.GetType())
                );
                GUI.enabled = wasEnabled;
                
                EditorGUI.PropertyField(
                    new Rect(rect.x + VariableTypeLength + Spacing, rect.y, rect.width - VariableTypeLength - Spacing, rect.height),
                    property.GetArrayElementAtIndex(index),
                    GUIContent.none
                );
            };

            reorderableList.elementHeightCallback += index
                => Mathf.Max(EditorGUIUtility.singleLineHeight, EditorGUI.GetPropertyHeight(property.GetArrayElementAtIndex(index), GUIContent.none));

            reorderableList.onAddDropdownCallback += (rect, _) =>
            {
                var types = VarioSettings.SupportedTypes.OrderBy(GetTypeName).ToList();
                new HierarchicalDropdown<Type>(types, GetTypeName, type =>
                {
                    property.InsertArrayElementAtIndex(property.arraySize);
                    var element = property.GetArrayElementAtIndex(property.arraySize - 1);
                    element.managedReferenceValue = Activator.CreateInstance(type);
                    property.serializedObject.ApplyModifiedProperties();
                }).Show(new Rect(rect.x - 100, rect.y, rect.width, rect.height));
            };
            
            propertyPathToReorderableList.Add(path, reorderableList);
            return reorderableList;
        }

        private string GetTypeName(Type type)
        {
            if(variableTypeToName.TryGetValue(type, out var result)) return result;

            var attr = type.GetCustomAttribute<VarioVariableName>();
            var name = attr != null ? attr.Name : type.Name;
            variableTypeToName[type] = name;

            return name;
        }
        
    }
}