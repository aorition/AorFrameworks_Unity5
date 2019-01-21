namespace com.emadkhezri.vertextool
{
    using UnityEngine;
    using UnityEditor;

    public class VertexToolWindow : EditorWindow
    {
        VertexToolData _data = new VertexToolData();
        public VertexInfoPanel _infoPanel;
        VertexToolSettingsPanel _settingsPanel;
        VertexToolSceneManager _sceneManager;

        [MenuItem("Window/Vertex Tool")]
        public static void ShowVertexInfo()
        {
            VertexToolWindow vertexInfoWindow = CreateInstance<VertexToolWindow>();
            vertexInfoWindow.titleContent = new GUIContent("Vertex Tool");
            vertexInfoWindow.Show();
        }

        private void OnEnable()
        {
            _infoPanel = new VertexInfoPanel(_data);
            _settingsPanel = new VertexToolSettingsPanel(_data);
            _sceneManager = new VertexToolSceneManager(_data);
            SceneView.onSceneGUIDelegate += _sceneManager.OnSceneGUI;
        }

        private void OnDisable()
        {
            SceneView.onSceneGUIDelegate -= _sceneManager.OnSceneGUI;
            GameObject[] pAllObjects = (GameObject[])Resources.FindObjectsOfTypeAll(typeof(GameObject));
            for (int i = 0; i < pAllObjects.Length; i++)
            {
                if (pAllObjects[i].name.Contains("_MeshCopy"))
                {
                    pAllObjects[i].transform.parent.GetComponent<MeshRenderer>().enabled = true;
                    GameObject.DestroyImmediate(pAllObjects[i]);
                }
            }
        }

        private void OnGUI()
        {
            //EditorGUILayout.LabelField("Vertex Tool", EditorStyles.boldLabel);
            _infoPanel.OnGUI();
            _settingsPanel.OnGUI();
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.wordWrap = true;
            EditorGUILayout.LabelField("勾选Is Edit Vertex 开始顶点编辑，鼠标左键选择顶点，ctrl + 鼠标左键点击可进行加选或者减选，ctrl + 鼠标左键拖动可框选", style);
        }
    } 
}
