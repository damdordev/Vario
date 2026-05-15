using UnityEditor;
using UnityEngine;

namespace Damdor.Vario.Editor
{
    [CustomPropertyDrawer(typeof(VarioVariable), true)]
    public class VarioVariablePropertyDrawer : PropertyDrawer
    {
        private const float NameSize = 0.4f;
        private const float Spacing = 20f;
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var valueProperty = property.FindPropertyRelative("value");
            var valuePropertyHeight = valueProperty != null
                ? EditorGUI.GetPropertyHeight(valueProperty, true)
                : EditorGUIUtility.singleLineHeight;
            return Mathf.Max(EditorGUIUtility.singleLineHeight, valuePropertyHeight);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var nameLength = NameSize * position.width;
            var nameProperty = property.FindPropertyRelative("name");
            var valueProperty = property.FindPropertyRelative("value");
            
            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.BeginChangeCheck();
            
            EditorGUI.PropertyField(
                new Rect(position.x, position.y, nameLength, EditorGUIUtility.singleLineHeight),
                nameProperty,
                GUIContent.none
            );

            if (valueProperty != null)
            {
                EditorGUI.PropertyField(
                    new Rect(
                        position.x + nameLength + Spacing,
                        position.y,
                        position.width - nameLength - Spacing,
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
                        position.x + nameLength + Spacing,
                        position.y,
                        position.width - nameLength - Spacing,
                        EditorGUIUtility.singleLineHeight
                    ),
                    "Not serializable"
                );
            }

            if (EditorGUI.EndChangeCheck())
            {
                property.serializedObject.ApplyModifiedProperties();
            }
            EditorGUI.EndProperty();
        }
    }
}