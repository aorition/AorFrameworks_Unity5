using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using AorBaseUtility;

namespace Framework.Graphic.editor
{
    [CustomEditor(typeof(RenderTextureCaptcher), true)]
    public class RenderTextureCaptcherEditor : UnityEditor.Editor
    {

        private RenderTextureCaptcher _target;

        private void Awake()
        {
            _target = target as RenderTextureCaptcher;
        }

        private void OnDisable()
        {
            //if (m_captchedTex)
            //{
            //    if (Application.isPlaying)
            //    {
            //        Destroy(m_captchedTex);
            //    }
            //    else
            //    {
            //        DestroyImmediate(m_captchedTex);
            //    }
            //    m_captchedTex = null;
            //}
        }

        //private Texture2D m_captchedTex;
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (_target.CapturedRT)
            {
                GUILayout.Space(12);

                GUILayout.BeginVertical("box");
                {
                    GUILayout.Space(5);

                    GUILayout.Label("RenderTexture Captcher Debug Tool");

                    GUILayout.Space(5);

                    RenderTextureCaptcherDebugHander hander = _target.GetComponent<RenderTextureCaptcherDebugHander>();

                    if (!hander)
                    {
                        if(GUILayout.Button("Create Captched Texture2D"))
                        {
                            hander = _target.gameObject.AddComponent<RenderTextureCaptcherDebugHander>();
                            hander.hideFlags = HideFlags.HideInInspector;
                            hander.CaptchedTexture2D = _target.GetCapturedTexture2D();
                        }
                    }
                    else
                    {
                        GUILayout.Label("Captched Texture2D");
                        hander.CaptchedTexture2D = (Texture2D)EditorGUILayout.ObjectField("", hander.CaptchedTexture2D, typeof(Texture2D), true);
                        if (GUILayout.Button("Save Texture2D To Project"))
                        {

                            string path = EditorUtility.SaveFilePanelInProject("Save", "RTCaptcherTex", "png", "", "");
                            if (hander.CaptchedTexture2D && !string.IsNullOrEmpty(path))
                            {
                                byte[] bytes = hander.CaptchedTexture2D.EncodeToPNG();
                                if(AorIO.SaveBytesToFile(path, bytes))
                                {
                                    AssetDatabase.Refresh();
                                    EditorUtility.DisplayDialog("success", "Texture2D save to " + path, "ok");
                                }
                            }
                        }
                        if (GUILayout.Button("Clear Captched Texture2D"))
                        {
                            try
                            {
                                if (Application.isPlaying)
                                {
                                    Destroy(hander);
                                }
                                else
                                {
                                    DestroyImmediate(hander);
                                }
                            }
                            finally
                            {
                                //do nothing
                            }
                        }
                    }

                    GUILayout.Space(5);
                }
                GUILayout.EndVertical();
                
            }

        }

    }
}
