using System;
using System.Collections.Generic;
using System.Reflection;
using Framework.JSON;
using UnityEngine;

namespace AorFramework
{

    /// <summary>
    /// 使用struct标识,用于判断Unity在加载此对象时是否反序列化失败,如果反序列化失败了.则启用内置JSON数据恢复逻辑
    /// </summary>
    [Serializable]
    public struct JScriptableObjectStatus
    {
        public JScriptableObjectStatus(object anyone)
        {
            this._isInit = true;
        }

        [SerializeField]
        private bool _isInit;
        public bool isInit
        {
            get { return _isInit;}
        }

    }

    /// <summary>
    /// 支持Json导出和导入的ScriptableObject
    /// </summary>
    public class JScriptableObject : ScriptableObject
    {

        public static T CreateFormJson<T>(string jsonText)
            where T : JScriptableObject
        {
            T obj = ScriptableObject.CreateInstance<T>();
            if (obj)
            {
                obj.SetupFormJson(jsonText);
                return obj;
            }
            return null;
        }

        public static JScriptableObject CreateFormJson(string jsonText, Type type)
        {
            JScriptableObject baseobj = ScriptableObject.CreateInstance(type) as JScriptableObject;
            if (baseobj)
            {
                baseobj.SetupFormJson(jsonText);
                return baseobj;
            }
            return null;
        }

        //-----------------------------------------------

        /// <summary>
        /// 这是一个神奇的标识字段
        /// 用于判断Unity在加载此对象时是否反序列化失败,如果反序列化失败了.则启用内置JSON数据恢复逻辑
        /// </summary>
        [SerializeField]
        protected JScriptableObjectStatus _status;
        [SerializeField]
        protected string _innerJsonValue;
//        [SerializeField]
//        protected bool m_justUseJsonData = false;

        protected virtual void OnEnable()
        {
            if (!_status.isInit)
            {
                if (!string.IsNullOrEmpty(_innerJsonValue))
                {
                    //新的JScriptableObject初始化,或者并没有内置json数据可以恢复.
                    //为了保证数据不会重复添加,这里会先尝试清理已经序列的数据
                    ClearSerializeData();
                    SetupFormJson(_innerJsonValue);
                }
                //打上正确的标识
                _status = new JScriptableObjectStatus(true);
            }
        }

        protected virtual void OnDisable()
        {

        }

        protected virtual void OnDestroy()
        {
//            if (m_justUseJsonData)
//            {
//                ClearSerializeData();
//            }
        }

        //---------------------------------------------------

        public virtual void SetupFormJson(string jsonText)
        {
            //*** 注意!!源生JsonUtility反序列化,在Dll需要动态加载的系统可能无法正常工作!(Unity5.X实测). 建议子类覆盖此方法实现反序列化.
            //            JsonUtility.FromJsonOverwrite(jsonText, this);

            //以实现通过内部JSON数据反序列化
            Type type = this.GetType();
            List<FieldInfo> FieldInfoList = new List<FieldInfo>();

            Dictionary<string, object> dic = JSONParser.DecodeToDic(jsonText);
            foreach (string key in dic.Keys)
            {
                FieldInfo info = type.GetField(key,BindingFlags.GetField | BindingFlags.Public | BindingFlags.Instance);
                if (info == null)
                {
                    info = type.GetField(key, BindingFlags.GetField| BindingFlags.NonPublic| BindingFlags.Instance);
                    if (info == null)
                    {
                        Debug.LogError("*** JScriptableObject.SetupFormJson Error :: JSON Text to Object Faild");
                        return;
                    }
                }
                FieldInfoList.Add(info);
            }
            if (FieldInfoList.Count > 0)
            {
                
                foreach (FieldInfo info in FieldInfoList)
                {
                    string fName = info.Name;
                    object fValue = JSONParser.ParseValue(info.FieldType, dic[fName]);
                    info.SetValue(this, fValue);
                }
            }
            else
            {
                //Error 没有找到可以序列化的字段
            }
        }


        //结论,改对象不需要在运行时支持JSON导出
//        public virtual string ToJsonString()
//        {
//            return JsonUtility.ToJson(this);
//        }

        /// <summary>
        /// 清除内置JSON数据
        /// </summary>
        public virtual void ClearInnerJsonData()
        {
            _innerJsonValue = string.Empty;
        }

        /// <summary>
        /// 清除序列化数据
        /// </summary>
        public virtual void ClearSerializeData()
        {
            _status = new JScriptableObjectStatus();
            //
        }

    }
}
