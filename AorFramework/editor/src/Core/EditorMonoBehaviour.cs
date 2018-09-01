using System;
using System.Collections;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Framework.Editor
{

    [InitializeOnLoad]
    public class FrameworkEditorBehaviour
    {
        static FrameworkEditorBehaviour()
        {
            Assembly assembly = Assembly.Load("UnityEditor.dll");
            var type = assembly.GetType("UnityEditor.EditorAssemblies");
            var method = type.GetMethod("SubclassesOf", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(Type) }, null);
            var e = method.Invoke(null, new object[] { typeof(FrameworkEditorBehaviour) }) as IEnumerable;
            foreach (Type editorMonoBehaviourClass in e)
            {
                method = editorMonoBehaviourClass.BaseType.GetMethod("OnFrameworkEditorBehaviour", BindingFlags.NonPublic | BindingFlags.Instance);
                if (method != null)
                {
                    method.Invoke(System.Activator.CreateInstance(editorMonoBehaviourClass), new object[0]);
                }
            }
        }

        private void OnFrameworkEditorBehaviour()
        {

            EditorApplication.update += Update;
            EditorApplication.hierarchyWindowChanged += OnHierarchyWindowChanged;
            EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGUI;
            EditorApplication.projectWindowChanged += OnProjectWindowChanged;
            EditorApplication.projectWindowItemOnGUI += ProjectWindowItemOnGUI;
            EditorApplication.modifierKeysChanged += OnModifierKeysChanged;


            // globalEventHandler
            EditorApplication.CallbackFunction function = () => OnGlobalEventHandler(Event.current);
            FieldInfo info = typeof(EditorApplication).GetField("globalEventHandler", BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic);
            EditorApplication.CallbackFunction functions = (EditorApplication.CallbackFunction)info.GetValue(null);
            functions += function;
            info.SetValue(null, (object)functions);


            EditorApplication.searchChanged += OnSearchChanged;

            EditorApplication.playmodeStateChanged += () => {
                if (EditorApplication.isPaused)
                {
                    OnPlaymodeStateChanged(PlayModeState.Paused);
                }
                if (EditorApplication.isPlaying)
                {
                    OnPlaymodeStateChanged(PlayModeState.Playing);
                }
                if (EditorApplication.isPlayingOrWillChangePlaymode)
                {
                    OnPlaymodeStateChanged(PlayModeState.PlayingOrWillChangePlaymode);
                }
            };

        }
        /// <summary>
        /// 每帧回调
        /// </summary>
        public virtual void Update()
        {
            //Debug.UiLog ("每一帧回调一次");
        }
        /// <summary>
        /// 层次视图发生变化
        /// </summary>
        public virtual void OnHierarchyWindowChanged()
        {
            //Debug.UiLog("层次视图发生变化");
        }
        public virtual void HierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
        {
            //	Debug.UiLog (string.Format ("{0} : {1} - {2}", EditorUtility.InstanceIDToObject (instanceID), instanceID, selectionRect));
        }
        /// <summary>
        /// 当资源视图发生变化
        /// </summary>
        public virtual void OnProjectWindowChanged()
        {
            //	Debug.UiLog ("当资源视图发生变化");
        }

        public virtual void ProjectWindowItemOnGUI(string guid, Rect selectionRect)
        {

        }
        /// <summary>
        /// "当触发键盘事件"
        /// </summary>
        public virtual void OnModifierKeysChanged()
        {
            //
        }
        /// <summary>
        /// 全局事件回调 + e;
        /// </summary>
        /// <param name="e"></param>
        public virtual void OnGlobalEventHandler(Event e)
        {

        }
        /// <summary>
        /// 搜索结果发生变化
        /// </summary>
        public virtual void OnSearchChanged()
        {
            //Debug.UiLog("搜索结果发生变化");
        }
        /// <summary>
        /// "游戏运行模式发生改变
        /// </summary>
        /// <param name="playModeState"></param>
        public virtual void OnPlaymodeStateChanged(PlayModeState playModeState)
        {
            //Debug.UiLog (， 点击 运行游戏 或者暂停游戏或者 帧运行游戏 按钮时触发: " + playModeState);
        }

        public enum PlayModeState
        {
            Playing,
            Paused,
            Stop,
            PlayingOrWillChangePlaymode
        }
    }

}

