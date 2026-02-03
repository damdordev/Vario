using System;

namespace Damdor.VariableStorage
{
    public class VariableTypeName : Attribute
    {
        public string Name { get; }
        
        public VariableTypeName(string name)
        {
            Name = name;
        }
    }
}