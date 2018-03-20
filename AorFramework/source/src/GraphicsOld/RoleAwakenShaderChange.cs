using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoleAwakenShaderChange : MonoBehaviour {

    private Renderer roleRenderer;
    private Material originalMaterial;
    private Material battleMaterial;

    private float originalUiLight;



    void Awake()
    {
        roleRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        roleRenderer.transform.parent.eulerAngles = new Vector3(0, 180, 0);
    }


    void OnEnable()
    {
        if (!roleRenderer)
            return;

        if (!originalMaterial)
        {
            originalMaterial = roleRenderer.material;
            originalUiLight = Shader.GetGlobalFloat("_UILight");
        }

        if (!battleMaterial)
        {
            battleMaterial = new Material(originalMaterial);
            battleMaterial.shader = Shader.Find("Custom/Light/Diffuse - Toon - Normal");   
        }

        if (battleMaterial)
        {
            roleRenderer.material = battleMaterial;

            Shader.SetGlobalFloat("_UILight", 1);
        }
    }


    void OnDisable()
    {
        if (roleRenderer && originalMaterial)
        {
            roleRenderer.material = originalMaterial;

            Shader.SetGlobalFloat("_UILight", originalUiLight);
        }       
    }


}
