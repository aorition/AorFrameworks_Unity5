using System.Collections.Generic;
using System.IO;
using AorFramework.editor;
using UnityEditor;
using UnityEngine;

namespace Framework.Graphic.Editor
{
    [CustomEditor(typeof(BasicNoisePerlin))]
    public sealed class BasicNoisePerlinEditor
        : BaseEditor<BasicNoisePerlin>
    {

        [MenuItem("Assets/GraphicsManagerAssets/VisualCameraAssets/NoiseSettingAsset")]
        public static void CreateAsset()
        {

            NoiseSettings info = ScriptableObject.CreateInstance<NoiseSettings>();
            string dir;
            string name = "unameNoiseSettings.asset";
            if (Selection.objects == null || Selection.objects.Length == 0)
            {
                dir = "Assets";
            }
            else
            {

                string dataPath = AssetDatabase.GetAssetPath(Selection.objects[0]);
                if (Selection.objects[0] is DefaultAsset)
                {
                    dir = dataPath;
                }
                else
                {
                    EditorAssetInfo ainfo = new EditorAssetInfo(dataPath);
                    dir = ainfo.dirPath;
                }
            }
            string path = dir + "/" + name;
            path = AssetDatabase.GenerateUniqueAssetPath(path);
            AssetDatabase.CreateAsset(info, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        List<NoiseSettings> mNoisePresets;
        string[] mNoisePresetNames;
        SerializedProperty m_Profile;

        protected override List<string> GetExcludedPropertiesInInspector()
        {
            List<string> excluded = base.GetExcludedPropertiesInInspector();
            excluded.Add(FieldPath(x => x.m_NoiseProfile));
            return excluded;
        }

        private void OnEnable()
        {
            if (!target)
            {
                if (Application.isPlaying)
                {
                    UnityEditor.Editor.DestroyObject(this);
                }
                else
                {
                    UnityEditor.Editor.DestroyImmediate(this);
                }

                return;
            }
            m_Profile = FindProperty(x => x.m_NoiseProfile);
            RebuildProfileList();
        }

        void RebuildProfileList()
        {
            mNoisePresets = FindAssetsByType<NoiseSettings>();
#if UNITY_2018_1_OR_NEWER
            if (ScriptableObjectUtility.CinemachineIsPackage)
                AddAssetsFromDirectory(
                    mNoisePresets, 
                    ScriptableObjectUtility.CinemachineInstallAssetPath + "/Presets/Noise");
#endif
            mNoisePresets.Insert(0, null);
            List<string> presetNameList = new List<string>();
            foreach (var n in mNoisePresets)
                presetNameList.Add((n == null) ? "(none)" : n.name);
            mNoisePresetNames = presetNameList.ToArray();
        }

        public override void OnInspectorGUI()
        {
            BeginInspector();

            if (m_Profile.objectReferenceValue == null)
                EditorGUILayout.HelpBox(
                    "A Noise Profile is required.  You may choose from among the NoiseSettings assets defined in the project.",
                    MessageType.Warning);

            Rect rect = EditorGUILayout.GetControlRect(true);
            float iconSize = rect.height + 4;
            rect.width -= iconSize;
            int preset = mNoisePresets.IndexOf((NoiseSettings)m_Profile.objectReferenceValue);
            preset = EditorGUI.Popup(rect, "Noise Profile", preset, mNoisePresetNames);
            NoiseSettings newProfile = preset < 0 ? null : mNoisePresets[preset];
            if ((NoiseSettings)m_Profile.objectReferenceValue != newProfile)
            {
                m_Profile.objectReferenceValue = newProfile;
                serializedObject.ApplyModifiedProperties();
            }
            rect.x += rect.width; rect.width = iconSize; rect.height = iconSize;
            if (GUI.Button(rect, EditorGUIUtility.IconContent("_Popup"), GUI.skin.label))
            {
                GenericMenu menu = new GenericMenu();
                if (m_Profile.objectReferenceValue != null)
                {
                    menu.AddItem(new GUIContent("Edit"), false, () => Selection.activeObject = m_Profile.objectReferenceValue);
                    menu.AddItem(new GUIContent("Clone"), false, () =>
                    {
                        m_Profile.objectReferenceValue = CreateProfile(
                            (NoiseSettings)m_Profile.objectReferenceValue);
                        RebuildProfileList();
                        serializedObject.ApplyModifiedProperties();
                    });
                    menu.AddItem(new GUIContent("Locate"), false, () => EditorGUIUtility.PingObject(m_Profile.objectReferenceValue));
                }
                menu.AddItem(new GUIContent("New"), false, () =>
                {
                    //Undo.RecordObject(Target, "Change Noise Profile");
                    m_Profile.objectReferenceValue = CreateProfile(null);
                    RebuildProfileList();
                    serializedObject.ApplyModifiedProperties();
                });
                menu.ShowAsContext();
            }

            DrawRemainingPropertiesInInspector();
        }

        public static List<T> FindAssetsByType<T>() where T : UnityEngine.Object
        {
            List<T> assets = new List<T>();
            string[] guids = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(T)));
            for (int i = 0; i < guids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
                if (asset != null)
                {
                    assets.Add(asset);
                }
            }
            return assets;
        }

        static void AddAssetsFromDirectory<T>(List<T> assets, string path) where T : UnityEngine.Object
        {
            try
            {
                var info = new DirectoryInfo(path);
                var fileInfo = info.GetFiles();
                foreach (var file in fileInfo)
                {
                    string name = path + "/" + file.Name;
                    T a = AssetDatabase.LoadAssetAtPath(name, typeof(T)) as T;
                    if (a != null)
                        assets.Add(a);
                }
            }
            catch
            {
            }
        }

        NoiseSettings CreateProfile(NoiseSettings copyFrom)
        {
//            var path = string.Empty;
//            var scene = Target.gameObject.scene;
//            if (string.IsNullOrEmpty(scene.path))
//                path = "Assets/";
//            else
//            {
//                var scenePath = Path.GetDirectoryName(scene.path);
//                var extPath = scene.name + "_Profiles";
//                var profilePath = scenePath + "/" + extPath;
//                if (!AssetDatabase.IsValidFolder(profilePath))
//                    AssetDatabase.CreateFolder(scenePath, extPath);
//                path = profilePath + "/";
//            }
//
            string path = EditorUtility.OpenFolderPanel("save file dir", "", "");
            if (!string.IsNullOrEmpty(path))
            {
                path = path.Replace(Application.dataPath, "Assets");
                path += "/" + Target.gameObject.name + "_NoiseSetting.asset";
                NoiseSettings profile = ScriptableObject.CreateInstance<NoiseSettings>();
                if (copyFrom != null)
                    profile.CopyFrom(copyFrom);
                path = AssetDatabase.GenerateUniqueAssetPath(path);
                AssetDatabase.CreateAsset(profile, path);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                return profile;
            }

            return null;
        }
    }
}
