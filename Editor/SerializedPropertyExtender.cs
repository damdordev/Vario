using System;
using System.Reflection;
using UnityEditor;

namespace Damdor.VariableStorage.Editor
{
    public static class SerializedPropertyExtender
    {
        public static Type GetPropertyType(this SerializedProperty property)
        {
            var parentType = property.serializedObject.targetObject.GetType();
    
            var path = property.propertyPath.Replace(".Array.data[", "[");
            var elements = path.Split('.');
    
            var currentType = parentType;
    
            foreach (var element in elements)
            {
                if (element.Contains("["))
                {
                    var name = element[..element.IndexOf("[", StringComparison.Ordinal)];
                    var field = GetFieldRecursive(currentType, name);
                    if (field == null) return null;
            
                    currentType = field.FieldType.IsArray 
                        ? field.FieldType.GetElementType() 
                        : field.FieldType.GetGenericArguments()[0];
                }
                else
                {
                    var field = GetFieldRecursive(currentType, element);
                    if (field == null) return null;
                    currentType = field.FieldType;
                }
            }
    
            return currentType;
        }

        private static FieldInfo GetFieldRecursive(Type type, string fieldName)
        {
            while (type != null)
            {
                var field = type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                if (field != null) return field;
                type = type.BaseType;
            }
            return null;
        }
        
    }
}