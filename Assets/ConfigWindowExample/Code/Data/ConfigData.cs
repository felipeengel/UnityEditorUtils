using UnityEngine;
using UnityEditor;

public class ConfigData : ScriptableObject 
{
    public string m_ConfigVersion;
    public BaseDataConfig[] m_GeneralConfigs;
    public BaseDataConfig[] m_SpecificConfigsA;
    public BaseDataConfig[] m_SpecificConfigsB;

#if UNITY_EDITOR
    public static void CreateConfigData(string dataPath)
    {
        string file = System.IO.Path.ChangeExtension(dataPath, "");
        file = "Assets" + file.Replace(Application.dataPath, "");
        file = AssetDatabase.GenerateUniqueAssetPath(file + "asset");
        var data = ScriptableObject.CreateInstance<ConfigData>();
        AssetDatabase.CreateAsset(data, file);
        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();
    }

    public void AddNewGeneralConfig(string name)
    {
        AddNewGeneratorData(ref m_GeneralConfigs, name);
    }

    public void AddNewSpecificAData(string name)
    {
        AddNewGeneratorData(ref m_SpecificConfigsA, name);
    }

    public void AddNewSpecificBData(string name)
    {
        AddNewGeneratorData(ref m_SpecificConfigsB, name);
    }

    private void AddNewGeneratorData(ref BaseDataConfig[] datas, string name)
    {
        System.Collections.Generic.List<BaseDataConfig> dataList = datas == null ? new System.Collections.Generic.List<BaseDataConfig>() :
            new System.Collections.Generic.List<BaseDataConfig>(datas);
        var data = ScriptableObject.CreateInstance<BaseDataConfig>();
        data.name = name;
        dataList.Add(data);
        AssetDatabase.AddObjectToAsset(data, this);
        datas = dataList.ToArray();
        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();
    }

    public void DeleteData(BaseDataConfig data)
    {
        bool deleted = DeleteData(data, ref m_GeneralConfigs);
        if (!deleted) deleted = DeleteData(data, ref m_SpecificConfigsA);
        if (!deleted) deleted = DeleteData(data, ref m_SpecificConfigsB);
    }

    private bool DeleteData(BaseDataConfig data, ref BaseDataConfig[] fromArray)
    {
        foreach (var cfgData in fromArray)
        {
            if (cfgData == data)
            {
                System.Collections.Generic.List<BaseDataConfig> list =  new System.Collections.Generic.List<BaseDataConfig>(fromArray);
                list.Remove(data);
                fromArray = list.ToArray();
                return true;
            }
        }
        return false;
    }

    public BaseDataConfig GetDataWithInstanceId(int instanceId)
    {
        BaseDataConfig data = null;
        data = GetDataWithInstanceId(m_GeneralConfigs, instanceId);
        if (data != null) return data;
        data = GetDataWithInstanceId(m_SpecificConfigsA, instanceId);
        if (data != null) return data;
        data = GetDataWithInstanceId(m_SpecificConfigsB, instanceId);
        if (data != null) return data;
        return null;
    }

    private BaseDataConfig GetDataWithInstanceId(BaseDataConfig[] datas, int instaceId)
    {
        foreach (var item in datas)
        {
            if (item.GetInstanceID() == instaceId)
                return item;
        }
        return null;
    }
#endif
}
