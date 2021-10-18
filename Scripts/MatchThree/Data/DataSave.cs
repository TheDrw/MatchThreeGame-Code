using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using PlayFab;
using PlayFab.ClientModels;
using System;

namespace MatchThree.Data
{
    interface ISaveAndLoad
    {
        void Load();
        void Save();
    }

    public interface ILoadedAsync
    {
        void LoadAsyncSuccess();
    }

    public static class DataSave
    {
        public static event Action OnDataLoadedFromServer = delegate { };
        public static event Action OnDataSavedToServer = delegate { };
        public static event Action OnDataDeletedFromServer = delegate { };

        public static void Load<T>(string fileName, T obj, ILoadedAsync loader = null) where T : class
        {
            if (!PlayFabClientAPI.IsClientLoggedIn())
            {
#if UNITY_EDITOR
                Debug.LogWarning("WRN : You are currently not logged in.");
#endif
                return;
            }

            var request = new GetUserDataRequest();
            PlayFabClientAPI.GetUserData
            (
                request,
                result =>
                {
                    string json = "";
                    if (result.Data == null || !result.Data.ContainsKey(fileName))
                    {
#if UNITY_EDITOR
                        Debug.Log($"Does not contain {fileName}.");
#endif
                        //JsonUtility.FromJsonOverwrite(json, obj);
                    }
                    else
                    {
#if UNITY_EDITOR
                        Debug.Log($"Success retreiving data for {fileName}");
#endif
                        json =  result.Data[fileName].Value;
                        JsonUtility.FromJsonOverwrite(json, obj);
                        loader?.LoadAsyncSuccess();
                    }

                    OnDataLoadedFromServer();
                }, 
                error =>
                {
#if UNITY_EDITOR
                    Debug.LogWarning($"Err loading data for {fileName}");
#endif
                }
            );
        }

        public static void Save<T>(string fileName, T obj)
        {
            if (!PlayFabClientAPI.IsClientLoggedIn())
            {
#if UNITY_EDITOR
                Debug.LogWarning("WRN : You are currently not logged in.");
#endif
                return;
            }

            string json = JsonUtility.ToJson(obj);

            var request = new UpdateUserDataRequest
            {
                Data = new Dictionary<string, string>
                {
                    { fileName, json }
                }
            };

            PlayFabClientAPI.UpdateUserData
            (
                request,
                result =>
                {
#if UNITY_EDITOR
                    Debug.Log($"Success sending data to server{fileName}");
#endif
                    OnDataSavedToServer();
                }, 
                error =>
                {
#if UNITY_EDITOR
                    Debug.LogWarning($"Error sending data to server {fileName}");
#endif
                }
            );
        }

        public static void Delete(string fileName)
        {
            if (!PlayFabClientAPI.IsClientLoggedIn())
            {
#if UNITY_EDITOR
                Debug.LogWarning("WRN : You are currently not logged in.");
#endif
                return;
            }

            var request = new UpdateUserDataRequest
            {
                Data = new Dictionary<string, string>
                {
                    { fileName, "{}" }
                }
            };


            PlayFabClientAPI.UpdateUserData
            (
                request,
                result =>
                {
#if UNITY_EDITOR
                    Debug.Log($"Success deleting data to server{fileName}");
#endif
                    OnDataDeletedFromServer();
                },
                error =>
                {
#if UNITY_EDITOR
                    Debug.LogWarning($"Error deleting data to server {fileName}");
#endif
                }
            );
        }

//        static string ReadFromFile(string fileName)
//        {
//            string path = GetFilePath(fileName);
//            if (File.Exists(path))
//            {
//                using (StreamReader reader = new StreamReader(path))
//                {
//                    string json = reader.ReadToEnd();
//                    return json;
//                }
//            }

//#if UNITY_EDITOR
//            Debug.LogWarning("File not found - creating one at save!");
//#endif
//            return "";
//        }

//        static void WriteToFile(string fileName, string json)
//        {
//            string path = GetFilePath(fileName);
//            FileStream fileStream = new FileStream(path, FileMode.Create);
//            using (StreamWriter writer = new StreamWriter(fileStream))
//            {
//                writer.Write(json);
//            }
        //}


//        private static string GetFilePath(string fileName)
//        {


//#if UNITY_EDITOR
//            return Application.persistentDataPath + $"/{fileName}";
//#elif UNITY_WEBGL
//            return Application.persistentDataPath + $"/{fileName}";
//#else
//            return Application.persistentDataPath + $"/{fileName}";
//#endif
//        }

        //private static string DefintelySuperSafeEncryptionDecrytion(string data, int key)
        //{
        //    StringBuilder input = new StringBuilder(data),
        //        output = new StringBuilder(data.Length);

        //    char ch = 'x';
        //    for (int i = 0; i < data.Length ; i++)
        //    {
        //        ch = input[i];
        //        ch = (char)(ch ^ key);
        //        output.Append(ch);
        //    }

        //    return output.ToString();
        //}
    }
}