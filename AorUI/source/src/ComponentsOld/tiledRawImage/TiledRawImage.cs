using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using ExoticUnity.App;
using ExoticUnity.GUI.AorUI.Debug;

namespace ExoticUnity.GUI.AorUI.Components
{


    /// <summary>
    /// 这个类,在功能上还有一些不周全. 待优化.
    /// </summary>
    [ExecuteInEditMode]
    public class TiledRawImage : RawImage, IMonoSwitch
    {

        [SerializeField]
        protected Vector2 _TexUnitScale = Vector2.one;

        public Vector2 TexUnitScale
        {
            get { return _TexUnitScale; }
            set
            {
                if (value != _TexUnitScale)
                {
                    _TexUnitScale = value;
                    isDirty = true;
                }
            }
        }

        protected Vector2 _Offset;
        public Vector2 Offset
        {
            get { return _Offset; }
            set
            {
                if (value != _Offset)
                {
                    _Offset = value;
                    isDirty = true;
                }
            }
        }

        [SerializeField]
        protected Vector2 _TexUnitSize;
        public Vector2 TexUnitSize
        {
            get { return _TexUnitSize; }
            set
            {
                if (value != _TexUnitSize)
                {
                    _TexUnitSize = value;
                    isDirty = true;
                }
            }
        }

        public void useNativeTexSize()
        {
            TexUnitSize = new Vector2(texture.width, texture.height);
        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            DrawUI();
        }

        protected void DrawUI()
        {
            if (canvas != null)
            {
                Rect rect = rectTransform.rect;
                this.uvRect = new Rect(_Offset.x,
                    _Offset.y,
                    rect.width / ((_TexUnitSize.x == 0 ? 1 : _TexUnitSize.x) * (_TexUnitScale.x == 0 ? 1 : _TexUnitScale.x)) * canvas.scaleFactor,
                    rect.height / ((_TexUnitSize.y == 0 ? 1 : _TexUnitSize.y) * (_TexUnitScale.y == 0 ? 1 : _TexUnitScale.y)) * canvas.scaleFactor
                    );
                isDirty = false;
            }
        }

        [SerializeField]
        protected bool isDirty;

        //------------------ IMonoSwitch 实现 ---------------
        #region IMonoSwitch 实现

        //--------------------------------------------------- 容易修改的区域




        protected override void Awake()
        {
            base.Awake();
            MonoSwitch.PublicStaticProcess(this);
        }

        protected override void Start()
        {
            base.Start();
        }

        //--------------------------------------------------- 容易修改的区域 End



        public virtual string ExportData()
        {
            return "";
        }
        public virtual void ImportData(string stringData)
        {

        }
        public void SetOtherParma(string target, string stringData)
        {
            if (target == GetType().ToString())
            {
                ImportData(stringData);
            }
        }

        public virtual void OnAwake()
        {

        }

        public void RemoveCall(string className)
        {
            if (className == GetType().ToString())
            {
                OnRemoved();
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();


        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }

        protected virtual void OnRemoved()
        {

        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        public virtual void Initialization()
        {

        }

        protected virtual void OnUpdate()
        {
            if (isDirty)
            {
                DrawUI();
                isDirty = false;
            }
        }

        protected virtual void OnEditorUpdate()
        {


        }
        protected virtual void OnEditorAwake()
        {


        }

        protected virtual void OnEditorStart()
        {

        }

        private void Update()
        {
            if (!Application.isPlaying)
            {
                OnEditorUpdate();

            }
            else
            {
                if (ExoticApplication.IsInited)
                    OnUpdate();
            }

        }

        void IMonoSwitch.OnEditorAwake()
        {
            //   throw new NotImplementedException();
        }




        #endregion
        //------------------ IMonoSwitch 实现 --------------- End
    }
}
