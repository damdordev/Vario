using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Damdor.VariableStorage.Editor
{
    [CustomPropertyDrawer(typeof(VariableStorage))]
    public class VariableStoragePropertyDrawer : PropertyDrawer
    {
        private readonly Dictionary<string, ReorderableList> propertyPathToReorderableList = new();
        
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
                EditorGUI.LabelField(rect, property.displayName);
            };

            reorderableList.drawElementCallback += (rect, index, _, _) =>
            {
                EditorGUI.PropertyField(
                    rect,
                    property.GetArrayElementAtIndex(index),
                    GUIContent.none
                );
            };

            reorderableList.onAddDropdownCallback += (_, _) =>
            {
                var menu = new GenericMenu();
                foreach (var type in VariableStorage.SupportedTypes)
                {
                    menu.AddItem(new GUIContent(type.Name), false, () =>
                    {
                        property.InsertArrayElementAtIndex(property.arraySize);
                        var element = property.GetArrayElementAtIndex(property.arraySize - 1);
                        element.managedReferenceValue = Activator.CreateInstance(type);
                        property.serializedObject.ApplyModifiedProperties();
                    });
                    menu.ShowAsContext();
                }
            };
            
            propertyPathToReorderableList.Add(path, reorderableList);
            return reorderableList;
        }
        
    }
}