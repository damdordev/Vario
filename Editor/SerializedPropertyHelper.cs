using System;
using System.Collections;
using System.Reflection;
using UnityEditor;

namespace Damdor.Vario.Editor
{
    internal static class SerializedPropertyHelper
    {
         public static Type GetPropertyType(SerializedProperty property)
        {
            var parentType = property.serializedObject.targetObject.GetType();
    
            var path = property.propertyPath.Replace(".Array.data[", "[");
            var elements = path.Split('.');
    
            var currentType = parentType;
            object currentObject = property.serializedObject.targetObject;
    
            foreach (var element in elements)
            {
                if (element.Contains("["))
                {
                    var startIndex = element.IndexOf("[", StringComparison.Ordinal);
                    var endIndex = element.IndexOf("]", StringComparison.Ordinal);
                    var name = element[..startIndex];
                    var index = int.Parse(element[(startIndex+1)..endIndex]);
                    var field = GetFieldRecursive(currentType, name);
                    if (field == null) return null;

                    if (currentObject != null)
                    {
                        if (field.FieldType.IsArray)
                        {
                            var array = (Array)field.GetValue(currentObject);
                            if(array == null || array.Length <= index || array.GetValue(index) == null) currentObject = null;
                            else
                            {
                                currentObject = array.GetValue(index);
                                currentType = currentObject.GetType();
                            }
                        }
                        else
                        {
                            var list = (IList)field.GetValue(currentObject);
                            if(list == null || list.Count <= index || list[index] == null) currentObject = null;
                            else
                            {
                                currentObject = list[index];
                                currentType = currentObject.GetType();
                            }
                        }
                    }

                    if (currentObject != null) continue;
                    if (currentObject != null)
                    {
                        currentObject = field.GetValue(currentObject);
                        if (currentObject != null) currentType = currentObject.GetType();
                    }

                    if (currentObject == null)
                    {
                        currentType = field.FieldType.IsArray
                            ? field.FieldType.GetElementType()
                            : field.FieldType.GetGenericArguments()[0];
                    }
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