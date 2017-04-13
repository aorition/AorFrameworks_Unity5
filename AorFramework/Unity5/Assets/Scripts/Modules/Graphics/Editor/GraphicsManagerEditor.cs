using UnityEngine;
using UnityEditor;
using YoukiaUnity.Graphics;
using YoukiaUnity.Graphics.FastShadowProjector;

[CustomEditor(typeof(GraphicsManager))]

public class GraphicsManagerEditor : UnityEditor.Editor
{
    static Color green = new Color(0.4f, 1, 0.8f, 1);

    public override void OnInspectorGUI()
    {
        GraphicsManager mgr = (GraphicsManager)target;

        //  base.OnInspectorGUI();

        EditorGUILayout.HelpBox("视效管理器，请保持游戏中常驻，请勿修改子节点和内部数据", MessageType.Warning);

        GUI.color = green;

        if (Application.isPlaying)
        {
            string cameraName = mgr.CurrentSubCamera == null ? "无" : mgr.CurrentSubCamera.gameObject.name;

            EditorGUILayout.HelpBox("当期渲染摄像机:" + cameraName + " FPS:" + mgr.FPS, MessageType.Info);
         //   EditorGUILayout.HelpBox("当前模式:"+ mgr.CurrentRenderMode, MessageType.Info);
        }

        else
        {
            EditorGUILayout.HelpBox("编辑器模式中", MessageType.Info);
        }


        int GraphicsQuality = EditorGUILayout.IntSlider(new GUIContent("当前视效等级", "Unity预设 Quality Level "), mgr.GraphicsQuality, 0, QualitySettings.names.Length - 1);

        GraphicsManager.RtSize finalSize = (GraphicsManager.RtSize)EditorGUILayout.EnumPopup(new GUIContent("显示分辨率", "相对屏幕分辨率大小的1，0.5，0.25"), mgr.FinalDrawRtSize);


      //  bool  fog = EditorGUILayout.Toggle(new GUIContent("大气雾效", "天空盒颜色作为雾效"), mgr.Fog);

        float CameraNearClip = EditorGUILayout.Slider("摄像机近裁面:", mgr.CameraNearClip, 0f, 100);
       // float CameraMiddleClip = EditorGUILayout.Slider("摄像机近远景分界:", mgr.CameraMiddleClip, CameraNearClip, 10000);
        float CameraFarClip = EditorGUILayout.Slider("摄像机远裁面:", mgr.CameraFarClip, mgr.CameraNearClip, 20000);

     


        GlobalProjectorManager.ShadowType ShadowType = (GlobalProjectorManager.ShadowType) EditorGUILayout.EnumPopup(new GUIContent("阴影类型", "帧数大杀器！"), mgr.ShadowType);
        GlobalProjectorManager.ShadowResolutions ShadowResolution = (GlobalProjectorManager.ShadowResolutions)EditorGUILayout.EnumPopup(new GUIContent("阴影类型", "帧数大杀器！"), mgr.ShadowResolution);


      //  bool DistantView = EditorGUILayout.Toggle(new GUIContent("远景绘制", "不绘制远景能提高帧数"), mgr.ShowLandscape);
        bool AlwayReDrawFarCamera = false;
        GraphicsManager.RtSize Size = GraphicsManager.RtSize.Full;
 

        //实际游戏配置表中读取，不直接在此设置
        float FogDestance = EditorGUILayout.Slider("大气雾距离:", mgr.FogDestance, 0f, 100);
        float FogDestiy = EditorGUILayout.Slider("大气雾密度:", mgr.FogDestiy, 0f, 100);

        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
           // mgr.Fog = fog;
            mgr.ShadowType = ShadowType;
            mgr.ShadowResolution = ShadowResolution;
          //  mgr.ShowLandscape = DistantView;
            mgr.AlwayReDrawFarTexture = AlwayReDrawFarCamera;

            mgr.FogDestiy = FogDestiy;
            mgr.FogDestance = FogDestance;

            mgr.CameraNearClip = CameraNearClip;
           // mgr.CameraMiddleClip = CameraMiddleClip;
            mgr.CameraFarClip = CameraFarClip;
            mgr.GraphicsQuality = GraphicsQuality;
            mgr.FinalDrawRtSize = finalSize;
        }


    }
}



