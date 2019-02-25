using UnityEngine;
using System;
using UnityEngine.UI;
using DG.Tweening;

public class MaskUIManager : MonoBehaviour
{

    private static MaskUIManager _instance;
    public static MaskUIManager Instance
    {
        get { return _instance; }
    }

    public static bool HasInstance
    {
        get { return _instance != null; }
    }

    public static MaskUIManager CreateInstance(GameObject target)
    {
        if (!_instance)
        {
            if (!target) target = new GameObject("MaskUIView");
            _instance = target.GetComponent<MaskUIManager>();
            if (!_instance) _instance = target.AddComponent<MaskUIManager>();
            if (!target.activeSelf) target.SetActive(true);
        }
        return _instance;
    }

    //-------------------------------------------------

    private Tweener _tweener;
    private Image _image;
    
    private void Awake()
    {
        if (!_instance)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            GameObject.Destroy(this);
        }

        //--------------------

        _image = GetComponent<Image>();
        if (!_image) _image = gameObject.AddComponent<Image>();
        //初始化
        _image.rectTransform.pivot = new Vector2(0.5f, 0.5f);
        _image.rectTransform.anchorMin = Vector2.zero;
        _image.rectTransform.anchorMax = Vector2.one;
        _image.rectTransform.localEulerAngles = Vector3.zero;
        _image.rectTransform.localScale = Vector3.one;
        _image.rectTransform.anchoredPosition3D = Vector3.zero;
        _image.rectTransform.sizeDelta = Vector3.zero;

        _image.color = new Color(0, 0, 0, 0);
        _image.enabled = false;
    }

    private void OnDestroy()
    {
        if (_tweener != null)
        {
            _tweener.Kill();
            _tweener = null;
        }
        _image = null;
    }

    public void EnableMask(float duration = 0.5f, Action onFinish = null) {

        if (_tweener != null) {
            _tweener.Kill();
        };

       // Debug.Log("MaskManager.EnableMask start"); 
        _image.color = new Color(0, 0, 0, 0);
        if(!_image.enabled) _image.enabled = true;

        _tweener = _image.DOColor(new Color(0, 0, 0, 0.75f), duration).OnComplete(()=> {
            if(onFinish != null) onFinish();
         //   Debug.Log("MaskManager.EnableMask finish");
        });

    }

    public void DisableMask(float duration = 0.5f, Action onFinish = null) {

        if (_tweener != null) _tweener.Kill();
      //  Debug.Log("MaskManager.DisableMask start");
        _image.color = new Color(0, 0, 0, 0.75f);
        _tweener = _image.DOColor(new Color(0, 0, 0, 0), duration).OnComplete(() => {
            if (_image.enabled) _image.enabled = false;
            if (onFinish != null) onFinish();
           // Debug.Log("MaskManager.DisableMask finish");
        });

    }

}
