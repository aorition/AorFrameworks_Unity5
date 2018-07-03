using System;
using System.Collections;
using System.Collections.Generic;
using AorBaseUtility;
using AorBaseUtility.MiniJSON;
using UnityEngine;

namespace Framework.NodeGraph
{
    public class NodeGraphLagDefind
    {
        public enum NodeGraphLagTag : int
        {
            EN = 0,
            CH,
        }

        public static NodeGraphLagTag LaguageTag = NodeGraphLagTag.CH;

        protected static Dictionary<string, List<string>> _parseLagJson(string json)
        {
            Dictionary<string, List<string>> labelsDic = new Dictionary<string, List<string>>();

            Dictionary<string, object> dic = Json.DecodeToDic(json);

            foreach (string key in dic.Keys)
            {
                if (!labelsDic.ContainsKey(key))
                {
                    if (dic[key] is IList)
                    {
                        List<string> vlist = new List<string>();
                        IList list = dic[key] as IList;
                        for (int i = 0; i < list.Count; i++)
                        {
                            string v = list[i].ToString();
                            vlist.Add(v);
                        }
                        labelsDic.Add(key, vlist);
                    }
                    else
                    {
                        Debug.LogError("*** NodeGraphLagDefind.Init labelsDic Error :: value is a type of error > " + key + " : " + dic[key] + "]");
                    }

                }
                else
                {
                    Debug.LogError("*** NodeGraphLagDefind.Init labelsDic Error :: On Same Key > " + key + " : value 1 : [" + labelsDic[key] + "],value 2 : [" + dic[key] + "]");
                }
            }

            if (labelsDic.Count > 0)
            {
                return labelsDic;
            }
            return null;
        }

        private static Dictionary<string, List<string>> _labelsDic;
        protected static Dictionary<string, List<string>> labelsDic
        {
            get
            {
                if (_labelsDic == null)
                {
                    //初始化
                    string json = AorIO.ReadStringFormFile(NodeGraphDefind.RESOURCE_LAGJSON);
                    if (!string.IsNullOrEmpty(json))
                    {
                        _labelsDic = _parseLagJson(json);
                    }
                }
                return _labelsDic;
            }
        }

        public static string Get(string key)
        {
            if (labelsDic == null) return key;
            if (labelsDic.ContainsKey(key))
            {
                return _labelsDic[key][(int) LaguageTag];
            }
            else
            {
                return "NonDefineLabel";
            }
        }
    }
}
