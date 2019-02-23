#pragma warning disable
using System;
using UnityEngine;
using System.Collections.Generic;
using Framework.Extends;

namespace Framework.Graphic.CustomLight
{
    /// <summary>
    /// 自定义灯光管理器
    /// </summary>
    [ExecuteInEditMode]
    [ImageEffectAllowedInSceneView]
    public class CustomLightManager
    {

        public const string UseCustomLightDef = "USECUSTOMLIGHT";

        private const string DefineSupportName = "CustomLightingSupport";

        private static CustomLightSupports _support;

        public static CustomLightManager _instance;
        public static CustomLightManager Instance
        {
            get {
                if (_instance == null)
                {
                    _instance = new CustomLightManager();
                }
                _instance._checkSupport();
                return _instance;
            }
        }

        public static bool isExist {
            get { return _instance != null; }
        }

        //--------------------------------------------------

        public bool AutoDispose = true;

        public event Action OnDispose;

        private CustomLightManager()
        {
            //
        }

        private bool _isDisposed = false;
        public virtual void Dispose()
        {

            resetLightsPrams();

            if (_support)
            {
                if(_support.gameObject) _support.gameObject.Dispose();
                _support = null;
            }

            _directionalLightInfo = null;

            _listsInfos.Clear();
            _tempLightInfos.Clear();

            if (OnDispose != null)
            {
                Action tmp = OnDispose;
                tmp();
                OnDispose = null;
            }

            if (_instance == this)
            {
                _instance = null;
            }

            _isDisposed = true;
        }

        private void _checkSupport()
        {


            if (_support && _support.gameObject)
            {
                if (_support.isEdtorMode == Application.isPlaying)
                {
                    GameObject del = _support.gameObject;
                    _support = null;
                    del.Dispose();
                }
            }

            if (!_support)
            {
                _support = GameObject.FindObjectOfType<CustomLightSupports>();
                if (!_support)
                {
                    _support = new GameObject(DefineSupportName).AddComponent<CustomLightSupports>();
                    _support.gameObject.hideFlags = HideFlags.DontSave;
                    CustomLightSupports.Main = _support.gameObject;
                    if (!Application.isPlaying)
                    {
                        _support.isEdtorMode = true;
                    }
                }
            }
        }

        protected CustomLightInfo _directionalLightInfo;
        protected readonly List<CustomLightInfo> _listsInfos = new List<CustomLightInfo>();

        /// <summary>
        /// 添加一个灯光到灯光系统里
        /// </summary>
        /// <param name="lightInfo">需要启用照明的灯光</param>
        public virtual void AddLight(CustomLightInfo lightInfo)
        {
            if (!_listsInfos.Contains(lightInfo))
            {
                _listsInfos.Add(lightInfo);
                if (lightInfo.lightType == LightType.Directional)
                    _directionalLightInfo = lightInfo;
            }
        }

        /// <summary>
        /// 从系统中移除一个灯光
        /// </summary>
        /// <param name="lightInfo">需要移除的灯光</param>
        public void RemoveLight(CustomLightInfo lightInfo)
        {
            if (_listsInfos.Contains(lightInfo))
                _listsInfos.Remove(lightInfo);
        }

        public virtual void Update()
        {
            if (_listsInfos.Count == 0)
            {

                if (AutoDispose)
                    Dispose();
                else
                {
                    _directionalLightInfo = null;
                    resetLightsPrams();
                }
                return;
            }

            calculateLightsPrams();
            updateLightsPrams();

        }

        private Vector4 rot;
        private Vector4 color;
        private Vector4 lightX = Vector4.zero;
        private Vector4 lightY = Vector4.zero;
        private Vector4 lightZ = Vector4.zero;
        private Vector4 Atten = Vector4.zero;

        private readonly List<CustomLightInfo> _tempLightInfos = new List<CustomLightInfo>();

        protected virtual bool verifyCustomLightInfo(CustomLightInfo info)
        {
            return (
                        info && info.gameObject
                        && info.gameObject.activeInHierarchy && info.gameObject.activeSelf
                        && info.intensity > 0
                        && info.enabled
                    );
        }

        protected virtual void calculateLightsPrams()
        {
            _tempLightInfos.Clear();
            foreach (CustomLightInfo info in _listsInfos)
            {
                if (info.lightType == LightType.Directional) continue;
                if (verifyCustomLightInfo(info))
                {
                    _tempLightInfos.Add(info);
                }
            }

            if (_directionalLightInfo == null && _tempLightInfos.Count == 0)
            {
                resetLightsPrams();
            }
            else
            {

                int num = Mathf.Min(_tempLightInfos.Count, 4);

                lightX = Vector4.zero;
                lightY = Vector4.zero;
                lightZ = Vector4.zero;
                Atten = Vector4.zero;

                for (int i = 0; i < num; i++)
                {

                    Shader.SetGlobalVector("_CustomLightColor" + i, _tempLightInfos[i].lightColor * _tempLightInfos[i].intensity);

                    if (i == 0)
                    {
                        lightX.x = _tempLightInfos[i].transform.position.x;
                        lightY.x = _tempLightInfos[i].transform.position.y;
                        lightZ.x = _tempLightInfos[i].transform.position.z;
                        Atten.x = _tempLightInfos[i].range * _tempLightInfos[i].range;

                    }
                    if (i == 1)
                    {
                        lightX.y = _tempLightInfos[i].transform.position.x;
                        lightY.y = _tempLightInfos[i].transform.position.y;
                        lightZ.y = _tempLightInfos[i].transform.position.z;
                        Atten.y = _tempLightInfos[i].range * _tempLightInfos[i].range;
                    }
                    if (i == 2)
                    {
                        lightX.z = _tempLightInfos[i].transform.position.x;
                        lightY.z = _tempLightInfos[i].transform.position.y;
                        lightZ.z = _tempLightInfos[i].transform.position.z;
                        Atten.z = _tempLightInfos[i].range * _tempLightInfos[i].range;
                    }

                    if (i == 3)
                    {
                        lightX.w = _tempLightInfos[i].transform.position.x;
                        lightY.w = _tempLightInfos[i].transform.position.y;
                        lightZ.w = _tempLightInfos[i].transform.position.z;
                        Atten.w = _tempLightInfos[i].range * _tempLightInfos[i].range;
                    }


                }

                rot = Vector4.zero;
                color = Vector4.zero;

                if (_directionalLightInfo != null)
                {
                    rot.x = -_directionalLightInfo.transform.forward.x;
                    rot.y = -_directionalLightInfo.transform.forward.y;
                    rot.z = -_directionalLightInfo.transform.forward.z;
                    rot.w = _directionalLightInfo.intensity;

                    color.x = _directionalLightInfo.lightColor.r;
                    color.y = _directionalLightInfo.lightColor.g;
                    color.z = _directionalLightInfo.lightColor.b;
                    color.w = _directionalLightInfo.lightColor.a * _directionalLightInfo.intensity;
                }
            }

        }

        protected virtual void updateLightsPrams()
        {
            Shader.SetGlobalVector("_CustomLightPosX", lightX);
            Shader.SetGlobalVector("_CustomLightPosY", lightY);
            Shader.SetGlobalVector("_CustomLightPosZ", lightZ);
            Shader.SetGlobalVector("_CustomLightAtten", Atten);

            Shader.SetGlobalVector("_DirectionalLightDir", rot);
            Shader.SetGlobalVector("_DirectionalLightColor", color);

            //编辑器模式拿不到roleLight,用sunlight
           // if (!Application.isPlaying)
             //   Shader.SetGlobalColor("_RoleDirectionalLightDir", rot);
        }

        protected virtual void resetLightsPrams()
        {
            Shader.SetGlobalVector("_CustomLightColor0", Vector4.zero);
            Shader.SetGlobalVector("_CustomLightColor1", Vector4.zero);
            Shader.SetGlobalVector("_CustomLightColor2", Vector4.zero);
            Shader.SetGlobalVector("_CustomLightColor3", Vector4.zero);

            Shader.SetGlobalVector("_CustomLightPosX", Vector4.zero);
            Shader.SetGlobalVector("_CustomLightPosY", Vector4.zero);
            Shader.SetGlobalVector("_CustomLightPosZ", Vector4.zero);
            Shader.SetGlobalVector("_CustomLightAtten", Vector4.zero);

            //dirlight
            Shader.SetGlobalVector("_DirectionalLightDir", Vector4.zero);
            Shader.SetGlobalVector("_DirectionalLightColor", Vector4.zero);

            //编辑器模式拿不到roleLight,用sunlight
           //if (!Application.isPlaying)
             //   Shader.SetGlobalColor("_RoleDirectionalLightDir", Vector4.zero);

        }

    }
}