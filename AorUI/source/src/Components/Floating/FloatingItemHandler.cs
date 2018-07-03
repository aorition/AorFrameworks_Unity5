using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Framework.UI
{
    public class FloatingItemHandler : MonoBehaviour
    {

        public static bool GlobalUpdateToggle = true;

        public static FloatingItemHandler CreateFloatingItemHandler(Transform floatingTarget, RectTransform displayUITarget, Camera workCamera, Canvas canvas, bool UseScaleByZDistance = false, float ItemScaleDef = 100f)
        {
            return CreateFloatingItemHandler(floatingTarget, displayUITarget, workCamera, canvas, Vector2.zero, Vector3.zero, UseScaleByZDistance, ItemScaleDef);
        }

        public static FloatingItemHandler CreateFloatingItemHandler(Transform floatingTarget, RectTransform displayUITarget, Camera workCamera, Canvas canvas, Vector2 srceenOffset, Vector3 worldOffset, bool UseScaleByZDistance = false, float ItemScaleDef = 100f)
        {
            FloatingItemHandler fih = displayUITarget.gameObject.AddComponent<FloatingItemHandler>();
            fih.FloatingTarget = floatingTarget;
            fih.SrceenOffset = srceenOffset;
            fih.WorldOffset = worldOffset;
            fih.WorkCamera = workCamera;
            fih.canvas = canvas;
            fih.UseScaleByZDistance = UseScaleByZDistance;
            fih.ItemScaleDef = ItemScaleDef;
            return fih;
        }

        [SerializeField]
        private Transform _FloatingTarget;
        public Transform FloatingTarget {
            set {
                _FloatingTarget = value;
                itemLastPosition = Vector3.zero;
            }

            get { return _FloatingTarget; }
        }
        public Vector2 SrceenOffset;
        public Vector3 WorldOffset;
        public Camera WorkCamera;
        public CanvasGroup _CanvasGroup;
        public Canvas canvas;

        public float DistanceLimit = 200f;

        public bool UpdateToggle = true;
        public bool UseScaleByZDistance = false;
        public float ItemScaleDef = 100f;
        public Vector2 UIScale = Vector2.one;
        public object param;
        private Vector3 cameraLastPosition;
        private Vector3 itemLastPosition;
        private float cameraLastFOV;

        [HideInInspector, NonSerialized]
        public RectTransform _rectT;
        private bool _isInit = false;
        public bool IsInit {
            get { return _isInit; }
        }

        private void Start()
        {
            if(!_isInit) __init();
        }

        private void Awake()
        {
            _rectT = gameObject.GetComponent<RectTransform>();

            //            _rectT.anchorMin = Vector2.zero;
            //            _rectT.anchorMax = Vector2.zero;
            //            _rectT.anchoredPosition = Vector2.zero;

            _rectT.anchorMin = new Vector2(0.5f, 0.5f);
            _rectT.anchorMax = new Vector2(0.5f, 0.5f);

        }

        private void __init()
        {
            _CanvasGroup = gameObject.GetComponent<CanvasGroup>();
            if (_CanvasGroup == null)
            {
                _CanvasGroup = gameObject.AddComponent<CanvasGroup>();
            }

            if (FloatingTarget != null && WorkCamera != null)
            {
                UpdatePosOnScreen();
            }

            _isInit = true;
        }
        void OnEnable()
        {
            if (_isInit && FloatingTarget != null && WorkCamera != null)
            {
                // _CanvasGroup.alpha = 0;
                UpdatePosOnScreen();
            }
        }
        
        public void UpdatePosOnScreen()
        {

            cameraLastPosition = WorkCamera.transform.position;
            itemLastPosition = FloatingTarget.position;
            cameraLastFOV = WorkCamera.fieldOfView;

            Vector3 posDir = (cameraLastPosition - itemLastPosition).normalized;
            float dis = Vector3.Distance(cameraLastPosition, itemLastPosition);
            if (posDir != Vector3.zero && Vector3.Dot(WorkCamera.transform.forward, posDir) < 0 && dis < DistanceLimit)
            {
                Vector3 ScreenPos = Vector3.zero;
                if (canvas && (canvas.renderMode == RenderMode.ScreenSpaceCamera || canvas.renderMode == RenderMode.WorldSpace) &&  canvas.worldCamera)
                {
                    //这个是UGUI坐标转世界坐标的算法

//                    Vector3 scr = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, obj.transform.position);
//                    scr.z = 0;
//                    scr.z = Mathf.Abs(Camera.main.transform.position.z - transform.position.z);
//                    currentObj.transform.position = Camera.main.ScreenToWorldPoint(scr);


                    Vector2 pos;
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, WorkCamera.WorldToScreenPoint(itemLastPosition), canvas.worldCamera, out pos);
                    ScreenPos = pos;
                }
                else
                {
                    ScreenPos = WorkCamera.WorldToScreenPoint(itemLastPosition + WorldOffset) - new Vector3((float)Screen.width / 2, (float)Screen.height / 2,0);
                }
                if (UseScaleByZDistance)
                {
                    float sd = ItemScaleDef / Vector3.Distance(cameraLastPosition, FloatingTarget.transform.position);
                    _rectT.localScale = UIScale * sd;
                    _rectT.anchoredPosition = new Vector2(ScreenPos.x + SrceenOffset.x * UIScale.x * sd,
                        ScreenPos.y + SrceenOffset.y * UIScale.y * sd);
                }
                else
                {
                    _rectT.localScale = UIScale;
                    _rectT.anchoredPosition = new Vector2(ScreenPos.x + SrceenOffset.x * UIScale.x,
                        ScreenPos.y + SrceenOffset.y * UIScale.y);
                }
                _CanvasGroup.alpha = 1;
                _CanvasGroup.interactable = true;
                _CanvasGroup.blocksRaycasts = true;

                //
                if (m_onVisible != null)
                {
                    m_onVisible.Invoke(this);
                }

            }
            else
            {
                _CanvasGroup.alpha = 0;
                _CanvasGroup.interactable = false;
                _CanvasGroup.blocksRaycasts = false;

                //
                if (m_onInvisible != null)
                {
                    m_onInvisible.Invoke(this);
                }

            }
        }

        private void LateUpdate()
        {
            if (FloatingTarget != null && WorkCamera != null && GlobalUpdateToggle && UpdateToggle && _updateDirty())
            {
                UpdatePosOnScreen();
            }
        }

        private bool _updateDirty()
        {
            return !(cameraLastPosition == WorkCamera.transform.position
                     && itemLastPosition == FloatingTarget.position 
                     && cameraLastFOV.Equals( WorkCamera.fieldOfView));
        }

        //---------------------------------------

        [Serializable]
        public class FloatItemVisibleEvent : UnityEvent<FloatingItemHandler> { }
        [Serializable]
        public class FloatItemInvisibleEvent : UnityEvent<FloatingItemHandler> { }


        [SerializeField]
        private FloatItemVisibleEvent m_onVisible = new FloatItemVisibleEvent();
        public FloatItemVisibleEvent onVisible {
            get { return m_onVisible; }
            set { m_onVisible = value; }
        }

        [SerializeField]
        private FloatItemInvisibleEvent m_onInvisible = new FloatItemInvisibleEvent();
        public FloatItemInvisibleEvent onInvisible {
            get { return m_onInvisible; }
            set { m_onInvisible = value; }
        }

    }

}
