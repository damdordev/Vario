#if DAMDOR_VARIO_UI_ELEMENTS

using System;
using UnityEditor;
using UnityEngine;

namespace Damdor.Vario.Editor
{
    [CustomPropertyDrawer(typeof(UIElementsPointer<>))]
    public class UIElementsPointerPropertyDrawer : PropertyDrawer
    {
        private const float ModeSize = 0.4f;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var modeProperty = property.FindPropertyRelative("mode");
            var elementProperty = property.FindPropertyRelative("element");
            var queryProperty = property.FindPropertyRelative("query");

            var mode = (UIElementsPointerMode)modeProperty.enumValueIndex;
            switch (mode)
            {
                case UIElementsPointerMode.Element:
                    return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing +
                           EditorGUI.GetPropertyHeight(elementProperty, true);
                case UIElementsPointerMode.Query:
                    return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing +
                           EditorGUI.GetPropertyHeight(queryProperty, true);
                default:
                    return EditorGUIUtility.singleLineHeight;
            }
        }
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var modeProperty = property.FindPropertyRelative("mode");
            var elementProperty = property.FindPropertyRelative("element");
            var queryProperty = property.FindPropertyRelative("query");
            
            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.BeginChangeCheck();

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label, EditorStyles.label);
            var modeLength = ModeSize * position.width;
            
            EditorGUI.PropertyField(
                new Rect(position.x, position.y, modeLength, EditorGUIUtility.singleLineHeight),
                modeProperty,
                GUIContent.none
            );

            var mode = (UIElementsPointerMode) modeProperty.enumValueIndex;
            switch (mode)
            {
                case UIElementsPointerMode.Element:
                        EditorGUI.PropertyField(
                            new Rect(
                                position.x + modeLength,
                                position.y,
                                position.width - modeLength,
                                EditorGUI.GetPropertyHeight(elementProperty, true)
                            ),
                            elementProperty,
                            GUIContent.none,
                            true
                        );
                        break;
                case UIElementsPointerMode.Query:
                    EditorGUI.PropertyField(
                        new Rect(
                            position.x + modeLength,
                            position.y,
                            position.width - modeLength,
                            EditorGUI.GetPropertyHeight(queryProperty, true)
                        ),
                        queryProperty,
                        GUIContent.none,
                        true
                    );
                    break;
                default:
                    EditorGUI.HelpBox(
                        new Rect(position.x + modeLength, position.y, position.width - modeLength, EditorGUIUtility.singleLineHeight),
                        $"Wrong mode: {mode}", 
                        MessageType.Error);
                    break;
            }

            if (EditorGUI.EndChangeCheck())
            {
                property.serializedObject.ApplyModifiedProperties();
            }
            
        }
        
    }
}

#endif
