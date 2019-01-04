using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EditorUtils
{
    public class PropertyInspector
    {
        private Rect m_Position;
        private Editor m_Editor = null;
        [SerializeField]
        private Vector2 m_ScrollPosition;
        private PropertyData m_CurrentProperty;
        private PropertyTab m_PropertyTab;

        internal PropertyInspector(PropertyTab propertyTab)
        {
            m_PropertyTab = propertyTab;
        }

        internal void SetProperty(PropertyData property)
        {
            m_Editor = null;
            m_CurrentProperty = null;
            if (property != null)
            {
                m_Editor = Editor.CreateEditor(property);
                m_CurrentProperty = property;
            }
        }

        internal void OnGUI(Rect pos)
        {
            m_Position = pos;
            if (m_Editor != null)
            {
                GUILayout.BeginArea(m_Position);
                m_ScrollPosition = EditorGUILayout.BeginScrollView(m_ScrollPosition);
                m_Editor.OnInspectorGUI();
                EditorGUILayout.EndScrollView();
                if (GUILayout.Button("Delete"))
                {
                    if (EditorUtility.DisplayDialog("Delete",
                        "Do you really want to delete " + m_CurrentProperty.name +
                        "?\nWARNING: This action can't be undone", "Accept", "Cancel"))
                    {
                        string path = AssetDatabase.GetAssetPath(m_CurrentProperty);
                        AssetDatabase.DeleteAsset(path);
                        AssetDatabase.Refresh();
                        m_PropertyTab.RefreshData();
                        SetProperty(null);
                    }
                }
                EditorGUILayout.Space();
                GUILayout.EndArea();
            }
        }
    }
}
