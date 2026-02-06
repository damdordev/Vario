using System;
using UnityEditor.IMGUI.Controls;

namespace Damdor.VariableStorage.Editor
{
    internal class StringDropdown : AdvancedDropdown
    {
        private readonly string[] choices;
        private readonly Action<string> onChoice;
            
        public StringDropdown(string[] choices, Action<string> onChoice) : base(new AdvancedDropdownState())
        {
            this.choices = choices;
            this.onChoice = onChoice;
        }

        protected override AdvancedDropdownItem BuildRoot()
        {
            var root = new AdvancedDropdownItem("");

            foreach (var variable in choices)
            {
                root.AddChild(new AdvancedDropdownItem(variable));
            }

            return root;
        }

        protected override void ItemSelected(AdvancedDropdownItem item)
        {
            onChoice?.Invoke(item.name);
        }
    }
}