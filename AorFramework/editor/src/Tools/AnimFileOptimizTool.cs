using System;
using System.Collections.Generic;

using UnityEditor;
using UnityEditor.Animations;
//using UnityEditorInternal;
using UnityEngine;

/// <summary>
/// supported version : unity 4.7 / 5.0 +
/// </summary>

public class AnimFileOptimizTool : Editor
{
    /// <summary>
    /// 已知bug 在选中prefab对动画FBX文件有交叉引用的时候，会报错
    /// </summary>
    [MenuItem("Youkia/角色动画工具/优化工具/预制体Animation脱壳优化")]
    public static void AnimationOptimize()
    {
        GameObject[] Selects = Selection.gameObjects;
        if (Selects == null || Selects.Length == 0)
        {
            EditorUtility.DisplayDialog("提示", "你没有选择任何东西~~~~", "OK");
            return;
        }

        List<string> _processRecord = new List<string>();

        int totalAmNum = 0;
        int totalArNum = 0;

        int i, len = Selects.Length;
        for (i = 0; i < len; i++)
        {

 //           string selectAssetPath = AssetDatabase.GetAssetPath(Selects[i]);
 //           if(string.IsNullOrEmpty(selectAssetPath) || selectAssetPath.ToLower().Contains(".fbx")) continue;

            GameObject _assetGameObject = null;
            GameObject _gameObject = null;
            bool _isNInst = false;
            PrefabType pt = PrefabUtility.GetPrefabType(Selects[i]);

            switch (pt)
            {
                case PrefabType.Prefab:
                    _assetGameObject = Selects[i];
                    _gameObject = (GameObject)PrefabUtility.InstantiatePrefab(_assetGameObject);
                    _isNInst = true;
                    break;
                case PrefabType.PrefabInstance:
                    _gameObject = Selects[i];
                    _assetGameObject = (GameObject)PrefabUtility.GetPrefabParent(PrefabUtility.FindPrefabRoot(_gameObject));
                    break;
                default:
                    _gameObject = Selects[i];
                    break;
            }
            if (_assetGameObject != null)
            {
                string _assetGameObjectPath = AssetDatabase.GetAssetPath(_assetGameObject);
                if (_processRecord.Contains(_assetGameObjectPath))
                {
                    //跳过重复资源
                    continue;
                }
                _processRecord.Add(_assetGameObjectPath);
            }


            List<Animation> animations = new List<Animation>();
            List<Animator> animators = new List<Animator>();

            Animation[] am = _gameObject.GetComponentsInChildren<Animation>();
            if (am != null && am.Length > 0)
            {
                animations.AddRange(am);
                totalAmNum += am.Length;
            }

            Animator[] ar = _gameObject.GetComponentsInChildren<Animator>();
            if (ar != null && ar.Length > 0)
            {
                animators.AddRange(ar);
                totalArNum += ar.Length;
            }

            List<string> delFbxPathList = new List<string>();

            if (animations.Count > 0)
            {
                OptimizeAM(animations, delFbxPathList);
            }

            if (animators.Count > 0)
            {
                OptimizeAR(animators, delFbxPathList);
            }

            //
            if (_assetGameObject != null)
            {
                //替换/保存 prefab
                PrefabUtility.ReplacePrefab(_gameObject, _assetGameObject, ReplacePrefabOptions.ReplaceNameBased);
            }

            if (delFbxPathList.Count > 0)
            {
                for (int d = 0; d < delFbxPathList.Count; d++)
                {
                    AssetDatabase.DeleteAsset(delFbxPathList[d]);
                }
                delFbxPathList.Clear();
            }

            if (_isNInst)
            {
                GameObject.DestroyImmediate(_gameObject);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        if (totalAmNum == 0 && totalArNum == 0)
        {
            EditorUtility.DisplayDialog("提示", "你没有选择任何包含Animation/Animator组件的GameObject", "OK");
        }
        else
        {
            EditorUtility.DisplayDialog("提示", "脱壳成功，处理" + totalAmNum + "个Animation, " + totalArNum + "个Animator", "OK");
        }

    }

    private static void OptimizeAM(List<Animation> list, List<string> delFbxPathList)
    {
        int i, len = list.Count;
        for (i = 0; i < len; i++)
        {
            Animation an = list[i];
            //
            string defaultAnimName = an.clip.name;
            List<AnimationClip> newClipList = new List<AnimationClip>(); 
            AnimationClip[] clips = AnimationUtility.GetAnimationClips(an.gameObject);
            foreach (AnimationClip ac in clips)
            {
                string fbxPath = AssetDatabase.GetAssetPath(ac);
                if (fbxPath.ToLower().Contains(".fbx"))
                {
                    if (!delFbxPathList.Contains(fbxPath))
                    {
                        delFbxPathList.Add(fbxPath);
                    }

                    AnimationClip newClip = ClipProcess(ac, fbxPath);
                    if (newClip != null)
                    {
                        newClipList.Add(newClip);
                    }
                }
                else
                {
                    newClipList.Add(ac);
                }
            }
            
            AnimationUtility.SetAnimationClips(an, newClipList.ToArray());

            //setDefaultClip
            foreach (AnimationClip clip in newClipList)
            {
                if (defaultAnimName == clip.name)
                {
                    an.clip = clip;
                }
            }

        }
    }

    private static AnimationClip ClipProcess(AnimationClip ac, string acPath)
    {

        string savePath = acPath.Substring(0, acPath.LastIndexOf("/") + 1) + ac.name + ".anim";
        AnimationClip newClip = new AnimationClip();
        EditorUtility.CopySerialized(ac, newClip);

        AssetDatabase.CreateAsset(newClip, savePath);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        return newClip;
    }

    private static void OptimizeAR(List<Animator> list, List<string> delFbxPathList)
    {
        int i, len = list.Count;
        for (i = 0; i < len; i++)
        {
            Animator an = list[i];
            //
//#if UNITY_5
            AnimatorController acl = (AnimatorController) an.runtimeAnimatorController;
            foreach (AnimatorControllerLayer layer in acl.layers)
            {
                AnimatorStateMachine asm = layer.stateMachine;
                foreach (ChildAnimatorState state in asm.states)
                {
                    AnimatorState ast = state.state;
                    AnimationClip ac = (AnimationClip)ast.motion;

                    string fbxPath = AssetDatabase.GetAssetPath(ac);
                    if (fbxPath.ToLower().Contains(".fbx"))
                    {
                        if (!delFbxPathList.Contains(fbxPath))
                        {
                            delFbxPathList.Add(fbxPath);
                        }

                        AnimationClip newClip = ClipProcess(ac, fbxPath);
                        ast.motion = newClip;
                    }

                }

            }
/*
#else
            AnimatorController acl = (AnimatorController)an.runtimeAnimatorController;

            for (int l = 0; l < acl.layerCount; l++)
            {
                AnimatorControllerLayer layer = acl.GetLayer(l);

                StateMachine sm = layer.stateMachine;

                for (int s = 0; s < sm.stateCount; s++)
                {
                    State state = sm.GetState(s);
                    AnimationClip ac = (AnimationClip)state.GetMotion();
                    string fbxPath = AssetDatabase.GetAssetPath(ac);
                    if (fbxPath.ToLower().Contains(".fbx"))
                    {
                        if (!delFbxPathList.Contains(fbxPath))
                        {
                            delFbxPathList.Add(fbxPath);
                        }

                        AnimationClip newClip = ClipProcess(ac, fbxPath);
                        state.SetAnimationClip(newClip);
                    }

                }
            }
#endif*/

        }
    }
}
