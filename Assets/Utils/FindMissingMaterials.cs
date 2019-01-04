using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class FindMissingMaterials : EditorWindow
{
    protected bool m_FromContextual = false;
    protected Vector2 m_ScrollPosition;

    protected struct MissingMaterials
    {
        public GameObject go;
        public string goName;
    }
    protected List<MissingMaterials> m_MissinMaterials;
    public static GUILayoutOption GL_WIDTH_50 = GUILayout.Width(200);

    [MenuItem("Window/EditorUtils/Find Missing Materials")]
    static public void FindMissing()
    {
        GetWindow<FindMissingMaterials>();
    }

    private void OnEnable()
    {
        Init();
    }

    private void Init()
    {
        m_MissinMaterials = new List<MissingMaterials>();
        m_ScrollPosition = Vector2.zero;
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Missing Materials", EditorStyles.boldLabel);
        if (!m_FromContextual)
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Find ALL missing Materials in Assets"))
                FindInAssets();
            EditorGUILayout.EndHorizontal();
        }
        if (m_MissinMaterials.Count == 0)
        {
            EditorGUILayout.Space();
            GUIStyle st = new GUIStyle(EditorStyles.helpBox);
            st.font = EditorStyles.boldFont;
            st.fontSize = EditorStyles.boldFont.fontSize + 4;
            EditorGUILayout.LabelField("NO Missing prefab foud", st);
        }

        m_ScrollPosition = EditorGUILayout.BeginScrollView(m_ScrollPosition);

        for (int i = 0; i < m_MissinMaterials.Count; ++i)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(m_MissinMaterials[i].go.name + ", Mat: "+ m_MissinMaterials[i].goName);
            if (GUILayout.Button(m_MissinMaterials[i].go.name, GL_WIDTH_50))
            {
                EditorGUIUtility.PingObject(m_MissinMaterials[i].go);
            }
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();
    }

    /// <summary>
    /// based on https://github.com/Unity-Technologies/ToolsCollection/blob/master/Assets/MissingScriptFinder/Editor/FindMissingScripts.cs
    /// </summary>
    void FindInAssets()
    {
        var assetGUIDs = AssetDatabase.FindAssets("t:GameObject");
        m_MissinMaterials.Clear();

        Debug.Log("Testing " + assetGUIDs.Length + " GameObject in Assets");

        foreach (string assetGuiD in assetGUIDs)
        {
            GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(assetGuiD));

            SearchForMaterials(obj);
        }
    }

    private void SearchForMaterials(GameObject go)
    {
        MeshRenderer[] meshRenderer = go.GetComponentsInChildren<MeshRenderer>();
        foreach (var mr in meshRenderer)
        {
            foreach (var mat in mr.sharedMaterials)
            {
                if (mat == null)
                {
                    MissingMaterials mm = new MissingMaterials();
                    mm.go = go;
                    mm.goName = mr.name;
                    if (!m_MissinMaterials.Contains(mm))
                        m_MissinMaterials.Add(mm);
                }
            }
        }
    }

    /// <summary>
    /// Find missing materials
    /// based on https://github.com/Unity-Technologies/ToolsCollection/blob/master/Assets/MissingScriptFinder/Editor/FindMissingScripts.cs
    /// </summary>
    [MenuItem("Assets/EditorUtils/Find Missing Materials")]
    private static void FindForMissingMaterials()
    {
        FindMissingMaterials window = GetWindow<FindMissingMaterials>();
        window.m_FromContextual = true;
        window.Init();

        string path = GetSelectedPath();
        Debug.Log("<color=red>Selected path=" + path + "</color>");
        DirectoryInfo di = new DirectoryInfo(path);
        FileInfo[] fileInf = di.GetFiles("*.prefab", SearchOption.AllDirectories);

        if (fileInf.Length == 0)
        {
            EditorUtility.DisplayDialog("No prefab found", "No prefab found inside the selected directory", "Accept");
            return;
        }
        foreach (FileInfo fileInfo in fileInf)
        {
            string fullPath = fileInfo.FullName.Replace(@"\", "/");
            string assetPath = "Assets" + fullPath.Replace(Application.dataPath, "");
            GameObject prefab = AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject)) as GameObject;
            
            if (prefab != null)
            {
                window.SearchForMaterials(prefab);
            }
        }
        //EditorUtility.DisplayDialog("Materials search", m_GosWithMissinMaterials.Count + " Missing materials found", "Accept");
        window.Show();
    }

    private static string GetSelectedPath()
    {
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);

        if (string.IsNullOrEmpty(path))
        {
            path = Application.dataPath + "/";
        }
        else if (Path.GetExtension(path) != "")
        {
            path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
        }
        return path + "/";
    }
}
