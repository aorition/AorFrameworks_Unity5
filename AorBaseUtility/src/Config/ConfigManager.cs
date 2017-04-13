using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using System;
using System.Text;
using System.Text.RegularExpressions;
using AorBaseUtility;

namespace AorFramework.core
{
    public delegate void ConfigDataCallBack(long id, string fieldname, ref string data);
    /// <summary>
    /// 配置管理类
    /// </summary>
    public class ConfigManager
    {
        //定义配置管理表
        private Dictionary<string, Dictionary<long, Dictionary<string, TConfig>>> _dicData = new Dictionary<string, Dictionary<long, Dictionary<string, TConfig>>>();
        //定义配置文件数据表
        private Dictionary<string, Dictionary<long, FileLineInfo>> _dicFileData = new Dictionary<string, Dictionary<long, FileLineInfo>>();
        //定义配置文件字段数据表
        private Dictionary<string, string[][]> _dicFileFieldInfo = new Dictionary<string, string[][]>();
        private ConfigManager() { }

        #region 外部调用

        private static bool _init = false;

        /// <summary>
        /// 是否打印错误提示
        /// </summary>
        public static bool IsDebug = true;
        /// <summary>
        /// 获取实例
        /// </summary>
        /// <returns></returns>
        private static ConfigManager _instance;

        private static object _iLock = new object();

        public static ConfigManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_iLock)
                    {
                        if (_instance == null)
                        {
                            _instance = new ConfigManager();
                        }
                    }
                }
                if (!_init)
                {
                    _init = true;
                }
                return _instance;
            }
        }

        /// <summary>
        /// 重置方法
        /// </summary>
        public static void Reset()
        {
            _init = false;
        }

        /// <summary>
        /// 遗弃实例
        /// ##注意: 遗弃实例后,调用Instance获取实例将返回null,如需创建新的实例,请在调用Instance之前,执行Reset重置方法.
        /// </summary>
        public void Destroy()
        {
            if (_dicData != null)
            {
                _dicData.Clear();
                _dicData = null;
            }
            _instance = null;
        }

        /// <summary>
        /// 导入配置数据
        /// </summary>
        /// <param name="strInfo"></param>
        public void ImportStrInfo(Type type, string strInfo, Type derivedType = null, ConfigDataCallBack callBack = null)
        {
            if (!IsConfigClass(type))
            {
                Debug("导入的配置类型不符合要求，type name = " + type.Name);
                return;
            }
            lock (_dicData)
            {
                if (!_dicData.ContainsKey(type.Name))
                    _dicData.Add(type.Name, new Dictionary<long, Dictionary<string, TConfig>>());
                if (_dicFileData.ContainsKey(type.Name) && derivedType == null)
                    return;

                string[] lineArr = strInfo.Split(new string[] { "\r\n" },
                        StringSplitOptions.RemoveEmptyEntries);
                //导入文件字段数据
                string[][] tempArr2 = new string[3][];
                for (int i = 0; i < 3; i++)
                {
                    tempArr2[i] = lineArr[i].Split('\t');
                }

                if (derivedType != null)
                {
                    //检查数据格式
                    if (!CheckConfig(derivedType, tempArr2[2], tempArr2[0]))
                        return;
                    if (!_dicFileFieldInfo.ContainsKey(derivedType.Name))
                        _dicFileFieldInfo.Add(derivedType.Name, tempArr2);
                }
                else
                {
                    //检查数据格式
                    if (!CheckConfig(type, tempArr2[2], tempArr2[0]))
                        return;
                    if (!_dicFileFieldInfo.ContainsKey(type.Name))
                        _dicFileFieldInfo.Add(type.Name, tempArr2);
                }

                //导入文件配置数据
                if (derivedType != null && _dicFileData.ContainsKey(type.Name))
                {
                    for (int i = 3; i < lineArr.Length; i++)
                    {
                        string str = lineArr[i].Contains("\t")
                            ? lineArr[i].Substring(0, lineArr[i].IndexOf("\t"))
                            : lineArr[i];
                        if (str.EndsWith("@"))
                            str = str.Remove(str.Length - 1);
                        long n;
                        if (long.TryParse(str, out n))
                        {
                            n = long.Parse(str);
                            FileLineInfo tempfi;
                            tempfi.index = i;
                            tempfi.LineData = lineArr[i].Split('\t');
                            for (int j = 0; j < tempfi.LineData.Length; j++)
                            {
                                if (callBack != null)
                                {
                                    callBack(i, _dicFileFieldInfo[derivedType.Name][2][j], ref tempfi.LineData[j]);
                                }
                            }
                            tempfi.derivedType = derivedType;
                            if (!_dicFileData[type.Name].ContainsKey(n))
                                _dicFileData[type.Name].Add(n, tempfi);
                            else
                            {
                                Debug("配置表" + derivedType.Name + "导入配置数据id错误（第" + (i + 1) + "行 , 第1列） ： 数据id（" + str +
                                        "）已导入，导入此配置数据失败!");
                            }
                        }
                        else
                        {
                            Debug("配置表" + type.Name + "导入配置数据id错误（第" + (i + 1) + "行 , 第1列） ： 数据id号（" + str +
                                        "）的格式不符合要求，导入此配置数据失败!");
                        }
                    }
                }
                else
                {
                    Dictionary<long, FileLineInfo> tempDic = new Dictionary<long, FileLineInfo>();
                    for (int i = 3; i < lineArr.Length; i++)
                    {
                        string str = lineArr[i].Contains("\t")
                            ? lineArr[i].Substring(0, lineArr[i].IndexOf("\t"))
                            : lineArr[i];
                        if (str.EndsWith("@"))
                            str = str.Remove(str.Length - 1);
                        long n;
                        if (long.TryParse(str, out n))
                        {
                            n = long.Parse(str);
                            FileLineInfo tempfi;
                            tempfi.index = i;
                            tempfi.LineData = lineArr[i].Split('\t');
                            for (int j = 0; j < tempfi.LineData.Length; j++)
                            {
                                if (callBack != null)
                                {
                                    callBack(i, _dicFileFieldInfo[type.Name][2][j], ref tempfi.LineData[j]);
                                }
                            }
                            tempfi.derivedType = derivedType;
                            if (!tempDic.ContainsKey(n))
                                tempDic.Add(n, tempfi);
                            else
                            {
                                Debug("配置表" + type.Name + "导入配置数据id错误（第" + (i + 1) + "行 , 第1列） ： 数据id（" + str +
                                        "）已导入，导入此配置数据失败!");
                            }
                        }
                        else
                        {
                            Debug("配置表" + type.Name + "导入配置数据id错误（第" + (i + 1) + "行 , 第1列） ： 数据id号（" + str +
                                        "）的格式不符合要求，导入此配置数据失败!");
                        }
                    }
                    _dicFileData.Add(type.Name, tempDic);
                }
            }
        }

        /// <summary>
        /// 导入配置数据(泛型)
        /// </summary>
        /// <param name="strInfo"></param>
        public void ImportStrInfo<T>(string strInfo, ConfigDataCallBack callBack = null) where T : TConfig
        {
            try
            {
                ImportStrInfo(typeof (T), strInfo, null, callBack);
            }
            catch (Exception)
            {
                Debug("导入配置表出错:" + "类型:" + typeof(T));
                throw;
            }
        }

        /// <summary>
        /// 导入配置数据(泛型)
        /// T1是T2的基类，解析时会将T2的数据放入到T1所在的字典中 
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="strInfo"></param>
        public void ImportStrInfo<T1, T2>(string strInfo, ConfigDataCallBack callBack = null)
            where T1 : TConfig
            where T2 : T1
        {
            try
            {
                ImportStrInfo(typeof(T1), strInfo, typeof(T2), callBack);
            }
            catch (Exception)
            {
                Debug("导入配置表出错:" + "类型:" + typeof(T2));
                throw;
            }
        }

        public bool DebugMode = false;

        /// <summary>
        /// 获取配置表集合
        /// </summary>
        /// <param name="configName"></param>
        /// <returns></returns>
        public TConfig[] GetConfigDic(Type type)
        {
            if (!IsExistDic(type.Name)) return null;
            List<TConfig> dicList = new List<TConfig>();
            Dictionary<long, FileLineInfo> targetDic = _dicFileData[type.Name];
            foreach (KeyValuePair<long, FileLineInfo> fileLineInfo in targetDic)
            {
                dicList.Add(_GetConfigFromDic(type, fileLineInfo.Key));
            }
            return dicList.ToArray();
        }

        /// <summary>
        /// 获取配置表集合(泛型)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T[] GetConfigDic<T>() where T : TConfig
        {
            Type t = typeof(T);
            if (!IsExistDic(t.Name)) return null;
            List<T> dicList = new List<T>();
            Dictionary<long, FileLineInfo> targetDic = _dicFileData[t.Name];
            foreach (KeyValuePair<long, FileLineInfo> fileLineInfo in targetDic)
            {
                dicList.Add(GetConfigFromDic<T>(fileLineInfo.Key));
            }
            return dicList.ToArray();
        }

        /// <summary>
        /// 清除配置表集合(泛型) 慎用!!!
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool ClearConfigDic<T>()
        {
            if (IsExistDic(typeof(T).Name))
            {
                lock (_dicData)
                {
                    _dicData[typeof(T).Name].Clear();
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        /// 获取某个配置表的配置
        /// </summary>
        /// <param name="configName"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public TConfig GetConfigFromDic(Type type, long id, params int[] ex)
        {
            try
            {
                string cfgkey = IndexsChangeToConfigKey(ex);
                StrInfoChangeToDic(type, id, ex);
                return IsExistConfigFromDic(type.Name, id, cfgkey) ? _dicData[type.Name][id][cfgkey] : null;
            }
            catch (Exception e)
            {
                Debug(type.Name + " id: " + id + " parse error! Because " + e);
                return null;
            }
        }

        private TConfig _GetConfigFromDic(Type type, long id, params int[] ex)
        {
            try
            {
                string cfgkey = IndexsChangeToConfigKey(ex);
                StrInfoChangeToDic(type, id, ex);
                return IsExistConfigFromDic(type.Name, id, cfgkey) ? _dicData[type.Name][id][cfgkey] : null;
            }
            catch (Exception e)
            {
                Debug(type.Name + " id: " + id + " parse error! Because " + e);
                throw;
            }
        }

        /// <summary>
        /// 获取某个配置表的配置(泛型)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public T GetConfigFromDic<T>(long id, params int[] ex) where T : TConfig
        {
            Type t = typeof(T);
            return (T)GetConfigFromDic(t, id, ex);
        }
        /// <summary>
        /// 判断某配置表是否存在(泛型)
        /// </summary>
        /// <param name="configName"></param>
        /// <returns></returns>
        public bool IsExistDic<T>()
        {
            return _dicData.ContainsKey(typeof(T).Name);
        }

        /// <summary>
        /// 判断某配置表中的配置是否存在(泛型)
        /// </summary>
        /// <param name="configName"></param>
        /// <param name="id"></param>
        /// <param name="cfgkey"></param>
        /// <returns></returns>
        public bool IsExistConfigFromDic<T>(long id, params int[] ex)
        {
            string configName = typeof(T).Name;
            string cfgkey = IndexsChangeToConfigKey(ex);
            return _dicData.ContainsKey(configName) && _dicData[configName].ContainsKey(id) && _dicData[configName][id].ContainsKey(cfgkey);
        }

        /// <summary>
        /// 更新配置表数据，若没有导入，则等同于导入配置表数据
        /// </summary>
        /// <param name="type">该类型为TConfig或其派生类</param>
        /// <param name="strInfo"></param>
        public void RefreshStrInfo(Type type, string strInfo)
        {
            string typeName = type.Name;
            if (_dicFileData.ContainsKey(typeName))
            {
                _dicFileData.Remove(typeName);
            }
            if (_dicFileFieldInfo.ContainsKey(typeName))
            {
                _dicFileFieldInfo.Remove(typeName);
            }
            ImportStrInfo(type, strInfo);
            if (_dicData.ContainsKey(typeName))
            {
                List<Dictionary<string, TConfig>> dList = new List<Dictionary<string, TConfig>>(_dicData[typeName].Values);
                List<long> dKeys = new List<long>(_dicData[typeName].Keys);
                for (int i = 0; i < dList.Count; i++)
                {
                    List<string> cfskeys = new List<string>(dList[i].Keys);
                    for (int j = 0; j < cfskeys.Count; j++)
                    {
                        TConfig instance = type.Assembly.CreateInstance(type.FullName) as TConfig;
                        StrInfoChangeToData(_dicFileData[type.Name][dKeys[i]].index,
                            _dicFileData[type.Name][dKeys[i]].LineData, _dicFileFieldInfo[type.Name][2],
                            ref instance, type, ConfigKeyChangeToIndexs(cfskeys[j]));
                        FieldInfo[] refFis = type.GetFields();
                        for (int k = 0; k < refFis.Length; k++)
                        {
                            type.GetField(refFis[k].Name).SetValue(_dicData[typeName][dKeys[i]][cfskeys[j]], type.GetField(refFis[k].Name).GetValue(instance));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 更新配置表数据（泛型），若没有导入，则等同于导入配置表数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="strInfo"></param>
        public void RefreshStrInfo<T>(string strInfo) where T : TConfig
        {
            RefreshStrInfo(typeof(T), strInfo);
        }

        /// <summary>
        /// 更新配置表数据，若没有导入，则等同于导入配置表数据
        /// </summary>
        /// <param name="baseType">该类型为TConfig或其派生类</param>
        /// <param name="derivedType">该类型为baseType的派生类</param>
        /// <param name="strInfo"></param>
        public void RefreshStrInfo(Type baseType, Type derivedType, string strInfo)
        {
            string typeName = baseType.Name;
            string derivedTypeName = derivedType.Name;
            List<long> idList = new List<long>();
            if (_dicFileData.ContainsKey(typeName))
            {
                List<FileLineInfo> fisList = new List<FileLineInfo>(_dicFileData[typeName].Values);
                for (int i = 0; i < fisList.Count; i++)
                {
                    if (fisList[i].derivedType == derivedType)
                    {
                        idList.Add(long.Parse(fisList[i].LineData[0]));
                        _dicFileData[typeName].Remove(long.Parse(fisList[i].LineData[0]));
                    }
                }
            }
            if (_dicFileFieldInfo.ContainsKey(derivedTypeName))
            {
                _dicFileFieldInfo.Remove(derivedTypeName);
            }
            ImportStrInfo(baseType, strInfo, derivedType);
            if (_dicData.ContainsKey(typeName))
            {
                List<Dictionary<string, TConfig>> dList = new List<Dictionary<string, TConfig>>(_dicData[typeName].Values);
                List<long> dKeys = new List<long>(_dicData[typeName].Keys);
                for (int i = 0; i < dList.Count; i++)
                {
                    if (idList.Contains(dKeys[i]))
                    {
                        List<string> cfskeys = new List<string>(dList[i].Keys);
                        for (int j = 0; j < cfskeys.Count; j++)
                        {
                            TConfig instance = derivedType.Assembly.CreateInstance(derivedType.FullName) as TConfig;
                            StrInfoChangeToData(_dicFileData[baseType.Name][dKeys[i]].index,
                                _dicFileData[baseType.Name][dKeys[i]].LineData, _dicFileFieldInfo[derivedType.Name][2],
                                ref instance, derivedType, ConfigKeyChangeToIndexs(cfskeys[j]));
                            FieldInfo[] refFis = derivedType.GetFields();
                            for (int k = 0; k < refFis.Length; k++)
                            {
                                derivedType.GetField(refFis[k].Name).SetValue(_dicData[typeName][dKeys[i]][cfskeys[j]], derivedType.GetField(refFis[k].Name).GetValue(instance));
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 更新配置表数据（泛型），若没有导入，则等同于导入配置表数据
        /// T1是T2的基类，解析时会将T2的数据放入到T1所在的字典中 
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="strInfo"></param>
        public void RefreshStrInfo<T1, T2>(string strInfo)
            where T1 : TConfig
            where T2 : T1
        {
            RefreshStrInfo(typeof(T1), typeof(T2), strInfo);
        }

        /// <summary>
        /// 删除指定TConfig实例
        /// ##注意:该方法用于编辑器管理配置文件时用，后台慎用
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool removeConfigFromDic<T>(long id) where T : TConfig
        {
            lock (_dicData)
            {
                return _dicData.ContainsKey(typeof(T).Name) ? _dicData[typeof(T).Name].Remove(id) : false;
            }
        }

        /// <summary>
        /// 添加TConfig实例到字典
        /// ##注意:该方法用于编辑器管理配置文件时用，后台慎用
        /// </summary>
        /// <param name="c"></param>
        public void addConfigToDic(TConfig c)
        {
            lock (_dicData)
            {
                string configName = c.GetType().Name;
                if (_dicData.ContainsKey(configName))
                {
                    if (_dicData[configName].ContainsKey(c.id))
                        Debug("类名为" + configName + "的配置表中已存在对应的实例！");
                    else
                    {
                        Dictionary<string, TConfig> sc = new Dictionary<string, TConfig>();
                        sc.Add("", c);
                        _dicData[configName].Add(c.id, sc);
                    }
                }
                else
                {
                    Dictionary<long, Dictionary<string, TConfig>> tempDic = new Dictionary<long, Dictionary<string, TConfig>>();
                    Dictionary<string, TConfig> sc = new Dictionary<string, TConfig>();
                    sc.Add("", c);
                    tempDic.Add(c.id, sc);
                    _dicData.Add(configName, tempDic);
                }
            }
        }

        /// <summary>
        /// 将指定TConfig表导成配置文件格式
        /// ## 注意形如（@TConfig:id_ex）格式的数据只会被还原为对应配置数据而无法被还原成原来格式
        /// ## 只能还原不带参数的TConfig信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public string ExportStrInfo<T>() where T : TConfig
        {
            return DicChangeToStrInfo<T>();
        }

        /// <summary>
        /// 获取配置类引用其他配置类的优先数组，优先数组内的Type应该在此配置类之前被导入配置管理器
        /// </summary>
        /// <typeparam name="T">传入类型</typeparam>
        /// <returns>如果该类不是TConfig或是继承TConfig的类，返回null，
        /// 如果该类没有引用其他类的关系，返回Type空数组，
        /// 否则返回一个Type优先数组，引用层次越高，优先级越大，排在前面的配置类应该优先被导入</returns>
        public Type[] GetConfigTypePriorityArray<T>() where T : TConfig
        {
            return GetConfigTypePriorityArray(typeof(T));
        }

        /// <summary>
        /// 获取配置类引用其他配置类的优先数组，优先数组内的Type应该在此配置类之前被导入配置管理器
        /// </summary>
        /// <typeparam name="T">传入类型</typeparam>
        /// <returns>如果该类不是TConfig或是继承TConfig的类，返回null，
        /// 如果该类没有引用其他类的关系，返回Type空数组，
        /// 否则返回一个Type优先数组，引用层次越高，优先级越大，排在前面的配置类应该优先被导入</returns>
        public Type[] GetConfigTypePriorityArray(Type t)
        {
            if (!IsConfigClass(t))
                return null;
            List<Type> refList = GetConfigTypeField(t);
            if (refList.Count == 0)
                return new Type[0];
            Queue<Type> queue = new Queue<Type>();
            List<Type> priorityList = new List<Type>();

            Type[] ttempArr = refList.ToArray();
            for (int i = 0; i < ttempArr.Length; i++)
            {
                queue.Enqueue(ttempArr[i]);
            }
            while (queue.Count > 0)
            {
                Type ttemp = queue.Dequeue();
                priorityList.Add(ttemp);
                ttempArr = GetConfigTypeField(ttemp).ToArray();
                for (int i = 0; i < ttempArr.Length; i++)
                {
                    queue.Enqueue(ttempArr[i]);
                }
            }
            priorityList.Reverse();
            return priorityList.ToArray();
        }

        #endregion

        #region 数据解析和判断

        void Debug(params string[] message)
        {
            if (IsDebug)
            {
                //Todo log
//                Log.Error(message);
            }
        }
        //定义保存在内存中的配置项数据结构
        struct FileLineInfo
        {
            public int index;
            public string[] LineData;
            public Type derivedType;
        }
        //附加数组转换为TConfig键值
        private string IndexsChangeToConfigKey(int[] indexs)
        {
            string skey = "";
            for (int i = 0; i < indexs.Length; i++)
            {
                skey += i != indexs.Length - 1 ? indexs[i] + "_" : indexs[i].ToString();
            }
            return skey;
        }

        //TConfig键值转换为附加数组
        private int[] ConfigKeyChangeToIndexs(string skey)
        {
            if (skey == "")
            {
                return new int[0];
            }
            string[] ss = skey.Split('_');
            int[] ids = new int[ss.Length];
            for (int i = 0; i < ids.Length; i++)
            {
                ids[i] = int.Parse(ss[i]);
            }
            return ids;
        }

        //解析strInfo,将解析好的配置表导入配置管理表 
        private void StrInfoChangeToDic(Type t, long id, params int[] ex)
        {
            string cfgkey = IndexsChangeToConfigKey(ex);
            if (!_dicFileData.ContainsKey(t.Name))
            {
                Debug("无法获取配置！请检查配置表" + t.Name + "是否被导入");
                return;
            }
            if (!_dicFileData[t.Name].ContainsKey(id))
                return;
            if (_dicData[t.Name].ContainsKey(id) && _dicData[t.Name][id].ContainsKey(cfgkey))
                return;

            Type baseType = null;
            //derivedType不为空的特殊处理
            if (_dicFileData[t.Name][id].derivedType != null)
            {
                baseType = t;
                t = _dicFileData[t.Name][id].derivedType;
            }

            TConfig instance = t.Assembly.CreateInstance(t.FullName) as TConfig;

            lock (_dicData)
            {
                Type dicType = baseType == null ? t : baseType;
                if (!_dicData[dicType.Name].ContainsKey(id))
                {
                    Dictionary<string, TConfig> cfgDic = new Dictionary<string, TConfig>();
                    StrInfoChangeToData(_dicFileData[dicType.Name][id].index, _dicFileData[dicType.Name][id].LineData,
                        _dicFileFieldInfo[t.Name][2], ref instance, t, ex);
                    if (instance != null)
                    {
                        cfgDic.Add(cfgkey, instance);
                        _dicData[dicType.Name].Add(instance.id, cfgDic);
                    }
                }
                else
                {
                    if (!_dicData[dicType.Name][id].ContainsKey(cfgkey))
                    {
                        StrInfoChangeToData(_dicFileData[dicType.Name][id].index, _dicFileData[dicType.Name][id].LineData,
                            _dicFileFieldInfo[t.Name][2], ref instance, t, ex);
                        if (instance != null)
                            _dicData[dicType.Name][id].Add(cfgkey, instance);
                    }
                }
            }
        }

        //解析一条配置
        private void StrInfoChangeToData(int line, string[] configInfo, string[] fieldName, ref TConfig instance, Type t, params int[] ex)
        {
            bool isCanBeWritten = true;
            //先单独处理id字段(默认为第1列数据)
            StrDataChangeToId(configInfo[0], t, line, ref instance);
            if (instance == null) return;
            for (int j = 1; j < configInfo.Length; j++)
            {
                FieldInfo fi = t.GetField(fieldName[j]);
                Type tt = fi.FieldType;
                object o;
                //处理字段被赋初值的数据
                if (configInfo[j].Equals(""))
                {
                    o = fi.GetValue(instance);
                    if (o != null)
                    {
                        fi.SetValue(instance, o);
                        continue;
                    }
                }
                //处理标记为ConfigAsset特性的string
                if (IsConfigAsset(fi) && configInfo[j] != "")
                {
                    instance.Assets.Add(configInfo[j]);
                }
                //处理自定义类型数据
                if (JudgeCustomClassIsConfigClass(tt))
                {
                    o = ConfigClassStrDataChangeToField(configInfo[j], tt, ref instance, ex);
                }
                //处理非自定义类型数据
                else
                {
                    o = StrDataChangeToField(configInfo[j], tt, ex);
                    if (o == null)
                    {
                        Debug("配置表" + t.Name + "导入配置数据错误（第" + (line + 1) + "行 , 第" + (j + 1) +
                            "列） ： （字段名：" + fieldName[j] + "）的数据\"" + configInfo[j] +
                            "\"的格式不符合要求，导入id为" + configInfo[0] + "的配置数据失败!");
                        isCanBeWritten = false;
                        continue;
                    }
                }
                fi.SetValue(instance, o);
            }
            if (!isCanBeWritten)
                instance = null;
        }

        //判断配置表是否存在
        private bool IsExistDic(string configName)
        {
            if (_dicData.ContainsKey(configName))
                return true;
            Debug("不存在类名为" + configName + "的配置表！");
            return false;
        }

        //判断某配置表中的配置是否存在
        private bool IsExistConfigFromDic(string configName, long id, string cfgkey)
        {
            if (_dicData.ContainsKey(configName))
            {
                if (_dicData[configName].ContainsKey(id))
                {
                    if (_dicData[configName][id].ContainsKey(cfgkey))
                        return true;
                    Debug("带附加参数的key值TConfig不存在！  " + "configName = " + configName + " , id = " + id + " , key = " +
                                cfgkey);
                    return false;
                }
                if (id != 0L)
                {
                    Debug("类名为" + configName + "的配置表中不存在ID为" + id + "的配置！");
                }
                return false;
            }
            Debug("不存在类名为" + configName + "的配置表！");
            return false;
        }
        //判断字段是否标记了ConfigAsset特性
        private bool IsConfigAsset(FieldInfo fi)
        {
            if (fi.FieldType.Name != "String")
            {
                return false;
            }
            Attribute[] a = Attribute.GetCustomAttributes(fi);
            for (int i = 0; i < a.Length; i++)
            {
                if (a[i].GetType() == typeof(ConfigAssetAttribute))
                {
                    return true;
                }
            }
            return false;
        }

        //字符串数据转换为自定义数据类型
        private object ConfigClassStrDataChangeToData(Type t, string data, ref TConfig instance, params int[] ex)
        {
            string[] refFlag = data.Split('@');
            long judgei;
            if (!long.TryParse(refFlag[0], out judgei))
            {
                Debug("引用的自定义类型" + t.Name + "的数据（" + data +
                    "）格式有误，返回空值");
                return null;
            }
            judgei = long.Parse(refFlag[0]);
            if (refFlag.Length == 1)
            {
                TConfig c = t.Assembly.CreateInstance(t.Name) as TConfig;
                c = _GetConfigFromDic(t, judgei, ex);
                if (c != null)
                {
                    foreach (string s in c.Assets)
                    {
                        instance.Assets.Add(s);
                    }
                }
                return c;
            }
            string[] tempStr = refFlag[1].Split('_');
            int[] tempInts = new int[tempStr.Length];
            try
            {
                for (int i = 0; i < tempStr.Length; i++)
                {
                    tempInts[i] = int.Parse(tempStr[i]);
                }
            }
            catch (Exception)
            {
                return null;
            }
            int[] targetInts = new int[ex.Length];
            for (int i = 0; i < targetInts.Length; i++)
            {
                targetInts[i] = i <= tempStr.Length - 1
                    ? ex[tempInts[i]]
                    : ex[i];
            }
            TConfig c2 = t.Assembly.CreateInstance(t.Name) as TConfig;
            c2 = _GetConfigFromDic(t, judgei, targetInts);
            foreach (string s in c2.Assets)
            {
                instance.Assets.Add(s);
            }
            return c2;
        }

        //判断自定义类型是否符合要求
        private bool JudgeCustomClassIsConfigClass(Type t)
        {
            if (t.IsArray)
            {
                if (t.Name.Contains("[][]"))
                    t = t.GetElementType().GetElementType();
                if (t.Name.Contains("[]"))
                    t = t.GetElementType();
            }
            else if (t.IsGenericType)
            {
                t = t.GetGenericArguments()[0];
            }
            return IsConfigClass(t);
        }

        //判断类型的基类是否是TConfig或者是TConfig本身
        private bool IsConfigClass(Type type)
        {
            if (type.IsInterface)
                return false;
            while (type != typeof(object))
            {
                if (type == typeof(TConfig))
                    return true;
                type = type.BaseType;
            }
            return false;
        }

        //检查传入的配置信息可能出现的各种错误，并打印对应错误信息
        private bool CheckConfig(Type t, string[] arrFieldName, string[] arrFieldType)
        {
            bool IsTrue = true;
            //检查字段名是否一一存在
            for (int i = 0; i < arrFieldName.Length; i++)
            {
                if (t.GetField(arrFieldName[i]) == null)
                {
                    Debug("配置表" + t.Name + "导入字段名(FieldName)错误（第3行，第" + (i + 1) + "列） ： 字段名\"" +
                            arrFieldName[i] + "\"在类\"" + t.Name + "\"中不存在，导入失败！");
                    IsTrue = false;
                }
            }
            if (!IsTrue) return false;
            //检查字段类型与对应字段名是否一一匹配
            for (int i = 0; i < arrFieldType.Length; i++)
            {
                FieldInfo fi = t.GetField(arrFieldName[i]);
                string typeName = RuntimeTypeChangeToTypeName(fi.FieldType);
                if (!DebugMode)
                {
                    if (!fi.IsInitOnly)
                    {
                        Debug("配置表" + t.Name + "写入权限错误（第1行，第" + (i + 1) + "列） ： 字段名\"" +
                                    arrFieldName[i] + "\"该字段不是readonly，导入失败！");
                        IsTrue = false;
                    }
                }
                if (!arrFieldType[i].Equals(typeName))
                {
                    Debug("配置表" + t.Name + "导入字段类型(FieldType)错误（第1行，第" + (i + 1) + "列） ： 类型\"" +
                        arrFieldType[i] + "\"与类型\"" + typeName + "\"不匹配，导入失败！");
                    IsTrue = false;
                }
            }
            return IsTrue;
        }
        //获取引用其他TConfig类型的字段类型表
        private List<Type> GetConfigTypeField(Type t)
        {
            FieldInfo[] fsInfos = t.GetFields();
            List<Type> targetInfos = new List<Type>();
            Type ttemp;
            for (int i = 0; i < fsInfos.Length; i++)
            {
                ttemp = fsInfos[i].FieldType;
                if (ttemp.IsArray)
                {
                    if (ttemp.Name.Contains("[][]"))
                        ttemp = ttemp.GetElementType().GetElementType();
                    else if (ttemp.Name.Contains("[]"))
                        ttemp = ttemp.GetElementType();
                }
                else if (ttemp.IsGenericType)
                    ttemp = ttemp.GetGenericArguments()[0];
                if (IsConfigClass(ttemp))
                    targetInfos.Add(ttemp);
            }
            return targetInfos;
        }
        #endregion

        #region 数据转换操作
        //字段类型名转为配置表字段类型名
        private string RuntimeTypeChangeToTypeName(Type t)
        {
            if (!t.IsArray)
                return !t.IsGenericType ? BaseTypeNameChange(t, "") : BaseTypeNameChange(t.GetGenericArguments()[0], "List");
            if (t.Name.Contains("[][]"))
                return BaseTypeNameChange(t.GetElementType().GetElementType(), "[][]");
            if (t.Name.Contains("[]"))
                return BaseTypeNameChange(t.GetElementType(), "[]");
            return null;
        }

        //基本数据类型别名转换为C#名
        private string BaseTypeNameChange(Type t, string strsuff)
        {
            string s = null;
            switch (t.Name)
            {
                case "String": s = "string"; break;
                case "Int32": s = "int"; break;
                case "UInt32": s = "uint"; break;
                case "UInt64": s = "ulong"; break;
                case "Byte": s = "byte"; break;
                case "SByte": s = "sbyte"; break;
                case "Int16": s = "short"; break;
                case "Char": s = "char"; break;
                case "UInt16": s = "ushort"; break;
                case "Single": s = "float"; break;
                case "Boolean": s = "bool"; break;
                case "Double": s = "double"; break;
                case "Int64": s = "long"; break;
            }
            if (s != null) return s + strsuff;
            if (t.IsEnum) return "enum" + strsuff;
            if (IsConfigClass(t)) return t.Name + strsuff;
            return null;
        }

        //字符串数据转换为基本数据类型
        private object StrValueChangeToBaseData(string data, Type t, params int[] ex)
        {
            if (data.Equals(""))
                return t.Name.Equals("String") ? "" : t.Assembly.CreateInstance(t.FullName);
            try
            {
                switch (t.Name)
                {
                    case "String":
                        return data;
                    case "Byte":
                        return byte.Parse(data);
                    case "SByte":
                        return sbyte.Parse(data);
                    case "Int16":
                        {
                            data = data.Replace ('_', '-');
                            return short.Parse (data);
                        }
                    case "UInt16":
                        {
                            data = data.Replace ('_', '-');
                            return ushort.Parse (data);
                        }
                    case "Int32":
                        {
                            data = data.Replace ('_', '-');
                            return int.Parse (data);
                        }
                    case "UInt32":
                        {
                            data = data.Replace ('_', '-');
                            return uint.Parse (data);
                        }
                    case "Int64":
                        {
                            data = data.Replace ('_', '-');
                            return long.Parse (data);
                        }
                    case "UInt64":
                        {
                            data = data.Replace ('_', '-');
                            return ulong.Parse (data);
                        }
                    case "Char":
                        return char.Parse(data);
                    case "Boolean":
                        return data.ToLower() == "true" || data == "1";
                    case "Single":
                        {
                            data = data.Replace ('_', '-');
                            return float.Parse (data);
                        }
                    case "Double":
                        {
                            data = data.Replace ('_', '-');
                            if (data.StartsWith ("@"))
                            {
                                Regex regex1 = new Regex (@"@(.+):(.+)_(.+)");
                                if (regex1.IsMatch (data))
                                    return StrValueChangeToConfigRefData (data, regex1, ex);
                                Regex regex2 = new Regex (@"@(.+):(.+)");
                                if (regex2.IsMatch (data))
                                    return StrValueChangeToConfigRefDataDefault (data, regex2);
                            }
                            return double.Parse (data);
                        }
                }
            }
            catch (Exception e)
            {
                return null;
            }
            return null;
        }

        private object StrValueChangeToConfigRefData(string data, Regex regex, params int[] ex)
        {
            string TypeName = regex.Match(data).Groups[1].Value;
            string id = regex.Match(data).Groups[2].Value;
            string index = regex.Match(data).Groups[3].Value;

            if (TypeName == "level")
            {
                long n;
                if (long.TryParse(id, out n))
                {
                    n = long.Parse(id);
                    ConfigLevelVar clv = (ConfigLevelVar)_GetConfigFromDic(typeof(ConfigLevelVar), n);

                    if (ex.Length == 0)
                        return clv.GetLevelVar(0);

                    int nn;
                    if (int.TryParse(index, out nn))
                    {
                        nn = int.Parse(index);
                        return nn >= ex.Length
                            ? clv.GetLevelVar(ex[ex.Length - 1])
                            : clv.GetLevelVar(ex[nn]);
                    }
                }
            }
            return null;
        }

        private object StrValueChangeToConfigRefDataDefault(string data, Regex regex)
        {
            string TypeName = regex.Match(data).Groups[1].Value;
            string id = regex.Match(data).Groups[2].Value;

            if (TypeName == "level")
            {
                long n;
                if (long.TryParse(id, out n))
                {
                    return ((ConfigLevelVar)GetConfigFromDic(typeof(ConfigLevelVar), long.Parse(id))).GetLevelVar(0);
                }
            }
            return null;
        }
        private void StrDataChangeToId(string data, Type t, int line, ref TConfig instance)
        {
            if (data.Equals(""))
            {
                Debug("配置表" + t.Name + "导入数据错误（第" + (line + 1) + "行 , 第1列） ： 数据的ID不能为空，导入数据失败!");
                instance = null;
                return;
            }
            object oid;
            if (data.EndsWith("@"))
            {
                data = data.Remove(data.Length - 1);
                oid = StrDataChangeToField(data, typeof(long));
                if (oid == null)
                {
                    Debug("配置表" + t.Name + "导入配置数据id错误（第" + (line + 1) + "行 , 第1列） ： 数据id号（" + data +
                            "@）的格式不符合要求，导入配置数据失败!");
                    instance = null;
                    return;
                }
                TConfig refConfig = _GetConfigFromDic(t.BaseType, (long)oid);
                if (refConfig == null)
                {
                    instance = null;
                    return;
                }
                FieldInfo[] refFis = refConfig.GetType().GetFields();
                for (int i = 0; i < refFis.Length; i++)
                {
                    t.GetField(refFis[i].Name).SetValue(instance, t.BaseType.GetField(refFis[i].Name).GetValue(refConfig));
                }
                return;
            }
            oid = StrDataChangeToField(data, typeof(long));
            if (oid == null)
            {
                Debug("配置表" + t.Name + "导入配置数据id错误（第" + (line + 1) + "行 , 第1列） ： 数据id号（" + data +
                        "）的格式不符合要求，导入配置数据失败!");
                instance = null;
                return;
            }
            t.GetField("id").SetValue(instance, oid);
        }

        //非自定义类型数据转换成对应字段数据
        private object StrDataChangeToField(string data, Type t, params int[] ex)
        {
            //处理非数组类型数据
            if (!t.IsArray)
            {
                //字符串数据转换为指定类型枚举数据
                if (t.IsEnum)
                    return data.Equals("") ? Enum.GetNames(t).GetValue(0) : Enum.Parse(t, data);
                //处理list泛型
                if (t.IsGenericType)
                {
                    Type ttl = t.GetGenericArguments()[0];
                    Type tl = typeof(List<>).MakeGenericType(ttl);
                    if (data.Equals(""))
                        return Activator.CreateInstance(tl);
                    IList listConfig = (IList)Activator.CreateInstance(tl);
                    string[] strData = data.Split('+');
                    if (tl.IsEnum)
                    {
                        for (int i = 0; i < strData.Length; i++)
                            listConfig.Add(Enum.Parse(ttl, strData[i]));
                    }
                    else
                    {
                        object o;
                        for (int i = 0; i < strData.Length; i++)
                        {
                            o = StrValueChangeToBaseData(strData[i], ttl, ex);
                            if (o == null) return null;
                            listConfig.Add(o);
                        }
                    }
                    return listConfig;
                }
                //修改基本类型数据
                return StrValueChangeToBaseData(data, t, ex);
            }
            //字符串数据转换为指定类型数组数据
            if (t.Name.Contains("[][]"))
            {
                Type tt2 = t.GetElementType().GetElementType();
                if (data.Equals(""))
                    return Array.CreateInstance(t.GetElementType(), 0);
                string[] line = data.Split('|');
                string[][] str2 = new string[line.Length][];
                for (int i = 0; i < line.Length; i++)
                    str2[i] = line[i].Split('+');

                Array arrConfig2 = Array.CreateInstance(t.GetElementType(), line.Length);
                if (tt2.IsEnum)
                {
                    for (int i = 0; i < line.Length; i++)
                    {
                        Array tempElem = Array.CreateInstance(tt2, str2[i].Length);
                        for (int j = 0; j < str2[i].Length; j++)
                            tempElem.SetValue(Enum.Parse(tt2, str2[i][j]), j);
                        arrConfig2.SetValue(tempElem, i);
                    }
                }
                else
                {
                    object odata;
                    for (int i = 0; i < line.Length; i++)
                    {
                        Array tempElem = Array.CreateInstance(tt2, str2[i].Length);
                        for (int j = 0; j < str2[i].Length; j++)
                        {
                            odata = StrValueChangeToBaseData(str2[i][j], tt2, ex);
                            if (odata == null) return null;
                            tempElem.SetValue(odata, j);
                        }
                        arrConfig2.SetValue(tempElem, i);
                    }
                }
                return arrConfig2;
            }
            if (t.Name.Contains("[]"))
            {
                Type tt1 = t.GetElementType();
                if (data.Equals(""))
                    return Array.CreateInstance(tt1, 0);
                string[] str1 = data.Split('+');
                int n = str1.Length;
                Array arrConfig1 = Array.CreateInstance(tt1, n);
                if (tt1.IsEnum)
                    for (int i = 0; i < n; i++)
                        arrConfig1.SetValue(Enum.Parse(tt1, str1[i]), i);
                else
                {
                    object odata;
                    for (int i = 0; i < n; i++)
                    {
                        odata = StrValueChangeToBaseData(str1[i], tt1, ex);
                        if (odata == null) return null;
                        arrConfig1.SetValue(odata, i);
                    }
                }
                return arrConfig1;
            }
            return null;
        }

        //自定义类型数据转换成对应字段数据
        private object ConfigClassStrDataChangeToField(string data, Type t, ref TConfig instance, params int[] ex)
        {
            //处理非数组
            if (!t.IsArray && !t.IsGenericType)
            {
                if (!_dicData.ContainsKey(t.Name))
                {
                    Debug("引用的自定义类型" + t.Name + "在配置管理表中不存在，返回空值");
                    return null;
                }
                if (data.Equals(""))
                    return t.Assembly.CreateInstance(t.Name);
                return ConfigClassStrDataChangeToData(t, data, ref instance, ex);
            }
            //处理数组
            if (t.IsArray)
            {
                if (t.Name.Contains("[][]"))
                {
                    Type tt2 = t.GetElementType().GetElementType();
                    if (!_dicData.ContainsKey(tt2.Name))
                    {
                        Debug("引用的自定义类型" + t.Name + "在配置管理表中不存在，返回空值");
                        return null;
                    }
                    if (data.Equals(""))
                        return Array.CreateInstance(t.GetElementType(), 0);
                    string[] line = data.Split('|');
                    string[][] str = new string[line.Length][];
                    for (int i = 0; i < line.Length; i++)
                        str[i] = line[i].Split('+');

                    Array arrConfig2 = Array.CreateInstance(t.GetElementType(), line.Length);
                    object o;
                    for (int i = 0; i < line.Length; i++)
                    {
                        Array tempElem = Array.CreateInstance(tt2, str[i].Length);
                        for (int j = 0; j < str[i].Length; j++)
                        {
                            o = ConfigClassStrDataChangeToData(tt2, str[i][j], ref instance, ex);
                            if (o == null) return null;
                            tempElem.SetValue(o, j);
                        }
                        arrConfig2.SetValue(tempElem, i);
                    }
                    return arrConfig2;
                }
                if (t.Name.Contains("[]"))
                {
                    Type tt = t.GetElementType();
                    if (!_dicData.ContainsKey(tt.Name))
                    {
                        Debug("引用的自定义类型" + t.Name + "在配置管理表中不存在，返回空值");
                        return null;
                    }
                    if (data.Equals(""))
                        return Array.CreateInstance(tt, 0);
                    string[] strData = data.Split('+');
                    Array arrConfig = Array.CreateInstance(tt, strData.Length);
                    object o;
                    for (int i = 0; i < strData.Length; i++)
                    {
                        o = ConfigClassStrDataChangeToData(tt, strData[i], ref instance, ex);
                        if (o == null) return null;
                        arrConfig.SetValue(o, i);
                    }
                    return arrConfig;
                }
                return null;
            }
            //处理list泛型
            if (t.IsGenericType)
            {
                Type ttl = t.GetGenericArguments()[0];
                Type tl = typeof(List<>);
                tl = tl.MakeGenericType(ttl);
                if (!_dicData.ContainsKey(ttl.Name))
                {
                    Debug("引用的自定义类型" + ttl.Name + "在配置管理表中不存在，返回空值");
                    return null;
                }
                if (data.Equals(""))
                    return Activator.CreateInstance(tl);
                IList listConfig = (IList)Activator.CreateInstance(tl);
                string[] strData = data.Split('+');
                object o;
                for (int i = 0; i < strData.Length; i++)
                {
                    o = ConfigClassStrDataChangeToData(ttl, strData[i], ref instance, ex);
                    if (o == null) return null;
                    listConfig.Add(o);
                }
                return listConfig;
            }
            return null;
        }
        //数据字典信息转换为配置文件格式字符串
        private string DicChangeToStrInfo<T>()
        {
            Type t = typeof(T);
            if (!IsConfigClass(t))
            {
                Debug("导出类型{0}错误！该类型未继承TConfig或不是TConfig", t.Name);
                return null;
            }
            if (!_dicData.ContainsKey(t.Name))
            {
                Debug("导出类型{0}不存在！该类型未在数据字典中", t.Name);
                return null;
            }

            List<FieldInfo[]> fisList = new List<FieldInfo[]>();
            List<FieldInfo> fiList = new List<FieldInfo>();

            while (t != typeof(object))
            {
                fisList.Add(t.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly));
                t = t.BaseType;
            }

            fisList.Reverse();
            foreach (FieldInfo[] fieldInfose in fisList)
            {
                foreach (FieldInfo fieldInfo in fieldInfose)
                {
                    if (fieldInfo.IsInitOnly)
                        fiList.Add(fieldInfo);
                }
            }

            StringBuilder strData = new StringBuilder();

            for (int i = 0; i < fiList.Count; i++)
            {
                strData.Append(RuntimeTypeChangeToTypeName(fiList[i].FieldType));
                strData.Append(i != fiList.Count - 1 ? "\t" : "\r\n");
            }
            for (int i = 0; i < fiList.Count; i++)
            {
                Attribute[] a = Attribute.GetCustomAttributes(fiList[i]);
                bool isHasCommment = false;
                for (int j = 0; j < a.Length; j++)
                {
                    if (a[j].GetType() == typeof(ConfigCommentAttribute))
                    {
                        strData.Append(a[j].GetType().GetField("comment").GetValue(a[j]));
                        isHasCommment = true;
                        break;
                    }
                }
                if (!isHasCommment)
                {
                    strData.Append("注释" + (i + 1));
                }
                strData.Append(i != fiList.Count - 1 ? "\t" : "\r\n");
            }
            for (int i = 0; i < fiList.Count; i++)
            {
                strData.Append(fiList[i].Name);
                strData.Append(i != fiList.Count - 1 ? "\t" : "\r\n");
            }

            foreach (Dictionary<string, TConfig> c in _dicData[typeof(T).Name].Values)
            {
                for (int i = 0; i < fiList.Count; i++)
                {
                    strData.Append(ConfigFieldValueChangeToStr(fiList[i].GetValue(c[""])));
                    strData.Append(i != fiList.Count - 1 ? "\t" : "\r\n");
                }
            }
            return strData.ToString();
        }
        //非数组类型数据转换为指定字符串
        private string NonArrayValueChangeToStr(object o)
        {
            Type t = o.GetType();
            if (t.IsEnum)
            {
                int val = (int)o;
                return val.ToString();
            }
            if (IsConfigClass(t))
            {
                TConfig c = (TConfig)o;
                return c.id.ToString();
            }
            if (t.Name == "Boolean")
            {
                return (bool)o ? "1" : "0";
            }
            return o.ToString();
        }
        //TConfig字段数据转换为指定字符串
        private string ConfigFieldValueChangeToStr(object o)
        {
            if (o == null) return "";
            Type t = o.GetType();
            if (t.IsGenericType)
            {
                IList instance = (IList)o;
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < instance.Count; i++)
                {
                    TConfig c = (TConfig)instance[i];
                    sb.Append(c.id);
                    if (i != instance.Count - 1)
                        sb.Append("+");
                }
                return sb.ToString();
            }
            if (!t.IsArray)
                return NonArrayValueChangeToStr(o);
            if (t.Name.Contains("[][]"))
            {
                StringBuilder sb = new StringBuilder();
                Array[] arr = (Array[])o;
                for (int i = 0; i < arr.Length; i++)
                {
                    for (int j = 0; j < arr[i].Length; j++)
                    {
                        sb.Append(NonArrayValueChangeToStr(arr[i].GetValue(j)));
                        if (j != arr[i].Length - 1)
                            sb.Append("+");
                    }
                    if (i != arr.Length - 1)
                        sb.Append("|");
                }
                return sb.ToString();
            }
            if (t.Name.Contains("[]"))
            {
                StringBuilder sb = new StringBuilder();
                Array arr = (Array)o;
                for (int i = 0; i < arr.Length; i++)
                {
                    sb.Append(NonArrayValueChangeToStr(arr.GetValue(i)));
                    if (i != arr.Length - 1)
                        sb.Append("+");
                }
                return sb.ToString();
            }
            return null;
        }
        #endregion
    }
}
