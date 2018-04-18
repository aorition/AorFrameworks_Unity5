using System;

using UnityEngine;
using UnityEngine.UI;

namespace ExoticUnity.GUI.AorUI.Components
{
    public class HPTip : MonoBehaviour
    {

        public static HPTip createHPTip(GameObject prefab)
        {
            HPTip ht = prefab.GetComponent<HPTip>();
            if (ht == null)
            {
                ht = prefab.AddComponent<HPTip>();
            }
            return ht;
        }

        public enum HPTipTypeEnum
        {
            addHP,
            subHP
        }

        public float SurvivalLimtTime = 2.0f;

        public Action DestroyCallBack;

        public HPTipTypeEnum HPTipType = HPTipTypeEnum.subHP;

        public string ValueString
        {
            set
            {
                if (_text != null)
                    _text.text = value;
            }

        }

        private Animator _animator;
        private Text _text;

        private void Awake()
        {
            _text = transform.FindChild("Text").GetComponent<Text>();
            _animator = _text.GetComponent<Animator>();
        }

        private void Start()
        {

            if (HPTipType == HPTipTypeEnum.subHP)
            {
                _text.color = Color.red;
            }
            else
            {
                _text.color = Color.green;
            }
        }

        private bool _isSurvival;
        private float _SurvivalTime;
        private void OnEnable()
        {
            _SurvivalTime = 0;
            _isSurvival = true;
            Start();
        }

        void Update()
        {

            if (_isSurvival)
            {
                _SurvivalTime += Time.deltaTime;
                if (_SurvivalTime >= SurvivalLimtTime)
                {
                    Destroy();
                }
            }

        }

        private void OnDestroy()
        {
            _text = null;
            _animator = null;
        }

        public void Destroy()
        {
            _isSurvival = false;
            if (DestroyCallBack != null)
            {
                DestroyCallBack();
            }
        }

    }
}
