using System.Collections.Generic;
using AorBaseUtility;
using UnityEngine;
using YoukiaCore;

namespace YoukiaUnity.Resource
{

    /// <summary>
    /// 资源保持器，用于保持对资源单位的引用，避免释放
    /// </summary>
    public class ResourceRefKeeper : MonoBehaviour
    {

        internal ResourcesManager.ResourceRef ResRef;

        /// <summary>
        /// 如果该物体结构改变了，就不能用于缓存下次使用，请标脏
        /// </summary>
        public bool isDirty = false;


        /// <summary>
        /// 引用的ID，用来辨别是否有重复引用
        /// </summary>
        public int RefID;

        /// <summary>
        /// 一个Debug的调试信息显示器
        /// </summary>
        public string DebugLabel;

        public void Debug()
        {

            string infoStr = ResRef.Debug();
            if (string.IsNullOrEmpty(infoStr))
                return;

            Dictionary<string, string> info = new Dictionary<string, string>();
            string[] parma = infoStr.Split('|');

            for (int i = 0; i < parma.Length; i++)
            {
                string[] each = parma[i].Split(',');

                //依赖去重显示
                if (!info.ContainsKey(each[0]))
                    info.Add(each[0], parma[i]);
            }

            foreach (KeyValuePair<string, string> pair in info)
            {
                DebugLabel += pair.Value + "\n";
            }
        }

        void Start()
        {

//            if (YKApplication.Instance.GetManager<ResourcesManager>().DebugMode)
//            {
//                Debug();
//            }

        }

        private void OnDestory()
        {

            if (ResRef.resUnit.ReferenceCount > 1)
            {
                Log.Info("不干净的移除:" + gameObject.name + ",此资源有一个以上的引用");
            }


        }

    }
}
