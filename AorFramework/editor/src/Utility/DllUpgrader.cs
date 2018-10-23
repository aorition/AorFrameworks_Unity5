using System;
using System.Text;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using AorBaseUtility;

namespace Framework.Editor.Utility
{

    public class DllUpgraderInitWindow : EditorWindow
    {

        public static void init()
        {
            EditorWindow window = EditorWindow.GetWindow<DllUpgraderInitWindow>(false, "DllUpgrader初始化向导 ...", true);
            window.minSize = new Vector2(800, 480);
        }

        private string _exportsPath;
        private List<string> _excludeFiles;
        private List<string> _inputPathList;
        private Vector2 _inputPathScrollPos;
        private Vector2 _inputPathScrollPos_e;
        private void Awake()
        {

            _excludeFiles = new List<string>();
            _inputPathList = new List<string>();
        }

        private void OnGUI()
        {
            GUILayout.Space(5);
            GUILayout.Label(new GUIContent("欢迎使用DllUpgrader初始化向导"), "LODLevelNotifyText");

            GUILayout.Label("请在输入区域添加路径序列,DllUpgrader会依照添加顺序将输入文件夹内的所有文件复制到输出路径.", "GroupBox");

            GUILayout.Space(10);

            GUILayout.BeginVertical(new GUIContent("输入路径"), "window", GUILayout.Height(90));
                _inputPathScrollPos = EditorGUILayout.BeginScrollView(_inputPathScrollPos, GUILayout.Height(90));

                int i, length = _inputPathList.Count;
                for (i = 0; i < length; i++)
                {

                    GUILayout.BeginHorizontal();

                        _inputPathList[i] = EditorGUILayout.TextField(_inputPathList[i]);
                        if (GUILayout.Button(new GUIContent("M", "修改输入路径"), GUILayout.Width(25)))
                        {
                            string modifPath = EditorUtility.OpenFolderPanel("请选择输入路径", "", "");
                            if (!string.IsNullOrEmpty(modifPath))
                            {
                                _inputPathList[i] = modifPath;
                            }
                            Repaint();
                        }
                        if (GUILayout.Button(new GUIContent("-", "移除该路径"), GUILayout.Width(25)))
                        {
                            _inputPathList.RemoveAt(i);
                            Repaint();
                        }

                    GUILayout.EndHorizontal();
                }

                EditorGUILayout.EndScrollView();
                
                GUILayout.BeginHorizontal();

                GUILayout.FlexibleSpace();

                    if (GUILayout.Button(new GUIContent("+", "添加输入路径"), GUILayout.Width(80), GUILayout.Height(20)))
                    {
                        string newPath = EditorUtility.OpenFolderPanel("请选择输入路径", "", "");
                        if (!string.IsNullOrEmpty(newPath))
                        {
                            _inputPathList.Add(newPath);
                        }
                        Repaint();
                    }

                GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            GUILayout.Space(10);

            GUILayout.BeginVertical(new GUIContent("排除文件列表", "在此列表下的路径将不会复制到目标路径"), "window", GUILayout.Height(120));

            _inputPathScrollPos_e = EditorGUILayout.BeginScrollView(_inputPathScrollPos_e, GUILayout.Height(90));

            length = _excludeFiles.Count;
            for (i = 0; i < length; i++)
            {

                GUILayout.BeginHorizontal();

                _excludeFiles[i] = EditorGUILayout.TextField(_excludeFiles[i]);
                if (GUILayout.Button(new GUIContent("M", "修改"), GUILayout.Width(25)))
                {
                    //                            string modifPath = EditorUtility.OpenFolderPanel("请选择需要排除的文件", "", "");
                    string modifPath = EditorUtility.OpenFilePanel("请选择需要排除的文件", "", "");
                    if (!string.IsNullOrEmpty(modifPath))
                    {
                        _excludeFiles[i] = modifPath;
                    }
                    Repaint();
                }
                if (GUILayout.Button(new GUIContent("-", "移除"), GUILayout.Width(25)))
                {
                    _excludeFiles.RemoveAt(i);
                    Repaint();
                }

                GUILayout.EndHorizontal();

            }

            EditorGUILayout.EndScrollView();

            GUILayout.BeginHorizontal();

                GUILayout.FlexibleSpace();

                if (GUILayout.Button(new GUIContent("+", "添加需要排除的文件"), GUILayout.Width(80), GUILayout.Height(20)))
                {
                    //                    string newPath = EditorUtility.OpenFolderPanel("请选择输入路径", "", "");
                    string newPath = EditorUtility.OpenFilePanel("请选择需要排除的文件", "", "");
                    if (!string.IsNullOrEmpty(newPath))
                    {
                        _excludeFiles.Add(newPath);
                    }
                    Repaint();
                }

            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            GUILayout.BeginVertical(new GUIContent("输出路径", "所有输入路径下的文件(包括子目录)都会复制到此文件夹下"), "window", GUILayout.Height(50));

            GUILayout.BeginHorizontal();

            _exportsPath = EditorGUILayout.TextField(_exportsPath, GUILayout.Height(32));
            if (GUILayout.Button(new GUIContent("选择"), GUILayout.Width(50), GUILayout.Height(32)))
            {
                _exportsPath = EditorUtility.OpenFolderPanel("请选择输出路径", "", "");
            }

            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            GUILayout.FlexibleSpace();

            if (string.IsNullOrEmpty(_exportsPath))
            {
                GUI.color = Color.grey;
                if (GUILayout.Button(new GUIContent("开始更新"), GUILayout.Height(36)))
                {
                }
            }
            else
            {
                GUI.color = Color.yellow;
                if (GUILayout.Button(new GUIContent("开始更新"), GUILayout.Height(36)))
                {
                    string cfg = Application.dataPath + "/" + DllUpgrader.CfgFileName;

                    if (DllUpgrader.UpgradeDlls(_inputPathList, _excludeFiles, _exportsPath, cfg))
                    {
                        AssetDatabase.Refresh();
                        EditorApplication.RepaintProjectWindow();
                        Close();
                    }
                }
            }

            //窗口大小显示
            //        GUI.color = Color.grey;
            //        GUILayout.FlexibleSpace();
            //        GUILayout.Label(Screen.width + "," + Screen.height);
        }

    }


    public class DllUpgrader : UnityEditor.Editor
    {

        public const string CfgFileName = "DllUpraderPathCFG.cfg";
        public const string BrigdeCfgFileName = "BridgeUpgraderPathCFG.cfg";

        public static bool UpgradeDlls(List<string> inputList, List<string> excludeList, string extporPath, string cfgPath)
        {

            if (inputList != null && inputList.Count > 0)
            {
                int i, length = inputList.Count;
                Debug.Log("Starting DLL Upgrade ....");
                for (i = 0; i < length; i++)
                {
                    AorIO.CopyDirectory(inputList[i], extporPath, excludeList, (src, target) =>
                    {
                        
                        //验证是否可以替换?
                        FileInfo srcFileInfo = new FileInfo(src);
                        FileInfo targetFileInfo = new FileInfo(target);
                        if (srcFileInfo.LastWriteTimeUtc.CompareTo(targetFileInfo.LastWriteTimeUtc) >= 0)
                        {
                            return true;
                        }
                        else
                        {
                            if(EditorUtility.DisplayDialog("警告", "源文件的修改时间比目标文件的修改时间旧,源文件可能不是最新的版本.\n源文件:" + src + "\n目标文件:" + target + "\n你确定继续使用源文件覆盖目标文件?", "确定", "跳过"))
                            {
                                return true;
                            }
                            return false;
                        }
                    });
                }
                Debug.Log("Starting DLL Upgrade Done !!");

                //save cfg
                StringBuilder cfgData = new StringBuilder();
                for (i = 0; i < length; i++)
                {
                    if (i > 0)
                    {
                        cfgData.Append('|');
                    }
                    cfgData.Append(inputList[i]);
                }
                cfgData.Append("\r\n");
                length = excludeList.Count;
                for (i = 0; i < length; i++)
                {
                    if (i > 0)
                    {
                        cfgData.Append('|');
                    }
                    cfgData.Append(excludeList[i]);
                }
                cfgData.Append("\r\n");
                cfgData.Append(extporPath);
                //
                FileStream fs = new FileStream(cfgPath, FileMode.OpenOrCreate, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs);
                sw.WriteLine(cfgData.ToString());
                sw.Close();
                fs.Close();
                return true;
            }
            else
            {
                EditorUtility.DisplayDialog("提示", "输入路径为空!\n请至少加入一条输入路径.", "确定");
            }
            return false;
        }

        [MenuItem("FrameworkTools/DLLs Update",false,101)]
        public static void UpgradeDllsMenu()
        {
            UpgradeDlls();
        }

        public static void UpgradeDlls()
        {

            string cfg = Application.dataPath + "/" + CfgFileName;

            if (File.Exists(cfg))
            {
                //存在
                string cfgStr = AorIO.ReadStringFormFile(cfg);

                cfgStr = cfgStr.Replace("\r\n", "@");
                string[] splits = cfgStr.Split('@');

                int u, ulen = splits.Length;
                int i, length;

                List<string> inputList = new List<string>();
                List<string> excludeList = new List<string>();
                string exportPath = null;

                for (u = 0; u < ulen; u++)
                {

                    string li = splits[u];
                    if (!string.IsNullOrEmpty(li))
                    {

                        if (u == 0)
                        {
                            //inputList
                            string[] inputLists = li.Split('|');
                            length = inputLists.Length;
                            for (i = 0; i < length; i++)
                            {
                                inputList.Add(inputLists[i]);
                            }
                        }

                        if (u == 1)
                        {
                            //excludeList
                            string[] excludeLists = li.Split('|');
                            length = excludeLists.Length;
                            for (i = 0; i < length; i++)
                            {
                                excludeList.Add(excludeLists[i]);
                            }
                        }

                        if (u == 2)
                        {
                            exportPath = li;
                        }

                    }


                }

                if (inputList.Count > 0 && !string.IsNullOrEmpty(exportPath))
                {
                    if (UpgradeDlls(inputList, excludeList, exportPath, cfg))
                    {
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                        EditorApplication.RepaintProjectWindow();
                    }
                }
                else
                {
                    if (File.Exists(cfg))
                    {
                        File.Delete(cfg);
                    }
                    DllUpgraderInitWindow.init();
                }
            }
            else
            {
                DllUpgraderInitWindow.init();
            }

        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUILayout.Space(10);

            GUI.color = Color.yellow;
            if (GUILayout.Button(new GUIContent("Dll Upgrade", "更新全部DLL文件")))
            {
                UpgradeDlls();
            }

        }
    }

}


