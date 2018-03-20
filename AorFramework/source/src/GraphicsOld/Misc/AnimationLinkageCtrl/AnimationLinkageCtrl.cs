using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

//using YoukiaUnity.App;

namespace YoukiaUnity.Misc
{


    public enum ALCLinkPorpType
    {
        Method,
        Property,
        Field,
    }

    public enum ALCMethodArgType
    {
        Float,
        Boolean,
        String,
    }

    [Serializable]
    public struct ALCMethodArg
    {
        public string value;
        public ALCMethodArgType valueType;
    }

    //    [Serializable]
    //    public struct ALCtrlLink
    //    {
    //        public float value;
    //        public bool UseTimeCurve;
    //        public AnimationCurve ValueCurve;
    //        public string linkScriptName;
    //        public string linkPorpName;
    //        public List<ALCMethodArg> linkMethodArgs;
    //        public ALCLinkPorpType linkPorpType;
    //        public bool isPublic;
    //        public bool isStatic;
    //    }

    /// <summary>
    /// 通用动画K帧用动画参数连接器
    /// 
    /// 注意：链接的字段只支持Float
    /// 
    /// </summary>
    public class AnimationLinkageCtrl : MonoBehaviour
    {

        //        [SerializeField]
        //        private ALCtrlLink[] ALCLinks;
        //
        //        private Component[] _linkedComponents;
        //        private object[] _linkedInfos;

        public bool Active = true;

        public float value;
        public bool UseValueForDisable;
        public float ValueForDisable;
        public bool UseTimeCurve;
        public AnimationCurve ValueCurve;
        public string linkScriptName;
        public string linkPorpName;
        public List<ALCMethodArg> linkMethodArgs;
        public ALCLinkPorpType linkPorpType;
        public bool isPublic;
        public bool isStatic;

        private UnityEngine.SceneManagement.Scene _currentScene;

        private Component _linkedComponent;
        private object _linkedInfo;

        float lifeTime;

        void OnEnable()
        {
            lifeTime = 0;

            //            if (ALCLinks != null && ALCLinks.Length > 0)
            //            {
            //                _linkedComponents = new Component[ALCLinks.Length];
            //                _linkedInfos = new object[ALCLinks.Length];
            //                linkComponent();
            //            }

            if (!string.IsNullOrEmpty(linkScriptName) && !string.IsNullOrEmpty(linkPorpName))
            {
                _currentScene = SceneManager.GetActiveScene();
                if (_currentScene != null)
                {
                    StartCoroutine(waitForLoadScene());
                }
            }
        }

        IEnumerator waitForLoadScene()
        {
            while (true)
            {
                if (_currentScene.isLoaded)
                {
                    linkComponent();
                    break;
                }
                yield return new WaitForEndOfFrame();
            }
        }

        void linkComponent()
        {
            List<GameObject> rootGameObjects = new List<GameObject>();
            _currentScene.GetRootGameObjects(rootGameObjects);
            if (rootGameObjects != null && rootGameObjects.Count > 0)
            {
                int i, len = rootGameObjects.Count;
                for (i = 0; i < len; i++)
                {

                    //                    int s, slen = ALCLinks.Length;
                    //                    for (s = 0; s < slen; s++)
                    //                    {
                    //                        _linkedComponents[s] = findComponentLoop(rootGameObjects[i].transform, s);
                    //                        if (_linkedComponents[s])
                    //                        {
                    //                            switch (ALCLinks[s].linkPorpType)
                    //                            {
                    //                                case ALCLinkPorpType.Property:
                    //                                    _linkedInfos[s] = _linkedComponents[s].GetType().GetProperty(ALCLinks[s].linkPorpName, (ALCLinks[s].isStatic ? BindingFlags.Static : BindingFlags.Instance) | BindingFlags.GetProperty | (ALCLinks[s].isPublic ? BindingFlags.Public : BindingFlags.NonPublic));
                    //                                    break;
                    //                                case ALCLinkPorpType.Method:
                    //                                    _linkedInfos[s] = _linkedComponents[s].GetType().GetMethod(ALCLinks[s].linkPorpName, (ALCLinks[s].isStatic ? BindingFlags.Static : BindingFlags.Instance) | BindingFlags.InvokeMethod | (ALCLinks[s].isPublic ? BindingFlags.Public : BindingFlags.NonPublic));
                    //                                    break;
                    //                                default:
                    //                                    _linkedInfos[s] = _linkedComponents[s].GetType().GetField(ALCLinks[s].linkPorpName, (ALCLinks[s].isStatic ? BindingFlags.Static : BindingFlags.Instance) | BindingFlags.GetField | (ALCLinks[s].isPublic ? BindingFlags.Public : BindingFlags.NonPublic));
                    //                                    break;
                    //                            }
                    //                            return;
                    //                        }
                    //                    }

                    _linkedComponent = findComponentLoop(rootGameObjects[i].transform);
                    if (_linkedComponent)
                    {
                        switch (linkPorpType)
                        {
                            case ALCLinkPorpType.Property:
                                _linkedInfo = _linkedComponent.GetType()
                                    .GetProperty(linkPorpName,
                                        (isStatic ? BindingFlags.Static : BindingFlags.Instance) |
                                        BindingFlags.GetProperty |
                                        (isPublic ? BindingFlags.Public : BindingFlags.NonPublic));
                                break;
                            case ALCLinkPorpType.Method:
                                _linkedInfo = _linkedComponent.GetType()
                                    .GetMethod(linkPorpName,
                                        (isStatic ? BindingFlags.Static : BindingFlags.Instance) |
                                        BindingFlags.InvokeMethod |
                                        (isPublic ? BindingFlags.Public : BindingFlags.NonPublic));
                                break;
                            default:
                                _linkedInfo = _linkedComponent.GetType()
                                    .GetField(linkPorpName,
                                        (isStatic ? BindingFlags.Static : BindingFlags.Instance) |
                                        BindingFlags.GetField |
                                        (isPublic ? BindingFlags.Public : BindingFlags.NonPublic));
                                break;
                        }
                        return;
                    }
                }
            }
        }

        Component findComponentLoop(Transform t)
        {
            Component cp = t.GetComponent(linkScriptName);
            if (!cp)
            {
                if (t.childCount > 0)
                {
                    int i, len = t.childCount;
                    for (i = 0; i < len; i++)
                    {
                        Transform st = t.GetChild(i);
                        cp = findComponentLoop(st);
                        if (cp)
                        {
                            return cp;
                        }
                    }
                    return null;
                }
                else
                {
                    return null;
                }
            }
            return cp;
        }

        //        Component findComponentLoop(Transform t, int index)
        //        {
        //            Component cp = t.GetComponent(ALCLinks[index].linkScriptName);
        //            if (!cp)
        //            {
        //                if (t.childCount > 0)
        //                {
        //                    int i, len = t.childCount;
        //                    for (i = 0; i < len; i++)
        //                    {
        //                        Transform st = t.GetChild(i);
        //                        cp = findComponentLoop(st, index);
        //                        if (cp)
        //                        {
        //                            return cp;
        //                        }
        //                    }
        //                    return null;
        //                }
        //                else
        //                {
        //                    return null;
        //                }
        //            }
        //            return cp;
        //        }

        void OnDisable()
        {
            //Time.timeScale = TimeContrl.getTimeSpeed();
            if (UseValueForDisable)
            {
                setValueUpdate();
            }
        }

        void Update()
        {
            //            if (_linkedComponents == null || _linkedComponents.Length == 0) return;

            if (_linkedComponent == null || _linkedInfo == null) return;

            lifeTime += Time.unscaledDeltaTime;

            //            int i, len = _linkedComponents.Length;
            //            for (i = 0; i < len; i++)
            //            {
            //
            //                if (_linkedComponents[i] != null && _linkedInfos[i] != null)
            //                {
            //
            //                    switch (ALCLinks[i].linkPorpType)
            //                    {
            //                        case ALCLinkPorpType.Property:
            //                            if (ALCLinks[i].UseTimeCurve)
            //                            {
            //                                (_linkedInfos[i] as PropertyInfo).SetValue(_linkedComponents[i], ALCLinks[i].ValueCurve.Evaluate(lifeTime), null);
            //                            }
            //                            else
            //                            {
            //                                (_linkedInfos[i] as PropertyInfo).SetValue(_linkedComponents[i], ALCLinks[i].value, null);
            //                            }
            //                            break;
            //                        case ALCLinkPorpType.Method:
            //                            (_linkedInfos[i] as MethodInfo).Invoke(_linkedComponents[i], createMethodArgObjs(ALCLinks[i].linkMethodArgs));
            //                            break;
            //                        default:
            //                            if (ALCLinks[i].UseTimeCurve)
            //                            {
            //                                (_linkedInfos[i] as FieldInfo).SetValue(_linkedComponents[i], ALCLinks[i].ValueCurve.Evaluate(lifeTime));
            //                            }
            //                            else
            //                            {
            //                                (_linkedInfos[i] as FieldInfo).SetValue(_linkedComponents[i], ALCLinks[i].value);
            //                            }
            //                            break;
            //                    }
            //
            //                }
            //
            //            }

            if (!Active) return;

            setValueUpdate();

        }

        private void setValueUpdate()
        {
            switch (linkPorpType)
            {
                case ALCLinkPorpType.Property:
                    if (UseTimeCurve)
                    {
                        (_linkedInfo as PropertyInfo).SetValue(_linkedComponent, ValueCurve.Evaluate(lifeTime), null);
                    }
                    else
                    {
                        (_linkedInfo as PropertyInfo).SetValue(_linkedComponent, value, null);
                    }
                    break;
                case ALCLinkPorpType.Method:
                    object[] args = createMethodArgObjs(linkMethodArgs);
                    (_linkedInfo as MethodInfo).Invoke(_linkedComponent, args);
                    break;
                default:
                    if (UseTimeCurve)
                    {
                        (_linkedInfo as FieldInfo).SetValue(_linkedComponent, ValueCurve.Evaluate(lifeTime));
                    }
                    else
                    {
                        (_linkedInfo as FieldInfo).SetValue(_linkedComponent, value);
                    }
                    break;
            }
        }

        private object[] createMethodArgObjs(List<ALCMethodArg> argsList)
        {

            if (argsList != null && argsList.Count > 0)
            {
                object[] objs = new object[argsList.Count];
                int i, len = argsList.Count;
                for (i = 0; i < len; i++)
                {

                    switch (argsList[i].valueType)
                    {
                        case ALCMethodArgType.Float:
                            objs[i] = float.Parse(argsList[i].value);
                            break;
                        case ALCMethodArgType.Boolean:
                            objs[i] = bool.Parse(argsList[i].value);
                            break;
                        default:
                            objs[i] = argsList[i].value;
                            break;
                    }

                    return objs;
                }
            }

            return null;
        }

    }

}

