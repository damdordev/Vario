using System;
using UnityEngine;

namespace Damdor.Vario
{
    [Serializable]
    public struct Vector3AxisFilter
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

        [SerializeField] private bool ignoreX;
        [SerializeField] private bool ignoreY;
        [SerializeField] private bool ignoreZ;
    }
}