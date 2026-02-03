#if DAMDOR_FOUNDATION

using System;
using Damdor.Foundation;

namespace Damdor.VariableStorage
{
    [Serializable]
    [VariableTypeName("timeSpan")]
    public class TimeSpanVariable : Variable<TimeSpan, SerializableTimeSpan>
    {
        protected override TimeSpan Convert(SerializableTimeSpan value) => value;
        protected override SerializableTimeSpan Convert(TimeSpan value) => value;
    }
}

#endif