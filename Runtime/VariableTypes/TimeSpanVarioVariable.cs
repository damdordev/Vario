#if DAMDOR_FOUNDATION

using System;
using Damdor.Foundation;

namespace Damdor.Vario
{
    [Serializable]
    [VarioVariableName("timeSpan")]
    public class TimeSpanVarioVariable : VarioVariable<TimeSpan, SerializableTimeSpan> {}
}

#endif