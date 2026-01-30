using System;

namespace Damdor.VariableStorage
{
    public struct VariableMetadata
    {
        public string Name { get; }
        public Type Type { get; }
        
        public VariableMetadata(string name, Type type)
        {
            Name = name;
            Type = type;
        }
        
        
        
    }
}