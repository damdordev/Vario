namespace Damdor.VariableStorage
{
    /// <summary>
    /// Interface that needs to be implemented by SerializableObject (like <c>MonoBehaviour</c> or <c>ScriptableObject</c>
    /// specifies used storage.
    /// This interface is required to build a proper editor - it's not used in runtime
    /// </summary>
    public interface IVariableStorageSource
    {
        /// <summary>
        /// Used storage
        /// </summary>
        VariableStorage Storage { get; }
    }
    
}