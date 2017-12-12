using System;
using System.Text;
using System.Collections.Generic;
using AorFramework.AorTile3D.runtime;
using UnityEngine;

namespace AorFramework.AorTile3D.runtimeEditor
{
    public class TileMapDataUtils
    {
        private static StringBuilder _buff;
        private static void _resetStringBuilder()
        {
            if (_buff == null)
            {
                _buff = new StringBuilder();
            }
            else
            {
                _buff.Length = 0;
            }
        }
        
        public static string ExprotTileDataInfoToText(AorTile3DScene scene)
        {
            _resetStringBuilder();
            //

            //borderCenter > 0
            int[] bc = scene.borderCenter;
            _buff.Append(bc[0] + "+" + bc[1] + "+" + bc[2] + "|");
            //borderHalfSize > 1
            int[] bhs = scene.borderHalfSize;
            _buff.Append(bhs[0] + "+" + bhs[1] + "+" + bhs[2]);

            return _buff.ToString();
        }

        public static AorTile3DScene CreateAorTile3DSceneWithText(string InfoStr, TileMapData tmapData)
        {
            string[] strSpList = InfoStr.Split('|');
            if (strSpList.Length < 2) return null;

            //borderCenter > 0
            int[] bc = new int[3];
            string[] bcSps = strSpList[0].Split('+');
            bc[0] = int.Parse(bcSps[0]);
            bc[1] = int.Parse(bcSps[1]);
            bc[2] = int.Parse(bcSps[2]);

            //borderHalfSize > 1
            int[] bhs = new int[3];
            string[] bhsSps = strSpList[1].Split('+');
            bhs[0] = int.Parse(bhsSps[0]);
            bhs[1] = int.Parse(bhsSps[1]);
            bhs[2] = int.Parse(bhsSps[2]);

            AorTile3DScene scene = new AorTile3DScene(tmapData, bc, bhs);
            return scene;
        }

        public static AorTile3DScene CreateAorTile3DSceneWithText(string tmapdataInfoStr, string tmapDataStr)
        {
            TileMapData tmpData = CreateTileDataFormText(tmapDataStr);
            if (tmpData != null)
            {
                return CreateAorTile3DSceneWithText(tmapdataInfoStr, tmpData);
            }
            return null;
        }

        public static string ExportTileDataToText(TileMapData data)
        {
            _resetStringBuilder();

            int i;
            int[] tileDataLength = data.tileDataLength;
            //tileSize > 0
            _buff.Append(data.tileSize[0] + "+" + data.tileSize[1] + "+" + data.tileSize[2] + "|");
            //tileDataLength > 1
            _buff.Append(tileDataLength[0] + "+" + tileDataLength[1] + "|");
            //_tileModelPath > 2
            Dictionary<ulong, string> mpDic = (Dictionary<ulong, string>)data.ref_GetField_Inst_NonPublic("_tileModelPath");
            i = 0;
            foreach (ulong key in mpDic.Keys)
            {
                if (i > 0)
                {
                    _buff.Append(",");
                }
                _buff.Append(key + "+" + mpDic[key]);
            }
            _buff.Append("|");
            //_addOneModelPath > 3
            Dictionary<ulong, string> ompDic = (Dictionary<ulong, string>)data.ref_GetField_Inst_NonPublic("_addOneModelPath");
            i = 0;
            foreach (ulong key in ompDic.Keys)
            {
                if (i > 0)
                {
                    _buff.Append(",");
                }
                _buff.Append(key + "+" + mpDic[key]);
            }
            _buff.Append("|");
            //_addOneDatas > 4
            Dictionary<ulong, TileAddOneData> taoDic = (Dictionary<ulong, TileAddOneData>)data.ref_GetField_Inst_NonPublic("_addOneDatas");
            i = 0;
            foreach (ulong key in taoDic.Keys)
            {
                if (i > 0)
                {
                    _buff.Append(",");
                }
                TileAddOneData taod = taoDic[key];
                _buff.Append(key
                    + "+" + taod.modelId
                    + "+" + taod.scale[0] + "*" + taod.scale[1] + "*" + taod.scale[2]
                    + "+" + taod.offset[0] + "*" + taod.offset[1] + "*" + taod.offset[2]
                    + "+" + taod.rotate[0] + "*" + taod.rotate[1] + "*" + taod.rotate[2]
                    );
            }
            _buff.Append("|");
            //_tileDatas > 5
            i = 0;
            int uMax = tileDataLength[0];
            int vMax = tileDataLength[1];
            TileData td;
            for (int u = 0; u < uMax; u++)
            {
                for (int v = 0; v < vMax; v++)
                {
                    if (i > 0)
                    {
                        _buff.Append(",");
                    }
                    td = data.GetTileData(u, v);
                    if (td != null)
                    {

                        string addOnesIdStr = string.Empty;
                        ulong[] addOnesId = td.addOnesId;
                        if (addOnesId != null && addOnesId.Length > 0)
                        {
                            for (int j = 0; j < addOnesId.Length; j++)
                            {
                                if (j > 0)
                                {
                                    addOnesIdStr += "*";
                                }
                                addOnesIdStr += addOnesId[j];
                            }
                        }

                        _buff.Append(td.modelId
                            + "+" + (int)td.orient
                            + "+" + addOnesIdStr
                            + "+" + td.height
                            );
                    }
                    else
                    {
                        _buff.Append("null");
                    }
                    i++;
                }
            }

            return _buff.ToString();
        }

        public static TileMapData CreateTileDataFormText(string text)
        {
            string[] strSpList = text.Split('|');

            if (strSpList.Length < 6) return null;
            //
            
            _resetStringBuilder();
            
            //tileSize > 0
            float[] ts = new float[3];
            string[] tsSps = strSpList[0].Split('+');
            ts[0] = float.Parse(tsSps[0]);
            ts[1] = float.Parse(tsSps[1]);
            ts[2] = float.Parse(tsSps[2]);
            //tileDataLength > 1
            int[] ls = new int[2];
            string[] lsSps = strSpList[1].Split('+');
            ls[0] = int.Parse(lsSps[0]);
            ls[1] = int.Parse(lsSps[1]);

            TileMapData tmData = new TileMapData(ts[0], ts[1], ts[2], ls[0], ls[1]);

            //_tileModelPath > 2
            Dictionary<ulong, string> mpDic = new Dictionary<ulong, string>();
            string[] mpDicList = strSpList[2].Split(',');
            int i, len = mpDicList.Length;
            for (i = 0; i < len; i++)
            {

                if (string.IsNullOrEmpty(mpDicList[i])) continue;

                string[] mpItemSps = mpDicList[i].Split('+');
                ulong k = ulong.Parse(mpItemSps[0]);
                string value = mpItemSps[1];
                mpDic.Add(k, value);
            }
            tmData.ref_SetField_Inst_NonPublic("_tileModelPath", mpDic);

            //_addOneModelPath > 3
            Dictionary<ulong, string> ompDic = new Dictionary<ulong, string>();
            string[] ompDicList = strSpList[3].Split(',');
            len = ompDicList.Length;
            for (i = 0; i < len; i++)
            {
                if(string.IsNullOrEmpty(ompDicList[i])) continue;
                string[] ompItemList = ompDicList[i].Split('+');
                ulong k = ulong.Parse(ompItemList[0]);
                string value = ompItemList[1];
                ompDic.Add(k, value);

            }
            tmData.ref_SetField_Inst_NonPublic("_addOneModelPath", ompDic);

            //_addOneDatas > 4
            Dictionary<ulong, TileAddOneData> taoDic = new Dictionary<ulong, TileAddOneData>();
            string[] taoDicList = strSpList[4].Split(',');
            len = taoDicList.Length;
            for (i = 0; i < len; i++)
            {

                if (string.IsNullOrEmpty(taoDicList[i]) || taoDicList[i].ToLower() == "null") continue;

                string[] taoItemSps = taoDicList[i].Split('+');
                ulong k = ulong.Parse(taoItemSps[0]);

                ulong mid = ulong.Parse(taoItemSps[1]);

                string[] sSps = taoItemSps[2].Split('*');
                float[] s = new float[3];
                s[0] = float.Parse(sSps[0]); 
                s[1] = float.Parse(sSps[1]); 
                s[2] = float.Parse(sSps[2]); 

                string[] oSps = taoItemSps[3].Split('*');
                float[] o = new float[3];
                o[0] = float.Parse(oSps[0]);
                o[1] = float.Parse(oSps[1]);
                o[2] = float.Parse(oSps[2]);

                string[] rSps = taoItemSps[4].Split('*');
                float[] r = new float[3];
                r[0] = float.Parse(rSps[0]);
                r[1] = float.Parse(rSps[1]);
                r[2] = float.Parse(rSps[2]);

                TileAddOneData taoData = new TileAddOneData(mid, s, o, r);
                taoDic.Add(k, taoData);
            }
            tmData.ref_SetField_Inst_NonPublic("_addOneDatas", taoDic);

            //_tileDatas > 5
            int uMax = ls[0];
            int vMax = ls[1];
            TileData[][] tileDatas = new TileData[uMax][];
            len = uMax;
            for (i = 0; i < len; i++)
            {
                tileDatas[i] = new TileData[vMax];
            }
            //
            string[] tdList = strSpList[5].Split(',');
            len = tdList.Length;
            int u, v;
            for (i = 0; i < len; i++)
            {
                v = i/uMax;
                u = i%uMax;

                if (string.IsNullOrEmpty(tdList[i]) || tdList[i].ToLower() == "null") continue;

                string[] tdItemSps = tdList[i].Split('+');

                ulong mid = ulong.Parse(tdItemSps[0]);
                TileOrientation tot = (TileOrientation) int.Parse(tdItemSps[1]);
                ulong[] adUlongs;
                string[] adStrSps = tdItemSps[2].Split('*');
                if (adStrSps.Length > 0 && !string.IsNullOrEmpty(adStrSps[0]))
                {
                    adUlongs = new ulong[adStrSps.Length];
                    for (int j = 0; j < adStrSps.Length; j++)
                    {
                        adUlongs[j] = ulong.Parse(adStrSps[j]);
                    }
                }
                else
                {
                    adUlongs = null;
                }
                int h = int.Parse(tdItemSps[3]);
                tileDatas[u][v] = new TileData(mid, tot, adUlongs, h);
            }
            tmData.ref_SetField_Inst_NonPublic("_tileDatas", tileDatas);

            return tmData;
        }

        public static string ExportTileDataToJSON(TileMapData data)
        {
            _resetStringBuilder();

            int i;
            int[] tileDataLength = data.tileDataLength;
            _buff.Append("{");
            _buff.Append("\"_tileSize\":[" + data.tileSize[0] + "," + data.tileSize[1] + "," + data.tileSize[2] + "]");
            _buff.Append(",\"_tileDataLength\":[" + tileDataLength[0] + "," + tileDataLength[1] + "]");
            //_tileModelPath
            _buff.Append(",\"_tileModelPath\":[");
            Dictionary<ulong, string> mpDic = (Dictionary < ulong, string>) data.ref_GetField_Inst_NonPublic("_tileModelPath");
            i = 0;
            foreach (ulong key in mpDic.Keys)
            {
                if (i > 0)
                {
                    _buff.Append(",");
                }
                _buff.Append("{\"id:\"" + key + ",\"path\":\"" + mpDic[key] + "\"}");
            }
            //_addOneModelPath
            _buff.Append("],\"_addOneModelPath\":[");
            Dictionary<ulong, string> ompDic = (Dictionary<ulong, string>)data.ref_GetField_Inst_NonPublic("_addOneModelPath");
            i = 0;
            foreach (ulong key in ompDic.Keys)
            {
                if (i > 0)
                {
                    _buff.Append(",");
                }
                _buff.Append("{\"id:\"" + key + ",\"path\":\"" + mpDic[key] + "\"}");
            }
            //_addOneDatas
            _buff.Append("],\"_addOneDatas\":[");
            Dictionary<ulong, TileAddOneData> taoDic = (Dictionary<ulong, TileAddOneData>)data.ref_GetField_Inst_NonPublic("_addOneDatas");
            i = 0;
            foreach (ulong key in taoDic.Keys)
            {
                if (i > 0)
                {
                    _buff.Append(",");
                }
                TileAddOneData taod = taoDic[key];
                _buff.Append("{\"id:\"" + key 
                    + ",\"data\":{\"_modelId\":" + taod.modelId
                    + ",\"_scale\":[" + taod.scale[0] + "," + taod.scale[1] + "," + taod.scale[2] + "]"
                    + ",\"_offset\":[" + taod.offset[0] + "," + taod.offset[1] + "," + taod.offset[2] + "]"
                    + ",\"_rotate\":[" + taod.rotate[0] + "," + taod.rotate[1] + "," + taod.rotate[2] + "]"
                    + "}"
                    );
            }
            //
            _buff.Append("],\"_tileDatas\":[");
            i = 0;
            int uMax = tileDataLength[0];
            int vMax = tileDataLength[1];
            TileData td;
            for (int u = 0; u < uMax; u++)
            {
                for (int v = 0; v < vMax; v++)
                {
                    if (i > 0)
                    {
                        _buff.Append(",");
                    }
                    td = data.GetTileData(u, v);
                    if (td != null)
                    {

                        string addOnesIdStr = string.Empty;
                        ulong[] addOnesId = td.addOnesId;
                        if (addOnesId != null && addOnesId.Length > 0)
                        {
                            for (int j = 0; j < addOnesId.Length; j++)
                            {
                                if (j > 0)
                                {
                                    addOnesIdStr += ",";
                                }
                                addOnesIdStr += addOnesId[j];
                            }
                        }

                        _buff.Append("{\"_modelId:\"" + td.modelId
                            + ",\"_orient\":" + (int)td.orient
                            + ",\"_addOnesId\":[" + addOnesIdStr
                            + "],\"_height\":" + td.height
                            + "}"
                            );
                    }
                    else
                    {
                        _buff.Append("null");
                    }
                    i++;
                }
            }
            _buff.Append("]}");

            return _buff.ToString();
        }

        public static TileMapData CreateTileDataFormJSON(string json)
        {
            return new TileMapData(1, 1, 1, 1, 1);
        }

    }
}
