#if DAMDOR_VARIO_UI_ELEMENTS

using System;
using Damdor.Foundation;
using UnityEngine;
using UnityEngine.UIElements;

namespace Damdor.Vario
{
    /// <summary>
    /// Defines how to get a <see cref="VisualElement"/>
    /// </summary>
    public enum UIElementsPointerMode
    {
        /// <summary>
        /// A direct reference to an element
        /// </summary>
        Element,
        /// <summary>
        /// A query that will be executed on a root element
        /// </summary>
        Query
    }
    
    /// <summary>
    /// A pointer to a <see cref="VisualElement"/> of type <c>T</c>.
    /// It can be either a direct reference to an element or a query that will be executed on a root element.
    /// </summary>
    /// <typeparam name="T">Type of the <see cref="VisualElement"/></typeparam>
    [Serializable]
    public struct UIElementsPointer<T> where T : VisualElement
    {
        /// <summary>
        /// Defines how to get a <see cref="VisualElement"/>
        /// </summary>
        public UIElementsPointerMode Mode
        {
            get => mode;
            set => mode = value;
        }

        /// <summary>
        /// A direct reference to an element.
        /// Used when <see cref="Mode"/> is <see cref="UIElementsPointerMode.Element"/>
        /// </summary>
        public VarioValue<T> Element
        {
            get => element;
            set => element = value;
        }

        /// <summary>
        /// A query that will be executed on a root element.
        /// Used when <see cref="Mode"/> is <see cref="UIElementsPointerMode.Query"/>
        /// </summary>
        public VarioValue<UiElementsQuery> Query
        {
            get => query;
            set => query = value;
        }
        
        [SerializeField] private UIElementsPointerMode mode;
        [SerializeField] private VarioValue<T> element;
        [SerializeField] private VarioValue<UiElementsQuery> query;
        
        /// <summary>
        /// Evaluates the pointer and returns the <see cref="VisualElement"/>
        /// </summary>
        /// <param name="storage">A <see cref="VarioStorage"/> that contains all the required variables</param>
        /// <returns>An element of type <c>T</c></returns>
        /// <exception cref="ArgumentOutOfRangeException">when <see cref="Mode"/> is not supported</exception>
        public T Evaluate(VarioStorage storage)
        {
            switch (mode)
            {
                case UIElementsPointerMode.Element:
                    return element.Evaluate(storage);
                case UIElementsPointerMode.Query:
                    var root = VarioInternalHelper.GetRoot(storage);
                    var q = query.Evaluate(storage);
                    return root.Q<T>(q.Name, q.ClassName);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
    }
}

#endif
