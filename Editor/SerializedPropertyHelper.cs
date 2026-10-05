using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;

namespace Damdor.Vario.Editor
{
    public static class SerializedPropertyHelper
    {
        public static Type GetPropertyType(SerializedProperty property)
        {
            if (property == null || property.serializedObject == null)
                return null;

            var targetObject = property.serializedObject.targetObject;
            if (targetObject == null)
                return null;

            var currentType = targetObject.GetType();
            object currentObject = targetObject;

            var path = property.propertyPath.Replace(".Array.data[", "[");
            var elements = path.Split('.');

            foreach (var element in elements)
            {
                if (element.Contains("["))
                {
                    var bracketStart = element.IndexOf('[', StringComparison.Ordinal);
                    var bracketEnd = element.IndexOf(']', StringComparison.Ordinal);
                    var fieldName = element[..bracketStart];

                    if (!int.TryParse(element[(bracketStart + 1)..bracketEnd], out var index))
                        return null;

                    var field = GetFieldRecursive(currentType, fieldName);
                    if (field == null)
                        return null;

                    if (currentObject != null)
                    {
                        var containerValue = field.GetValue(currentObject);
                        if (containerValue is Array array && index >= 0 && index < array.Length)
                        {
                            currentObject = array.GetValue(index);
                        }
                        else if (containerValue is IList list && index >= 0 && index < list.Count)
                        {
                            currentObject = list[index];
                        }
                        else
                        {
                            currentObject = null;
                        }
                    }

                    if (currentObject != null)
                    {
                        currentType = currentObject.GetType();
                    }
                    else
                    {
                        currentType = GetCollectionElementType(field.FieldType);
                    }
                }
                else
                {
                    var field = GetFieldRecursive(currentType, element);
                    if (field == null)
                        return null;

                    currentObject = currentObject != null ? field.GetValue(currentObject) : null;
                    currentType = currentObject != null ? currentObject.GetType() : field.FieldType;
                }

                if (currentType == null)
                    return null;
            }

            return currentType;
        }

        private static Type GetCollectionElementType(Type type)
        {
            if (type == null)
                return null;

            if (type.IsArray)
                return type.GetElementType();

            var current = type;
            while (current != null && current != typeof(object))
            {
                if (current.IsGenericType)
                {
                    var genericArgs = current.GetGenericArguments();
                    if (genericArgs.Length > 0)
                        return genericArgs[0];
                }
                current = current.BaseType;
            }

            foreach (var iface in type.GetInterfaces())
            {
                if (iface.IsGenericType && iface.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    return iface.GetGenericArguments()[0];
                }
            }

            return typeof(object);
        }

        private static FieldInfo GetFieldRecursive(Type type, string fieldName)
        {
            const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;
            while (type != null && type != typeof(object))
            {
                var field = type.GetField(fieldName, flags);
                if (field != null)
                    return field;
                type = type.BaseType;
            }
            return null;
        }
    }
}