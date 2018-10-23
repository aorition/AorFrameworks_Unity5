using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Framework.Editor.Utility
{
    public class CameraBrowserWindow : EditorWindow
    {

        [MenuItem("FrameworkTools/辅助工具/场景相机查看器", false)]
        public static void init()
        {
            CameraBrowserWindow w = EditorWindow.GetWindow<CameraBrowserWindow>();
        }

        void Reset()
        {
            Awake();
        }

        void Awake()
        {
            //Debug.UiLog("Awake *******************");
            getCullingMaskStr();
            getList();
            initGUIStyles();
        }

        private GUIStyle _GUIStyle_CameraTitle;

        private Vector2 scrollPosition = Vector2.zero;
        private string search = string.Empty;

        private int updateNum = 0;
        private int updateDelayMax = 500;

        private Camera[] _Clist;
        private string[] _CMaskStr;

        private void getList()
        {

            List<GameObject> pAllObjects = GetAllObjectsInScene(false);
            List<Camera> c = new List<Camera>();

            foreach (GameObject g in pAllObjects)
            {
                Camera ca = g.GetComponent<Camera>();
                if (ca != null)
                {
                    c.Add(ca);
                }
            }

            c.Sort((Camera p1, Camera p2) => {
                return p1.depth.CompareTo(p2.depth);
            });

            c.Reverse();

            _Clist = c.ToArray();
        }

        public List<GameObject> GetAllObjectsInScene(bool bOnlyRoot)
        {
            GameObject[] pAllObjects = (GameObject[])Resources.FindObjectsOfTypeAll(typeof(GameObject));

            List<GameObject> pReturn = new List<GameObject>();

            foreach (GameObject pObject in pAllObjects)
            {
                if (bOnlyRoot)
                {
                    if (pObject.transform.parent != null)
                    {
                        continue;
                    }
                }

                if (pObject.hideFlags == HideFlags.NotEditable || pObject.hideFlags == HideFlags.HideAndDontSave)
                {
                    continue;
                }

                if (Application.isEditor)
                {
                    string sAssetPath = AssetDatabase.GetAssetPath(pObject.transform.root.gameObject);
                    if (!string.IsNullOrEmpty(sAssetPath))
                    {
                        continue;
                    }
                }


                pReturn.Add(pObject);
            }

            return pReturn;
        }

        private void getCullingMaskStr()
        {

            List<string> cl = new List<string>();

            for (int i = 0; i < 32; i++)
            {

                string n = LayerMask.LayerToName(i);

                if (n != "")
                {
                    cl.Add(n);
                }
            }

            _CMaskStr = cl.ToArray();
            cl.Clear();
        }

        void Update()
        {
            updateNum++;
            if (updateNum > updateDelayMax)
            {
                getCullingMaskStr();
                getList();
                updateNum = 0;
            }
        }

        private void initGUIStyles()
        {
            _GUIStyle_CameraTitle = new GUIStyle();
            _GUIStyle_CameraTitle.normal.textColor = Color.white;
            _GUIStyle_CameraTitle.fontSize = 12;
            _GUIStyle_CameraTitle.fontStyle = FontStyle.Bold;
        }

        void OnGUI()
        {

            if (_Clist == null) return;

            GUILayout.BeginVertical("HelpBox");

            GUILayout.BeginHorizontal();
            GUILayout.Label("场景中相机总数:");
            GUILayout.FlexibleSpace();
            GUILayout.Label(_Clist.Length + " ");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("查找:");
            search = EditorGUILayout.TextField(search);
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            GUILayout.Space(Screen.width * 0.65f);
            GUI.color = Color.yellow;
            GUILayout.Label("快捷操作:");
            GUILayout.FlexibleSpace();
            GUI.color = Color.green;
            if (GUILayout.Button(new GUIContent("+ MC", "为选中的相机对象添加增强工具(MaxCamera)"), "minibuttonleft"))
            {
                addMaxCamera();
            }
            GUI.color = Color.red;
            if (GUILayout.Button(new GUIContent("- ALL MC", "移除场景中所有相机的MaxCamera"), "minibuttonright"))
            {
                delAllMaxCamera();
            }
            GUI.color = Color.white;
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            //foreach (GUIStyle style in GUI.skin.customStyles)

            GUILayout.BeginVertical("PopupBackground");
            GUILayout.Space(5);
            foreach (Camera c in _Clist)
            {
                //过滤
                if (c.gameObject.name.ToLower().Contains(search.ToLower()))
                {
                    //


                    if (checkActived(c))
                    {
                        drawListCull(c);
                    }
                    else
                    {
                        drawListCull_Hiden(c);
                    }

                    GUILayout.Space(15);

                }
            }
            GUILayout.Space(5);
            GUILayout.EndVertical();

            GUILayout.EndScrollView();
        }

        private bool checkActived(Camera c)
        {

            if (c.enabled && c.gameObject.activeInHierarchy && c.gameObject.activeSelf)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void addMaxCamera()
        {
            /*
            if (Selection.gameObjects.Length < 1) {
                EditorUtility.DisplayDialog("提示", "您没有选择需要添加组件的对象", "关闭");
                return;
            }

            int i, length = Selection.gameObjects.Length;
            for (i = 0; i < length; i++) {
                Camera ca = Selection.gameObjects[i].GetComponent<Camera>();
                if (ca != null) {
                    MaxCamera mc = Selection.gameObjects[i].GetComponent<MaxCamera>();
                    if (mc == null) {
                        Selection.gameObjects[i].AddComponent<MaxCamera>();
                    }
                }
            }*/
        }

        private void delAllMaxCamera()
        {
            /*
            if (EditorUtility.DisplayDialog("警告", "此项操作将移除场景所有相机的MaxCamera组件!\n你确定要移除场景中所有的MaxCamera?", "确定", "取消")) {
                foreach (Camera c in _Clist) {
                    MaxCamera mc = c.gameObject.GetComponent<MaxCamera>();
                    if (mc != null) {
                        GameObject.DestroyImmediate(mc);
                    }
                }
            }*/
        }

        private void drawListCull_Hiden(Camera c)
        {
            GUILayout.BeginHorizontal("PreBackground", GUILayout.Height(80));

            GUI.color = Color.gray;

            GUILayout.BeginVertical(GUILayout.Width(Screen.width * 0.7f), GUILayout.MaxWidth(460), GUILayout.Height(80));
            if (GUILayout.Button(new GUIContent(c.gameObject.name, "快速选择"), "GUIEditor.BreadcrumbLeft"))
            {
                Selection.activeGameObject = c.gameObject;
                Selection.activeObject = c.gameObject;
            }
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            GUILayout.Space(50);//左边留白50
            GUILayout.BeginVertical();
            GUILayout.Label("Clear Flags", "MiniLabel");
            //c.clearFlags = (CameraClearFlags)
            EditorGUILayout.EnumPopup(c.clearFlags, "helpbox");
            GUILayout.EndVertical();
            GUILayout.BeginVertical();
            GUILayout.Label("Culling Mask", "MiniLabel");
            //c.cullingMask = 
            EditorGUILayout.MaskField(c.cullingMask, _CMaskStr, "helpbox");
            GUILayout.EndVertical();
            GUILayout.BeginVertical();
            GUILayout.Label("Depth", "MiniLabel");
            //c.depth = 
            EditorGUILayout.FloatField(c.depth, "helpbox");
            GUILayout.EndHorizontal();
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
            GUILayout.EndVertical();

            GUILayout.FlexibleSpace();

            GUILayout.BeginVertical(GUILayout.Height(60));
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            Component fl = c.GetComponent("FlareLayer");
            if (fl != null)
            {
                GUI.color = Color.green;
                if (GUILayout.Button(new GUIContent("FL", "has Flare Layer"), "helpbox"))
                {
                    Selection.activeGameObject = c.gameObject;
                    Selection.activeObject = c.gameObject;
                }
                GUI.color = Color.white;
            }
            GUILayer gl = c.GetComponent<GUILayer>();
            if (gl != null)
            {
                GUI.color = new Color(0.949f, 0.8f, 0.976f);
                if (GUILayout.Button(new GUIContent("GL", "has GUI Layer"), "helpbox"))
                {
                    Selection.activeGameObject = c.gameObject;
                    Selection.activeObject = c.gameObject;
                }
                GUI.color = Color.white;
            }
            AudioListener al = c.GetComponent<AudioListener>();
            if (al != null)
            {
                GUI.color = new Color(0.949f, 0.431f, 0.086f);
                if (GUILayout.Button(new GUIContent("AL", "has Audio Listener"), "helpbox"))
                {
                    Selection.activeGameObject = c.gameObject;
                    Selection.activeObject = c.gameObject;
                }
                GUI.color = Color.white;
            }
            if (c.orthographic)
            {
                GUI.color = Color.white;
                if (GUILayout.Button(new GUIContent("OC", "is Orthographic Camera"), "helpbox"))
                {
                    Selection.activeGameObject = c.gameObject;
                    Selection.activeObject = c.gameObject;
                }
                GUI.color = Color.white;
            }
            if (c.targetTexture != null)
            {
                GUI.color = new Color(0.203f, 0.671f, 0.992f);
                if (GUILayout.Button(new GUIContent("RT", "is Render Textrue Camera"), "helpbox"))
                {
                    Selection.activeObject = c.targetTexture;
                }
                GUI.color = Color.white;
            }
            /*
            MaxCamera mc = c.gameObject.GetComponent<MaxCamera>();
            if (mc != null) {
                GUI.color = Color.yellow;
                if (GUILayout.Button(new GUIContent("MAX", "has MAXCamera"), "helpbox")) {
                    Selection.activeGameObject = c.gameObject;
                    Selection.activeObject = c.gameObject;
                }
                GUI.color = Color.white;
            }
            */
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();



            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            GUI.color = Color.white;

            GUILayout.EndHorizontal();
        }

        private void drawListCull(Camera c)
        {
            GUILayout.BeginHorizontal("LockedHeaderBackground", GUILayout.Height(80));

            GUILayout.BeginVertical(GUILayout.Width(Screen.width * 0.7f), GUILayout.MaxWidth(600), GUILayout.Height(80));
            if (GUILayout.Button(new GUIContent(c.gameObject.name, "快速选择"), "GUIEditor.BreadcrumbLeft"))
            {
                Selection.activeGameObject = c.gameObject;
                Selection.activeObject = c.gameObject;
            }
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            GUILayout.Space(50);//左边留白50
            GUILayout.BeginVertical();
            GUILayout.Label("Clear Flags", "MiniLabel");
            //c.clearFlags = (CameraClearFlags)
            EditorGUILayout.EnumPopup(c.clearFlags, "TL tab left");
            GUILayout.EndVertical();
            GUILayout.BeginVertical();
            GUILayout.Label("Culling Mask", "MiniLabel");
            //c.cullingMask = 
            EditorGUILayout.MaskField(c.cullingMask, _CMaskStr, "TL tab left");
            GUILayout.EndVertical();
            GUILayout.BeginVertical();
            GUILayout.Label("Depth", "MiniLabel");
            //c.depth = 
            EditorGUILayout.FloatField(c.depth, "TL tab right");
            GUILayout.EndHorizontal();
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
            GUILayout.EndVertical();

            GUILayout.FlexibleSpace();

            GUILayout.BeginVertical(GUILayout.Height(60));
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            Component fl = c.GetComponent("FlareLayer");
            if (fl != null)
            {
                GUI.color = Color.green;
                if (GUILayout.Button(new GUIContent("FL", "has Flare Layer"), "helpbox"))
                {
                    Selection.activeGameObject = c.gameObject;
                    Selection.activeObject = c.gameObject;
                }
                GUI.color = Color.white;
            }
            GUILayer gl = c.GetComponent<GUILayer>();
            if (gl != null)
            {
                GUI.color = new Color(0.949f, 0.8f, 0.976f);
                if (GUILayout.Button(new GUIContent("GL", "has GUI Layer"), "helpbox"))
                {
                    Selection.activeGameObject = c.gameObject;
                    Selection.activeObject = c.gameObject;
                }
                GUI.color = Color.white;
            }
            AudioListener al = c.GetComponent<AudioListener>();
            if (al != null)
            {
                GUI.color = new Color(0.949f, 0.431f, 0.086f);
                if (GUILayout.Button(new GUIContent("AL", "has Audio Listener"), "helpbox"))
                {
                    Selection.activeGameObject = c.gameObject;
                    Selection.activeObject = c.gameObject;
                }
                GUI.color = Color.white;
            }
            if (c.orthographic)
            {
                GUI.color = Color.white;
                if (GUILayout.Button(new GUIContent("OC", "is Orthographic Camera"), "helpbox"))
                {
                    Selection.activeGameObject = c.gameObject;
                    Selection.activeObject = c.gameObject;
                }
                GUI.color = Color.white;
            }
            if (c.targetTexture != null)
            {
                GUI.color = new Color(0.203f, 0.671f, 0.992f);
                if (GUILayout.Button(new GUIContent("RT", "is Render Textrue Camera"), "helpbox"))
                {
                    Selection.activeObject = c.targetTexture;
                }
                GUI.color = Color.white;
            }
            /*
            MaxCamera mc = c.gameObject.GetComponent<MaxCamera>();
            if (mc != null) {
                GUI.color = Color.yellow;
                if (GUILayout.Button(new GUIContent("MAX", "has MAXCamera"), "helpbox")) {
                    Selection.activeGameObject = c.gameObject;
                    Selection.activeObject = c.gameObject;
                }
                GUI.color = Color.white;
            }
            */
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();



            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
        }

    }
}

