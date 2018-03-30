using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YoukiaUnity.CinemaSystem;

/// <summary>
/// Cinema 动态对象 材质修改器
/// 
/// 挂载到CinemaCharacter/CinemaObject上对其内部Material做操作. 须配合CinemaMaterialPropSetter
/// 
/// </summary>
public class CinemaMaterialHandler : MonoBehaviour
{
    private CinemaMaterialPropSetter[] _propSetters;
    private List<Material> _materials; 
    private GameObject catchedObject;


    void Awake()
    {
		_getPropSetters ();
        _getCatchedObejct();
    }

    void Update()
    {

        if (!catchedObject)
        {
            _getCatchedObejct();
        }

        if (catchedObject)
        {

            if (_materials == null)
            {
                _getMaterialsInCatched();
            }

            if (_propSetters != null && _propSetters.Length > 0 && _materials != null && _materials.Count > 0)
            {
                for (int i = 0; i < _propSetters.Length; i++)
                {

                    CinemaMaterialPropSetter propSetter = _propSetters[i];

                    for (int j = 0; j < _materials.Count; j++)
                    {
                        Material mat = _materials[j];

                        if (propSetter.type == CinemaMaterialPropSetterType.Color)
                        {
                            mat.SetColor(propSetter.ShaderPropName, propSetter.color);
                        }
                        else
                        {
                            mat.SetFloat(propSetter.ShaderPropName, propSetter.Value);
                        }
                    }

                }
            }

        }
    }

    private void _getPropSetters()
    {
        _propSetters = GetComponents<CinemaMaterialPropSetter>();
    }

    private void _getCatchedObejct()
    {

        CinemaCharacter cCharacter = gameObject.GetComponent<CinemaCharacter>();
        if (cCharacter)
        {
            catchedObject = cCharacter.LoadedGameObject;
        }
        else
        {
            CinemaObject cObj = gameObject.GetComponent<CinemaObject>();
            if (cObj)
            {
                catchedObject = cObj.LoadedGameObject;
            }
        }
    }

    private void _getMaterialsInCatched()
    {
        Renderer[] renderers = catchedObject.GetComponentsInChildren<Renderer>();
        if (renderers != null && renderers.Length > 0)
        {
            _materials = new List<Material>();

            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i].materials != null && renderers[i].materials.Length > 0)
                {
                    _materials.AddRange(renderers[i].materials);
                }
            }

        }
    }

}
