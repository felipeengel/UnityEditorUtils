using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace EditorUtils
{
    public class PropertyTab
    {
        private Dictionary<int, PropertyData> m_Properties;
        public Dictionary<int, PropertyData> Properties
        {
            get
            {
                return m_Properties;
            }
        }
        private Rect m_Position;

        [SerializeField]
        private TreeViewState m_PropertyTreeState;
        private PropertyTree m_PropertyTreeView;
        private PropertyInspector m_PropertyInspector;

        private ResizablePanelRect m_resizablePanel;

        internal PropertyTab()
        {
            m_Properties = new Dictionary<int, PropertyData>();
            m_PropertyInspector = new PropertyInspector(this);
            m_resizablePanel = new ResizablePanelRect();
        }

        public void OnEnable(Rect pos, EditorWindow parent)
        {
            if (m_PropertyTreeState == null)
                m_PropertyTreeState = new TreeViewState();
            m_PropertyTreeView = new PropertyTree(m_PropertyTreeState, this);

            m_resizablePanel.OnEnable(pos, parent);

            RefreshData();
        }

        public void OnGUI(Rect pos)
        {
            m_Position = pos;
            EditorGUILayout.Space();
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Add Generic Property", GUILayout.MaxWidth(150f)))
            {
                CreateProperty(false);
            }
            if (GUILayout.Button("Add Specific Property", GUILayout.MaxWidth(150f)))
            {
                CreateProperty(true);
            }
            GUILayout.EndHorizontal();
            EditorGUILayout.Space();

            if (m_Properties.Count > 0)
            {
                m_resizablePanel.OnGUI(m_Position);

                float halfWidth = m_resizablePanel.LeftPanelWidth();
                m_PropertyTreeView.OnGUI(new Rect(m_Position.x, m_Position.y + 30, halfWidth, m_Position.height - 30));
                m_PropertyInspector.OnGUI(new Rect(m_Position.x + halfWidth, m_Position.y + 30, 
                    m_resizablePanel.RightPanelWidth(), m_Position.height - 30));

                m_resizablePanel.Repaint();
            }
        }

        public void SetPropertyItem(IList<TreeViewItem> selected)
        {
            if (selected == null || selected.Count == 0 || selected[0] == null)
            {
                m_PropertyInspector.SetProperty(null);
            }
            else if (selected.Count == 1)
            {
                PropertyData selectedConfig = m_Properties[selected[0].id];
                m_PropertyInspector.SetProperty(selectedConfig);
            }
            else
            {
                m_PropertyInspector.SetProperty(null);
            }
        }

        public void RefreshData()
        {
            m_Properties.Clear();
            string[] GUIds = AssetDatabase.FindAssets("t:PropertyData");
            foreach (string guid in GUIds)
            {
                PropertyData config = AssetDatabase.LoadAssetAtPath<PropertyData>(AssetDatabase.GUIDToAssetPath(guid));
                m_Properties.Add(config.GetInstanceID(), config);
            }

            m_PropertyTreeView.Reload();
        }

        public void RenameItem(TreeViewItem item, string newName)
        {
            PropertyData selectedConfig = m_Properties[item.id];
            string selectedPath = AssetDatabase.GetAssetPath(selectedConfig);
            AssetDatabase.RenameAsset(selectedPath, newName);
            RefreshAndSelect(selectedConfig);
        }

        private void CreateProperty(bool isSpecific)
        {
            string binPath = EditorUtility.SaveFilePanel("Create new Property", Application.dataPath, "newProperty", "asset");
            if (!string.IsNullOrEmpty(binPath))
            {
                string file = System.IO.Path.ChangeExtension(binPath, "");
                file = "Assets" + file.Replace(Application.dataPath, "");
                file = AssetDatabase.GenerateUniqueAssetPath(file + "asset");
                PropertyData createdConfig;
                if (isSpecific)
                {
                    PropertyData config = ScriptableObject.CreateInstance<PropertyData>();
                    config.isSpecific = isSpecific;
                    AssetDatabase.CreateAsset(config, file);
                    createdConfig = config;
                }
                else
                {
                    PropertyData config = ScriptableObject.CreateInstance<PropertyData>();
                    config.isSpecific = isSpecific;
                    AssetDatabase.CreateAsset(config, file);
                    createdConfig = config;
                }
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                RefreshAndSelect(createdConfig);
            }
        }

        private void RefreshAndSelect(PropertyData config)
        {
            RefreshData();
            m_PropertyTreeView.SetSelection(new List<int> { config.GetInstanceID() });
            m_PropertyInspector.SetProperty(config);
        }
    }
}
