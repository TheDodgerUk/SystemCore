using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class StreamingAssetsContents
{

    public enum ContentsEnum
    {
        InBuild,
        ServerAndStored,
        None,
    }
    // File.Exist()  does not work on adroid in StreamingAssets folder

    //https://stackoverflow.com/questions/35582074/access-unity-streamingassets-on-android
    //To solve this problem I created pre-compilation script which will run before the compilation; this script will list the names of the files you want to access later in text file
    // changed to on awake in Editor_Mode for creating the file
    // read it straight after writing, if writing ahs been done or not 


    // this looks like were Android is storing data
    //fileName:/storage/emulated/0/android/data/com.musictribe.viewer/files/bundles/android/environments/a2f884d8-4327-496d-83f8-8e71e959908c.unity3d0
    //This PC\Quest\Internal shared storage\Android\data\com.musictribe.viewer\files\bundles\android

    private List<string> m_FileNames = new List<string>();
    private const string FILE_PATH = "/StreamingAssetsContents";


    private static string TOTAL_PATH => Device.StreamingPath + "/StreamingAssetsContents";
    private static string TOTAL_PATH_LOCAL_ASSET => Device.StreamingPath + "/StreamingAssetsLocal";
    private string BundlePath => Device.StreamingPath + $"/bundles/{GlobalConsts.CurrentPlatformName()}/";


    public StreamingAssetsContents()
    {
//#if UNITY_IOS
//        Json.FullSerialiser.ReadFromFileAsync<List<string>>(TOTAL_PATH, (data) =>
//        {
//            Debug.LogError("Read data Count: " + data.Count);
//
//            m_FileNames = data;
//        });
//#else
        Json.AndroidNet.ReadFromFileAsync<List<string>>(Core.Mono, showError: false, TOTAL_PATH, JsonLibraryType.JsonNet, (data) =>
        {
            Debug.Log("StreamingAssetsContents Read data Count: " + data.Count);

            m_FileNames = data; 
        });

        Json.AndroidNet.ReadFromFileAsync<List<string>>(Core.Mono, showError: false, TOTAL_PATH_LOCAL_ASSET, JsonLibraryType.JsonNet, (data) =>
        {
            if (data != null)
            {
                Debug.Log("TOTAL_PATH_LOCAL_ASSET Read data Count: " + data.Count);

                Core.AssetsLocalRef.DecalsLocalRef.SetList(data);
                Core.AssetsLocalRef.VisualEffectLocalRef.SetList(data);
                Core.AssetsLocalRef.GameObjectLocalRef.SetList(data);
            }

        });
        //#endif
    }

    public static void WriteData()
    {
        // Do the preprocessing here
        string[] fileEntries = Directory.GetFiles(Device.StreamingPath, "*.*", SearchOption.AllDirectories);
        var list = fileEntries.ToList();
        for (int i = 0; i < list.Count; i++)
        {
            list[i] = list[i].Substring(Device.StreamingPath.Length + 1);
            list[i] = list[i].ToLower();
            list[i] = list[i].SanitiseSlashes();
        }
        Json.JsonNet.WriteToFile(list, TOTAL_PATH, true);
    }

    public static void WriteDataLocal()
    {
        var path = Device.StreamingPath;
        path = path.Replace("StreamingAssets", "");
        path = path.Remove(path.Length - 1, 1);
        var directories = Directory.GetDirectories(path,
                    "*.*",
                    SearchOption.TopDirectoryOnly).Where(f => f.Contains("LocalAssets_"))
                    .ToList();

        if (directories != null && directories.Count != 0)
        {
            string[] fileEntries = Directory.GetFiles(directories[0].SanitiseSlashes(), "*.*", SearchOption.AllDirectories);
            var list = fileEntries.ToList();
            for (int i = 0; i < list.Count; i++)
            {
                list[i] = list[i].Substring(Device.StreamingPath.Length + 1);
                list[i] = list[i].ToLower();
                list[i] = list[i].SanitiseSlashes();
            }
            Json.JsonNet.WriteToFile(list, TOTAL_PATH_LOCAL_ASSET, true);
        }
    }

    public ContentsEnum FileExist(string bundleName, string extraMessage, bool isJson)
    {
        bundleName = bundleName.SanitiseSlashes();
        if (isJson == true)
        {
            if (bundleName.EndsWith(GlobalConsts.JSON, System.StringComparison.CurrentCultureIgnoreCase) == false)
            {
                bundleName += GlobalConsts.JSON;
            }
        }

        string path = $"{bundleName}";
        if (bundleName.ToLower().SanitiseSlashes().StartsWith(Device.StreamingPath.ToLower().SanitiseSlashes()) == false)
        {
            path = $"{BundlePath}{bundleName}";
        }
        string fileNameLong = path;
        string fileNameShort = path;
        if (path.StartsWith(Device.StreamingPath, System.StringComparison.CurrentCultureIgnoreCase) == true)
        {
            fileNameShort = path.Substring(Device.StreamingPath.Length + 1);
        }


        bool returnValue = InBuild(fileNameLong, fileNameShort);
        bool returnValueLower = InBuild(fileNameLong.ToLower(), fileNameShort.ToLower());
        if(returnValue == true || returnValueLower == true)
        {
            return ContentsEnum.InBuild;
        }
        string bundleNameLong = bundleName;
        bool returnCaseSensitiveAndroid = StreamingAssetsPathCaseSensitiveExist(ref bundleName, ref bundleNameLong);
        if (returnCaseSensitiveAndroid == true)
        {
            return ContentsEnum.ServerAndStored;
        }

        return ContentsEnum.None;
    }

    private bool StreamingAssetsPathCaseSensitiveExist(ref string fileName, ref string fileNameShort)
    {
        GlobalConsts.ConvertToAndroidPath(ref fileNameShort);
        GlobalConsts.ConvertToAndroidPath(ref fileName);
        return InServerAndStored(fileName, fileNameShort);
    }

    private bool InBuild(string fileName, string fileNameShort)
    {
        return (m_FileNames.Contains(fileNameShort)|| m_FileNames.Contains(fileName));
    }

    private bool InServerAndStored(string fileName, string fileNameShort)
    {
        return File.Exists(fileNameShort) ||File.Exists(fileName);
    }
}