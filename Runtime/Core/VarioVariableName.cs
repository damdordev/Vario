using System;

namespace Damdor.Vario
{
    /// <summary>
    /// Defines name for variable type. Used only to create a proper editor
    /// </summary>
    public class VarioVariableName : Attribute
    {
        /// <summary>
        /// Name of variable
        /// </summary>
        public string Name { get; }
        
        public VarioVariableName(string name)
        {
            Name = name;
        }
    }
}