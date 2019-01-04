using UnityEditor;
using UnityEngine;

namespace EditorUtils
{
    public class ResizablePanelRect
    {
        private Rect m_Position;
        private EditorWindow m_Parent = null;
        private Rect m_HorizontalSplitterRect;
        private bool m_ResizingHorizontalSplitter = false;
        private float m_HorizontalSplitterPercent;
        private const float k_SplitterWidth = 3f;

        public bool ResizingHorizontalSplitter { get { return m_ResizingHorizontalSplitter;} }

        internal ResizablePanelRect()
        {
            m_HorizontalSplitterPercent = 0.4f;
        }

        public void OnEnable(Rect pos, EditorWindow parent) 
        {
            m_Parent = parent;
            m_Position = pos;
            m_HorizontalSplitterRect = new Rect(
                    (int)(m_Position.x + m_Position.width * m_HorizontalSplitterPercent),
                    m_Position.y,
                    k_SplitterWidth,
                    m_Position.height);
        }

        public void OnGUI(Rect pos)
        {
            m_Position = pos;

            m_HorizontalSplitterRect.x = (int)(m_Position.width * m_HorizontalSplitterPercent);
            m_HorizontalSplitterRect.height = m_Position.height;

            EditorGUIUtility.AddCursorRect(m_HorizontalSplitterRect, MouseCursor.ResizeHorizontal);
            if (Event.current.type == EventType.MouseDown && m_HorizontalSplitterRect.Contains(Event.current.mousePosition))
                m_ResizingHorizontalSplitter = true;

            if (m_ResizingHorizontalSplitter)
            {
                m_HorizontalSplitterPercent = Mathf.Clamp(Event.current.mousePosition.x / m_Position.width, 0.1f, 0.9f);
                m_HorizontalSplitterRect.x = (int)(m_Position.width * m_HorizontalSplitterPercent);
            }

            if (Event.current.type == EventType.MouseUp)
                m_ResizingHorizontalSplitter = false;
        }

        public void Repaint()
        {
            if (ResizingHorizontalSplitter)
                m_Parent.Repaint();
        }

        public float LeftPanelWidth()
        {
            return m_HorizontalSplitterRect.x;
        }

        public float RightPanelWidth()
        {
            return (m_Position.width - m_HorizontalSplitterRect.x) - k_SplitterWidth;
        }
    }
}
