using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Framework.Editor
{
    /// <summary>
    /// Mesh 编辑界面
    /// </summary>
    public class MeshEditToolWindow : EditorWindow
    {
        [MenuItem("Test/test")]
        public static MeshEditToolWindow init()
        {
            MeshEditToolWindow window = EditorWindow.GetWindow<MeshEditToolWindow>();
            window.titleContent = new GUIContent("MeshEditToolWindow");
            return window;
        }

        //-----------------------------------------------

        private enum METWindowStatus
        {
            Default,
            Modify,
        }

        private METWindowStatus _status;

        private GameObject _currentSelcet;
        private GameObject[] _Selcets;

        private void _resetSelection()
        {
            _currentSelcet = null;
            _Selcets = null;
        }

        private void OnGUI()
        {

            //------------ Test code

            if (GUILayout.Button("test"))
            {
                string aa = EditorResourcesUtility.lightSkinSourcePath;
            }

            //------------

            if (_status == METWindowStatus.Default)
            {
                _currentSelcet = Selection.activeGameObject;
                _Selcets = Selection.gameObjects;
            }


            if (_status == METWindowStatus.Modify)
            {
                if(!_currentSelcet)
                    _currentSelcet = Selection.activeGameObject;

                if(_Selcets != null)
                    _Selcets = Selection.gameObjects;

                if (_currentSelcet && _Selcets != null && _Selcets.Length == 1)
                {
                    //单选

                    //验证
                    Renderer renderer = _currentSelcet.GetComponent<Renderer>();
                    if (!renderer)
                    {
                        _resetSelection();
                        return;
                    }



                }
                else if (_currentSelcet && _Selcets != null && _Selcets.Length > 1)
                {
                    //多选

                    //Todo ...

                }

            }

        }


    }
}
