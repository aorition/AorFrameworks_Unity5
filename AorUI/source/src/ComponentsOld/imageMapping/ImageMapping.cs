using System;
using System.Collections.Generic;
using ExoticUnity.App;
using ExoticUnity.GUI.AorUI.Core;
using ExoticUnity.Misc;
using UnityEngine;
using UnityEngine.UI;
using YoukiaBridge.Storage;
using ExoticUnity.Resource;

namespace ExoticUnity.GUI.AorUI.Components
{

    public class ImageMapping : AorUIComponent, IResLoadBeforeInstantiate
    {

        public void Next()
        {
            if (_index + 1 >= _imageList.Count)
            {
                Index = 0;
            }
            else
            {
                Index++;
            }
        }

        public void Previous()
        {
            if (_index - 1 < 0)
            {
                Index = _imageList.Count - 1;
            }
            else
            {
                Index--;
            }
        }

        [SerializeField, SetProperty("Index")]
        private int _index;
        public int Index {
            get { return _index; }
            set {
                if (Application.isEditor)
                {
                    _index = value;
                    _isDirty = true;
                }
                else
                {
                    if (value != _index)
                    {
                        _index = value;
                        _isDirty = true;
                    }
                }
            }
        }
        
        [NonSerialized]
        private List<Sprite> _imageList;
        public List<Sprite> ImageList {
            get { return _imageList; }
        }


        private List<ResourceRefKeeper> _imgRefKeepers; 

        private Image _image;

        [SerializeField]
        private List<string> _imagePaths; 
        
        //InitInterface
        private bool _isSerializedInit = false;
        public void OnResourcesLoad(Action finish)
        {
            if (_imagePaths != null && _imagePaths.Count > 0)
            {
                _imgRefKeepers = new List<ResourceRefKeeper>();
                _imageList = new List<Sprite>();
                SerializedInitLoop(0, finish);
            }
            else
            {
                _isSerializedInit = true;
                if (finish != null)
                {
                    finish();
                }
                _isDirty = true;
            }
        }

        private void SerializedInitLoop(int i, Action finish)
        {
            if (i < _imagePaths.Count)
            {
                AorUIAssetLoader.LoadSprite(_imagePaths[i], (s, o) =>
                {
                    _imageList.Add(s);
                    if (o != null && o.Length > 0)
                    {
                        ResourceRefKeeper kp = o[0] as ResourceRefKeeper;
                        if (kp != null)
                        {
                            _imgRefKeepers.Add(kp);
                        }
                    }
                    i ++;
                    SerializedInitLoop(i, finish);
                });
            }
            else
            {
                _isSerializedInit = true;
                if (finish != null)
                    finish();
                _isDirty = true;
            }
        }

        public override void OnAwake()
        {
            if (!_isSerializedInit)
            {
                OnResourcesLoad(null);
            }
            _image = GetComponent<Image>();
        }

        // Use this for initialization
        protected override void Initialization()
        {
            base.Initialization();
        }

        protected override void OnDestroy()
        {

            if (_imgRefKeepers != null)
            {
                _imgRefKeepers.Clear();
            }

            if (_imageList != null)
            {
                _imageList.Clear();
            }

            _image = null;

            base.OnDestroy();
        }

        protected override void DrawUI()
        {

            if (_imageList == null)
                return;

            if (_index >= 0 && _index < _imageList.Count)
            {

                if (_image == null)
                {
                    _image = GetComponent<Image>();
                }

                _image.sprite = _imageList[_index];
                _image.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                base.DrawUI();
            }
        }

        public void setNewImageList(List<Sprite> newImageList)
        {
            _imageList = newImageList;
            if (_imageList != null && _imageList.Count > 0)
            {
                _isDirty = true;
            }
        }
        
    }

}


