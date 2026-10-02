using UnityEditor;
using UnityEngine;

namespace Damdor.Vario.Editor
{
    [CustomPropertyDrawer(typeof(Vector4AxisFilter))]
    public class Vector4AxisFilterPropertyDrawer : PropertyDrawer
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
            var ignoreZProperty = property.FindPropertyRelative("ignoreZ");
            var ignoreWProperty = property.FindPropertyRelative("ignoreW");

            label = EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, label);
            
            var axisWidth = AxisSize;
            if(AxisSize > position.width / 4) axisWidth = position.width / 4;

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
            
            ShowAxis(
                new Rect(position.x + 2f * axisWidth, position.y, axisWidth, position.height),
                ignoreZProperty,
                "z"
            );
            
            ShowAxis(
                new Rect(position.x + 3f * axisWidth, position.y, axisWidth, position.height),
                ignoreWProperty,
                "w"
            );

            EditorGUI.EndProperty();
        }

        private static void ShowAxis(Rect rect, SerializedProperty property, string label)
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