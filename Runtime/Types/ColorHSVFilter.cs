using System;
using UnityEngine;

namespace Damdor.Vario
{
    [Serializable]
    public struct ColorHSVFilter
    {
        public bool H
        {
            get => !ignoreH;
            set => ignoreH = !value;
        }

        public bool S
        {
            get => !ignoreS;
            set => ignoreS = !value;
        }

        public bool V
        {
            get => !ignoreV;
            set => ignoreV = !value;
        }
        
        public bool A
        {
            get => !ignoreA;
            set => ignoreA = !value;
        }

        [SerializeField] private bool ignoreH;
        [SerializeField] private bool ignoreS;
        [SerializeField] private bool ignoreV;
        [SerializeField] private bool ignoreA;
    }
}