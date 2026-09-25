using UnityEngine.UIElements;

namespace Damdor.Vario
{
    /// <summary>
    /// A helper class for Vario
    /// </summary>
    public static class VarioHelper
    {
        /// <summary>
        /// Puts a root <see cref="UIDocument"/> to the <see cref="VarioStorage"/>
        /// </summary>
        /// <param name="storage">A storage to put the document to</param>
        /// <param name="document">A document to put</param>
        public static void PutRoot(VarioStorage storage, UIDocument document)
        {
            storage.Update(VarioInternalHelper.UiDocumentVariableName, document);
        }
        
        /// <summary>
        /// Puts a root <see cref="UIDocument"/> and a root query to the <see cref="VarioStorage"/>
        /// </summary>
        /// <param name="storage">A storage to put the document to</param>
        /// <param name="document">A document to put</param>
        /// <param name="rootQuery">A query that will be used to find a root element in the document</param>
        public static void PutRoot(VarioStorage storage, UIDocument document, UiElementsQuery rootQuery)
        {
            PutRoot(storage, document);
            storage.Update(VarioInternalHelper.UiRootQueryVariableName, rootQuery);
        }
        
        /// <summary>
        /// Puts a root <see cref="VisualElement"/> to the <see cref="VarioStorage"/>
        /// </summary>
        /// <param name="storage">A storage to put the element to</param>
        /// <param name="root">An element to put</param>
        public static void PutRoot(VarioStorage storage, VisualElement root)
        {
            storage.Update(VarioInternalHelper.UiRootVariableName, root);
        }        
    }
}
