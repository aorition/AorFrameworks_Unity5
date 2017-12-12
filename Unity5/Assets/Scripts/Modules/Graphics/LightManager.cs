using UnityEngine;
using System.Collections.Generic;

namespace YoukiaUnity.Graphics
{

    /// <summary>
    /// 灯光管理器
    /// </summary>
    [ExecuteInEditMode]
    public class LightManager : MonoBehaviour
    {

        public static LightManager _instance;

        //是否是在编辑器模式下的实例 **灯光系统无时无刻都是在运行的，必须区分编辑器和运行时的单例**
        public bool isEditorInstance = false;

        private bool isDirty = false;


        public LightInfo DirectionalLight
        {
            get
            {
                if (Application.isEditor)
                {
                    return editorSunLight;
                }
                else
                {
                    return GraphicsManager.GetInstance().SunLight;
                }

            }
        }



        List<LightInfo> lights = new List<LightInfo>();
        private LightInfo editorSunLight;
        public static bool isExist
        {

            get { return _instance != null; }

        }

        /// <summary>
        /// 管理器单利
        /// </summary>
        public static LightManager Instance
        {
            get
            {

                if (_instance == null)
                {

                    if (!Application.isPlaying)
                    {
                        GameObject obj = GameObject.Find("LightManager");
                        if (obj == null)
                        {
                            obj = new GameObject("LightManager");
                            _instance = obj.AddComponent<LightManager>();
                        }
                        else
                        {
                            _instance = obj.GetComponent<LightManager>();

                        }

                        _instance.isEditorInstance = true;

                    }
                    else
                    {
                        GameObject obj = new GameObject("LightManager");
                        _instance = obj.AddComponent<LightManager>();
                        DontDestroyOnLoad(obj);

                    }




                }


                return _instance;
            }


        }
        /// <summary>
        /// 添加一个灯光到灯光系统里
        /// </summary>
        /// <param name="light">需要启用照明的灯光</param>
        public void AddLight(LightInfo light)
        {
            if (light.light.type == LightType.Directional)
            {
                editorSunLight = light;
                return;
            }

            if (!lights.Contains(light))
                lights.Add(light);


        }

        /// <summary>
        /// 从系统中移除一个灯光
        /// </summary>
        /// <param name="light">需要移除的灯光</param>
        public void RemoveLight(LightInfo light)
        {
            if (light.light.type == LightType.Directional)
            {
                return;
            }



            if (lights.Contains(light))
                lights.Remove(light);

            //    lights.Add(light);


        }

        private void setNoLightColor()
        {
            Shader.SetGlobalVector("_CustomLightColor0", Vector4.zero);
            Shader.SetGlobalVector("_CustomLightColor1", Vector4.zero);
            Shader.SetGlobalVector("_CustomLightColor2", Vector4.zero);
            Shader.SetGlobalVector("_CustomLightColor3", Vector4.zero);

            Shader.SetGlobalVector("_CustomLightPosX", Vector4.zero);
            Shader.SetGlobalVector("_CustomLightPosY", Vector4.zero);
            Shader.SetGlobalVector("_CustomLightPosZ", Vector4.zero);
            Shader.SetGlobalVector("_CustomLightAtten", Vector4.zero);

            //dirlight
            Shader.SetGlobalVector("_DirectionalLightDir", Vector4.zero);
            Shader.SetGlobalVector("_DirectionalLightColor", Vector4.zero);
        }


        List<LightInfo> lists = new List<LightInfo>();
        Vector4 rot;
        Vector4 color;
        Vector4 lightX = Vector4.zero;
        Vector4 lightY = Vector4.zero;
        Vector4 lightZ = Vector4.zero;
        Vector4 Atten = Vector4.zero;

        private void UpdateShader()
        {
            if (DirectionalLight == null && lights.Count == 0 && isDirty)
            {

                setNoLightColor();
                isDirty = false;
                return;
            }



            lists.Clear();

            foreach (LightInfo Info in lights)
            {
                if (Info == null || Info.gameObject == null || Info.gameObject.activeInHierarchy == false || Info.gameObject.activeSelf == false || Info.light.intensity <= 0)
                    continue;

                //  if (!Info.isVisible)
                //    continue;

                lists.Add(Info);

            }

            if (DirectionalLight == null && lists.Count == 0 && isDirty)
            {
                setNoLightColor();
                isDirty = false;

            }
            else
            {

                int num = Mathf.Min(lists.Count, 4);
                lightX = Vector4.zero;
                lightY = Vector4.zero;
                lightZ = Vector4.zero;
                Atten = Vector4.zero;

                for (int i = 0; i < num; i++)
                {

                    Shader.SetGlobalVector("_CustomLightColor" + i, lists[i].light.color * lists[i].light.intensity);

                    if (i == 0)
                    {
                        lightX.x = lists[i].transform.position.x;
                        lightY.x = lists[i].transform.position.y;
                        lightZ.x = lists[i].transform.position.z;
                        Atten.x = lists[i].light.range * lists[i].light.range;

                    }
                    if (i == 1)
                    {
                        lightX.y = lists[i].transform.position.x;
                        lightY.y = lists[i].transform.position.y;
                        lightZ.y = lists[i].transform.position.z;
                        Atten.y = lists[i].light.range * lists[i].light.range;
                    }
                    if (i == 2)
                    {
                        lightX.z = lists[i].transform.position.x;
                        lightY.z = lists[i].transform.position.y;
                        lightZ.z = lists[i].transform.position.z;
                        Atten.z = lists[i].light.range * lists[i].light.range;
                    }

                    if (i == 3)
                    {
                        lightX.w = lists[i].transform.position.x;
                        lightY.w = lists[i].transform.position.y;
                        lightZ.w = lists[i].transform.position.z;
                        Atten.w = lists[i].light.range * lists[i].light.range;
                    }


                }

                Shader.SetGlobalVector("_CustomLightPosX", lightX);
                Shader.SetGlobalVector("_CustomLightPosY", lightY);
                Shader.SetGlobalVector("_CustomLightPosZ", lightZ);
                Shader.SetGlobalVector("_CustomLightAtten", Atten);

                if (DirectionalLight != null)
                {
                    rot.x = -DirectionalLight.transform.forward.x;
                    rot.y = -DirectionalLight.transform.forward.y;
                    rot.z = -DirectionalLight.transform.forward.z;
                    rot.w = DirectionalLight.light.intensity;

                    color.x = DirectionalLight.light.color.r;
                    color.y = DirectionalLight.light.color.g;
                    color.z = DirectionalLight.light.color.b;
                    color.w = DirectionalLight.light.color.a;


                    Shader.SetGlobalVector("_DirectionalLightDir", rot);
                    Shader.SetGlobalVector("_DirectionalLightColor", color);

                }

                isDirty = true;
            }

        }

        // Update is called once per frame
        void Update()
        {
            if (isEditorInstance && Application.isPlaying)
            {
                DestroyImmediate(gameObject);
                return;
            }


            if (!isEditorInstance && !Application.isPlaying)
            {
                DestroyImmediate(gameObject);
                return;
            }



            UpdateShader();
        }
    }
}


