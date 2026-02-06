using System;

namespace Damdor.VariableStorage
{
    /// <summary>
    /// Defines name for variable type. Used only to create a proper editor
    /// </summary>
    public class VariableTypeName : Attribute
    {
        /// <summary>
        /// Name of variable
        /// </summary>
        public string Name { get; }
        
        public VariableTypeName(string name)
        {
            Name = name;
        }
    }
}