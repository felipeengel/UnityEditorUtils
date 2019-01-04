using UnityEngine;
using UnityEditor;
using System;

public class TextFieldEditorWindow : PopupWindowContent
{
    private string m_Text;
    private System.Action<string> m_SelectedCallback;

    public TextFieldEditorWindow( System.Action<string> selectedCallback)
    {
        m_SelectedCallback = selectedCallback;
    }

    public override Vector2 GetWindowSize()
    {
        return new Vector2(400, 30);
    }

    public override void OnGUI(Rect rect)
    {
        EditorGUILayout.BeginHorizontal();
        m_Text = EditorGUILayout.TextField("Enter Name", m_Text);
        if (GUILayout.Button("Accept")) 
        {
            editorWindow.Close();
            m_SelectedCallback(m_Text);
        }
        if (GUILayout.Button("Close")) 
        {
            editorWindow.Close();
        }
        EditorGUILayout.EndHorizontal();
    }
}
