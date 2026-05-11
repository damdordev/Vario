using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.IMGUI.Controls;

namespace Damdor.Vario.Editor
{
    public class HierarchicalDropdown<T> : AdvancedDropdown
    {
        private readonly T[] choices;
        private readonly Converter<T, string> getName;
        private readonly Action<T> onChoice;

        private readonly Dictionary<string, AdvancedDropdownItem> parents = new();
        private readonly Dictionary<AdvancedDropdownItem, T> itemToChoice = new();
            
        public HierarchicalDropdown(IEnumerable<T> choices, Converter<T, string> getName, Action<T> onChoice)
            : base(new AdvancedDropdownState())
        {
            this.choices = choices.ToArray();
            this.getName = getName;
            this.onChoice = onChoice;
        }

        protected override AdvancedDropdownItem BuildRoot()
        {
            var root = new AdvancedDropdownItem("");
            parents[""] = root;

            foreach (var choice in choices)
            {
                var path = getName(choice);
                var parent = GetParent(path);
                var item = new AdvancedDropdownItem(GetRawName(path));
                itemToChoice[item] = choice;
                parent.AddChild(item);
            }

            return root;
        }

        protected override void ItemSelected(AdvancedDropdownItem item)
        {
            onChoice?.Invoke(itemToChoice[item]);
        }

        private AdvancedDropdownItem GetParent(string path)
        {
            if (!path.Contains('/')) return parents[""];
            var parentPath = path[..path.LastIndexOf('/')];
            if (parents.TryGetValue(parentPath, out var parent)) return parent;

            var parentName = GetRawName(parentPath);
            var grandparent = GetParent(parentPath);
            parent = new AdvancedDropdownItem(parentName);
            parents[parentPath] = parent;
            grandparent.AddChild(parent);
            return parent;
        }

        private static string GetRawName(string path) => path.Contains('/')
            ? path[(path.LastIndexOf('/') + 1)..]
            : path;
        
    }
}