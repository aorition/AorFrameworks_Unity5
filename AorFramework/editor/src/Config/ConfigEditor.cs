//------------------------------------------------------------------------
// |                                                                   |
// | by:Qcbf                                                           |
// |                                       |
// |                                                                   |
//-----------------------------------------------------------------------

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System;

public class ConfigEditor : EditorWindow
{
	[MenuItem ("FUnityExtends/Open ConfigPlus Editor #%&F")]
	public static void Conver ()
	{
		ConfigEditor win = EditorWindow.GetWindow<ConfigEditor> (true, "ConfigEditor", true);
	}

	private int mSelectedCfgIndex;
	private static string mFilter;
	private string[] mFilterList;
	private string mExcelPath;

	private Dictionary<string, string> mCfgPathsDic;
	private string[] mCfgNames;

	private int mRenderCount;
	private Vector2 mContentScroll;
	private GUIStyle mGUIStyle;

	public ConfigEditor ()
	{
		mGUIStyle = new GUIStyle ();
		mGUIStyle.fontSize = 14;
		mGUIStyle.normal.textColor = new Color (0.6f, 0.6f, 0.6f);
		mGUIStyle.margin = new RectOffset (5, 5, 5, 5);
		mExcelPath = PlayerPrefs.GetString ("ConfigEditorExcelPath", "");
		load ();
	}


	private void OnDestroy ()
	{
	}

	private void Update ()
	{
		mRenderCount++;
	}


	private void load ()
	{
		string[] cfgPaths = Directory.GetFiles (ConfigEncoding.getConfigPath (), "*.txt", SearchOption.AllDirectories);
		mCfgPathsDic = new Dictionary<string, string> (cfgPaths.Length);
		mCfgNames = new string[cfgPaths.Length];
		for (int i = 0; i < cfgPaths.Length; i++)
		{
			mCfgNames[i] = Path.GetFileName (cfgPaths[i]);
			if (mCfgPathsDic.ContainsKey (mCfgNames[i]))
			{
//				YoukiaCore.Log.error (" An element with the same key already exists in the dictionary.", mCfgNames[i]);
				continue;
			}
			mCfgPathsDic.Add (mCfgNames[i], cfgPaths[i]);
		}
	}


	private void OnGUI ()
	{
		GUILayout.BeginHorizontal ();
		selecteConfigGUI ();
		GUILayout.EndHorizontal ();

		actionGUI ();
	}


	private void actionGUI ()
	{
		if (GUILayout.Button ("Open") || Event.current.keyCode == KeyCode.Return)
		{
			System.Diagnostics.Process.Start (mExcelPath, "/et " + mCfgPathsDic[mFilterList[mSelectedCfgIndex]]);
			Close ();
		}
		if (GUILayout.Button ("SetExcelPath[" + mExcelPath + "]"))
		{
			string path = EditorUtility.OpenFilePanel (@"Select Excel Path[例子:C:\Program Files (x86)\WPS Office\ksolaunch.exe]", "", "exe");
			if (!string.IsNullOrEmpty(path))
			{
				mExcelPath = path;
				PlayerPrefs.SetString ("ConfigEditorExcelPath", mExcelPath);
			}
		}
	}


	private void selecteConfigGUI ()
	{
		GUI.SetNextControlName ("A");
		string temp = EditorGUILayout.TextField (mFilter);
		//if (GUILayout.Button ("Filter") || mFilterList == null)
		if (mFilter != temp || mFilterList == null)
		{
			mFilter = temp;
			List<string> filter = new List<string> ();
			for (int i = 0; i < mCfgNames.Length; i++)
			{
				if (string.IsNullOrEmpty(mFilter) || mCfgNames[i].IndexOf (mFilter, StringComparison.CurrentCultureIgnoreCase) != -1)
					filter.Add (mCfgNames[i]);
			}
			mFilterList = filter.ToArray ();
		}

		mSelectedCfgIndex = EditorGUILayout.Popup (mSelectedCfgIndex, mFilterList);

		if (mRenderCount == 10)
			GUI.FocusControl ("A");
	}






}



