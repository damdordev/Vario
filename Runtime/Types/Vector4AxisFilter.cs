using System;
using UnityEngine;

namespace Damdor.Vario
{
    [Serializable]
    public struct Vector4AxisFilter
    {
        public bool X
        {
            get => !ignoreX;
            set => ignoreX = !value;
        }

        public bool Y
        {
            get => !ignoreY;
            set => ignoreY = !value;
        }

        public bool Z
        {
            get => !ignoreZ;
            set => ignoreZ = !value;
        }

        public bool W
        {
            get => !ignoreW;
            set => ignoreW = !value;
        }

        [SerializeField] private bool ignoreX;
        [SerializeField] private bool ignoreY;
        [SerializeField] private bool ignoreZ;
        [SerializeField] private bool ignoreW;
    }
}