using UnityEditor;
using UnityEngine;

namespace Damdor.Vario.Editor
{
    [CustomPropertyDrawer(typeof(ColorChannelFilter))]
    public class ColorChannelFilterPropertyDrawer : PropertyDrawer
    {
        private const float LabelSize = 25f;
        private const float AxisSize = 100f;
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var ignoreRProperty = property.FindPropertyRelative("ignoreR");
            var ignoreGProperty = property.FindPropertyRelative("ignoreG");
            var ignoreBProperty = property.FindPropertyRelative("ignoreB");
            var ignoreAProperty = property.FindPropertyRelative("ignoreA");

            label = EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, label);
            
            var axisWidth = AxisSize;
            if(AxisSize > position.width / 4) axisWidth = position.width / 4;

            ShowAxis(
                new Rect(position.x, position.y, axisWidth, position.height),
                ignoreRProperty,
                "r"
            );
            
            ShowAxis(
                new Rect(position.x + axisWidth, position.y, axisWidth, position.height),
                ignoreGProperty,
                "g"
            );
            
            ShowAxis(
                new Rect(position.x + 2f * axisWidth, position.y, axisWidth, position.height),
                ignoreBProperty,
                "b"
            );
            
            ShowAxis(
                new Rect(position.x + 3f * axisWidth, position.y, axisWidth, position.height),
                ignoreAProperty,
                "a"
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