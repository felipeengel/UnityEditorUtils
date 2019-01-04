using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace EditorUtils
{
    class PropertyTree : TreeView
    {
        PropertyTab m_PropertyTab;
        internal PropertyTree(TreeViewState s, PropertyTab parent) : base(s)
        {
            m_PropertyTab = parent;
            showBorder = true;
        }

        protected override TreeViewItem BuildRoot()
        {
            var root = new TreeViewItem(0, -1);
            root.children = new List<TreeViewItem>();
            foreach (KeyValuePair<int, PropertyData> property in m_PropertyTab.Properties)
            {
                root.AddChild(new TreeViewItem(
                    property.Key, 0, property.Value.name));
            }
            return root;
        }

        protected override void SelectionChanged(IList<int> selectedIds)
        {
            base.SelectionChanged(selectedIds);

            if (selectedIds == null)
                return;

            if (selectedIds.Count > 0)
            {
                m_PropertyTab.SetPropertyItem(FindRows(selectedIds));
            }
            else
            {
                m_PropertyTab.SetPropertyItem(null);
            }
        }

        protected override bool CanMultiSelect(TreeViewItem item)
        {
            return false;
        }

        protected override bool CanRename(TreeViewItem item)
        {
            return item != null && item.displayName.Length > 0;
        }

        protected override void RenameEnded(RenameEndedArgs args)
        {
            base.RenameEnded(args);
            if (args.newName.Length > 0 && args.newName != args.originalName)
            {
                args.acceptedRename = true;

                TreeViewItem item = FindItem(args.itemID, rootItem);
                m_PropertyTab.RenameItem(item, args.newName);
            }
            else
            {
                args.acceptedRename = false;
            }
        }
    }
}
