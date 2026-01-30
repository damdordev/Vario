using System.Collections.Generic;

namespace Damdor.VariableStorage
{
    public interface IVariableStorage
    {
        bool Contains<T>(string name);
        T Get<T>(string name);
        void Set<T>(string name, T value) {}
        void Remove<T>(string name){}
        bool Rename<T>(string oldName, string newName);
        IEnumerable<VariableMetadata> AllVariables { get; }
    }
}