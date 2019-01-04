using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EditorUtils
{
    public class ConfigInspector
    {
        private Rect m_Position;
        private Editor m_Editor = null;
        [SerializeField]
        private Vector2 m_ScrollPosition;
        private BaseDataConfig m_CurrentGeneratorData;
        private ConfigTab m_ConfigTab;

        internal ConfigInspector(ConfigTab configTab)
        {
             m_ConfigTab = configTab;
        }

        internal void SetConfig(BaseDataConfig generatorData)
        {
            m_Editor = null;
            m_CurrentGeneratorData = null;
            if (generatorData != null)
            {
                m_Editor = Editor.CreateEditor(generatorData);
                m_CurrentGeneratorData = generatorData;
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
                        "Do you really want to delete " + m_CurrentGeneratorData.name +
                        "?\nWARNING: This action can't be undone", "Accept", "Cancel"))
                    {
                        m_ConfigTab.DeleteData(m_CurrentGeneratorData);
                        SetConfig(null);
                    }
                }
                EditorGUILayout.Space();
                GUILayout.EndArea();
            }
        }

    }

}
