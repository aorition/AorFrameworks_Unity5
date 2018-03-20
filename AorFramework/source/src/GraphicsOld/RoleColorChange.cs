using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YoukiaUnity.View;

public class RoleColorChange : MonoBehaviour
{
    private GameObject mParent;
    private GameObject mBody;

    private Renderer roleRenderer;
    private Material curlMaterial;

    private Color originalColor;

    private PivotPointData pivotPointData;
    private bool _init = false;

    void OnEnable()
    {
        init();
    }

    private void init()
    {
		if (!transform.parent) return;

        mParent = transform.parent.parent.gameObject;
        if(mParent)
            pivotPointData = mParent.GetComponent<PivotPointData>();
        Debug.Log(mParent.name);

        if (!pivotPointData)
        {
            pivotPointData = mParent.GetComponentInChildren<PivotPointData>();
        }
        if(pivotPointData)
            mBody = pivotPointData.GetPivot<GameObject>("body");

        if (mBody)
        {
            roleRenderer = mBody.GetComponent<SkinnedMeshRenderer>();
        }
        if (roleRenderer)
        {
            curlMaterial = roleRenderer.material;
        }

        if (curlMaterial && curlMaterial.HasProperty("_Color"))
        {
            originalColor = curlMaterial.GetColor("_Color");
            curlMaterial.SetColor("_Color", Color.red);
        }

        _init = true;

    }


    void OnDisable()
    {
        if (roleRenderer && curlMaterial)
        {
            curlMaterial.SetColor("_Color", originalColor);
        }

        _init = false;
    }

    private void Update()
    {
        if (!_init) init();
    }

}
