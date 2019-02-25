using UnityEditor;
using UnityEngine;
public class SplineDataExport : EditorWindow {

    private static string _SplineDataExport;
    public static void init(string JsonData) {
        _SplineDataExport = JsonData;
        EditorWindow.GetWindow<SplineDataExport>("导出数据");
    }

    void OnGUI() {
        // EditorGUILayout.SelectableLabel(_SplineDataExport);
        EditorGUILayout.TextArea(_SplineDataExport, "AS TextArea", GUILayout.Width(Screen.width), GUILayout.Height(Screen.height));
    }

    void OnDestroy() {
        _SplineDataExport = null;
    }

}