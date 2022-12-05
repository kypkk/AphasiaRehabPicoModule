using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using DataSync;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;
using UnityEngine;
using UnityEngine.Networking;

namespace LabData
{
    public class LabTools
    {

        public static T GetData<T>(LabDataBase data) where T : LabDataBase
        {
            return data is T @base ? @base : null;
        }

    /// <summary>
    /// 資料儲存位置。
    /// see https://docs.unity3d.com/ScriptReference/Application-persistentDataPath.html
    /// </summary>
    public static string DataPath =>
#if UNITY_ANDROID
        Application.persistentDataPath;
#else
        Application.dataPath;
#endif

    /// <summary>
    /// 在 DataPath 內新增資料夾
    /// </summary>
    /// <param name="folderName"></param>
    /// <param name="isNew">如果資料夾已存在，用 "名稱_時間" 新增一個；否則回傳原路徑</param>
        public static string CreatSaveDataFolder(string folderName, bool isNew = false)
        {
            string finalPath = Path.Combine(DataPath, folderName);
            if (Directory.Exists(finalPath))
            {
                if (isNew)
                {
                    var tempPath = folderName + "_" + DateTime.Now.Millisecond.ToString();
                    Directory.CreateDirectory(tempPath);
                    return tempPath;
                }
                else
                {
                    // Debug.Log("Folder Exist!");
                    return folderName;
                }
            }
            else
            {
                Directory.CreateDirectory(folderName);
                Debug.Log("Success Create: " + folderName);
                return folderName;
            }
        }

        /// <summary>
        /// 建立 (空的) 文件
        /// </summary>
        /// <param name="path"></param>
        public static void CreateData(string path)
        {
            if (!File.Exists(path))
            {
                File.Create(path).Dispose();
                Debug.Log("Success Create: " + path);
            }            
        }

        /// <summary>
        /// 根據 T 的類型讀入相應的 config (路徑為 {LabTool.DataPath}/{gameDataPath}/{T}.json)
        /// 若不存在，會依照 default constructor 去寫一個
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="isOverride">若需要覆蓋舊的 config，傳入 true</param>
        /// <param name="gameDataPath">預設為 /GameData </param>
        /// <returns></returns>
        public static T GetConfig<T>(bool isOverride = false, string gameDataPath = "/GameData") where T : class, new()
        {
            // 先設定為 ConfigData 資料夾位置，並檢查是否存在
            var path = DataPath + gameDataPath + "/ConfigData";
      
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            // 路徑
            path = Path.Combine(path, typeof(T).Name + ".json");

            // override: 先刪除檔案
            if (isOverride && File.Exists(path))
            {
                File.Delete(path);
            }

            // 如果不存在就寫入
            if (!File.Exists(path))
            {
                // 創建一個新的 T，序列化
                var json = JsonConvert.SerializeObject(new T());
                var fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                StreamWriter sw = new StreamWriter(fs);
                sw.Write(json);
                sw.Close();
            }

            // 讀入設定檔並回傳
            StreamReader sr = new StreamReader(path);
            var data = JsonConvert.DeserializeObject<T>(sr.ReadToEnd());
            sr.Close();
            return data;
        }

        /// <summary>
        /// 写数据，数据类型必须继承LabDataBase，dataName为需要写的数据名字，isOverWrite选择是否要覆盖已有文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="dataName"></param>
        /// <param name="isOverWrite"></param>
        /// <returns></returns>
        public static void WriteData<T>(T t, string dataName, bool isOverWrite = false, string filePath = "/GameData") where T : LabDataBase, new()
        {
            var path = DataPath + filePath + "/" + typeof(T).Name + "/" + dataName + ".json";

            if (!File.Exists(path))
            {
                var json = JsonConvert.SerializeObject(t);
                var fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                StreamWriter sw = new StreamWriter(fs);
                sw.Write(json);
                sw.Close();
            }
            else if (File.Exists(path) && isOverWrite)
            {
                var json = JsonConvert.SerializeObject(t);
                var fs = new FileStream(path, FileMode.Truncate, FileAccess.ReadWrite);
                fs.Close();
                fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                StreamWriter sw = new StreamWriter(fs);
                sw.Write(json);
                sw.Close();
            }
            else
            {
                Debug.LogError("需要重写数据，请在参数中设置isOverWrite=true");
            }
        }

    /// <summary>
    /// 通过类型T和名字获取指定的数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="t"></param>
    /// <param name="dataName"></param>
    /// <returns></returns>
    public static T GetData<T>(string dataName, string filePath = "/GameData") where T : LabDataBase
    {
      // folder
      string path = DataPath + filePath;
      if (filePath.StartsWith("/StreamingAssets/")) // Android 的 Streaming Assets
      {
        path = Path.Combine(Application.streamingAssetsPath, filePath.Replace("/StreamingAssets/", ""));
      }
      // combine file name
      path = Path.Combine(path, typeof(T).Name, dataName + ".json");

      // StreamReader sr = new StreamReader(path);
      // var data = JsonConvert.DeserializeObject<T>(sr.ReadToEnd());
      // sr.Close();
      var req = UnityWebRequest.Get(path);
      req.SendWebRequest();
      while (!req.isDone) { }

      if (req.isNetworkError || req.isHttpError)
      {
        Debug.LogError("数据文件不存在！" + path);
        Debug.LogError("Reason: " + req.error);
        return default;
      }

      var data = JsonConvert.DeserializeObject<T>(req.downloadHandler.text);
      return data;
    }

    /// <summary>
    /// 從 JSON 解析 T
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="t"></param>
    /// <param name="dataName"></param>
    /// <returns></returns>
    public static T GetDataByString<T>(string jsonString) where T : class
        {
            JSchema schema = new JSchemaGenerator().Generate(typeof(T));
            Debug.Log(schema);
            JToken token = JToken.Parse(jsonString);

            if (token.IsValid(schema))
            {
                return JsonConvert.DeserializeObject<T>(jsonString);
            }
            else
            {
                throw new InvalidDataException("無法解析此字串");
            }

        }
        /// <summary>
        /// 删除数据文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataName"></param>
        public static void DeleteData<T>(string dataName, string filePath = "/GameData") where T : LabDataBase
        {
            var path = DataPath + filePath + "/" + typeof(T).Name + "/" + dataName + ".json";

            if (!File.Exists(path))
            {
                Debug.LogError("数据文件不存在！");
            }
            else
            {
                File.Delete(path);
            }
        }

        /// <summary>
        /// 获取对应数据的文件夹内的所有文件的名字List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="isGetName"></param>
        /// <returns></returns>
        public static List<string> GetDataName<T>(bool isGetName = false, string filePath = "/GameData") where T : LabDataBase
        {
            var path = DataPath + filePath + "/" + typeof(T).Name;
            if (Directory.Exists(path))
            {
                var root = new DirectoryInfo(path);
                var files = root.GetFiles();
                if (files.Length <= 0)
                {
                    Debug.LogError("没有可用数据文件！");
                    return null;
                }
                List<string> tempList = new List<string>();
                foreach (var fileInfo in files)
                {
                    if (".json".ToLower().IndexOf(fileInfo.Extension.ToLower(), StringComparison.Ordinal) >= 0)
                    {
                        tempList.Add(fileInfo.Name.Split('.').First());
                    }
                }

                return tempList;
            }

            Debug.LogError("数据文件夹不存在！");
            return null;
        }

        ///// <summary>
        /////通过Key获取多语言对应的值
        ///// </summary>
        ///// <param name="key"></param>
        ///// <returns></returns>
        //public static string GetCurrentCultureValue(params string[] key)
        //{
        //    return string.Join("", key.Select(p =>
        //    {
        //        var translate = LocalizationManager.GetTranslation(p);
        //        if (p != null && string.IsNullOrEmpty(translate))
        //        {
        //            return p;
        //        }
        //        return translate;
        //    }));
        //}

        public static Type GetScriptType(string name)
        {
            return Type.GetType("UIFrameWork." + name);
        }
    }
}

