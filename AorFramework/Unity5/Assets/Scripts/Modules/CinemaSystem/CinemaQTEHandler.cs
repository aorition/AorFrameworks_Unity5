using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

namespace YoukiaUnity.CinemaSystem
{

    public enum QTE_TYPE
    {
        /// <summary>
        /// 飞球QTE
        /// </summary>
        ballQTE,
        /// <summary>
        /// 点屏幕QTE
        /// </summary>
        clickQTE,
        /// <summary>
        /// 点按钮QTE
        /// </summary>
        btnQTE,
        /// <summary>
        /// 轨迹QTE
        /// </summary>
        trackQTE,
    }


    public enum QTE_TARGET
    {
        BATTLE,
        STORY,
    }

    public class CinemaQTEHandler : MonoBehaviour
    {
        
        public bool isDestroyed = false;
        private void Awake()
        {
            _innerEvent.AddListener(_OnInnerQTEEnd);
        }

        public void StartQTE(QTE_TYPE qteType, QTE_TARGET qte)
        {
            //            GlobalEvent.dispatch(eEventName.BattleQTEStart, qteType, QTE_TARGET.STORY);
            if (m_OnCinemaQTEStartEvent != null)
            {
                m_OnCinemaQTEStartEvent.Invoke(new object[] { qteType , qte });
            }

            if (onCinemaQTEStartEvent != null)
            {
                onCinemaQTEStartEvent(new object[] { qteType, qte });
            }
        }

        private void _OnInnerQTEEnd(object[] param)
        {

            if (m_OnCinemaQTEEndEvent != null)
            {
                m_OnCinemaQTEEndEvent.Invoke(param);
            }

            if (onCinemaQTEEndEvent != null)
            {
                Action<object[]> doing = onCinemaQTEEndEvent;
                doing(param);
            }

            isDestroyed = true;
            if (Application.isPlaying)
            {
                GameObject.Destroy(this);
            }
            else
            {
                GameObject.DestroyImmediate(this);
            }
        }
        
        public void setup(Action<object[]> onQTEEndEventDo)
        {
            onCinemaQTEEndEvent = onQTEEndEventDo;
        }

        private void OnDestroy()
        {

            _innerEvent.RemoveAllListeners();
            _innerEvent = null;

            onCinemaQTEStartEvent = null;
            onCinemaQTEEndEvent = null;

            if (OnCinemaQTEStartEvent != null)
            {
                OnCinemaQTEStartEvent.RemoveAllListeners();
            }
            OnCinemaQTEStartEvent = null;

            if (OnCinemaQTEEndEvent != null)
            {
                OnCinemaQTEEndEvent.RemoveAllListeners();
            }
            OnCinemaQTEEndEvent = null;

        }

        //-------- Events

        private class CinemaQTEInnerEvent : UnityEvent<object[]> { }
        private CinemaQTEInnerEvent _innerEvent = new CinemaQTEInnerEvent();

        [SerializeField]
        private CinemaQTEStartEvent m_OnCinemaQTEStartEvent = new CinemaQTEStartEvent();
        public CinemaQTEStartEvent OnCinemaQTEStartEvent
        {
            get { return m_OnCinemaQTEStartEvent; }
            set { m_OnCinemaQTEStartEvent = value; }
        }
        public Action<object[]> onCinemaQTEStartEvent;

        [SerializeField]
        private CinemaQTEEndEvent m_OnCinemaQTEEndEvent = new CinemaQTEEndEvent();
        public CinemaQTEEndEvent OnCinemaQTEEndEvent
        {
            get { return m_OnCinemaQTEEndEvent; }
            set { m_OnCinemaQTEEndEvent = value; }
        }
        public Action<object[]> onCinemaQTEEndEvent;
        
    }
    
    public class CinemaQTEStartEvent : UnityEvent<object[]> {}
    public class CinemaQTEEndEvent : UnityEvent<object[]> {}

}
