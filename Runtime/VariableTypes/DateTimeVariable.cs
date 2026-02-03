#if DAMDOR_FOUNDATION

using System;
using Damdor.Foundation;

namespace Damdor.VariableStorage
{
    [Serializable]
    [VariableTypeName("dateTime")]
    public class DateTimeVariable : Variable<DateTime, SerializableDateTime>
    {
        protected override DateTime Convert(SerializableDateTime value) => value;
        protected override SerializableDateTime Convert(DateTime value) => value;
    }
}

#endif