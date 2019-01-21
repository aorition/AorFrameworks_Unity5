using UnityEngine;
using UnityEditor;

namespace Framework.Editor
{
    public class TagAndLayerUtility
    {
        
        /// <summary>
        /// 加载层
        /// </summary>
        /// <param name="layer"></param>
        public static void AddLayer(string layer)
        {
            if (!IsHasLayer(layer))
            {
                //加载项目设置层以及tag值管理 资源
                SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
                SerializedProperty it = tagManager.GetIterator();//获取层或tag值所有列表信息
                while (it.NextVisible(true))//判断向后是否还有信息，如果没有则返回false
                {
#if UNITY_5
                if (it.name=="layers")
                {
                    //层默认是32个，只能从第8个开始写入自己的层
                    for (int i = 8; i < it.arraySize; i++)
                    {
                        SerializedProperty dataPoint = it.GetArrayElementAtIndex(i);//获取层信息
                        if (string.IsNullOrEmpty(dataPoint.stringValue))//如果制定层内为空，则可以填写自己的层名称
                        {
                            dataPoint.stringValue = layer;//设置名字
                            tagManager.ApplyModifiedProperties();//保存修改的属性
                            return;
                        }
                    }
                }
 
#else
                    if (it.name.StartsWith("User Layer"))
                    {
                        if (it.type == "string")
                        {
                            if (string.IsNullOrEmpty(it.stringValue))
                            {
                                it.stringValue = layer;
                                tagManager.ApplyModifiedProperties();
                                return;
                            }
                        }
                    }
#endif
                }
            }
        }

        /// <summary>
        /// 判断是否已经有层
        /// </summary>
        /// <param name="layer"></param>
        /// <returns></returns>
        public static bool IsHasLayer(string layer)
        {
            for (int i = 0; i < UnityEditorInternal.InternalEditorUtility.layers.Length; i++)
            {
                if (UnityEditorInternal.InternalEditorUtility.layers[i].Contains(layer))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 添加Tag值
        /// </summary>
        /// <param name="tag"></param>
        public static void AddTag(string tag)
        {
            if (!IsHasTag(tag))
            {
                SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
                SerializedProperty it = tagManager.GetIterator();
                while (it.NextVisible(true))
                {
                    if (it.name == "tags")
                    {
#if UNITY_5_6 || UNITY_5
                        it.arraySize++; 
                    SerializedProperty dataPoint = it.GetArrayElementAtIndex(it.arraySize - 1);
                    dataPoint.stringValue = tag;
                    tagManager.ApplyModifiedProperties();
                    return;
#else
                        for (int i = 0; i < it.arraySize; i++)
                        {
                            SerializedProperty dataPoint = it.GetArrayElementAtIndex(i);
                            if (string.IsNullOrEmpty(dataPoint.stringValue))
                            {
                                dataPoint.stringValue = tag; tagManager.ApplyModifiedProperties();
                                return;
                            }
                        }
#endif
                    }
                }
            }
        }

        /// <summary>
        /// 判断是否有tag值
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static bool IsHasTag(string tag)
        {
            for (int i = 0; i < UnityEditorInternal.InternalEditorUtility.tags.Length; i++)
            {
                if (UnityEditorInternal.InternalEditorUtility.tags[i].Contains(tag))
                {
                    return true;
                }
            }
            return false;
        }

    }
}