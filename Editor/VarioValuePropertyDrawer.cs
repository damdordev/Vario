using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Damdor.Vario.Editor
{
    [CustomPropertyDrawer(typeof(VarioValue<>))]
    public class VarioValuePropertyDrawer : PropertyDrawer
    {
        private const float Spacing = 20f;
        private GUIContent settingsIcon;
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var sourceProperty = property.FindPropertyRelative("source");
            var valueProperty = property.FindPropertyRelative("value");
            var valueHeight = valueProperty != null
                ? EditorGUI.GetPropertyHeight(valueProperty, true)
                : EditorGUIUtility.singleLineHeight;
            
            var source = (ValueSource) sourceProperty.enumValueIndex;
            return source switch
            {
                ValueSource.Raw => Mathf.Max(EditorGUIUtility.singleLineHeight, valueHeight),
                ValueSource.Storage => EditorGUIUtility.singleLineHeight,
                _ => throw new ArgumentOutOfRangeException($"Wrong value source: {source}")
            };
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var sourceProperty = property.FindPropertyRelative("source");
            var nameProperty = property.FindPropertyRelative("name");
            var valueProperty = property.FindPropertyRelative("value");
            settingsIcon ??= EditorGUIUtility.IconContent("_Popup");
            
            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.BeginChangeCheck();

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label, EditorStyles.label);

            var buttonRect = new Rect(position.x, position.y, Spacing, EditorGUIUtility.singleLineHeight);
            if (GUI.Button(
                    buttonRect,
                    settingsIcon,
                    EditorStyles.iconButton)
               )
            {
                ShowContextMenu(buttonRect, sourceProperty);
            }

            var source = (ValueSource) sourceProperty.enumValueIndex;
            switch (source)
            {
                case ValueSource.Raw:
                    if (valueProperty != null)
                    {
                        EditorGUI.PropertyField(
                            new Rect(
                                position.x + Spacing,
                                position.y,
                                position.width - Spacing,
                                position.height
                            ),
                            valueProperty,
                            GUIContent.none,
                            true
                        );
                    }
                    else
                    {
                        EditorGUI.LabelField(
                            new Rect(
                                position.x + Spacing,
                                position.y,
                                position.width - Spacing,
                                position.height
                            ),
                            "Not serializable"
                        );
                    }

                    break;
                case ValueSource.Storage:
                    var variables = GetVariables(property);
                    if (variables == null)
                    {
                        EditorGUI.PropertyField(
                            new Rect(
                                position.x + Spacing,
                                position.y,
                                position.width - Spacing,
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
                            position.x + Spacing,
                            position.y,
                            position.width - Spacing,
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
            var type = SerializedPropertyHelper.GetPropertyType(property).GetGenericArguments()[0];
            var variables = new List<string>();
            if (storage != null)
            {
                variables.AddRange(storage.Variables.Where(t => t.Type == type).Select(v => v.Name));
            }

            foreach (var globaStorage in VarioSettings.GlobalStorages)
            {
                variables.AddRange(globaStorage.Variables.Where(t => t.Type == type).Select(v => v.Name));
            }
            
            return variables.Distinct().OrderBy(v => v).ToArray();
        }
        
        private static void ShowContextMenu(Rect buttonRect, SerializedProperty property)
        {
            var menu = new GenericMenu();
            var currentType = (ValueSource) property.enumValueIndex;

            menu.AddItem(new GUIContent("Raw"), currentType == ValueSource.Raw, _ =>
            {
                property.enumValueIndex = (int)ValueSource.Raw;
                property.serializedObject.ApplyModifiedProperties();
            }, "Raw");
            menu.AddItem(new GUIContent("Storage"), currentType == ValueSource.Storage, _ =>
            {
                property.enumValueIndex = (int)ValueSource.Storage;
                property.serializedObject.ApplyModifiedProperties();
            }, "Storage");
            
            menu.DropDown(buttonRect);
        }

    }
}