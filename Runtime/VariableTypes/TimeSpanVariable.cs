#if DAMDOR_FOUNDATION

using System;
using Damdor.Foundation;

namespace Damdor.VariableStorage
{
    [Serializable]
    [VariableTypeName("timeSpan")]
    public class TimeSpanVariable : Variable<TimeSpan, SerializableTimeSpan> {}
}

#endif