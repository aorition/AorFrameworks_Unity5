using System.Collections;
using System.Collections.Generic;
using AorBaseUtility;
using Framework.Graphic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GraphicsManager))]
public class GraphicsManagerEditor : Editor
{

    private GraphicsManager _target;

    private void Awake()
    {
        _target = target as GraphicsManager;
    }

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        //
        if (Application.isPlaying)
        {
            GUILayout.Space(12);
            GUILayout.BeginVertical("box");
            GUILayout.Label("GraphicsManager运行时管理器");
            GUILayout.EndVertical();
            GUILayout.Space(12);
            _draw_baseParams();
            GUILayout.Space(10);
            _draw_visualCameraList();

            GUILayout.Space(10);
            _draw_debugTools();
            Repaint();
        }
        else
        {
            base.OnInspectorGUI();
        }

    }

    private void _draw_baseParams()
    {
        GUILayout.BeginVertical("box");

        GUILayout.Space(5);
        GUILayout.Label("BaseParams:");
        GUILayout.Space(5);

        //UseFixedUpdate
        bool usefix = (bool)_target.ref_GetField_Inst_Public("UseFixedUpdate");
        bool nUsefix = EditorGUILayout.Toggle("Use Fixed Update", usefix);
        if (!nUsefix.Equals(usefix))
        {
            _target.ref_SetField_Inst_Public("UseFixedUpdate", nUsefix);
        }

        //AllowVisualCameraParamCover
        bool AllowVisualCameraParamCover = (bool)_target.ref_GetField_Inst_Public("AllowVisualCameraParamCover");
        bool nAllowVisualCameraParamCover = EditorGUILayout.Toggle("Allow Visual Camera Param Cover", AllowVisualCameraParamCover);
        if (!nAllowVisualCameraParamCover.Equals(AllowVisualCameraParamCover))
        {
            _target.ref_SetField_Inst_Public("AllowVisualCameraParamCover", nAllowVisualCameraParamCover);
        }

        //_mainCamera
        UnityEngine.Object mainCamera = (UnityEngine.Object)_target.ref_GetField_Inst_NonPublic("_mainCamera");
        UnityEngine.Object nMainCamera = EditorGUILayout.ObjectField("Main Camera", mainCamera, typeof (Camera), true);
        if (nMainCamera && !nMainCamera.Equals(mainCamera))
        {
            _target.ref_SetField_Inst_NonPublic("_mainCamera", nMainCamera);
        }

        //_UIEffRoot
        UnityEngine.Object UIEffRoot = (UnityEngine.Object)_target.ref_GetField_Inst_NonPublic("_UIEffRoot");
        UnityEngine.Object nUIEffRoot = EditorGUILayout.ObjectField("UIEffRoot", UIEffRoot, typeof(RectTransform), true);
        if (nUIEffRoot && !nUIEffRoot.Equals(UIEffRoot))
        {
            _target.ref_SetField_Inst_NonPublic("_UIEffRoot", nUIEffRoot);
        }

        GUILayout.EndVertical();
    }

    private List<VisualCamera> _delVList = new List<VisualCamera>();
    private List<VisualCamera> _vclist = new List<VisualCamera>();
    private VisualCamera _currVCam;

    private void _updateVCdatas()
    {
        _currVCam = (VisualCamera)_target.ref_GetField_Inst_NonPublic("_currentVisualCamera");
        List<VisualCamera> vcList = (List<VisualCamera>)_target.ref_GetField_Inst_NonPublic("_visualCameras");

        if (vcList != null && vcList.Count > 0)
        {

            int i, len = vcList.Count;
            for (i = 0; i < len; i++)
            {
                if (!_vclist.Contains(vcList[i]))
                {
                    _vclist.Add(vcList[i]);
                }
            }

            len = _vclist.Count;
            for (i = 0; i < len; i++)
            {
                if (!vcList.Contains(_vclist[i]))
                {
                    _delVList.Add(vcList[i]);
                }
            }

            len = _delVList.Count;
            for (i = 0; i < len; i++)
            {
                _vclist.Remove(_delVList[i]);
            }

        }
        else
        {
            _vclist.Clear();
        }

    }

    private void _draw_visualCameraList()
    {
        GUILayout.BeginVertical("box");

        GUILayout.Space(5);
        GUILayout.Label("虚拟相机列表:");
        GUILayout.Space(5);

        _updateVCdatas();

        if (_vclist != null && _vclist.Count > 0)
        {
            for (int i = 0; i < _vclist.Count; i++)
            {
                VisualCamera vc = _vclist[i];
                _draw_vcListItem(vc, _currVCam);
            }
        }
        else
        {
            GUILayout.Label("暂无VisualCamera");
        }

        GUILayout.Space(5);

        GUILayout.EndVertical();
    }

    private void _draw_vcListItem(VisualCamera vcam, VisualCamera cVcam)
    {

        if (vcam.Equals(cVcam))
        {
            GUI.color = Color.yellow;
        }
        else
        {
            GUI.color = Color.white;
        }

        GUILayout.BeginVertical("box");

        GUILayout.BeginHorizontal();

        GUILayout.Space(Mathf.Max(5,Screen.width * 0.1f));

        //name
        if (GUILayout.Button(vcam.gameObject.name,GUILayout.Width(80)))
        {
            Selection.activeGameObject = vcam.gameObject;
        }

        GUILayout.FlexibleSpace();

        //level
        GUILayout.Label(new GUIContent("Level"));
        GUILayout.Space(Mathf.Max(5, Screen.width * 0.1f));
        float nlevel = EditorGUILayout.FloatField(vcam.Level,GUILayout.Width(65));
        if (!nlevel.Equals(vcam.Level))
        {
            vcam.Level = nlevel;
        }

        //solo
        bool nSolo = EditorGUILayout.ToggleLeft("Solo", vcam.Solo,GUILayout.Width(45));
        if (!nSolo.Equals(vcam.Solo))
        {
            vcam.Solo = nSolo;
        }

        GUILayout.Space(Mathf.Max(5, Screen.width * 0.1f));

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        GUILayout.Space(Mathf.Max(40, Screen.width*0.4f));
        //solo
        GUILayout.Label(new GUIContent("Interpolation"));
        GUILayout.Space(Mathf.Max(5, Screen.width * 0.1f));
        float nInterpolation = EditorGUILayout.Slider(vcam.Interpolation, 0, 1);
        if (!nInterpolation.Equals(vcam.Interpolation))
        {
            vcam.Interpolation = nInterpolation;
        }

        GUILayout.EndHorizontal();

        GUILayout.EndVertical();

        GUI.color = Color.white;

    }

    private float _d_CameraShake_time = 1f;
    private Vector3 _d_CameraShake_power = new Vector3(0.2f, 0.3f, 0.2f);
    private int _d_CameraShake_vibrato = 100;

    private Color _d_Fade_Color = Color.black;
    private float _d_Fade_Time = 1f;

    private void _draw_debugTools()
    {
        GUILayout.BeginVertical("box");

        GUILayout.Space(5);
        GUILayout.Label("Debug工具列表:");
        GUILayout.Space(5);

        //-----------------------------------------

        if (_target.UIEffRoot)
        {

            GUILayout.BeginVertical("box");
            GUILayout.Label(new GUIContent("CameraShake", "相机震动"));
            _d_CameraShake_time = EditorGUILayout.FloatField("time", _d_CameraShake_time);
            _d_CameraShake_power = EditorGUILayout.Vector3Field("power", _d_CameraShake_power);
            _d_CameraShake_vibrato = EditorGUILayout.IntField("vibrato", _d_CameraShake_vibrato);

            if (GUILayout.Button("Do"))
            {
                _target.Effect.CameraShake(_d_CameraShake_time, _d_CameraShake_power.x, _d_CameraShake_power.y,
                    _d_CameraShake_power.z, _d_CameraShake_vibrato);
            }
            GUILayout.EndVertical();

            GUILayout.Space(5);

            GUILayout.BeginVertical("box");
            GUILayout.Label(new GUIContent("FadeIn", "相机淡入淡出"));
            _d_Fade_Color = EditorGUILayout.ColorField("Color", _d_Fade_Color);
            _d_Fade_Time = EditorGUILayout.FloatField("Time", _d_Fade_Time);

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("FadeIn"))
            {
                _target.Effect.FadeIn(_d_Fade_Color, _d_Fade_Time);
            }
            if (GUILayout.Button("FadeOut"))
            {
                _target.Effect.FadeOut(_d_Fade_Color, _d_Fade_Time);
            }
            GUILayout.EndHorizontal();
            if (GUILayout.Button("ClearFade"))
            {
                _target.Effect.ClearFade();
            }
            GUILayout.EndVertical();

        }

        GUILayout.Space(5);

        //-----------------------------------------

        GUILayout.Space(5);

        GUILayout.EndVertical();
    }

}
