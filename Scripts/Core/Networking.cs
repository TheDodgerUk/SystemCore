using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Video;


public class Networking
{
    private NetworkingMono m_NetworkingMono;

    public Networking(NetworkingMono networkingMono) => m_NetworkingMono = networkingMono;

    public NetworkingUsers Users => m_NetworkingMono.Users;

    public bool? IsConnected => m_NetworkingMono.IsConnected;

    public void LoadManifestJsonFile<T>(string fileName, Action<T> returnData) where T : class => m_NetworkingMono.LoadManifestJsonFile(fileName, returnData);


    public void DeleteManifestJsonFile(string fileName, Action<bool> callback) => m_NetworkingMono.DeleteManifestJsonFile(fileName, callback);
    public void SaveManifestJsonFile<T>(string fileName, T generalData, Action<bool> callback) where T : class => m_NetworkingMono.SaveManifestJsonFile<T>(fileName, generalData, callback);
    public void GetManifestJsonList(string endsWith, Action<List<string>> callback) => m_NetworkingMono.GetManifestJsonList(endsWith, callback);

    public void LoadAssetJsonFile<T>(string fileName, bool asyncConvert, Action<T> returnData) where T : class => m_NetworkingMono.LoadAssetJsonFile<T>(fileName, asyncConvert, returnData);

    public void GetCustomFileList(Action<List<string>> callback) => m_NetworkingMono.GetCustomFileList(callback);

    public void GetCustomFileListAudio(Action<List<string>> callback) => m_NetworkingMono.GetCustomFileListAudio(callback);

    public void SetLoadingScreen(LoadingScreenBase loadingScreen) => m_NetworkingMono.SetLoadingScreen(loadingScreen);

    public void GetCustomFileTexture(string fileName, Action<Texture> callback) => m_NetworkingMono.GetCustomFileTexture(fileName, callback);
    public void GetCustomFileAudioClip(string fileName, Action<AudioClip> callback) => m_NetworkingMono.GetCustomFileAudioClip(fileName, callback);

    public void DownloadCustomFileJsonFile<T>(string fileName, bool asyncConvert, Action<T> returnData) where T : class => m_NetworkingMono.GetCustomFileJsonFile<T>(fileName, asyncConvert, returnData);

    public void DownloadCustomFileJsonFiles<T>(List<string> fileName, bool asyncConvert, Action<Dictionary<string, T>> returnData) where T : class => m_NetworkingMono.GetCustomFileJsonFiles<T>(fileName, asyncConvert, returnData);

    public void UploadCustomFileJsonFiles(List<string> fileNames, Action<float> progressCallback, Action callback)  => m_NetworkingMono.UploadCustomUserFiles(fileNames, progressCallback, callback);


    public void GetAssetBundle(string fileName, Action<float> progressCallback, Action<NetworkingMono.AssetBundleData> callback) => m_NetworkingMono.GetAssetBundle(fileName, progressCallback, callback);
    public void UploadCustomUserFile(string path, Action<float> progressCallback, Action callback) => m_NetworkingMono.UploadCustomUserFile(path, progressCallback, callback);

    
    public void DownloadCustomUserFile(string path, Action<byte[]> callback, Action<float> progress) => m_NetworkingMono.DownloadCustomFile(path, callback, progress);
    public void RemoveCustomFile(string path, Action<float> progress, Action callback) => m_NetworkingMono.RemoveCustomFile(path, progress, callback);

    public void GetAssetVersionNumber(CatalogueEntry entry, Action<int> callback) => m_NetworkingMono.GetAssetVersionNumber(entry, callback);


    public void UploadCustomUserFiles(List<string> paths, Action<float> progressCallback, Action callback)
    {
        TaskAction action = new TaskAction(paths.Count, () =>
        {
            callback?.Invoke();
        });

        foreach(var item in paths)
        {
            m_NetworkingMono.UploadCustomUserFile(item, progressCallback, () =>
            {
                action.Increment();
            });
        }      
    }

    public void DownloadCustomUserFiles(List<string> paths, Action<float> percentage , Action<Dictionary<string, byte[]>> callback)
    {
        Dictionary<string, byte[]> collectedData = new Dictionary<string, byte[]>();

        TaskAction action = new TaskAction(paths.Count, () =>
         {
             callback.Invoke(collectedData);
         });

        for (int i = 0; i < paths.Count; i++)
        {
            int index = i;

            m_NetworkingMono.DownloadCustomFile(paths[index], (colletedData) =>
            {
                collectedData.Add(paths[index], colletedData);
                
                action.Increment();

                float percentageAmount = (float)action.GetCount / (float)paths.Count;
                percentage?.Invoke(percentageAmount);
            }, null);
        }

    }



}


public class NetworkingMono : MonoBehaviour
{
    public class JsonData
    {
        public string m_jsonFileName;
        public string m_jsonData;
    }

    public class AssetBundleData
    {
        public AssetBundle m_AssetBundle;
        public byte[] m_Data;
    }

    private const string CUSTOM_USER_DATA = "CustomUserData";
    private const string CUSTOM_USER_DATA_MANIFEST = "CustomUserData/Manifest";

    private const string SECTRET = "2edbd85f271f7d4535e19725eafe4372f450fe8e39a39d01d4c7d2fa4be0bb160e30373340710387d4a9ff6f9561a39518623ebe356e850b6a15120f82d539ffde07ae4a0c260c966a0c6b15dd869f5e1a4fbe1fc02c45f65012529bd5d7d745b2b9cd5bd4b491e3abe7e45d0f9f109e04a7abfd7d453be6a7a00df261f330a4c27284fb62f42bcde10a5992bde9a16d3b6f4a2d3f716cd6145a3df21a3cbb22e7f6ee5b9bce9fd1891bfb6ae49c9e70f1808abcfd3ceeb5e0769eecd61f80f3c0cb5ebbf019aa656df273c6b3dcde7c51bafcc61d07b87dde67b467a3a8e1dd5ee44f54e42fc8f2725e66bbe3399bf70727dd8ee62f013af22f72a9ba706a16dba214d386cde5d0dc01bea33582e5b50004f9a6547d111181c804fb2ca9081df7c903556c39caa36d24f98f65570dd9a70a9cecd7f39ec3a45e667e02bb847810f5ca975b0dc99c7bbb57ada9a7da8b52aba60af3523dd6e1191c352aaf90ca67bd36af3ce7ecb2dc03c910387a0d6acd5e1db997007691c8b5a6cc0f1c80d039ec9a25cb784040ecd99922b5bb667a3747eed8b65b74340db4fe49db7b632e2e22412a6d95b75e931dc30076496a24f5eedc9909bc6351e6ea121882b89fda68ba578e91ec779466aff046b6474eb8e10f1d5beba98d72d6042fa020b3193965148f81a7aeaba3e5db3ba2673995eca1684f91e0a5be52467c8d578e2425cb";
    private const string BASE_TOKEN_ADDRESS = "https://cloud.midasconsoles.com/api/v2";
    private const string TOKEN_ADDRESS = BASE_TOKEN_ADDRESS + "/auth/tokens";
    private const string UUID = "c67d049a-9854-4e4a-916d-1be9fafde57a";


    private const string BASE_UPLOAD = "https://room-builder.cloud.midasconsoles.com/builder/v1/";
    private const string BASE_DOWNLOAD = "https://room-builder.cloud.midasconsoles.com/builder/v1/asset/signedUrl/";

    private const string MANIFEST_FULL_LIST = BASE_UPLOAD + "manifest";
    private const string MANIFEST_SINGLE_ITEM = MANIFEST_FULL_LIST + "/";
    private const string ASSET_READ_FAST = BASE_DOWNLOAD;// + "asset/";
    private const string ASSET_READ_SLOW = BASE_UPLOAD + "asset/";
    private const string ASSET_WRITE = BASE_UPLOAD + "asset";

    private const string AUT = "Authorization";
    private string BEARING = "";

    private const string INTERNAL_PROOF = "bd5280b8bd8b3820ffc069947710a85e6d9186c3e261e6f358b0a05a68262b6a";
    private const string INTERNAL_PROOFSALT = "f72c997758886bb4de7a41dc8ea319e5f19ae3a690c4990d435fd3ac865cef7f93d48ea6ffb129df8222e67ebd1df8b30b4e6a27d03827fb45372ac58afd8719";

    private LoadingScreenBase m_LoadingScreen;

    public bool? IsConnected { get; private set; }
    public NetworkingUsers Users { get; private set; }

    [InspectorButton]
    private void DebugPrintUserNames() => Users.DebugPrintUserNames();

    [InspectorButton]
    private void DebugClearUsersNames() => Users.DebugClearUsersNames();

    [InspectorButton]
    private void DebugClearCurrentUser() => Users.ClearCurrentUser();


    public void UploadCustomUserFiles(List<string> paths, Action<float> progress, Action callback) => UploadCustomFiles(Users.CurrentUser.m_UserName.ToLower(), paths, progress, callback);
    public void UploadCustomUserFile(string path, Action<float> progress, Action callback) => UploadCustomFile(Users.CurrentUser.m_UserName.ToLower(), path, progress, callback);
    public void RemoveCustomFile(string path, Action<float> progress, Action callback) => RemoveCustomFile(Users.CurrentUser.m_UserName.ToLower(), path, progress, callback);

    public void DownloadCustomFile(string path, Action<byte[]> callback, Action<float> progress) => DownloadCustomFile(Users.CurrentUser.m_UserName.ToLower(), path, callback, progress);

    public void GetCustomFileList(Action<List<string>> callback) => GetCustomFileList(Users.CurrentUser.m_UserName.ToLower(), callback);

    public void GetCustomFileAudioClip(string fileName, Action<AudioClip> callback) => GetCustomFileAudioClip(Users.CurrentUser.m_UserName.ToLower(), fileName, callback);
    public void GetCustomFileTexture(string fileName, Action<Texture> callback) => GetCustomFileTexture(Users.CurrentUser.m_UserName.ToLower(), fileName, callback);



    public void GetCustomFileListAudio(Action<List<string>> callback)
    {
        GetCustomFileList(Users.CurrentUser.m_UserName.ToLower(), (items) =>
        {
            List<string> allItems = new List<string>();
            foreach (var item in items)
            {
                if (item.EndsWith("mp3") || item.EndsWith("wav"))
                {
                    allItems.Add(item);
                }
            }
            callback?.Invoke(allItems);
        });
    }

    private void Awake()
    {
        IsConnected = null;
        StartCoroutine(RetrieveToken());
    }


    [InspectorButton]
    private void PrintUsersCustomFileList()
    {
        GetCustomFileList(Users.CurrentUser.m_UserName.ToLower(), (data) =>
        {
            if (data != null)
            {
                foreach (var item in data)
                {
                    Debug.LogError(item);
                }
            }
        });
    }


    [InspectorButton]
    private void OpenPersistentDataPath()
    {
        string filePathFixed = Application.persistentDataPath.Replace(@"/", @"\");
        Debug.LogError(filePathFixed);
#if UNITY_EDITOR_OSX
        Debug.LogError($"There is no short cut for this, on ios , find {filePathFixed}");
#else
        System.Diagnostics.Process.Start("explorer.exe", filePathFixed);
#endif

    }

    [InspectorButton]
    private void DebugManifestJsonList(Action<List<string>> callback)
    {
        StartCoroutine(InternalGetManifestJsonList("", (list) =>
        {
            Debug.LogError($"Count {list.Count}");
            for (int i = 0; i < list.Count; i++)
            {
                Debug.LogError($"Item {i}   {list[i]}");
            }
        }));
    }

    private IEnumerator RetrieveToken()
    {
        var InternalSecretPlusProofSalt = SECTRET + INTERNAL_PROOFSALT;
        var InternalValidCheck = SHA256HexHashString(InternalSecretPlusProofSalt).ToLower();
        if (InternalValidCheck != INTERNAL_PROOF)
        {
            Debug.LogError($"InternalValidCheck {InternalValidCheck}");
            Debug.LogError($"INTERNAL_PROOF INVALID {INTERNAL_PROOF}");
            IsConnected = false;
            yield break;
        }


        UnityEngine.WWWForm form = new UnityEngine.WWWForm();
        form.AddField("client", UUID);
        UnityWebRequest requestU = UnityWebRequest.Post(TOKEN_ADDRESS, form);

        yield return requestU.SendWebRequest();

        if (IsNetworkResponseValid(requestU) == false)
        {
            Debug.LogError($"responseWeb {requestU.error}");
            Debug.LogError($"Cant connect to web server check server    TOKEN_ADDRESS: {TOKEN_ADDRESS} ");
            IsConnected = false;
            Users = new NetworkingUsers();
            requestU.Dispose();
            yield break;
        }

        var parseData = JObject.Parse(requestU.downloadHandler.text);
        var proofSalt = Convert.ToString(parseData["client"]["proofSalt"]);
        var proof = Convert.ToString(parseData["client"]["proof"]);
        var salt = Convert.ToString(parseData["client"]["salt"]);
        var responsePath = Convert.ToString(parseData["responsePath"]);
        responsePath = BASE_TOKEN_ADDRESS + responsePath;


        var secretPlusProofSalt = SECTRET + proofSalt;
        var validCheck = SHA256HexHashString(secretPlusProofSalt).ToLower();
        if (validCheck != proof)
        {
            IsConnected = false;
            Users = new NetworkingUsers();
            yield break;
        }

        var secretPlusSalt = SECTRET + salt;
        var response = SHA256HexHashString(secretPlusSalt).ToLower();


        UnityEngine.WWWForm form1 = new UnityEngine.WWWForm();
        form1.AddField("clientResponse", response);
        UnityWebRequest responseWeb = UnityWebRequest.Post(responsePath, form1);

        yield return responseWeb.SendWebRequest();
        parseData = JObject.Parse(responseWeb.downloadHandler.text);
        var token = Convert.ToString(parseData["token"]);

        BEARING = "Bearer " + token;
        IsConnected = true;
        Users = new NetworkingUsers();
        responseWeb.Dispose();
        requestU.Dispose();
    }

    private bool IsNetworkResponseValid(UnityWebRequest request) => (request.isNetworkError == false && string.IsNullOrEmpty(request.error) == true);


    public void GetManifestJsonList(string endsWith, Action<List<string>> callback) => StartCoroutine(InternalGetManifestJsonList(endsWith, callback));

    public void DeleteManifestJsonFile(string fileName, Action<bool> callback) => StartCoroutine(InternalDeleteManifestJsonFile(fileName, callback));

    public void LoadManifestJsonFile<T>(string fileName, Action<T> returnData) where T : class
    {
        StartCoroutine(InternalLoadManifestJsonFile(fileName, (text) =>
        {
            Json.JsonNet.ReadFromTextAsync<T>(text, (data) =>
            {
                returnData?.Invoke(data);
            });
        }));
    }


    public void SetLoadingScreen(LoadingScreenBase loadingScreen) => m_LoadingScreen = loadingScreen;

    public void LoadAssetJsonFile<T>(string fileName, bool asyncConvert, Action<T> returnData) where T : class
    {

        if (fileName.EndsWith(GlobalConsts.JSON, StringComparison.CurrentCultureIgnoreCase) == false)
        {
            fileName += GlobalConsts.JSON;
        }


        StartCoroutine(InternalLoadJsonFile(fileName, (jsonData) =>
        {
            if ((jsonData == null) || (string.IsNullOrEmpty(jsonData.m_jsonData) == true))
            {
                Debug.LogError($"InternalLoadJsonFile failed, Original filename {fileName}");
                returnData?.Invoke(null);
                return;
            }
            else
            {
                Debug.Log($"Reading JSON file. Async: {asyncConvert} Filename: {fileName}");

#if !UNITY_WEBGL
                if (asyncConvert == true)
                {
                    Json.JsonNet.ReadFromTextAsync<T>(jsonData.m_jsonData, (data) =>
                    {
                        returnData?.Invoke(data);
                        return;
                    });
                }
                else
#endif
                {
                    var data = Json.JsonNet.ReadFromText<T>(jsonData.m_jsonData);
                    returnData?.Invoke(data);
                    return;
                }
            }
        }));
    }


    public void GetCustomFileJsonFile<T>(string fileName, bool asyncConvert, Action<T> returnData) where T : class
    {

        if (fileName.EndsWith(GlobalConsts.JSON, StringComparison.CurrentCultureIgnoreCase) == false)
        {
            fileName += GlobalConsts.JSON;
        }

        if (Users.CurrentUser != null)
        {
            StartCoroutine(InternalGetCustomJsonFile(Users.CurrentUser.m_UserName.ToLower(), fileName, (jsonData) =>
            {
                if (string.IsNullOrEmpty(jsonData.m_jsonData) == true)
                {
                    returnData?.Invoke(null);
                    return;
                }
                else
                {
                    if (asyncConvert == true)
                    {
                        Json.JsonNet.ReadFromTextAsync<T>(jsonData.m_jsonData, (data) =>
                        {
                            returnData?.Invoke(data);
                            return;
                        });
                    }
                    else
                    {
                        var data = Json.JsonNet.ReadFromText<T>(jsonData.m_jsonData);
                        returnData?.Invoke(data);
                        return;
                    }
                }
            }));
        }
        else
        {
            returnData?.Invoke(null);
        }
    }

    public void GetCustomFileJsonFiles<T>(List<string> fileNames, bool asyncConvert, Action<Dictionary<string, T>> returnData) where T : class
    {
        Dictionary<string, T> collectedData = new Dictionary<string, T>();
        TaskAction task = new TaskAction(fileNames.Count, () =>
        {
            returnData?.Invoke(collectedData);
        });


        for (int i = 0; i < fileNames.Count; i++)
        {
            int index = i;
            if (fileNames[index].EndsWith(GlobalConsts.JSON, StringComparison.CurrentCultureIgnoreCase) == false)
            {
                fileNames[index] += GlobalConsts.JSON;
            }

            if (Users.CurrentUser != null)
            {
                StartCoroutine(InternalGetCustomJsonFile(Users.CurrentUser.m_UserName.ToLower(), fileNames[index], (jsonData) =>
                {
                    if (string.IsNullOrEmpty(jsonData.m_jsonData) == true)
                    {
                        collectedData.Add(fileNames[index], null);
                        task.Increment();
                    }
                    else
                    {
                        if (asyncConvert == true)
                        {
                            Json.JsonNet.ReadFromTextAsync<T>(jsonData.m_jsonData, (data) =>
                            {
                                collectedData.Add(fileNames[index], data);
                                task.Increment();
                            });
                        }
                        else
                        {
                            var data = Json.JsonNet.ReadFromText<T>(jsonData.m_jsonData);
                            collectedData.Add(fileNames[index], data);
                            task.Increment();
                        }
                    }
                }));
            }
            else
            {
                returnData?.Invoke(null);
            }
        }
    }


    public void SaveManifestJsonFile<T>(string fileName, T generalData, Action<bool> callback) where T : class
    {
        string jsonString = Json.JsonNet.WriteToText<T>(generalData, true);
        StartCoroutine(InternalSaveManifestJsonFile(fileName, jsonString, callback));
    }

    public void GetAssetBundle(string fileName, Action<float> progressCallback, Action<AssetBundleData> callback)
    {
        StartCoroutine(InternalGetAssetBundle(fileName, progressCallback, callback));
    }



    #region maths
    private string SHA256HexHashString(string StringIn)
    {
        string hashString;
        using (var sha256 = SHA256Managed.Create())
        {

            var bytes = new byte[StringIn.Length / 2];
            for (var i = 0; i < bytes.Length; i++)
            {
                var s = StringIn.Substring(i * 2, 2);
                bytes[i] = Convert.ToByte(s, 16);
            }

            var hash = sha256.ComputeHash(bytes);
            hashString = ToHex(hash);
        }
        return hashString;
    }

    private string ToHex(byte[] bytes)
    {
        StringBuilder result = new StringBuilder(bytes.Length * 2);

        for (int i = 0; i < bytes.Length; i++)
            result.Append(bytes[i].ToString("x2"));

        return result.ToString();
    }
    #endregion

    private void DisplayMessage(string message)
    {
        if (m_LoadingScreen != null)
        {
            m_LoadingScreen.SetMessage(message);
        }
        Debug.LogError(message);
    }

    private IEnumerator InternalGetManifestJsonList(string endsWith, Action<List<string>> callback)
    {
        while (IsConnected == null)
        {
            DisplayMessage($"Stuck on InternalGetManifestJsonList {endsWith}");
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForFixedUpdate();

        if (IsConnected == false)
        {
            Debug.LogError("Not connected to the web server check server");
            callback?.Invoke(null);
            yield break;
        }

        string file = MANIFEST_FULL_LIST;
        UnityWebRequest requestU = UnityWebRequest.Get(file);
        requestU.SetRequestHeader(AUT, BEARING);
        requestU.SetRequestHeader("Content-Type", "application/json");
        requestU.SetRequestHeader("Accept", "application/json");

        yield return requestU.SendWebRequest();


        Json.JsonNet.ReadFromTextAsync<List<string>>(requestU.downloadHandler.text, (data) =>
        {
            List<string> seperateList = new List<string>();
            for (int i = 0; i < data.Count; i++)
            {
                var item = data[i];
                if (item.EndsWith(endsWith) == true)
                {
                    var strippedItem = item.Replace(endsWith, "");
                    seperateList.Add(strippedItem);
                }
            }
            requestU.Dispose();
            callback?.Invoke(seperateList);
        });
    }

    private IEnumerator InternalSaveManifestJsonFile(string fullFileName, string jsonString, Action<bool> callback)
    {
        while (IsConnected == null)
        {
            DisplayMessage($"Stuck on InternalSaveManifestJsonFile {fullFileName}");
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForFixedUpdate();

        if (IsConnected == false)
        {
            Debug.LogError("Not connected to the web server check server");
            callback?.Invoke(false);
            yield break;
        }

        string file = MANIFEST_SINGLE_ITEM + fullFileName;
        UnityWebRequest requestU = UnityWebRequest.Put(file, jsonString);
        requestU.SetRequestHeader(AUT, BEARING);
        requestU.SetRequestHeader("Content-Type", "application/json");
        requestU.SetRequestHeader("Accept", "application/json");

        yield return requestU.SendWebRequest();

        if (IsNetworkResponseValid(requestU) == true)
        {
            while (requestU.uploadProgress < 1)
            {
                yield return new WaitForEndOfFrame();
            }
            Debug.Log($"PutFile requestU.uploadedBytes {requestU.uploadedBytes}   uploadedBytes {requestU.uploadHandler.data}   progress {requestU.uploadProgress * 100}%    {jsonString},    ");
            requestU.Dispose();
            callback?.Invoke(true);
        }
        else
        {
            Debug.LogError($"PutFile FAIL:    isHttpError:{requestU.isHttpError}  {requestU.error}  {file}");
            requestU.Dispose();
            callback?.Invoke(false);
        }

    }

    private IEnumerator InternalLoadManifestJsonFile(string fullFileName, Action<string> callback)
    {
        while (IsConnected == null)
        {
            DisplayMessage($"Stuck on InternalLoadManifestJsonFile {fullFileName}");
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForFixedUpdate();

        if (IsConnected == false)
        {
            Debug.LogError("Not connected to the web server check server");
            callback?.Invoke(null);
            yield break;
        }

        string file = MANIFEST_SINGLE_ITEM + fullFileName;
        UnityWebRequest requestU = UnityWebRequest.Get(file);
        requestU.SetRequestHeader(AUT, BEARING);
        requestU.SetRequestHeader("Content-Type", "application/json");
        requestU.SetRequestHeader("Accept", "application/json");

        yield return requestU.SendWebRequest();

        if (IsNetworkResponseValid(requestU) == true)
        {
            while (requestU.downloadProgress < 1)
            {
                yield return new WaitForEndOfFrame();
            }
            ConsoleExtra.Log($"GetFile {file}", null, ConsoleExtraEnum.EDebugType.StartUp);
            callback?.Invoke(requestU.downloadHandler.text);
            requestU.Dispose();
        }
        else
        {
            Debug.LogError($"GetFile FAIL:    isHttpError:{requestU.isHttpError}  {requestU.error}  {file}");
            requestU.Dispose();
            callback?.Invoke(null);
        }

    }

    private IEnumerator InternalDeleteManifestJsonFile(string fileName, Action<bool> callback)
    {
        while (IsConnected == null)
        {
            DisplayMessage($"Stuck on InternalDeleteManifestJsonFile {fileName}");
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForFixedUpdate();

        if (IsConnected == false)
        {
            Debug.LogError("Not connected to the web server check server");
            callback?.Invoke(false);
            yield break;
        }

        string file = MANIFEST_SINGLE_ITEM + fileName;
        UnityWebRequest requestU = UnityWebRequest.Delete(file);
        requestU.SetRequestHeader(AUT, BEARING);
        requestU.SetRequestHeader("Content-Type", "application/json");
        requestU.SetRequestHeader("Accept", "application/json");

        yield return requestU.SendWebRequest();

        if (IsNetworkResponseValid(requestU) == true)
        {
            Debug.Log($"Delete isHttpError:{requestU.isHttpError}  {requestU.error}  {file}");
            requestU.Dispose();
            callback?.Invoke(true);
        }
        else
        {
            Debug.LogError($"Delete FAIL: isHttpError:{requestU.isHttpError}  {requestU.error}  {file}");
            requestU.Dispose();
            callback?.Invoke(false);
        }
    }


    private IEnumerator InternalGetAssetBundle(string fileName, Action<float> progressCallback, Action<AssetBundleData> callback)
    {
        fileName = GetCorrectFilename(fileName);
        while (IsConnected == null)
        {
            DisplayMessage($"Stuck on InternalGetAssetBundle {fileName}");
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForFixedUpdate();

        if (IsConnected == false)
        {
            Debug.LogError("Not connected to the web server check, server is down");
            callback?.Invoke(null);
            yield break;
        }

        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();

        string file = ASSET_READ_FAST + fileName.ToLower();

#if UNITY_EDITOR
        Debug.Log($"Downloading Asset Bundle Data from: {file}");
#endif

        //file = file.ToLower();
        UnityWebRequest requestU = UnityWebRequest.Get(file);
        ConsoleExtra.Log($"AssetBundle UnityWebRequest  {file}", null, ConsoleExtraEnum.EDebugType.NetworkAsset);
        requestU.SetRequestHeader(AUT, BEARING);

        yield return requestU.SendWebRequest();

        while (requestU.downloadProgress < 1)
        {
            yield return new WaitForEndOfFrame();
            Debug.LogError($"{requestU.downloadProgress}");
        }

#if PLATFORM_WEBGL
        UnityWebRequest bundleWebRequest = UnityWebRequestAssetBundle.GetAssetBundle(requestU.downloadHandler.text);
        requestU.Dispose();
        requestU = null;

        StartCoroutine(TrackProgress(file, bundleWebRequest, progressCallback));

        yield return bundleWebRequest.SendWebRequest();

        if (IsNetworkResponseValid(bundleWebRequest) == true)
        {
            while (bundleWebRequest.downloadProgress < 1)
            {
                yield return new WaitForEndOfFrame();
            }

            yield return new WaitForFixedUpdate();

            stopwatch.Stop();
            ConsoleExtra.Log($"GetAssetBundle time: {TimeFromMilliseconds(stopwatch.ElapsedMilliseconds)} {file}", null, ConsoleExtraEnum.EDebugType.NetworkAsset);

            try
            {
                AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(bundleWebRequest);
                AssetBundleData assetBundleData = new AssetBundleData();
                assetBundleData.m_AssetBundle = bundle;
                // there is no raw data to save ,  assetBundleData.m_Data = bundleWebRequest.downloadHandler.data;
                bundleWebRequest.Dispose();
                bundleWebRequest = null;
                callback?.Invoke(assetBundleData);
            }
            catch (Exception e)
            {
                Debug.LogError($"GetAssetBundle ERROR {e.Message}");
                bundleWebRequest.Dispose();
                bundleWebRequest = null;
                callback?.Invoke(null);
            }
        }
        else
        {
            stopwatch.Stop();
            Debug.LogError($"GetAssetBundle FAIL: isHttpError:{bundleWebRequest.isHttpError}  {bundleWebRequest.error}  {file}");
            bundleWebRequest.Dispose();
            bundleWebRequest = null;
            callback?.Invoke(null);
        }
#else
        if (string.IsNullOrEmpty(requestU.error) == true)
        {
            UnityWebRequest requestUSecond = UnityWebRequest.Get(requestU.downloadHandler.text);
            StartCoroutine(TrackProgress(file, requestUSecond, progressCallback));
            yield return requestUSecond.SendWebRequest();

            if (IsNetworkResponseValid(requestUSecond) == true)
            {
                while (requestUSecond.downloadProgress < 1)
                {
                    yield return new WaitForEndOfFrame();
                }
                yield return new WaitForFixedUpdate();
                stopwatch.Stop();
                ConsoleExtra.Log($"GetAssetBundle time: {TimeFromMilliseconds(stopwatch.ElapsedMilliseconds)} {file}", null, ConsoleExtraEnum.EDebugType.NetworkAsset);
                try
                {
                    AssetBundle bundle = AssetBundle.LoadFromMemory(requestUSecond.downloadHandler.data);
                    AssetBundleData assetBundleData = new AssetBundleData();
                    assetBundleData.m_AssetBundle = bundle;
                    assetBundleData.m_Data = requestUSecond.downloadHandler.data;
                    requestUSecond.Dispose();
                    requestU.Dispose();
                    callback?.Invoke(assetBundleData);
                }
                catch (Exception e)
                {
                    Debug.Log($"GetAssetBundle ERROR {e.Message}");
                    requestUSecond.Dispose();
                    requestU.Dispose();
                    callback?.Invoke(null);
                }

            }
            else
            {
                stopwatch.Stop();
                Debug.LogError($"GetAssetBundle FAIL: isHttpError:{requestUSecond.isHttpError}  {requestUSecond.error}  {file}");
                requestUSecond.Dispose();
                requestU.Dispose();
                callback?.Invoke(null);
            }
        }
        else
        {
            stopwatch.Stop();
            Debug.LogError($"GetAssetBundle FAIL: isHttpError:{requestU.isHttpError}  {requestU.error}  {file}");
            requestU.Dispose();
            callback?.Invoke(null);
        }
#endif
    }


    private IEnumerator TrackProgress(string fileName, UnityWebRequest request, Action<float> progress)
    {
        if (request != null)
        {
            while (request != null && request.isDone == false)
            {
                progress?.Invoke(Mathf.Clamp01(request.downloadProgress));
                yield return new WaitForEndOfFrame();
            }
        }
        else
        {
            Debug.LogError($"Request was null, fileName : {fileName}");
            progress?.Invoke(1f);
        }
    }

    private IEnumerator InternalLoadJsonFile(string fileName, Action<JsonData> callback)
    {
        Debug.Log($"InternalLoadJsonFile: {fileName}");

        fileName = GetCorrectFilename(fileName);

        while (IsConnected == null)
        {
            DisplayMessage($"Stuck on InternalLoadJsonFile {fileName}");
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForFixedUpdate();

        if (IsConnected == false)
        {
            Debug.LogError("Not connected to the web server check, server is down");
            callback?.Invoke(null);
            yield break;
        }

        string file = ASSET_READ_SLOW + fileName;
        file = file.ToLower();
        UnityWebRequest requestU = UnityWebRequest.Get(file);
        requestU.SetRequestHeader(AUT, BEARING);

        Debug.Log($"SendWebRequest:  {fileName} URL: {requestU.url}");
        yield return requestU.SendWebRequest();

        Debug.Log($"Check request! {fileName}");
        if (IsNetworkResponseValid(requestU) == true)
        {
            Debug.Log($"Starting download {fileName}");
            while (requestU.downloadProgress < 1)
            {
                Debug.Log($"Download Progress: {fileName} {requestU.downloadProgress} ({fileName})");
                yield return null;
            }
            yield return null;
            ConsoleExtra.Log($"GetJsonFile  {file}", null, ConsoleExtraEnum.EDebugType.NetworkAsset);
            JsonData newJsonData = new JsonData();
            newJsonData.m_jsonFileName = file;
            newJsonData.m_jsonData = requestU.downloadHandler.text;
            Debug.Log($"Download Complete: {fileName}");
            requestU.Dispose();
            callback?.Invoke(newJsonData);
        }
        else
        {
            JsonData newJsonData = new JsonData();
            newJsonData.m_jsonFileName = file;
            Debug.LogError($"GetJsonFile FAIL: isHttpError:{requestU.isHttpError}  {requestU.error}  {file}");
            requestU.Dispose();
            callback?.Invoke(newJsonData);
        }
    }

    private string GetCorrectFilename(string fullFilename)
    {
        fullFilename = fullFilename.ToLower();
        if (fullFilename.StartsWith(Application.streamingAssetsPath.ToLower()) == true)
        {
            fullFilename = fullFilename.Replace(Application.streamingAssetsPath.ToLower(), "");
            fullFilename = fullFilename.Remove(0, 1); // remove first / 
        }
        return fullFilename;
    }


    public void GetCustomFileList(string userName, Action<List<string>> callback) => StartCoroutine(InternalGetCustomFileList(userName, callback));

    private IEnumerator InternalGetCustomFileList(string username, Action<List<string>> callback)
    {
        string customListFileName = $"{CUSTOM_USER_DATA_MANIFEST}/{username.ToLower()}";
        while (IsConnected == null)
        {
            DisplayMessage($"Stuck on InternalGetCustomFileList {customListFileName}");
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForFixedUpdate();

        if (IsConnected == false)
        {
            Debug.LogError("Not connected to the web server check, server is down");
            callback?.Invoke(null);
            yield break;
        }

        string file = ASSET_READ_SLOW + customListFileName;
        file = file.ToLower();
        UnityWebRequest requestU = UnityWebRequest.Get(file);
        ConsoleExtra.Log($"GetCustomFileList UnityWebRequest  {file}", null, ConsoleExtraEnum.EDebugType.NetworkAsset);
        requestU.SetRequestHeader(AUT, BEARING);

        yield return requestU.SendWebRequest();

        if (IsNetworkResponseValid(requestU) == true)
        {
            while (requestU.downloadProgress < 1)
            {
                yield return new WaitForEndOfFrame();
            }
            yield return new WaitForFixedUpdate();

            Json.JsonNet.ReadFromTextAsync<List<string>>(requestU.downloadHandler.text, (data) =>
            {
                requestU.Dispose();
                callback?.Invoke(data);
            });

        }
        else
        {
            Debug.LogError($"InternalGetCustomFileList FAIL: isHttpError:{requestU.isHttpError}  {requestU.error}  {file}");
            requestU.Dispose();
            callback?.Invoke(null);
        }
    }


    public void RemoveCustomFile(string userName, string fullPath, Action<float> progress, Action callback)
    {
        var data = File.ReadAllBytes(fullPath);
        var fileName = Path.GetFileName(fullPath);
        string folderName = $"{CUSTOM_USER_DATA}/{userName}";
        StartCoroutine(InternalRemoveCustomFile(folderName, fileName, data, () =>
        {
            StartCoroutine(InternalGetCustomFileList(userName, (info) =>
            {
                if (info == null)
                {
                    info = new List<string>();
                }
                info.Add(fileName);
                string dataString = Json.JsonNet.WriteToText(info, true);
                var fileData = Encoding.ASCII.GetBytes(dataString);
                string customListFolderName = $"{CUSTOM_USER_DATA_MANIFEST}";

                customListFolderName = customListFolderName.ToLower();
                userName = userName.ToLower();
                StartCoroutine(InternalUploadCustomFile(customListFolderName, userName, fileData, progress, callback));
            }));

        }));
    }

    private IEnumerator InternalRemoveCustomFile(string folderName, string fileName, byte[] data, Action callback)
    {
        fileName = fileName.ToLower();
        folderName = folderName.ToLower();
        while (string.IsNullOrEmpty(BEARING))
        {
            yield return new WaitForSecondsRealtime(0.1f);
        }

        string file = CUSTOM_USER_DATA + "/" + fileName;
        UnityWebRequest requestU = UnityWebRequest.Delete(file);
        requestU.SetRequestHeader(AUT, BEARING);

        yield return requestU.SendWebRequest();
        try
        {
            if (IsNetworkResponseValid(requestU) == true)
            {
                string message = $"InternalRemoveCustomFile folderName:{file}        {folderName}  fileName:{fileName}";
                Debug.Log($"{message}");
                requestU.Dispose();
                callback?.Invoke();
                yield break;
            }

        }
        catch (Exception e)
        {
            Debug.LogError($"Exception  {e.Message}");
            requestU.Dispose();
            callback?.Invoke();
        }
    }

    public void UploadCustomFile(string userName, string fullPath, Action<float> progress, Action callback)
    {
        if (File.Exists(fullPath))
        {
            var data = File.ReadAllBytes(fullPath);
            var fileName = Path.GetFileName(fullPath);
            string folderName = $"{CUSTOM_USER_DATA}/{userName}";
            StartCoroutine(InternalUploadCustomFile(folderName, fileName, data, progress, () =>
            {
                StartCoroutine(InternalGetCustomFileList(userName, (info) =>
                {
                    if (info == null)
                    {
                        info = new List<string>();
                    }
                    info.Add(fileName);
                    info = info.Distinct().ToList();
                    string dataString = Json.JsonNet.WriteToText(info, true);
                    var fileData = Encoding.ASCII.GetBytes(dataString);
                    string customListFolderName = $"{CUSTOM_USER_DATA_MANIFEST}";

                    customListFolderName = customListFolderName.ToLower();
                    userName = userName.ToLower();
                    StartCoroutine(InternalUploadCustomFile(customListFolderName, userName, fileData, null, callback));
                }));

            }));
        }
        else
        {
            Debug.LogError($"File does not exist {fullPath}");
            callback?.Invoke();
        }
    }

    public void UploadCustomFiles(string userName, List<string> fullPaths, Action<float> progress, Action callback)
    {

        TaskAction task = new TaskAction(fullPaths.Count, () =>
        {
            callback.Invoke();
        });
        foreach (var fullPath in fullPaths)
        {


            if (File.Exists(fullPath))
            {
                var data = File.ReadAllBytes(fullPath);
                var fileName = Path.GetFileName(fullPath);
                string folderName = $"{CUSTOM_USER_DATA}/{userName}";
                StartCoroutine(InternalUploadCustomFile(folderName, fileName, data, progress, () =>
                {
                    StartCoroutine(InternalGetCustomFileList(userName, (info) =>
                    {
                        if (info == null)
                        {
                            info = new List<string>();
                        }
                        info.Add(fileName);
                        info = info.Distinct().ToList();
                        string dataString = Json.JsonNet.WriteToText(info, true);
                        var fileData = Encoding.ASCII.GetBytes(dataString);
                        string customListFolderName = $"{CUSTOM_USER_DATA_MANIFEST}";

                        customListFolderName = customListFolderName.ToLower();
                        userName = userName.ToLower();
                        StartCoroutine(InternalUploadCustomFile(customListFolderName, userName, fileData, null, () => task.Increment()));
                    }));

                }));
            }
            else
            {
                Debug.LogError($"File does not exist {fullPath}");
                task.Increment();
            }
        }
    }

    private IEnumerator InternalUploadCustomFile(string folderName, string fileName, byte[] data, Action<float> progress, Action callback)
    {
        fileName = fileName.ToLower();
        folderName = folderName.ToLower();
        while (string.IsNullOrEmpty(BEARING))
        {
            yield return new WaitForSecondsRealtime(0.1f);
        }

        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();

        formData.Add(new MultipartFormFileSection("file", data, fileName, "multipart/form-data"));
        formData.Add(new MultipartFormDataSection("folder", folderName));

        string webAsset = ASSET_WRITE;

        UnityWebRequest requestU = UnityWebRequest.Post(webAsset, formData);
        requestU.SetRequestHeader(AUT, BEARING);

        requestU.SendWebRequest();

        if (IsNetworkResponseValid(requestU) == true)
        {
            while (requestU.uploadProgress < 1)
            {
                progress?.Invoke(requestU.uploadProgress);
                yield return null;
            }
            yield return new WaitForFixedUpdate();

            string message = $"UploadCustomFile: folderName:{webAsset}        {folderName}  fileName:{fileName}";
            ConsoleExtra.Log($"{message}", null, ConsoleExtraEnum.EDebugType.NetworkAsset);
            requestU.Dispose();
            callback?.Invoke();
        }
    }


    public void DownloadCustomFile(string userName, string fileName, Action<byte[]> callback, Action<float> progress) => StartCoroutine(InternalDownloadCustomFile(userName, fileName, callback, progress));


    private IEnumerator InternalDownloadCustomFile(string userName, string fileName, Action<byte[]> callback, Action<float> progress)
    {
        string folderName = $"{CUSTOM_USER_DATA}/{userName}/";
        fileName = GetCorrectFilename(fileName);
        fileName = Path.GetFileName(fileName);
        while (IsConnected == null)
        {
            DisplayMessage($"Stuck on InternalGetCustomFileTexture {folderName}");
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForFixedUpdate();

        if (IsConnected == false)
        {
            Debug.LogError("Not connected to the web server check, server is down");
            callback?.Invoke(null);
            yield break;
        }

        string file = ASSET_READ_SLOW + folderName + fileName;
        file = file.ToLower();
        UnityWebRequest requestU = UnityWebRequest.Get(file);
        requestU.SetRequestHeader(AUT, BEARING);

        requestU.SendWebRequest();

        if (IsNetworkResponseValid(requestU) == true)
        {
            while (requestU.downloadProgress < 1)
            {
                // the downlados are all over the place Debug.LogError($"fileName {fileName}  {requestU.downloadProgress}");
                progress?.Invoke(requestU.downloadProgress);
                yield return null;
            }
            yield return new WaitForFixedUpdate();

            ConsoleExtra.Log($"DownloadCustomFile  {file}", null, ConsoleExtraEnum.EDebugType.NetworkAsset);
            callback?.Invoke(requestU.downloadHandler.data);
            requestU.Dispose();
        }
    }

    private IEnumerator InternalGetCustomJsonFile(string userName, string fileName, Action<JsonData> callback)
    {
        string folderName = $"{CUSTOM_USER_DATA}/{userName}/";
        fileName = GetCorrectFilename(fileName);
        fileName = Path.GetFileName(fileName);

        while (IsConnected == null)
        {
            DisplayMessage($"Stuck on InternalGetCustomFileTexture {folderName}");
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForFixedUpdate();

        if (IsConnected == false)
        {
            Debug.LogError("Not connected to the web server check, server is down");
            callback?.Invoke(null);
            yield break;
        }

        string file = ASSET_READ_SLOW + folderName + fileName;
        file = file.ToLower();
        UnityWebRequest requestU = UnityWebRequest.Get(file);
        requestU.SetRequestHeader(AUT, BEARING);

        requestU.SendWebRequest();

        if (IsNetworkResponseValid(requestU) == true)
        {
            while (requestU.downloadProgress < 1)
            {
                yield return new WaitForEndOfFrame();
            }
            yield return new WaitForFixedUpdate();
            ConsoleExtra.Log($"GetJsonFile  {file}", null, ConsoleExtraEnum.EDebugType.NetworkAsset);
            JsonData newJsonData = new JsonData();
            newJsonData.m_jsonFileName = file;
            if (requestU.result == UnityWebRequest.Result.Success)
            {
                newJsonData.m_jsonData = requestU.downloadHandler.text;
            }
            else
            {
                newJsonData.m_jsonData = "";
            }
            requestU.Dispose();
            callback?.Invoke(newJsonData);
        }
        else
        {
            JsonData newJsonData = new JsonData();
            newJsonData.m_jsonFileName = file;
            Debug.LogError($"GetJsonFile FAIL: isHttpError:{requestU.isHttpError}  {requestU.error}  {file}");
            requestU.Dispose();
            callback?.Invoke(newJsonData);
        }
    }


    public void GetCustomFileTexture(string userName, string fileName, Action<Texture> callback) => StartCoroutine(InternalGetCustomFileTexture(userName, fileName, callback));
    private Dictionary<string, Texture> m_CachedTexture = new Dictionary<string, Texture>();
    private IEnumerator InternalGetCustomFileTexture(string userName, string fileName, Action<Texture> callback)
    {
        string folderName = $"{CUSTOM_USER_DATA}/{userName}/";
        fileName = GetCorrectFilename(fileName);
        if (m_CachedAudioClip.ContainsKey(fileName) == true)
        {
            callback?.Invoke(m_CachedTexture[fileName]);
            yield break;
        }
        while (IsConnected == null)
        {
            DisplayMessage($"Stuck on InternalGetCustomFileTexture {folderName}");
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForFixedUpdate();

        if (IsConnected == false)
        {
            Debug.LogError("Not connected to the web server check, server is down");
            callback?.Invoke(null);
            yield break;
        }

        string file = ASSET_READ_SLOW + folderName + fileName;
        file = file.ToLower();
        UnityWebRequest requestU = UnityWebRequestTexture.GetTexture(file);
        requestU.SetRequestHeader(AUT, BEARING);

        yield return requestU.SendWebRequest();

        if (IsNetworkResponseValid(requestU) == true)
        {
            while (requestU.downloadProgress < 1)
            {
                yield return new WaitForEndOfFrame();
            }
            yield return new WaitForFixedUpdate();

            ConsoleExtra.Log($"AssetBundleTexture  {file}", null, ConsoleExtraEnum.EDebugType.NetworkAsset);
            Texture texture = DownloadHandlerTexture.GetContent(requestU);
            if (m_CachedTexture.ContainsKey(fileName) == false)
            {
                m_CachedTexture.Add(fileName, texture);
            }
            callback?.Invoke(texture);
            requestU.Dispose();
        }
        else
        {
            Debug.LogError($"GetAssetBundle FAIL: isHttpError:{requestU.isHttpError}  {requestU.error}  {file}");
            callback?.Invoke(null);
            requestU.Dispose();
        }
    }

    public void GetCustomFileAudioClip(string userName, string fileName, Action<AudioClip> callback) => StartCoroutine(InternalGetCustomFileAudioClip(userName, fileName, callback));

    private Dictionary<string, AudioClip> m_CachedAudioClip = new Dictionary<string, AudioClip>();
    private IEnumerator InternalGetCustomFileAudioClip(string userName, string fileName, Action<AudioClip> callback)
    {

        string folderName = $"{CUSTOM_USER_DATA}/{userName}/";
        fileName = GetCorrectFilename(fileName);
        if (m_CachedAudioClip.ContainsKey(fileName) == true)
        {
            callback?.Invoke(m_CachedAudioClip[fileName]);
            yield break;
        }

        while (IsConnected == null)
        {
            DisplayMessage($"Stuck on InternalGetCustomFileTexture {folderName}");
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForFixedUpdate();

        if (IsConnected == false)
        {
            Debug.LogError("Not connected to the web server check, server is down");
            callback?.Invoke(null);
            yield break;
        }

        string file = ASSET_READ_SLOW + folderName + fileName;
        file = file.ToLower();

        AudioType audioType = AudioType.UNKNOWN;
        if (file.EndsWith(".wav"))
        {
            audioType = AudioType.WAV;
        }
        UnityWebRequest requestU = UnityWebRequestMultimedia.GetAudioClip(file, audioType);
        requestU.SetRequestHeader(AUT, BEARING);

        yield return requestU.SendWebRequest();

        if (IsNetworkResponseValid(requestU) == true)
        {
            while (requestU.downloadProgress < 1)
            {
                yield return new WaitForEndOfFrame();
            }
            yield return new WaitForFixedUpdate();

            ConsoleExtra.Log($"AssetBundleAudio  {file}", null, ConsoleExtraEnum.EDebugType.NetworkAsset);

            AudioClip clip = DownloadHandlerAudioClip.GetContent(requestU);
            if (m_CachedAudioClip.ContainsKey(fileName) == false)
            {
                m_CachedAudioClip.Add(fileName, clip);
            }
            callback?.Invoke(clip);
            requestU.Dispose();

        }
        else
        {
            Debug.LogError($"GetAssetBundle FAIL: isHttpError:{requestU.isHttpError}  {requestU.error}  {file}");
            callback?.Invoke(null);
            requestU.Dispose();
        }
    }



    public void GetAssetVersionNumber(CatalogueEntry entry, Action<int> callback)
    {
        var path = Catalogue.GetEntryPath(entry.Guid);
        Core.Network.LoadAssetJsonFile<CatalogueEntry>(path, false, (itemOther) =>
        {
            if (itemOther != null)
            {
                callback?.Invoke(itemOther.Version);
            }
            else
            {
                callback?.Invoke(-1);
            }
        });
    }


    private string TimeFromMilliseconds(double mil)
    {
        TimeSpan t = TimeSpan.FromMilliseconds(mil);
        return string.Format("{0:D2}:{1:D2}:{2:D2}",
               (int)t.TotalHours,
               t.Minutes,
               t.Seconds);
    }

}
