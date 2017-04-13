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

public class ConfigEncoding : EditorWindow
{
    public const string CONFIG_ENCODING_CACHE_NAME = "F_UnityExtends_";


	[MenuItem ("FUnityExtends/ConfigEncoding/Conver To UTF-8 #%&C")]
    public static void Conver()
    {
		string currentPaths = getConfigPath ();
        if (currentPaths.Length < 0)
        {
            EditorUtility.DisplayDialog("Error", "need settings a config directory", "ok");
            return;
        }
        ConverEncoding(currentPaths);
    }
	[MenuItem ("FUnityExtends/ConfigEncoding/Clear Path")]
	public static void ClearPath ()
	{
		PlayerPrefs.DeleteKey (CONFIG_ENCODING_CACHE_NAME + "Paths");
	}

    [MenuItem("FUnityExtends/ConfigEncoding/Settings")]
    public static void Settings()
    {
        EditorWindow.GetWindow<ConfigEncoding>(false, "ConfigEncoding", true);
    }

	//[MenuItem ("GameObject/Do Something _f")]
	//public static void key ()
	//{
	//	Conver ();
	//}


	public static string getConfigPath ()
	{
		string defaultPath = Application.dataPath + "/Resources/Config";
		string currentPaths = PlayerPrefs.GetString (CONFIG_ENCODING_CACHE_NAME + "Paths", defaultPath);
		return currentPaths;
	}


    private void OnGUI()
    {
        string currentPaths = PlayerPrefs.GetString(CONFIG_ENCODING_CACHE_NAME + "Paths", "No Selected");
        if (GUILayout.Button("Setting ConfigPlus Directory Paths \n current: \n" + currentPaths, GUILayout.Height(80))) 
        {
            string paths = EditorUtility.OpenFolderPanel("", "", "");
            if (paths.Length > 0)
            {
                PlayerPrefs.SetString(CONFIG_ENCODING_CACHE_NAME + "Paths", paths);
            }
            
        }

    }



    private static void ConverEncoding(string paths)
    {
        int count = 0;
		string[] fileNames = Directory.GetFiles (paths, "*", SearchOption.AllDirectories);
        foreach (string i in fileNames)
        {
			if (Path.GetExtension (i) != ".txt" && Path.GetExtension (i) != ".csv" && Path.GetExtension (i) != ".fcfg") continue;
            FileStream stream = new FileStream(i, FileMode.Open);
            Encoding encoding = getEncoding(stream, Encoding.Default);
            stream.Close();
            string content = File.ReadAllText(i, encoding);
            File.Delete(i);
            File.WriteAllText(i, content, new UTF8Encoding());
            count++;
        }
        AssetDatabase.Refresh();
		EditorUtility.DisplayDialog ("Tips", "Convert To UTF-8 Done : " + count.ToString (), "Sure");
    }

    public static Encoding getEncoding(FileStream stream, Encoding defaultEncoding = null)
    {
        Encoding targetEncoding = defaultEncoding;
        if (stream != null && stream.Length >= 2)
        {
            byte byte1 = 0;
            byte byte2 = 0;
            byte byte3 = 0;
            byte byte4 = 0;
            long origPos = stream.Seek(0, SeekOrigin.Begin);
            stream.Seek(0, SeekOrigin.Begin);

            int nByte = stream.ReadByte();
            byte1 = Convert.ToByte(nByte);
            byte2 = Convert.ToByte(stream.ReadByte());
            if (stream.Length >= 3)
            {
                byte3 = Convert.ToByte(stream.ReadByte());
            }
            if (stream.Length >= 4)
            {
                byte4 = Convert.ToByte(stream.ReadByte());
            }
            if (byte1 == 0xFE && byte2 == 0xFF)//UnicodeBe
            {
                targetEncoding = Encoding.BigEndianUnicode;
            }
            if (byte1 == 0xFF && byte2 == 0xFE && byte3 != 0xFF)//Unicode
            {
                targetEncoding = Encoding.Unicode;
            }
            if (byte1 == 0xEF && byte2 == 0xBB && byte3 == 0xBF)//UTF8
            {
                targetEncoding = Encoding.UTF8;
            }
            stream.Seek(origPos, SeekOrigin.Begin);
        }
		if (targetEncoding == defaultEncoding)
		{
			byte[] bytes = new byte[stream.Length];
			stream.Read (bytes, 0, bytes.Length);
			if (isUtf8 (bytes))
			{
				return Encoding.UTF8;
			}
		}
		
        return targetEncoding;
    }

	private static bool isUtf8 ( byte[] rawtext )
	{
		int score = 0;
		int i, rawtextlen = 0;
		int goodbytes = 0, asciibytes = 0;

		// Maybe also use UTF8 Byte Order Mark:  EF BB BF

		// Check to see if characters fit into acceptable ranges
		rawtextlen = rawtext.Length;
		for (i = 0; i < rawtextlen; i++)
		{
			if ((rawtext[i] & (byte)0x7F) == rawtext[i])
			{  // One byte
				asciibytes++;
			}
			else
			{
				int m_rawInt0 = Convert.ToInt16 (rawtext[i]);
				int m_rawInt1 = Convert.ToInt16 (rawtext[i + 1]);
				int m_rawInt2 = Convert.ToInt16 (rawtext[i + 2]);

				if (256 - 64 <= m_rawInt0 && m_rawInt0 <= 256 - 33 && // Two bytes
				 i + 1 < rawtextlen &&
				 256 - 128 <= m_rawInt1 && m_rawInt1 <= 256 - 65)
				{
					goodbytes += 2;
					i++;
				}
				else if (256 - 32 <= m_rawInt0 && m_rawInt0 <= 256 - 17 && // Three bytes
				 i + 2 < rawtextlen &&
				 256 - 128 <= m_rawInt1 && m_rawInt1 <= 256 - 65 &&
				 256 - 128 <= m_rawInt2 && m_rawInt2 <= 256 - 65)
				{
					goodbytes += 3;
					i += 2;
				}
			}
		}

		if (asciibytes == rawtextlen) { return false; }

		score = (int)(100 * ((float)goodbytes / (float)(rawtextlen - asciibytes)));

		if (score > 98)
		{
			return score > 80;
		}
		else if (score > 95 && goodbytes > 30)
		{
			return score > 80;
		}
		else
		{
			return false;
		}

	}





}



