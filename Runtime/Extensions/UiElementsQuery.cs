#if DAMDOR_VARIO_UI_ELEMENTS

using System;
using UnityEngine;

namespace Damdor.Vario
{
    [Serializable]
    public struct UiElementsQuery : IEquatable<UiElementsQuery>
    {
        [SerializeField] private string name;
        [SerializeField] private string className;

        public string Name
        {
            get => name;
            set => name = value;
        }

        public string ClassName
        {
            get => className;
            set => className = value;
        }

        public bool Equals(UiElementsQuery other)
        {
            return Name == other.Name && ClassName == other.ClassName;
        }

        public override bool Equals(object obj)
        {
            return obj is UiElementsQuery other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (ClassName != null ? ClassName.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(UiElementsQuery left, UiElementsQuery right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(UiElementsQuery left, UiElementsQuery right)
        {
            return !left.Equals(right);
        }
    }
}
#endif