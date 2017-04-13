using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AorBaseUtility
{

    /// <summary>
    /// IO存储封装类
    /// </summary>
    public class AorIO
    {

        public static Encoding encoding = Encoding.UTF8;

        public static string getUniPathFormat(string path)
        {
            return path.Replace("\\", "/");
        }

        public static void CopyDirectory(string srcdir, string desdir)
        {

            if (string.IsNullOrEmpty(srcdir) || string.IsNullOrEmpty(desdir))
            {
                return;
            }

            if (!Directory.Exists(desdir))
            {
                Directory.CreateDirectory(desdir);
            }

            string[] filePaths = Directory.GetFileSystemEntries(srcdir);

            foreach (string filePath in filePaths)
            {

                string fileName = filePath.Substring(filePath.LastIndexOf("\\") + 1);
                string srcPath = getUniPathFormat(filePath);
                string subDesPath = getUniPathFormat(desdir + "\\" + fileName);

                if (Directory.Exists(filePath))
                {
                    CopyDirectory(filePath, subDesPath);
                }
                else
                {
                    //                Debug.Log("DU> copy " + srcPath + " -> " + subDesPath);
                    if (File.Exists(subDesPath))
                    {
                        File.Delete(subDesPath);
                    }
                    File.Copy(srcPath, subDesPath, true);
                }
            }

        }

        public static void CopyDirectory(string srcdir, string desdir, List<string> excludeList)
        {

            if (string.IsNullOrEmpty(srcdir) || string.IsNullOrEmpty(desdir))
            {
                return;
            }

            if (!Directory.Exists(desdir))
            {
                Directory.CreateDirectory(desdir);
            }

            string[] filePaths = Directory.GetFileSystemEntries(srcdir);

            foreach (string filePath in filePaths)
            {

                string fileName = filePath.Substring(filePath.LastIndexOf("\\") + 1);
                string srcPath = getUniPathFormat(filePath);
                string subDesPath = getUniPathFormat(desdir + "\\" + fileName);

                if (Directory.Exists(filePath))
                {
                    CopyDirectory(filePath, subDesPath, excludeList);
                }
                else
                {
                    if (!excludeList.Contains(srcPath))
                    {
                        //                    Debug.Log("DU> copy " + srcPath + " -> " + subDesPath);
                        if (File.Exists(subDesPath))
                        {
                            File.Delete(subDesPath);
                        }
                        File.Copy(srcPath, subDesPath, true);
                    }
                    else
                    {
                        //                    Debug.Log("DU> exclude " + subDesPath);
                    }
                }
            }

        }

        public static byte[] ReadBytesFormFile(string path)
        {
            if (File.Exists(path))
            {
                FileStream fs = new FileStream(path, FileMode.Open);
                long size = fs.Length;
                byte[] data = new byte[size];
                fs.Read(data, 0, data.Length);
                fs.Close();
                return data;
            }
            else
            {
                //            Debug.LogError("AorIO.ReadStringFormFile Error :: 读取数据失败, 没有找到该文件 : " + path);
                return null;
            }
        }

        public static bool SaveBytesToFile(string path, byte[] data)
        {
            if (data != null && data.Length > 0)
            {
                //Create the file.
                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                int dirSplit = path.LastIndexOf('/');
                if (dirSplit != -1)
                {
                    string dir = path.Substring(0, dirSplit);

                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }
                }

                FileStream fs = new FileStream(path, FileMode.Create);
                fs.Write(data, 0, data.Length);
                fs.Close();

                return true;
            }
            else
            {
                //            Debug.LogError("AorIO.SaveBytesToFile Error :: 保存数据失败,数据不能为空!");
                return false;
            }
        }

        public static string ReadStringFormFile(string path)
        {
            if (File.Exists(path))
            {
                return File.ReadAllText(path, encoding);
            }
            else
            {
                //            Debug.LogError("AorIO.ReadStringFormFile Error :: 读取数据失败, 没有找到该文件 : " + path);
                return null;
            }
        }

        public static bool SaveStringToFile(string path, string fileStr)
        {
            // string projectPath = Application.dataPath + defaultConfigFilesPath.Replace("Assets", "") + fileName + defaultConfigAssetExt;
            if (fileStr != null && fileStr.Trim() != "")
            {
                //Create the file.
                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                int dirSplit = path.LastIndexOf('/');
                if (dirSplit != -1)
                {
                    string dir = path.Substring(0, dirSplit);

                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }
                }

                StreamWriter FileWriter = new StreamWriter(path, false, encoding);
                FileWriter.Write(fileStr);
                FileWriter.Flush();

                FileWriter.Close();
                FileWriter.Dispose();

                return true;
            }
            else
            {
                //            Debug.LogError("AorIO.SaveStringToFile Error :: 保存数据失败,数据不能为空!");
                return false;
            }
        }

        public static bool CreateEmptyTxtFile(string path)
        {
            //Create the file.
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            int dirSplit = path.LastIndexOf('/');
            if (dirSplit != -1)
            {
                string dir = path.Substring(0, dirSplit);

                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
            }

            StreamWriter FileWriter = new StreamWriter(path, false, encoding);
            FileWriter.Write("\r\n");
            FileWriter.Flush();

            FileWriter.Close();
            FileWriter.Dispose();

            return true;
        }

    }

}


