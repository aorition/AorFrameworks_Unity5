using System.Collections;
using UnityEngine;

namespace Framework.Graphic
{

    [RequireComponent(typeof (MeshRenderer))]
    public class RoleGlowEffectHandler : MonoBehaviour
    {
        private static readonly string RoleGlowLayerDefine = "RoleGlowLayer";

        private void Awake()
        {
            //检查SubCamera组件-> RoleGlowEffectCam 是否存在
            GraphicsManager.RequestGraphicsManager(() =>
            {
                Camera camera = GraphicsManager.instance.GetSubCamera("RoleGlowEffectCam");
                if (!camera)
                {

                    GameObject gecGo = new GameObject("RoleGlowEffectCam");
                    camera = gecGo.AddComponent<Camera>();
                }

                RoleGlowEffectCamera rgCamera = camera.gameObject.GetComponent<RoleGlowEffectCamera>();
                if (!rgCamera) camera.gameObject.AddComponent<RoleGlowEffectCamera>();
            });

            _renderer = GetComponent<MeshRenderer>();
        }

        public Color GlowColor = Color.white;
        public float GlowPower = 1f;

        private MeshRenderer _renderer;

        private int _srcLayerNum;
//
//    private List<int> _matIdx;
//    private Shader _vcShader;
//    private List<string> _srcSdNameList;

        private void OnEnable()
        {
            //修改对象Layer
            _srcLayerNum = gameObject.layer;
            gameObject.layer = LayerMask.NameToLayer(RoleGlowLayerDefine);

            //
//        replaceSknMats();
        }

        private void Update()
        {
            if (_renderer)
            {
                for (int i = 0; i < _renderer.materials.Length; i++)
                {
                    _renderer.materials[i].SetFloat("_fPower", GlowPower);
                    _renderer.materials[i].SetColor("_fColor", GlowColor);
                }
            }
        }

        /// <summary>
        /// 因为SkeletonRenderer会强制指定预设材质,强行替换材质或者shader会产生不可预计的问题,故放弃自动替换材质的功能
        /// </summary>
//    private void replaceSknMats()
//    {
//        if (_matIdx == null)
//        {
//
//            if (!_vcShader)
//            {
//                _vcShader = Shader.Find("Spine/Skeleton-VC");
//            }
//
//            _renderer = gameObject.GetComponent<MeshRenderer>();
//            if (_renderer)
//            {
//                _matIdx = new List<int>();
//                _srcSdNameList = new List<string>();
//                for (int i = 0; i < _renderer.sharedMaterials.Length; i++)
//                {
//                    if (_renderer.sharedMaterials[i].shader.name.ToLower().StartsWith("spine/"))
//                    {
//                        _matIdx.Add(i);
//                        string sky = _renderer.sharedMaterials[i].shader.name;
//                        _srcSdNameList.Add(sky);
//                        _renderer.sharedMaterials[i].shader = _vcShader;
//                    }
//                }
//
//            }
//        }
//        else
//        {
//            for (int i = 0; i < _matIdx.Count; i++)
//            {
//                int idx = _matIdx[i];
//                _renderer.sharedMaterials[idx].shader = _vcShader;
//            }
//        }
//    }

        private void OnDisable()
        {
            //恢复对象原始Layer
            gameObject.layer = _srcLayerNum;
            //
//        for (int i = 0; i < _matIdx.Count; i++)
//        {
//            int idx = _matIdx[i];
//            _renderer.sharedMaterials[idx].shader = Shader.Find(_srcSdNameList[i]);
//        }
        }

    }
}