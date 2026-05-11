using System;
using System.Collections.Generic;

namespace Damdor.Vario
{
    /// <summary>
    /// Provides pooling for VarioVariable and VarioStorage instances to reduce allocations.
    /// </summary>
    internal static class VarioPooling
    {
        /// <summary>
        /// Gets or sets the maximum number of variables of each type to be stored in the pool.
        /// When the pool size is reduced, excess variables are discarded.
        /// </summary>
        public static int VariablePoolSize
        {
            get => variablePoolSize;
            set
            {
                variablePoolSize = value;
                foreach (var pool in variablePools.Values) ApplyMaxPoolSize(pool, variablePoolSize);
            }
        }
        
        /// <summary>
        /// Gets or sets the maximum number of storages to be stored in the pool.
        /// When the pool size is reduced, excess storages are discarded.
        /// </summary>
        public static int StoragePoolSize
        {
            get => storagePoolSize;
            set
            {
                storagePoolSize = value;
                ApplyMaxPoolSize(storagePool, storagePoolSize);
            }
        }
        
        private static readonly Dictionary<Type, Stack<VarioVariable>> variablePools = new();
        private static readonly Stack<VarioStorage> storagePool = new();
        private static int variablePoolSize = 100;
        private static int storagePoolSize = 100;

        /// <summary>
        /// Retrieves a variable of type <typeparamref name="T"/> from the pool or creates a new one if the pool is empty.
        /// </summary>
        public static VarioVariable<T> PopVariable<T>()
        {
            var type = typeof(T);
            return !variablePools.TryGetValue(type, out var pool) || pool.Count == 0
                ? VarioSettings.Create<T>()
                : (VarioVariable<T>)pool.Pop();
        }
        
        /// <summary>
        /// Releases a variable back to the pool.
        /// </summary>
        public static void ReleaseVariable(VarioVariable variable)
        {
            var type = variable.Type;
            if (!variablePools.TryGetValue(type, out var pool))
            {
                pool = new Stack<VarioVariable>();
                variablePools[type] = pool;
            }
            variable.Reset();
            if(pool.Count < variablePoolSize) pool.Push(variable);
        }
        
        /// <summary>
        /// Retrieves a storage from the pool or creates a new one if the pool is empty.
        /// </summary>
        public static VarioStorage PopStorage()
        {
            return storagePool.Count == 0
                ? new VarioStorage()
                : storagePool.Pop();
        }
        
        /// <summary>
        /// Releases a storage back to the pool.
        /// </summary>
        public static void ReleaseStorage(VarioStorage storage)
        {
            storage.Reset();
            if(storagePool.Count < storagePoolSize) storagePool.Push(storage);
        }

        private static void ApplyMaxPoolSize<T>(Stack<T> pool, int maxSize)
        {
            while (pool.Count > maxSize)
            {
                pool.Pop();
            }
        }
        
    }
}