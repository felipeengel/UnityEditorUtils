using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using System.Collections.Generic;

namespace EditorUtils
{
    public class ConfigTab 
    {
        private Rect m_Position;
        [SerializeField]
        private TreeViewState m_ConfigTreeState;
        private ConfigTree m_ConfigTreeView;
        private ConfigData m_SelectedConfig;
        public ConfigData SelectedConfig
        {
            get { return m_SelectedConfig; }
        }
        private ConfigInspector m_ConfigInspector;

        private ResizablePanelRect m_resizablePanel;
        private const float k_TopHeight = 60f;
        private const float k_TopMargin = 5f;
        private const float k_TopHeightWithMargin = k_TopHeight + k_TopMargin;
        private const float k_ButtonHeight = 25f;
        private string[] m_ConfigNames;
        private string[] m_ConfigGuids;
        [SerializeField]
        private int m_SelectedConfigIndex = 0;

        internal ConfigTab()
        {
            m_ConfigInspector = new ConfigInspector(this);
            m_resizablePanel = new ResizablePanelRect();
        }

        public void OnEnable(Rect pos, EditorWindow parent)
        {
            m_Position = pos;
            m_resizablePanel.OnEnable(pos, parent);
            
            if (m_ConfigTreeState == null)
                m_ConfigTreeState = new TreeViewState();
            m_ConfigTreeView = new ConfigTree(m_ConfigTreeState, this);
            GetAllConfigurations();
        }

        public void OnGUI(Rect pos)
        {
            m_Position = pos;

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            m_SelectedConfigIndex = EditorGUILayout.Popup("Configuration", m_SelectedConfigIndex, m_ConfigNames);
            if (GUILayout.Button("Create New", GUILayout.MaxWidth(100f)))
            {
                    string binPath = Application.streamingAssetsPath + "/Data/GeneralConfig.asset";
                    binPath = EditorUtility.SaveFilePanel("Create new Config", binPath, "newConfig",
                        "asset");
                    if (!string.IsNullOrEmpty(binPath))
                    {
                        ConfigData.CreateConfigData(binPath);
                        GetAllConfigurations();
                        SetConfingSelected(System.IO.Path.GetFileNameWithoutExtension(binPath));
                        GUI.changed = true;
                    }
            }
            if (EditorGUI.EndChangeCheck())
            {
                m_SelectedConfig = m_SelectedConfigIndex == 0 ? null :
                    AssetDatabase.LoadAssetAtPath<ConfigData>(AssetDatabase.GUIDToAssetPath(m_ConfigGuids[m_SelectedConfigIndex - 1]));
            }
            EditorGUILayout.EndHorizontal();

            if (m_SelectedConfig != null)
            {
                RefreshData();
                EditorGUILayout.Space();
                EditorGUI.BeginChangeCheck();
                
                SerializedObject serializedObject = new SerializedObject(m_SelectedConfig);
                serializedObject.Update();
                SerializedProperty version = serializedObject.FindProperty("m_ConfigVersion");
                version.stringValue = EditorGUILayout.TextField("Version ", version.stringValue);
                
                m_resizablePanel.OnGUI(m_Position);

                float panelTop = m_Position.y + k_TopHeight;
                float panelBottom = (m_Position.height - k_TopHeightWithMargin) - (k_ButtonHeight + k_TopMargin);
                float buttonBottom = panelTop + panelBottom + k_TopMargin;

                var bundleTreeRect = new Rect(
                    m_Position.x,
                    panelTop,
                    m_resizablePanel.LeftPanelWidth(),
                    panelBottom);

                float panelLeft = m_Position.x + m_resizablePanel.LeftPanelWidth();
                float panelWidth = m_resizablePanel.RightPanelWidth();

                m_ConfigTreeView.OnGUI(bundleTreeRect);
                m_ConfigInspector.OnGUI(new Rect(
                    panelLeft, 
                    panelTop,
                    panelWidth,
                    panelBottom));

                m_resizablePanel.Repaint();

                var buttonTreeRect = new Rect(
                    m_Position.x + 15f,
                    buttonBottom,
                    m_Position.width - 15f*2f,
                    20f);
                
                
                if (EditorGUI.EndChangeCheck())
                {
                    string guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(m_SelectedConfig));
                    if (serializedObject.targetObject != m_SelectedConfig)
                    {
                        Debug.Log("--------><color=green>Changed="+guid+"</color>");
                        ConfigWarning.AddChangedGuid(guid);
                    }
                    else{
                        ConfigWarning.RemoveChangedGuid(guid);
                    } 
                }
                serializedObject.ApplyModifiedProperties();
                if (GUI.Button(buttonTreeRect, "Save binary"))
                {
                    string binPath = Application.streamingAssetsPath + "/Data/GeneralConfig.asset";
                    binPath = EditorUtility.SaveFilePanel("Save Binary Config", binPath, m_SelectedConfig.name,
                        "asset");
                    if (!string.IsNullOrEmpty(binPath))
                    {
                        // SerializationUtil.SerializeObjectToFile(m_SelectedConfig, binPath,
                        //     m_SelectedConfig.CreateSurrogateSelector<ConfigData>());
                        EditorUtility.DisplayDialog("Saved", "Config saved in " + binPath, "Accept");
                        AssetDatabase.Refresh();
                        
                        string guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(m_SelectedConfig));
                        ConfigWarning.RemoveChangedGuid(guid);
                    }
                }
                EditorGUILayout.Space();
            }
        }

        public void SetConfigItem(BaseDataConfig generatorData)
        {
            m_ConfigInspector.SetConfig(generatorData);
        }

        public void RefreshData()
        {
            m_ConfigTreeView.Reload();
        }

        public void DeleteData(BaseDataConfig data)
        {
            m_SelectedConfig.DeleteData(data);

            string dataPath = AssetDatabase.GetAssetPath(data);
            string configPath = AssetDatabase.GetAssetPath(m_SelectedConfig);
            if (configPath.Equals(dataPath))
            {
                UnityEngine.GameObject.DestroyImmediate(data, true);
            }
            else
            {
                AssetDatabase.DeleteAsset(dataPath);
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            RefreshData();
        }

        private void GetAllConfigurations()
        {
            m_ConfigGuids = AssetDatabase.FindAssets("t:ConfigData");
            m_ConfigNames = new string[m_ConfigGuids.Length + 1];
            m_ConfigNames[0] = "---- Select Config ----";
            int i = 1;
            foreach (var guid in m_ConfigGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                m_ConfigNames[i++] = System.IO.Path.GetFileNameWithoutExtension(path);
            }
        }

        private void SetConfingSelected(string configName)
        {
            int i = 0;
            foreach (var theName in  m_ConfigNames)
            {
                if (configName.Equals(theName))
                {
                    m_SelectedConfigIndex = i;
                    break;
                }
                i++;
            }
        }

        public void InitWithConfig(string configName)
        {
            GetAllConfigurations();
            SetConfingSelected(configName);
            m_SelectedConfig =  AssetDatabase.LoadAssetAtPath<ConfigData>(AssetDatabase.GUIDToAssetPath(m_ConfigGuids[m_SelectedConfigIndex - 1]));
        }
    }

}
