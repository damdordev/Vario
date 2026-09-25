#if DAMDOR_VARIO_UI_ELEMENTS

using UnityEngine.UIElements;

namespace Damdor.Vario
{
    public static class VisualElementExtension
    {
        public static T Q<T>(this VisualElement visualElement, UiElementsQuery query) where T : VisualElement
        {
            var name = !string.IsNullOrWhiteSpace(query.Name) ? query.Name : null;
            var className = !string.IsNullOrWhiteSpace(query.ClassName) ? query.ClassName : null;

            return name != null || className != null ? visualElement.Q<T>(name, className) : (T)visualElement;
        }
    }
}

#endif