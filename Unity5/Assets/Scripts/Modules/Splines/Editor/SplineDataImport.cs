
using Framework.module;
using UnityEditor;
using UnityEngine;

public class SplineDataImport : EditorWindow {

    private static Spline _spline;

    public static void init(Spline spline) {
        _spline = spline;
        EditorWindow.GetWindow<SplineDataImport>("导入数据");
    }

    private string _importData;
    void OnGUI() {
        // EditorGUILayout.SelectableLabel(_SplineDataExport);
        //EditorGUILayout.TextArea(_SplineDataExport, "AS TextArea", GUILayout.Width(Screen.width), GUILayout.Height(Screen.height));
        EditorGUILayout.BeginVertical();
        _importData = EditorGUILayout.TextField(_importData, GUILayout.Width(Screen.width), GUILayout.Height(Screen.height * 0.7f));

        if (GUILayout.Button("导入", GUILayout.Height(Screen.height * 0.3f))) {

            _spline.ImportData_StringList(_importData);

            this.Close();
        }

        EditorGUILayout.EndVertical();
    }

    void OnDestroy() {
        _spline = null;
    }

}