using Framework.Editor;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Text.RegularExpressions;
using AorBaseUtility;
using UnityEditor;
using UnityEngine;

namespace Framework.NodeGraph
{
    public class HierarchyObjRenamerController : NodeController
    {

        public override void update(bool updateParentLoop = true)
        {

            bool useEditorSelection = (bool)nodeGUI.data.ref_GetField_Inst_Public("UseEditorSelection");

            _shotNum = (int)nodeGUI.data.ref_GetField_Inst_Public("ShotNum");

            List<string> resultInfoList = new List<string>();
            List<int> instanceList = new List<int>();

            List<GameObject> gameObjectList = new List<GameObject>();

            string renameKey = (string)nodeGUI.data.ref_GetField_Inst_Public("RenameKey");

            if (string.IsNullOrEmpty(renameKey))
            {
                return;
            }

            int i = 0;
            int len;

            if (useEditorSelection)
            {

                gameObjectList.AddRange(Selection.gameObjects);

            }
            else
            {

                //获取上级节点数据 (PrefabInput)
                ConnectionPointGUI cpg2 = NodeGraphBase.Instance.GetConnectionPointGui(m_nodeGUI.id, 100, ConnectionPointInoutType.MutiInput);
                List<ConnectionGUI> clist2 = NodeGraphBase.Instance.GetContainsConnectionGUI(cpg2);
                if (clist2 != null)
                {

                    List<int> parentInsIdData = new List<int>();

                    len = clist2.Count;
                    for (i = 0; i < len; i++)
                    {
                        int[] pd = (int[]) clist2[i].GetConnectionValue(updateParentLoop);
                        if (pd != null)
                        {
                            //去重复
                            for (int a = 0; a < pd.Length; a++)
                            {
                                if (!parentInsIdData.Contains(pd[a]))
                                {
                                    parentInsIdData.Add(pd[a]);
                                }
                            }
                        }
                    }

                    //查找Prefab

                    len = parentInsIdData.Count;
                    for (i = 0; i < len; i++)
                    {
                        GameObject go = (GameObject) EditorUtility.InstanceIDToObject(parentInsIdData[i]);
                        if (go)
                        {
                            gameObjectList.Add(go);
                        }
                    }
                    
                }

            }

            if (gameObjectList.Count > 0)
            {

                gameObjectList.Sort((a, b) =>
                {
                    if (a.transform.GetSiblingIndex() >= b.transform.GetSiblingIndex())
                    {
                        return 1;
                    }
                    else
                    {
                        return -1;
                    }
                });

                len = gameObjectList.Count;
                for (i = 0; i < len; i++)
                {
                    EditorUtility.DisplayProgressBar("Processing ...", "Processing ..." + i + " / " + len,
                        Mathf.Round((float)i / len * 10000) * 0.01f);
                    GameObject go = gameObjectList[i];

                    _renameGo(go, renameKey, ref instanceList, ref resultInfoList);
                }

                EditorUtility.ClearProgressBar();

            }

            //输出 。。。
            if (resultInfoList.Count > 0)
            {
                m_nodeGUI.data.ref_SetField_Inst_Public("ResultInfo", resultInfoList.ToArray());
            }
            else
            {
                m_nodeGUI.data.ref_SetField_Inst_Public("ResultInfo", null);
            }

            if (instanceList.Count > 0)
            {
                m_nodeGUI.data.ref_SetField_Inst_Public("InstancesPath", instanceList.ToArray());
            }

            //
            NodeGraphBase.TimeInterval_Request_SAVESHOTCUTGRAPH = true; //申请延迟保存快照
            base.update(false);
        }

        private static Regex _nRegex = new Regex("\\[n\\:*\\d*\\]");
        private static Regex _nOgRegex = new Regex("\\[r\\?*\\w*\\:*\\w*\\]");
        private int _shotNum = 0;

        /// <summary>
        ///
        /// renameKey 解析规则:
        /// 
        /// [n:数值] 数值表示该序列的固定位数
        ///
        /// </summary>
        private void _renameGo(GameObject g, string renameKey, ref List<int> instanceList, ref List<string> resultInfoList)
        {
            string rk = renameKey;
            Match m = _nRegex.Match(renameKey);
            if (m.Success)
            {
                string ty = m.Value.Substring(1, m.Value.Length - 2);
                bool nn = ty.Contains(":");
                if (nn)
                {
                    int sn = int.Parse(ty.Split(':')[1]);
                    rk = rk.Replace(m.Value, _shotNum.ToString().PadLeft(sn, '0'));
                }
                else
                {
                    rk = rk.Replace(m.Value, _shotNum.ToString());
                }

                _shotNum ++;
            }

            string oname = g.name;

            Match r = _nOgRegex.Match(renameKey);
            if (r.Success)
            {
                string non = oname;

                string ty;

                if (r.Value.Contains("?"))
                {
                    ty = r.Value.Substring(3, r.Value.Length - 4);

                    string fk;
                    string rkz = "";

                    if (ty.Contains(":"))
                    {
                        string[] ss = ty.Split(':');
                        fk = ss[0];
                        rkz = ss[1];

                    }
                    else
                    {
                        fk = ty;
                    }

                    if (non.Contains(fk))
                    {
                        non = non.Replace(fk, rkz);
                    }

                }

                rk = rk.Replace(r.Value, non);
            }

            g.name = rk;
            int ins = g.GetInstanceID();
            if (!instanceList.Contains(ins))
            {
                resultInfoList.Add(oname + " > " + rk);
                instanceList.Add(ins);
            }

            EditorUtility.SetDirty(g);
        }

    }
}
