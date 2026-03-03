#if DAMDOR_FOUNDATION

using System;
using Damdor.Foundation;

namespace Damdor.Vario
{
    [Serializable]
    [VarioVariableName("dateTime")]
    public class DateTimeVarioVariable : VarioVariable<DateTime, SerializableDateTime> { }
}

#endif