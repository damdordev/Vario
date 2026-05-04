using System;

namespace Damdor.Vario
{
    [AttributeUsage(AttributeTargets.Class)]
    public class VarioVariableAttribute : Attribute
    {
        public string Name { get; }

        public VarioVariableAttribute(string name)
        {
            Name = name;
        }
    }
}