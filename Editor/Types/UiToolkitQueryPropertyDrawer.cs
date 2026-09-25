using UnityEditor;
using UnityEngine;

namespace Damdor.Vario.Editor
{
    [CustomPropertyDrawer(typeof(UiElementsQuery))]
    public class UiToolkitQueryPropertyDrawer : PropertyDrawer
    {
        private const float LabelWidth = 50f;
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return label != GUIContent.none
                ? 3f * EditorGUIUtility.singleLineHeight + 2f * EditorGUIUtility.standardVerticalSpacing
                : 2f * EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        }
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var nameProperty = property.FindPropertyRelative("name");
            var classNameProperty = property.FindPropertyRelative("className");

            var emptyLabel = string.IsNullOrEmpty(label.text);
            EditorGUI.BeginProperty(position, label, property);
            var yDelta = !emptyLabel
                ? EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing
                : 0f;

            if(!emptyLabel)
            {
                EditorGUI.LabelField(
                    new Rect(position.x, position.y, LabelWidth, EditorGUIUtility.singleLineHeight),
                    label
                );
                EditorGUI.indentLevel++;
            }
            
            EditorGUI.LabelField(
                new Rect(position.x, position.y + yDelta, LabelWidth, EditorGUIUtility.singleLineHeight),
                "Name"
            );

            EditorGUI.PropertyField(
                new Rect(
                    position.x + LabelWidth,
                    position.y + yDelta,
                    position.width - LabelWidth,
                    EditorGUIUtility.singleLineHeight
                ),
                nameProperty,
                GUIContent.none
            );

            EditorGUI.LabelField(
                new Rect(
                    position.x,
                    position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing + yDelta,
                    LabelWidth,
                    EditorGUIUtility.singleLineHeight
                ),
                "Class"
            );
            
            EditorGUI.PropertyField(
                new Rect(
                    position.x + LabelWidth,
                    position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing + yDelta,
                    position.width - LabelWidth,
                    EditorGUIUtility.singleLineHeight
                ),
                classNameProperty,
                GUIContent.none
            );
            
            if(!emptyLabel) EditorGUI.indentLevel--;
            
            EditorGUI.EndProperty();
        }
    }
}