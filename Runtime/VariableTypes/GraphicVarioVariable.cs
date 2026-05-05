#if DAMDOR_VARIO_UGUI

using System;
using UnityEngine.UI;

namespace Damdor.Vario
{
    [Serializable]
    [VarioVariable("Graphic")]
    public class GraphicVarioVariable : VarioVariable<Graphic> { }
}

#endif