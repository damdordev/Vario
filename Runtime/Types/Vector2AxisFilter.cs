using System;
using UnityEngine;

namespace Damdor.Vario
{
    [Serializable]
    public struct Vector2AxisFilter
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

        [SerializeField] private bool ignoreX;
        [SerializeField] private bool ignoreY;
    }
}