#if DAMDOR_VARIO_UGUI

using System;
using UnityEngine.UI;

namespace Damdor.Vario
{
    [Serializable]
    [VarioVariable("Image")]
    public class ImageVarioVariable : VarioVariable<Image> { }
}

#endif