#if DAMDOR_VARIO_UGUI

using System;
using UnityEngine;

namespace Damdor.Vario
{
    [Serializable]
    [VarioVariable("CanvasGroup")]
    public class CanvasGroupVarioVariable : VarioVariable<CanvasGroup> { }
}

#endif