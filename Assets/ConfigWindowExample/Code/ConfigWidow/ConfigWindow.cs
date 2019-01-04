using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace EditorUtils
{
    public class ConfigWindow : EditorWindow
    {
        private ConfigTab m_GeneratorsTab;
        private PropertyTab m_PropertyTab;

        enum Tabs
        {
            GeneratorConfig,
            PropertyConfig
        }
        [SerializeField]
        private Tabs m_Tabs;

        private const float k_ToolbarPadding = 15;
        private const float k_MenubarPadding = 32;

        [MenuItem("Window/EditorUtils/Config Window %#c")]
        public static void ShowWindow()
        {
            ConfigWindow window = GetWindow<ConfigWindow>();
            window.titleContent = new GUIContent("Generator Config");
            window.Show();
        }

        public static void ShowWindow(string withConfigName)
        {
            ConfigWindow window = GetWindow<ConfigWindow>();
            window.titleContent = new GUIContent("Generator Config");
            window.Show();
            if (window.m_GeneratorsTab != null)
            {
                window.m_GeneratorsTab.InitWithConfig(withConfigName);
            }
        }

        private void OnEnable()
        {
            if (m_GeneratorsTab == null)
                m_GeneratorsTab = new ConfigTab();
            if (m_PropertyTab == null)
                m_PropertyTab = new PropertyTab();
            Rect subWinRect = GetSubWindowArea();
            m_GeneratorsTab.OnEnable(subWinRect, this);
            m_PropertyTab.OnEnable(subWinRect, this);
        }

        private void OnGUI()
        {
            ToggleTabs();
            switch (m_Tabs)
            {
                case Tabs.GeneratorConfig:
                    m_GeneratorsTab.OnGUI(GetSubWindowArea());
                    break;
                case Tabs.PropertyConfig:
                    m_PropertyTab.OnGUI(GetSubWindowArea());
                    break;
            }
        }

        private void ToggleTabs()
        {
            GUILayout.BeginHorizontal();
            { 
                GUILayout.Space(k_ToolbarPadding);
                float toolbarWidth = position.width - k_ToolbarPadding * 4;
                string[] labels = new string[2] { "Configuration", "Properties" };
                m_Tabs = (Tabs)GUILayout.Toolbar((int)m_Tabs, labels, "LargeButton", GUILayout.Width(toolbarWidth));
                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();
        }

        private Rect GetSubWindowArea()
        {
            float padding = k_MenubarPadding;
            Rect subPos = new Rect(0, padding, position.width, position.height - padding);
            return subPos;
        }
    }
}