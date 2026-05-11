using System;
using System.Collections.Generic;

namespace Damdor.Vario
{
    internal static class VarioPooling
    {
        private static readonly Dictionary<Type, Stack<VarioVariable>> variablePools = new();
        private static readonly Stack<VarioStorage> storagePools = new();

        public static VarioVariable<T> PopVariable<T>()
        {
            var type = typeof(T);
            return !variablePools.TryGetValue(type, out var pool) || pool.Count == 0
                ? VarioSettings.Create<T>()
                : (VarioVariable<T>)pool.Pop();
        }
        
        public static void ReleaseVariable(VarioVariable variable)
        {
            var type = variable.Type;
            if (!variablePools.TryGetValue(type, out var pool))
            {
                pool = new Stack<VarioVariable>();
                variablePools[type] = pool;
            }
            variable.Reset();
            pool.Push(variable);
        }
        
        public static VarioStorage PopStorage()
        {
            return storagePools.Count == 0
                ? new VarioStorage()
                : storagePools.Pop();
        }
        
        public static void ReleaseStorage(VarioStorage storage)
        {
            storagePools.Push(storage);
        }
        
    }
}