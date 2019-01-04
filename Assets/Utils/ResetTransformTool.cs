
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEditor.SceneManagement;

public class ResetTransformTool : MonoBehaviour 
{
	[MenuItem("Assets/EditorUtils/Reset ALL prefab transforms")]
	private static void ResetPrefabsTransform()
	{
		string path = GetSelectedPath();
		Debug.Log("<color=red>Selected path=" + path + "</color>");
		DirectoryInfo di = new DirectoryInfo(path);
		FileInfo[] fileInf = di.GetFiles("*.prefab", SearchOption.AllDirectories);

		if (fileInf.Length == 0)
		{
			EditorUtility.DisplayDialog("No prefab found", "No prefab found inside the selected directory", "Accept");
			return;
		}
		int count = 0;
		foreach (FileInfo fileInfo in fileInf)
		{
			string fullPath = fileInfo.FullName.Replace(@"\", "/");
			string assetPath = "Assets" + fullPath.Replace(Application.dataPath, "");
			GameObject prefab = AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject)) as GameObject;
			count++;
			if (prefab != null)
			{
				Transform t = prefab.GetComponent<Transform>();
				ResetTrans(t);
			}
		}
		AssetDatabase.Refresh();
		EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
		EditorUtility.DisplayDialog("Prefab reseted", count + " Prefabs reseted", "Accept");
	}

	private static void ResetTrans(Transform t)
	{
		t.localPosition = Vector3.zero;
		t.localRotation = Quaternion.identity;
		t.localScale = Vector3.one;
		foreach (Transform child in t)
		{
			ResetTrans(child);
		}
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
