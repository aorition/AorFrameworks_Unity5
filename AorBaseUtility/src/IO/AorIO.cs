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

        public static string GetFullPath(string strPath, string strRootPath)
        {
            if (string.IsNullOrEmpty(strPath) || !strPath.Contains(":"))
            {
                return string.Format("{0}{1}", strRootPath, strPath);
            }
            else
            {
                return strPath;
            }
        }

        public static void CopyDirectory(string srcdir, string desdir)
        {
            m_CopyDirectory(srcdir, desdir, null, null);
        }

        public static void CopyDirectory(string srcdir, string desdir, List<string> excludeList)
        {
            m_CopyDirectory(srcdir, desdir, excludeList, null);
        }

        public static void CopyDirectory(string srcdir, string desdir, List<string> excludeList, Func<string, string, bool> replaceCondition)
        {
            m_CopyDirectory(srcdir, desdir, excludeList, replaceCondition);
        }
        private static void m_CopyDirectory(string srcdir, string desdir, List<string> excludeList,Func<string,string,bool> replaceCondition)
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
                    m_CopyDirectory(filePath, subDesPath, excludeList, replaceCondition);
                }
                else
                {
                    if (excludeList != null)
                    {
                        if (!excludeList.Contains(srcPath))
                        {
                            if (File.Exists(subDesPath) && replaceCondition != null)
                            {
                                if(!replaceCondition(srcPath, subDesPath)) continue;
                            }
                            m_copyFile(srcPath, subDesPath, true);
                        }
                    }
                    else
                    {
                        if (replaceCondition != null)
                        {
                            if (!replaceCondition(srcPath, subDesPath)) continue;
                        }
                        m_copyFile(srcPath, subDesPath, true);
                    }
                }
            }

        }

        private static void m_copyFile(string srcPath, string targetPath, bool overwrite)
        {
            if (overwrite)
            {
                if (File.Exists(targetPath))
                {
                    File.Delete(targetPath);
                }
                File.Copy(srcPath, targetPath, true);
            }
            else
            {
                File.Copy(srcPath, targetPath);
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


