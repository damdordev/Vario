using UnityEditor;
using UnityEngine;

namespace Damdor.Vario.Editor
{
    [CustomPropertyDrawer(typeof(Vector2AxisFilter))]
    public class Vector2AxisFilterPropertyDrawer : PropertyDrawer
    {
        private const float LabelSize = 25f;
        private const float AxisSize = 100f;
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var ignoreXProperty = property.FindPropertyRelative("ignoreX");
            var ignoreYProperty = property.FindPropertyRelative("ignoreY");

            label = EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, label);
            
            var axisWidth = AxisSize;
            if(AxisSize > position.width / 2) axisWidth = position.width / 2;

            ShowAxis(
                new Rect(position.x, position.y, axisWidth, position.height),
                ignoreXProperty,
                "x"
            );
            
            ShowAxis(
                new Rect(position.x + axisWidth, position.y, axisWidth, position.height),
                ignoreYProperty,
                "y"
            );

            EditorGUI.EndProperty();
        }

        private void ShowAxis(Rect rect, SerializedProperty property, string label)
        {
            EditorGUI.LabelField(
                new Rect(rect.x, rect.y, LabelSize, rect.height),
                label
            );
            EditorHelper.ShowRevertedBool(
                new Rect(rect.x + LabelSize, rect.y, rect.width - LabelSize, rect.height),
                property
            );
        }
    }
}