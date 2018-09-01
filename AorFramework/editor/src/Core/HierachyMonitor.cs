using System;
using UnityEditor;
using UnityEngine;

namespace Framework.Editor
{

    /// <summary>
    /// Hierachy 监视器
    /// </summary>
    public class HierachyMonitor : FrameworkEditorBehaviour
    {
        public override void Update()
        {
            if (Application.isPlaying) return;
//            AorUIEditorGlobal.update();
        }

        public override void OnPlaymodeStateChanged(PlayModeState playModeState)
        {
            //Debug.UiLog ("游戏运行模式发生改变， 点击 运行游戏 或者暂停游戏或者 帧运行游戏 按钮时触发: " + playModeState);
        }

        public override void OnGlobalEventHandler(Event e)
        {
            //Debug.UiLog ("全局事件回调: " + e);
        }

        public override void HierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
        {

            if (Application.isPlaying) return;

            //Debug.UiLog (string.Format ("{0} : {1} - {2}", EditorUtility.InstanceIDToObject (instanceID), instanceID, selectionRect));
            GameObject go = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (go)
            {

                //PrefabLinker
//                PrefabLinker pl = go.GetComponent<PrefabLinker>();
//                if (pl)
//                {
//
//                    //检查go是否是链接状态的Prefab
//                    UnityEngine.Object o = PrefabUtility.GetPrefabParent(go);
//                    if (o != null)
//                    {
//
//                        string n = go.name;
//                        Transform p = null;
//                        int z = go.transform.GetSiblingIndex();
//                        if (go.transform.parent != null)
//                        {
//                            p = go.transform.parent;
//                        }
//
//                        UnityEngine.Object asset = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(pl.SourceGUID), typeof(UnityEngine.Object));
//                        if (asset != null)
//                        {
//                            GameObject ngo = GameObject.Instantiate(asset) as GameObject;
//                            if (ngo != null)
//                            {
//
//                                //替换
//                                PrefabLinkerEditTool.ReplaceLinkerReference(go, ngo);
//
//                            }
//                            else
//                            {
//                                string ap = AssetDatabase.GetAssetPath(go);
//                                UnityEngine.Debug.LogError("警告! 你的PrefabLinker[" + ap + "]可能已经坏了. 请重新保存PrefabLinker!");
//                                Selection.activeObject = pl.gameObject;
//                            }
//                            //
//                            //                        //自动解开Proxy 
//                            //                        UprefabEditTool.installRefSubLoop(ngo.transform);
//                            return;
//                        }
//                        else
//                        {
//                            string ap = AssetDatabase.GetAssetPath(go);
//                            UnityEngine.Debug.LogError("警告! 你的PrefabLinker[" + ap + "]的引用链接可能已经坏了 : RefPath == " + (string.IsNullOrEmpty(pl.RefPath) ? "空" : pl.RefPath) + ",请更新引用. 请重新保存PrefabLinker!");
//                            //  Selection.activeObject = o;
//                        }
//
//                    }
//
//                    //这里是UprefabUnitBase的tip生成逻辑
//                    if (!string.IsNullOrEmpty(pl.RefPath))
//                    {
//                        Rect r = new Rect(selectionRect.xMax - 100, selectionRect.y, 100, selectionRect.height);
//                        GUI.Label(r, "(linker)", AorUIStyleDefine.getProxyTipStyle);
//                    }
//                }

                //AorSwitchableUI
                //            AorSwitchableUI uub = go.GetComponent<AorSwitchableUI>();
                //            if (uub != null) {
                //
                //                //检查go是否是链接状态的Prefab
                //                UnityEngine.Object o = PrefabUtility.GetPrefabParent(go);
                //                if (o != null) {
                //
                //                    string n = go.name;
                //                    Transform p = null;
                //                    int z = go.transform.GetSiblingIndex();
                //                    if (go.transform.parent != null) {
                //                        p = go.transform.parent;
                //                    }
                //
                //                    string assetPath = AssetDatabase.GetAssetPath(o);
                //                    string resPath = assetPath.Replace("Assets/Resources/", "").Replace(".prefab", "");
                //
                //                    UnityEngine.Object asset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(UnityEngine.Object));
                //                    if (asset != null) {
                //                        GameObject ngo = GameObject.Instantiate(asset) as GameObject;
                //                        if (ngo != null) {
                //                            ngo.name = n;
                //                            if (p != null) {
                //                                ngo.transform.SetParent(p, false);
                //                                ngo.transform.SetSiblingIndex(z);
                //                            }
                //                            PrefabUtility.DisconnectPrefabInstance(ngo);
                //
                //                            AorSwitchableUI ngoUub = ngo.GetComponent<AorSwitchableUI>();
                //                            ngoUub.Path = resPath;
                //
                //                            GameObject.DestroyImmediate(go);
                //                        }
                //                        else {
                //                            UnityEngine.Debug.LogError("警告! 你的Uprefab可能已经坏了. 请重新保存Uprefab!");
                //                            Selection.activeObject = uub.gameObject;
                //                        }
                //                        return;
                //                    }
                //                    else {
                //                        //应该不会发生的情况 ..
                //                        UnityEngine.Debug.LogError("Error! AorSwitchableUI.Path属性为空! :: [" + go.transform.GetHierarchyPath() + "]");
                //                    }
                //
                //                }
                //                
                //                //这里是UprefabUnitBase的tip生成逻辑
                //                if (!string.IsNullOrEmpty(uub.Path)) {
                //                    Rect r = new Rect(selectionRect.xMax - 100, selectionRect.y, 100, selectionRect.height);
                //                    GUI.Label(r, "(SwitchableUI)", AorUIStyleDefine.getProxyTipStyle);
                //                }

                //自动解开Proxy 
                //UprefabEditTool.installRefSubLoop(go.transform);

            }
            /*
            //UprefabProxy
            UprefabProxy up = go.GetComponent<UprefabProxy>();
            if (up != null) {
                //这里是UprefabProxy自解逻辑
                string p = up.proxyData.RefPath;
                UnityEngine.Object Uprefab = AssetDatabase.LoadAssetAtPath(p, typeof(GameObject));
                up.InstallRef(Uprefab);
            }*/
        }

    public override void OnHierarchyWindowChanged()
    {
        //Debug.UiLog ("层次视图发生变化");
    }

    public override void OnModifierKeysChanged()
    {
        //Debug.UiLog ("当触发键盘事件");
    }

    public override void OnProjectWindowChanged()
    {
        //	Debug.UiLog ("当资源视图发生变化");
    }

    public override void ProjectWindowItemOnGUI(string guid, Rect selectionRect)
    {
        //根据GUID得到资源的准确路径
        //Debug.UiLog (string.Format ("{0} : {1} - {2}", AssetDatabase.GUIDToAssetPath (guid), guid, selectionRect));
    }
}

}

