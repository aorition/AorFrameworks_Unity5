using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AorBaseUtility.Extends;
using NodeGraph.SupportLib;
using UnityEditor;
using UnityEngine;

namespace Framework.NodeGraph
{
    public class CheckReferenceController : NodeController
    {
        public override void update(bool updateParentLoop = true)
        {
            //获取上级节点数据
            ConnectionPointGUI cpg = NodeGraphBase.Instance.GetConnectionPointGui(m_nodeGUI.id, 100, ConnectionPointInoutType.MutiInput);
            List<ConnectionGUI> clist = NodeGraphBase.Instance.GetContainsConnectionGUI(cpg);
            if (clist != null)
            {

                List<string> parentData = new List<string>();

                int i, len = clist.Count;
                for (i = 0; i < len; i++)
                {
                    string[] pd = (string[])clist[i].GetConnectionValue(updateParentLoop);
                    if (pd != null)
                    {
                        //去重复
                        for (int a = 0; a < pd.Length; a++)
                        {
                            if (!parentData.Contains(pd[a]))
                            {
                                parentData.Add(pd[a]);

                            }
                        }
                    }
                }


                int _actionid = (int)m_nodeGUI.data.ref_GetField_Inst_Public("ActionID");
                if (_actionid == 1)
                {
                    ActionOne(parentData);
                }


                if(_actionid ==2)
                {
                    ActionTwo(parentData);
                }
                if (_actionid == 3)
                {
                    ActionThree(parentData);
                }
                if (_actionid == 4)
                {
                    ActionFour(parentData);
                }


                NodeGraphBase.TimeInterval_Request_SAVESHOTCUTGRAPH = true; //申请延迟保存快照
                base.update(updateParentLoop);
            }
        }
        private void ActionOne(List<string> parentData)
        {
            //获取指定文件夹资源
            string[] _searchPath = (string[])m_nodeGUI.data.ref_GetField_Inst_Public("SearchPath");
            List<string> resourcePath = new List<string>();
            for (int q = 0; q < _searchPath.Length; q++)
            {
                if (_searchPath[q].Contains("."))
                {

                    resourcePath.Add(_searchPath[q]);
                }
                else
                {
                    string[] _st = Directory.GetFiles(_searchPath[q], "*.*", SearchOption.AllDirectories);
                    for (int j = 0; j < _st.Length; j++)
                    {
                        _st[j].Replace("\\", "/");
                        _st[j].Replace(Application.dataPath + "/", "Assets/");
                        resourcePath.Add(_st[j]);
                    }
                }
            }

            //查找资源依赖
            if (resourcePath != null && resourcePath.Count > 0 && parentData != null && parentData.Capacity > 0)
            {
                Dictionary<string, List<string>> _dependence = new Dictionary<string, List<string>>();
                Dictionary<string, List<string>> _reDependence = new Dictionary<string, List<string>>();
                for (int j = 0; j < resourcePath.Count; j++)
                {

                    string[] _dep = AssetDatabase.GetDependencies(resourcePath[j]);
                    string _self=null ;
                  
                   
                    if (_dep != null && _dep.Length > 0)
                    {
                       
                        for (int k = 0; k < _dep.Length; k++)
                        {
                          
                            UnityEngine.Object _d = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(_dep[k]);
                            UnityEngine.Object _r = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(resourcePath[j]);
                            
                            if (_d.Equals(_r))
                            {
                                _self = _dep[k];
                            }

                        }
                        if (_self!=null && _dep.Contains<string>(_self ))
                        {
                            List<string> _st = _dep.ToList();
                            _st.Remove(_self );
                            _dep = _st.ToArray();
                        }
                     
                        _dependence.Add(resourcePath[j], _dep.ToList());


                    }
                }

                for (int j = 0; j < parentData.Count; j++)
                {
                    foreach (KeyValuePair<string, List<string>> pair in _dependence)
                    {


                        if (pair.Value.Exists(x => { return (x == parentData[j]) || (AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(x) == AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(parentData[j])); }))
                        {
                            if (_reDependence.ContainsKey(parentData[j]))
                            {
                                List<string> result = new List<string>();
                                if (_reDependence.TryGetValue(parentData[j], out result))
                                {
                                    result.Add(pair.Key);
                                }
                                else
                                {
                                    Debug.LogError("字典value为空");
                                }
                            }
                            else
                            {
                                List<string> result = new List<string>();
                                result.Add(pair.Key);
                                _reDependence.Add(parentData[j], result);
                            }
                        }

                    }
                }


                if (_reDependence != null && _reDependence.Count > 0)
                {
                    m_nodeGUI.data.ref_SetField_Inst_Public("Redependence", _reDependence);
                }
                else
                {
                    m_nodeGUI.data.ref_SetField_Inst_Public("Redependence", null);
                }
            }
        }

        private void ActionTwo(List<string> parentData)
        {
           
            if(parentData  !=null &&parentData .Count  >0)
            {
                Dictionary<string, List<string>> _refence_dic = new Dictionary<string, List<string>>();
                for(int i=0;i< parentData.Count ;i++)
                {
                    if(_refence_dic.ContainsKey  (parentData[i ]))
                    {
                        List<string> _value = new List<string>();
                        if (_refence_dic .TryGetValue (parentData[i],out  _value))
                        {
                          string []_st=  AssetDatabase.GetDependencies(parentData[i]);
                            if(_st !=null &&_st .Length >0)
                            {
                                for (int j = 0; j < _st.Length; j++)
                                {
                                    _value.Add(_st[i]);
                                }
                               
                            }
                        }
                        else
                        {
                            
                            Debug.LogError("value为空");
                        }
                    }
                    else
                    {
                        _refence_dic.Add(parentData[i], AssetDatabase.GetDependencies(parentData[i]).ToList<string>());
                      
                    }


                }
                if(_refence_dic !=null &&_refence_dic .Count >0)
                {
                   
                    m_nodeGUI.data.ref_SetField_Inst_Public("Redependence_Down", _refence_dic);
                }
            }

            
        }

        private void ActionThree(List<string> parentData)
        {

            //获取指定文件夹资源
            string[] _searchPath = (string[])m_nodeGUI.data.ref_GetField_Inst_Public("SearchPath");
            List<string> resourcePath = new List<string>();
            for (int q = 0; q < _searchPath.Length; q++)
            {
                if (_searchPath[q].Contains("."))
                {

                    resourcePath.Add(_searchPath[q]);
                }
                else
                {
                    string[] _st = Directory.GetFiles(_searchPath[q], "*.*", SearchOption.AllDirectories);
                    for (int j = 0; j < _st.Length; j++)
                    {
                        _st[j].Replace("\\", "/");
                        _st[j].Replace(Application.dataPath + "/", "Assets/");
                        resourcePath.Add(_st[j]);
                    }
                }
            }
           string[]  _assetPaths = resourcePath.ToArray();
     
            Dictionary<string, List<string>> _crossDic = new Dictionary<string, List <string>>();
             if (parentData != null && parentData.Count > 0)
            {
                for (int i = 0; i < parentData.Count; i++)
                {
                    List<string> tops = new List<string>();
                    findTop(parentData[i], ref tops, ref _assetPaths);
                    if(tops !=null &&tops.Count >0 )
                    {
                       
                        _crossDic.Add(parentData[i], tops);
                    }
                    
                }
                if (_crossDic !=null &&_crossDic.Count >0)
                {
                    m_nodeGUI.data.ref_SetField_Inst_Public("Redependence_Cross", _crossDic);
                }
            }


        }

        private void ActionFour(List<string> parentData)
        {
            //获取指定文件夹资源
            string[] _searchPath = (string[])m_nodeGUI.data.ref_GetField_Inst_Public("SearchPath");
            List<string> resourcePath = new List<string>();
            for (int q = 0; q < _searchPath.Length; q++)
            {
                if (_searchPath[q].Contains("."))
                {

                    resourcePath.Add(_searchPath[q]);
                }
                else
                {
                    string[] _st = Directory.GetFiles(_searchPath[q], "*.*", SearchOption.AllDirectories);
                    for (int j = 0; j < _st.Length; j++)
                    {
                        _st[j].Replace("\\", "/");
                        _st[j].Replace(Application.dataPath + "/", "Assets/");
                        resourcePath.Add(_st[j]);
                    }
                }
            }
            string[] _assetPaths = resourcePath.ToArray();

            Dictionary<string, List<string>> _dependenceDic = new Dictionary<string, List<string>>();
            for (int i=0;i<_assetPaths.Length;i++)
            {
                List<string> _st = AssetDatabase.GetDependencies(_assetPaths[i]).ToList ();
                if (!_dependenceDic.ContainsKey (_assetPaths [i ]))
                {
                    _dependenceDic.Add(_assetPaths[i], _st);
                }
            }
            Dictionary<string, List<string>> _referenceNoneDic = new Dictionary<string, List<string>>();
            for(int i=0;i<parentData.Count;i++)
            {
                foreach (KeyValuePair <string ,List <string >> dic in _dependenceDic )
                {
                    if(!dic.Value .Contains (parentData [i]))
                    {
                        if(_referenceNoneDic.ContainsKey (parentData [i]))
                        {
                            List<string> _value;
                            if(_referenceNoneDic .TryGetValue (parentData [i],out _value ))
                            {
                                _value.Add(dic.Key);
                            }
                        }
                        else
                        {
                            List<string> _value1=new List<string> ();
                            _value1.Add(dic.Key);
                            _referenceNoneDic.Add(parentData[i], _value1);
                        }
                    }
                }
            }


            if (_referenceNoneDic !=null &&_referenceNoneDic .Count >0)
            {
                m_nodeGUI.data.ref_SetField_Inst_Public("Redependence_None", _referenceNoneDic);
            }

          


        }

        private static void findTop(string kvInfo, ref List<string> tops, ref string[] allExportAssets)
        {


            string lastTop;
            int i, len = allExportAssets.Length;

            for (i = 0; i < len; i++)
            {

                if (allExportAssets[i] == kvInfo)
                    continue;

                string[] deps = AssetDatabase.GetDependencies(allExportAssets[i]);

                if (deps.Length <= 1)
                    continue;


                //如果子节点包含它
                if (deps.Contains(kvInfo))
                {
                    lastTop = allExportAssets[i];

                    if (new EditorAssetInfo(lastTop).suffix != "prefab")
                    {
                        //继续递归找prefab顶
                        findTop(lastTop, ref tops, ref allExportAssets);
                    }
                    else
                    {

                        tops.Add(lastTop);
                    }

                }

               

            }
        }


    }
   
}



   
