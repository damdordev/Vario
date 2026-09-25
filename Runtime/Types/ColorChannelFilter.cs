using System;
using UnityEngine;

namespace Damdor.Vario
{
    [Serializable]
    public struct ColorChannelFilter
    {
        public bool R
        {
            get => !ignoreR;
            set => ignoreR = !value;
        }

        public bool G
        {
            get => !ignoreG;
            set => ignoreG = !value;
        }

        public bool B
        {
            get => !ignoreB;
            set => ignoreB = !value;
        }

        public bool A
        {
            get => !ignoreA;
            set => ignoreA = !value;
        }

        [SerializeField] private bool ignoreR;
        [SerializeField] private bool ignoreG;
        [SerializeField] private bool ignoreB;
        [SerializeField] private bool ignoreA;
    }
}