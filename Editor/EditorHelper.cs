using UnityEditor;
using UnityEngine;

namespace Damdor.Vario.Editor
{
    public static class EditorHelper
    {
        // ReSharper disable once UnusedMember.Global
        public static void ShowRevertedBool(Rect rect, SerializedProperty property, GUIContent content)
        {
            EditorGUI.BeginChangeCheck();
            var value = EditorGUI.Toggle(rect, content, !property.boolValue);
            if (EditorGUI.EndChangeCheck())
            {
                property.boolValue = !value;
            }
        }
        
        public static void ShowRevertedBool(Rect rect, SerializedProperty property)
        {
            EditorGUI.BeginChangeCheck();
            var value = EditorGUI.Toggle(rect, !property.boolValue);
            if (EditorGUI.EndChangeCheck())
            {
                property.boolValue = !value;
            }
        }
    }
}