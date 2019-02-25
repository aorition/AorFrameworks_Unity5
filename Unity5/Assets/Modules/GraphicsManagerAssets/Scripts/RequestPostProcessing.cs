using System.Collections;
using System.Collections.Generic;
using Framework.Graphic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class RequestPostProcessing : MonoBehaviour
{

    [SerializeField]
    private PostProcessingProfile _profile;

    private PostProcessingProfile _orginalProfile;

    private PostProcessingBehaviour _PPBehaviour;
    private bool _isInit = false;
    
    private void OnEnable()
    {
        if (!_isInit)
        {
            Init();
        }
    }

    private void Init()
    {
        GraphicsManager.RequestGraphicsManager(() =>
        {
            if (_isInit) return;
            Camera cam = GraphicsManager.instance.GetSubCamera("FinalOutput");
            if (cam)
            {
                _PPBehaviour = cam.GetComponent<PostProcessingBehaviour>();

                if (_PPBehaviour)
                {
                    _orginalProfile = _PPBehaviour.profile;
                    _PPBehaviour.profile = _profile;
                    _isInit = true;
                }
            }
        });

    }

    private void Update()
    {
        if (!_isInit)
        {
            Init();
        }
    }

    private void OnDisable()
    {
        if (_isInit)
        {
            _PPBehaviour.profile = _orginalProfile;
            _isInit = false;
        }
    }
}
