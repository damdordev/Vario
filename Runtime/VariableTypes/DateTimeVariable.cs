#if DAMDOR_FOUNDATION

using System;
using Damdor.Foundation;

namespace Damdor.VariableStorage
{
    [Serializable]
    [VariableTypeName("dateTime")]
    public class DateTimeVariable : Variable<DateTime, SerializableDateTime> { }
}

#endif