using System;
using System.Collections.Generic;
using AorFramework.AorTile3D.runtime;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.AorTile3D.Scripts.runtimeEditor
{
    public class BuildSceneView : MonoBehaviour
    {

        public Action<BuildSceneView> OnBuildBtnClick;

        private Button cancelBtm;
        private Button buildBtn;

        private InputField ifd_UnitSizeU;
        private InputField ifd_UnitSizeV;
        private InputField ifd_UnitSizeW;

        public float[] GetIFDUnitSize()
        {
            if (!string.IsNullOrEmpty(ifd_UnitSizeU.text)
                && !string.IsNullOrEmpty(ifd_UnitSizeV.text)
                && !string.IsNullOrEmpty(ifd_UnitSizeW.text)
                )
            {
                try
                {
                    float u = float.Parse(ifd_UnitSizeU.text);
                    float v = float.Parse(ifd_UnitSizeV.text);
                    float w = float.Parse(ifd_UnitSizeW.text);

                    return new [] {u, v, w};

                }
                catch (Exception ex)
                {
                    AorTile3DManager.ThrowError(ex);
                    return null;
                }
            }
            return null;
        }

        private InputField ifd_MapSizeU;
        private InputField ifd_MapSizeV;

        public int[] GetIFDMapSize()
        {
            if (!string.IsNullOrEmpty(ifd_MapSizeU.text)
                && !string.IsNullOrEmpty(ifd_MapSizeU.text)
                )
            {
                try
                {
                    int u = int.Parse(ifd_MapSizeU.text);
                    int v = int.Parse(ifd_MapSizeV.text);

                    return new[] { u, v};

                }
                catch (Exception ex)
                {
                    AorTile3DManager.ThrowError(ex);
                    return null;
                }
            }
            return null;
        }

        private void Awake()
        {
            cancelBtm = transform.Find("win/cancelBtn").GetComponent<Button>();
            cancelBtm.onClick.AddListener(Dispose);

            Transform innerT = transform.Find("win/innerArea");

            ifd_MapSizeU = innerT.Find("label0_sub0_input").GetComponent<InputField>();
            ifd_MapSizeV = innerT.Find("label0_sub1_input").GetComponent<InputField>();

            ifd_UnitSizeU = innerT.Find("label1_sub0_input").GetComponent<InputField>();
            ifd_UnitSizeV = innerT.Find("label1_sub1_input").GetComponent<InputField>();
            ifd_UnitSizeW = innerT.Find("label1_sub2_input").GetComponent<InputField>();

            buildBtn = innerT.Find("buildBtn").GetComponent<Button>();

            setupUI();
        }

        private void setupUI()
        {

            ifd_MapSizeU.text = "100";
            ifd_MapSizeV.text = "100";

            ifd_UnitSizeU.text = "1";
            ifd_UnitSizeV.text = "1";
            ifd_UnitSizeW.text = "1";

            buildBtn.onClick.AddListener(Build);
        }

        private void Build()
        {
            if (OnBuildBtnClick != null) OnBuildBtnClick(this);
            Dispose();
        }

        private void Dispose()
        {
            GameObject.Destroy(gameObject);
        }

    }
}
