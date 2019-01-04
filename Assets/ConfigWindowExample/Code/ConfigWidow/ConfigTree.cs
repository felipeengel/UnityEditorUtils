using System.Collections.Generic;
using System.Linq;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEditor;

namespace EditorUtils
{

    public class ConfigTree : TreeView
    {
        private ConfigTab mConfigTab;
        private const int k_GeneralTreeId = -2;
        private const int k_SettingsTreeId = -3;
        private const int k_SettingsATreeId = -30;
        private const int k_SettingsBTreeId = -300;

        internal ConfigTree(TreeViewState s, ConfigTab parent) : base(s)
        {
            mConfigTab = parent;
            showBorder = true;
        }

        protected override TreeViewItem BuildRoot()
        {
            var root = new TreeViewItem(-1, -1);
            root.children = new List<TreeViewItem>();

            ConfigData config = mConfigTab.SelectedConfig;
            root.AddChild(CreateGeneralItems(config));
            root.AddChild(CreateSpecificSettingsItems(config));

            return root;
        }

        protected override void SelectionChanged(IList<int> selectedIds)
        {
            base.SelectionChanged(selectedIds);

            if (selectedIds == null)
                return;

            if (selectedIds.Count > 0)
            {
                mConfigTab.SetConfigItem(GetSelectedConfigItem(FindRows(selectedIds)));
            }
            else
            {
                mConfigTab.SetConfigItem(null);
            }
        }

        protected override bool CanMultiSelect(TreeViewItem item)
        {
            return false;
        }

        protected override void RowGUI(RowGUIArgs args)
        {
            base.RowGUI(args);
            if (IsSpecialTree(args.item.id) && 
                args.item.id != k_SettingsTreeId)
            {
                var width = 16;
                var edgeRect = new Rect(args.rowRect.xMax - width, args.rowRect.y, width, args.rowRect.height);
                if (GUI.Button(edgeRect, "+"))
                {
                    CreateNewConfigData(edgeRect, args.item.id);
                }
            }
        }

        private TreeViewItem CreateGeneralItems(ConfigData config)
        {
            return CreateTreeItems(k_GeneralTreeId, "Config 1", 0, config.m_GeneralConfigs);
        }

        private TreeViewItem CreateSpecificSettingsItems(ConfigData config)
        {
            var allParent = new TreeViewItem(k_SettingsTreeId, 0, "Settings 2");

            var parent = CreateTreeItems(k_SettingsATreeId, "Settings A", 1, config.m_SpecificConfigsA);
            allParent.AddChild(parent);

            var parent2 = CreateTreeItems(k_SettingsBTreeId, "Settings B", 1, config.m_SpecificConfigsB);
            allParent.AddChild(parent2);

            return allParent;
        }

        private TreeViewItem CreateTreeItems(int id, string name, int startDepth, BaseDataConfig[] configs)
        {
            var parentTree = new TreeViewItem(id, startDepth, name);
            if (configs != null)
            {
                foreach (var configData in configs)
                {
                    if (configData != null)
                    {
                        parentTree.AddChild(new TreeViewItem(
                            configData.GetInstanceID(), startDepth + 1,
                            configData.name));
                    }
                }
            }

            return parentTree;
        }

        private BaseDataConfig GetSelectedConfigItem(IList<TreeViewItem> selected)
        {
            if (selected == null || selected.Count == 0 || selected[0] == null)
            {
                return null;
            }
            if (selected.Count == 1)
            {
                if (!IsSpecialTree(selected[0].id))
                {
                    return mConfigTab.SelectedConfig.GetDataWithInstanceId(selected[0].id);
                }
            }
            return null;
        }

        private void CreateNewConfigData(Rect pos, int id)
        {
            PopupWindow.Show(pos, new TextFieldEditorWindow((string name)=>
            {
                if (!string.IsNullOrEmpty(name))
                {
                    switch (id)
                    {
                        case k_GeneralTreeId:
                            mConfigTab.SelectedConfig.AddNewGeneralConfig(name);
                        break;
                        case k_SettingsATreeId:
                            mConfigTab.SelectedConfig.AddNewSpecificAData(name);
                        break;
                        case k_SettingsBTreeId:
                            mConfigTab.SelectedConfig.AddNewSpecificBData(name);
                        break;
                    }
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    mConfigTab.RefreshData();
                }
            }));
        }

        private bool IsSpecialTree(int id)
        {
            return id == k_GeneralTreeId || 
                id == k_SettingsTreeId || 
                id == k_SettingsATreeId || 
                id == k_SettingsBTreeId;
        }
    }
}
