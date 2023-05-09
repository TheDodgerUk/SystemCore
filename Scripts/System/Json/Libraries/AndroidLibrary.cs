using System;
using System.Collections;
using System.IO;
using UnityEngine;

namespace JsonPlugin
{

    public class AndroidLibrary
    {
        private static readonly FullSerializerLibrary FullSerialiserAndroid = new FullSerializerLibrary();

        public void ReadFromFileAsync<T>(UnityEngine.MonoBehaviour mono, bool showError, string path, JsonLibraryType jsonType, Action<T> callback) where T : class
        {
#if PLATFORM_IOS

            mono.StartCoroutine(GetRequest<T>(showError, path, jsonType, (data) =>
            {
                mono.StartCoroutine(CallbackAfterFrame(data, callback));
            }));
#else
            mono.StartCoroutine(GetRequest<T>(showError, path, jsonType, callback));
#endif
        }

#if PLATFORM_IOS
        IEnumerator CallbackAfterFrame<T>(T data, Action<T> callback) where T : class
        {
            yield return new WaitForEndOfFrame();

            callback?.Invoke(data);
        }
#endif


        IEnumerator GetRequest<T>(bool showError, string path, JsonLibraryType jsonType, Action<T> callback) where T : class
        {
            var stack = Environment.StackTrace;
            if (path.EndsWith(GlobalConsts.JSON, StringComparison.CurrentCultureIgnoreCase) == false)
            {
                path += GlobalConsts.JSON;
            }

            //Debug.LogError($"Pre Converted  {path}");
            var pathType = GlobalConsts.ConvertToAndroidPath(ref path);
            //Debug.LogError($"Converted {pathType}   {path}");

            String jsonString = string.Empty;
            if (pathType == GlobalConsts.PathType.StreamingAssetsPath)
            {
                ConsoleExtra.Log($"GetRequest Streaming Assets Web Request: {path}", null, ConsoleExtraEnum.EDebugType.Json);

                //Application.streamingAssetsPath jar:file:///data/app/com.musictribe.roomviewer-okK9RuB6p7T0P4NiUXAZpw==/base.apk!/assets     only likes UnityWebRequest
                UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequest.Get(path);
                yield return www.SendWebRequest();
                while (!www.isDone)
                {
                    yield return null;
                }
                jsonString = www.downloadHandler.text;
                if (string.IsNullOrEmpty(jsonString) == true)
                {
                    if (showError == true)
                    {
                        Debug.LogError($"Could not load file :{path}  {stack}");
                    }
                    else
                    {
                        Debug.Log($"Could not load file :{path}  {stack}");
                    }
                    callback?.Invoke(null);
                }
                else
                {
                    try
                    {
                        T converted = DeserialiseData<T>(path, jsonString, jsonType, stack);
                        if (converted == null)
                        {
                            TryEverything<T>(path, jsonString, stack);
                        }
                        if (path.CaseInsensitiveContains("CatalogueData/description.json") == true)
                        {
                            Debug.LogError($"Full convert {converted}");
                        }

                        callback?.Invoke(converted);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Could not DeserialiseData: {path}, Error: {e} String: {jsonString}");
                        callback?.Invoke(null);
                    }
                }
            }
            else
            {
                ConsoleExtra.Log($"Read from file async: {path}", null, ConsoleExtraEnum.EDebugType.Json);

                // Core.Contents.FileExist input jar: file:///data/app/com.musictribe.roomviewer-Az9ySY9fUrUUvUlsU6ODSg==/base.apk!/assets/BundleStreamingAssets/LanguageData/english.json
                // returns Converted validPath path bundlestreamingassets/languagedata/english.json

                if (File.Exists(path) == true)
                {
                    var newPath = path.Remove(path.Length - GlobalConsts.JSON.Length);
                    switch (jsonType)
                    {
                        case JsonLibraryType.FullSerializer:
                            Json.FullSerialiser.ReadFromFileAsync<T>(newPath, (aysncData) =>
                            {
                                callback?.Invoke(aysncData);
                            });
                            break;
                        case JsonLibraryType.JsonNet:
                            Json.JsonNet.ReadFromFileAsync<T>(newPath, (aysncData) =>
                            {
                                callback?.Invoke(aysncData);
                            });
                            break;
                        case JsonLibraryType.Unity:
                            Json.Unity.ReadFromFileAsync<T>(newPath, (aysncData) =>
                            {
                                callback?.Invoke(aysncData);
                            });
                            break;
                        default:
                            break;
                    }


                    // jsonString = File.ReadAllText(path); // this caused issues
                }
                else
                {
                    Debug.LogError($"Could not find File: {path}");
                    callback?.Invoke(null);
                }
            }
        }


        private void TryEverything<T>(string path, string jsonString, string stack)
        {
            T converted1 = DeserialiseData<T>(path, jsonString, JsonLibraryType.FullSerializer, stack);
            T converted2 = DeserialiseData<T>(path, jsonString, JsonLibraryType.JsonNet, stack);
            T converted3 = DeserialiseData<T>(path, jsonString, JsonLibraryType.Unity, stack);
            if (converted1 == null)
            {
                Debug.LogError($"Testing   Json file: { path}, was not parsed  {JsonLibraryType.FullSerializer}");
            }
            else
            {
                Debug.LogError($"Testing   Json file: { path}, was  PARSED  {JsonLibraryType.FullSerializer}");
            }

            if (converted2 == null)
            {
                Debug.LogError($"Testing   Json file: { path}, was not parsed  {JsonLibraryType.JsonNet}");
            }
            else
            {
                Debug.LogError($"Testing   Json file: { path}, was  PARSED  {JsonLibraryType.JsonNet}");
            }

            if (converted3 == null)
            {
                Debug.LogError($"Testing   Json file: { path}, was not parsed  {JsonLibraryType.Unity}");
            }
            else
            {
                Debug.LogError($"Testing   Json file: { path}, was  PARSED  {JsonLibraryType.Unity}");
            }
        }

        public T DeserialiseData<T>(string path, string jsonText, JsonLibraryType unityType, string stack)
        {
            switch (unityType)
            {
                case JsonLibraryType.FullSerializer:
                    try
                    {
                        return FullSerialiserAndroid.FullDeserialiseData<T>(jsonText);
                    }
                    catch(Exception e)
                    {
                        Debug.LogError($"DeserialiseDataAysnc :{unityType} , path: {path} , Error Message :{e.Message} , stack:{stack}");
                    }
                    break;
                case JsonLibraryType.JsonNet:
                    try
                    {
                        return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(jsonText, JsonNetLibrary.GetDeserialiseSettings());
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"DeserialiseDataAysnc :{unityType} , path: {path} , Error Message :{e.Message} , stack:{stack}");
                    }
                    break;
                case JsonLibraryType.Unity:
                    try
                    {
                        return JsonUtility.FromJson<T>(jsonText);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"DeserialiseDataAysnc :{unityType} , path: {path} , Error Message :{e.Message} , stack:{stack}");
                    }
                    break;
                default:
                    break;
            }
            return JsonUtility.FromJson<T>(jsonText);
        }
    }
}
