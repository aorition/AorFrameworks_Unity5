using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;


public class MaterialSnapShotWindow : EditorWindow
{

    public static Dictionary<string, MaterialSnapShot> SnapShotDic = new Dictionary<string, MaterialSnapShot>();
    public static SnapShotRecord Record;
    [MenuItem("FrameworkTools/图形/材质球快照工具")]
    public static void OpenMaterialBySnapShotTool()
    {
        MaterialSnapShotWindow.init();

    }

    public static void init()
    {

        MaterialSnapShotWindow w = EditorWindow.GetWindow<MaterialSnapShotWindow>("材质球快照工具");

    }



    /// <summary>
    /// 创建预制体的材质球快照
    /// </summary>
    /// <param name="name">快照名字</param>
    public void CteateMaterialSnapShot()
    {
        GameObject obj = Selection.activeGameObject;

        if (obj == null)
            return;

        string name = obj.name + "  :" + DateTime.Now;
        List<Material> matList = getAllmat(obj);
        MaterialSnapShot shot = new MaterialSnapShot(matList);

        if (!SnapShotDic.ContainsKey(name))
            SnapShotDic.Add(name, shot);


    }

    static void getMatLoop(Transform tran, ref List<Material> list)
    {
        
        if (tran.GetComponent<Renderer>() != null && tran.GetComponent<Renderer>().sharedMaterials != null && tran.GetComponent<Renderer>().sharedMaterials.Length > 0)
        {
            for (int i = 0; i < tran.GetComponent<Renderer>().sharedMaterials.Length; i++)
            {
                if (tran.GetComponent<Renderer>().sharedMaterials[i] == null)
                {
                    continue;
                }


                if (!list.Contains(tran.GetComponent<Renderer>().sharedMaterials[i]))
                    list.Add(tran.GetComponent<Renderer>().sharedMaterials[i]);
            }
        }

        if (tran.childCount > 0)
        {

            foreach (Transform each in tran)
            {
                getMatLoop(each, ref list);
            }

        }



    }

    static List<Material> getAllmat(GameObject obj)
    {
        List<Material> matList = new List<Material>();


        getMatLoop(obj.transform, ref matList);


        //遍历预制体获得所有mat
        return matList;

    }

    void Save()
    {
        if (SnapShotDic.Count == 0)
            return;


        if (Record == null)
        {
            getRecord();
        }


        if (Record != null)
        {
            Record.Save(SnapShotDic);
            EditorUtility.SetDirty(Record);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }

    void Load()
    {

        if (Record == null)
        {
            getRecord();


        }

        if (Record != null)
            SnapShotDic = Record.Load();


    }

    void getRecord()
    {
        Record = AssetDatabase.LoadAssetAtPath("Assets/SnapShotRecord.asset", typeof(SnapShotRecord)) as SnapShotRecord;
        if (Record == null)
        {
            Record = ScriptableObject.CreateInstance<SnapShotRecord>();
            AssetDatabase.CreateAsset(Record, "Assets/SnapShotRecord.asset");
        }

    }

    void OnGUI()
    {

        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical();
        GUILayout.Label("快照操作---------");

        if (GUILayout.Button("读取快照"))
        {
            Load();
        }

        if (GUILayout.Button("保存快照"))
        {
            Save();
        }
        if (GUILayout.Button("创建快照"))
        {
            CteateMaterialSnapShot();
        }
        GUILayout.EndVertical();


        //列表

        GUILayout.BeginVertical();
        GUILayout.Label("已存快照列表");


        GUILayout.BeginHorizontal();
        if (SnapShotDic.Count > 0)
        {
            string[] nameArray = SnapShotDic.Keys.ToArray();
            int index = GUILayout.SelectionGrid(-1, nameArray, 1);
            if (index >= 0)
            {
                string restoreName = nameArray[index];
                SnapShotDic[restoreName].Restore();
            }



            string[] delArray = new string[nameArray.Length];
            for (int i = 0; i < delArray.Length; i++)
            {
                delArray[i] = "删除";
            }

            int delIndex = GUILayout.SelectionGrid(-1, delArray, 1);
            if (delIndex >= 0)
            {
                SnapShotDic.Remove(nameArray[delIndex]);
            }
        }
        GUILayout.EndHorizontal();


        GUILayout.EndVertical();

        GUILayout.EndHorizontal();


    }



}

public class MaterialSnapShot
{

    public Dictionary<Material, Shader> MaterialDic;


    public MaterialSnapShot(List<Material> matlist)
    {
        MaterialDic = new Dictionary<Material, Shader>();
        for (int i = 0; i < matlist.Count; i++)
        {
            if (matlist[i] == null)
                continue;

            if (!MaterialDic.ContainsKey(matlist[i]))
            {
                MaterialDic.Add(matlist[i], matlist[i].shader);
            }

        }


    }
    public MaterialSnapShot(List<Material> matlist, List<Shader> shaderlist)
    {
        MaterialDic = new Dictionary<Material, Shader>();
        for (int i = 0; i < matlist.Count; i++)
        {

            if (!MaterialDic.ContainsKey(matlist[i]))
            {
                MaterialDic.Add(matlist[i], shaderlist[i]);
            }

        }


    }

    public void Restore()
    {

        foreach (KeyValuePair<Material, Shader> each in MaterialDic)
        {
            if (each.Key != null)
            {
                each.Key.shader = each.Value;
            }
        }


    }


}



public class SnapShotRecord : ScriptableObject
{
    [Serializable]
    public struct SnapShotData
    {
        public Material[] Materials;
        public Shader[] Shaders;
    }

    public List<string> Names;
    public List<SnapShotData> SnapShotDatas;

    public Dictionary<string, MaterialSnapShot> Load()
    {
        Dictionary<string, MaterialSnapShot> dic = new Dictionary<string, MaterialSnapShot>();

        if (Names != null && Names.Count > 0)
        {

            for (int i = 0; i < Names.Count; i++)
            {
                List<Material> matlist = new List<Material>();
                for (int j = 0; j < SnapShotDatas[i].Materials.Length; j++)
                {
                    matlist.Add(SnapShotDatas[i].Materials[j]);
                }

                List<Shader> shaderlist = new List<Shader>();
                for (int j = 0; j < SnapShotDatas[i].Shaders.Length; j++)
                {
                    shaderlist.Add(SnapShotDatas[i].Shaders[j]);
                }


                MaterialSnapShot shot = new MaterialSnapShot(matlist, shaderlist);

                dic.Add(Names[i], shot);

            }



        }

        return dic;
    }


    public void Save(Dictionary<string, MaterialSnapShot> dic)
    {
        Names = new List<string>();
        SnapShotDatas = new List<SnapShotData>();


        foreach (string each in dic.Keys)
        {
            Names.Add(each);

        }

        foreach (MaterialSnapShot each in dic.Values)
        {
            SnapShotData data = new SnapShotData();
            data.Materials = each.MaterialDic.Keys.ToArray();
            data.Shaders = each.MaterialDic.Values.ToArray();
            SnapShotDatas.Add(data);
        }


    }


}

