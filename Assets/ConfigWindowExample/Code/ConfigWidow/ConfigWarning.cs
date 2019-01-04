using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace EditorUtils
{
    [InitializeOnLoad]
    public class ConfigWarning 
    {
        static Texture2D texturePanel;
        static List<string> changedGUIDs;
        static ConfigWarning()
        {
            changedGUIDs = new List<string>();
            texturePanel = AssetDatabase.LoadAssetAtPath ("Assets/Artwork/UI/base/Forms/Sprites/WarningEmptyState.png", typeof(Texture2D)) as Texture2D;
            EditorApplication.projectWindowItemOnGUI += ProjectWindowItem;
        }

        static void ProjectWindowItem (string guid, Rect selectionRect)
        {
            if (changedGUIDs.Contains(guid))
            {
                Rect r = new Rect (selectionRect); 
                r.x = r.width - 10;
                r.width = 20;
                GUI.Label (r, texturePanel);
            }
        }

        public static void AddChangedGuid(string guid)
        {
            if (!changedGUIDs.Contains(guid))
            {
                changedGUIDs.Add(guid);
                EditorApplication.RepaintProjectWindow();
            }
        }
        public static void RemoveChangedGuid(string guid)
        {
            if (changedGUIDs.Contains(guid))
            {
                changedGUIDs.Remove(guid);
            }
        }
    }
}
