using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;

// this is to find were we fix the shaders
// FIX_SHADERS


[Serializable]
public class GlobalConsts
{
    public static readonly List<Color> GLOVE_COLOURS = new List<Color>
    {
        Color.red,
        Color.green,
        Color.blue,
        Color.yellow,
    };

    public const string CollisionObj = "PlacementCollision";
    public const string ColliderName = "GeneratedCollider";
    public const string RootModel = "RootModel";

    public static string RootCatalogueItemTag = "RootCatalogueItem";

    public const string CenterOfMass = "CenterOfMass_RigidBody";

    public const int MAX_SEQUENTAILACTION_SIZE = 50;
    public const int MAX_OUTLINE_RENDERERS = 50;
    public const float LOADINGBAR_PERCENTAGE = 0.5f;

    // do a search for this 
    public const float AVATAR_SAFETY_SYNC_TIMER = 3f;

    public const int FINGER_POKE_FRAMES = 5;

    public static readonly Quaternion LASER_LINE_OFFSET_ROTATION = Quaternion.Euler(0f, 180f, 0f);

    public const int NUMBER_OF_GENERICS = 7;
    public const string JSON = ".json";
    // Calalogue program and main program need to match, closley 
    public const string OLD_DATA = "Data";
    public const string VIDEOCLIP_STREAMING = "VideoClipStreamingAsset";

#if !CATALOG_PROGRAM
    public static string LocalBundleStreamingAssetsDirectory => $"{GlobalConsts.BUNDLE_STREAMING}/{Core.Environment.CurrentEnvironment.OverloadEnvironmentName()}/{Core.Environment.CurrentEnvironment.OverloadVariantName()}";
    public static string LocalBundleStreamingAssetsDirectoryData => $"{GlobalConsts.LocalBundleStreamingAssetsDirectory}/{GlobalConsts.OLD_DATA}";

    public static string LocalBundleStreamingAssetsDirectoryNonOverload => $"{GlobalConsts.BUNDLE_STREAMING}/{Core.Environment.CurrentEnvironment.NonOverloadEnvironmentName()}/{Core.Environment.CurrentEnvironment.NonOverloadVariantName()}";
    public static string LocalBundleStreamingAssetsDirectoryDataNonOverload => $"{GlobalConsts.LocalBundleStreamingAssetsDirectoryNonOverload}/{GlobalConsts.OLD_DATA}";

    public static string LocalBundleStreamingAssetsDirectoryVideo => $"{GlobalConsts.LocalBundleStreamingAssetsDirectory}/{GlobalConsts.VIDEOCLIP_STREAMING}";
#endif
    public static string JSON_SAVEABLE_SCENE_FILE_BASE => $"{Device.StreamingPath}/SaveableScene/";

    public const string STARTING_ENVIRONMENT = "LoadingMenu";
    public static string BUNDLE_STREAMING_SINGLE => $"BundleStreamingAssets";

    public static string BUNDLE_STREAMING => $"{Device.StreamingPath}/{BUNDLE_STREAMING_SINGLE}";
    public static string BUNDLE_STREAMING_LANGUAGES => $"{BUNDLE_STREAMING}/LanguageData/";
    public static string BUNDLE_STREAMING_BRANDS => $"{BUNDLE_STREAMING}/BrandData/";
    public static string BUNDLE_STREAMING_ENVIRONMENT_DATA => $"{BUNDLE_STREAMING}/EnvironmentData";

    public static string BUNDLE_STREAMING_STRUCTURE_FILE => $"{BUNDLE_STREAMING_ENVIRONMENT_DATA}/Structure";


    public const string GENERIC_MATERIAL = "GenericMaterial_";

    //#if def VR_INTERACTION  // wrapped around items to only load VR_INTERACTION, good for non interactions like builder

    public static readonly List<string> ExternalSourceAssetBundles = new List<string>()
    {
            { "cataloguethumbnail" },
            { "logos" },
            { "hologram" },
            { "weightdistribution" },
            { "cataloguethumbnailgroup" },
    };

    public static List<string> GetExternalSourceAssetBundlesInMask(int mask)
    {
        //https://answers.unity.com/questions/319940/maskfield-selected-values.html
        List<string> newList = new List<string>();
        for (int i = 0; i < ExternalSourceAssetBundles.Count; i++)
        {
            int layer = 1 << i;
            if ((mask & layer) != 0)
            {
                newList.Add(ExternalSourceAssetBundles[i]);
            }
        }
        return newList;
    }

    
    public enum ControllerType
    {
        LaserPointer,
        PhysicsHand,
    }


    public enum InteractionType
    {
        Physics,
        ControllerVR,
        Placement,
    }

    public enum ExtraFromServer
    {
        AudioClipAssetBundle,
        GameObjectAssetBundle,
        MaterialAssetBundle,
        Texture2DAssetBundle,
        VideoClipAssetBundle,
        TextAssetBundle,
        CatalogueThumbnail,
        CatalogueThumbnailGroup
    }

    public static List<ExtraFromServer> GetExtraFromServerInMask(int mask)
    {
        //https://answers.unity.com/questions/319940/maskfield-selected-values.html
        List<ExtraFromServer> newList = new List<ExtraFromServer>();
        for (int i = 0; i < ExternalSourceAssetBundles.Count; i++)
        {
            int layer = 1 << i;
            if ((mask & layer) != 0)
            {
                newList.Add((ExtraFromServer)i);
            }
        }
        return newList;
    }

    public static readonly Dictionary<string, string[]> ExtraAssetBundles = new Dictionary<string, string[]>()
        {
            {ExtraFromServer.AudioClipAssetBundle.ToString(), new string[] { ".wav", ".mp3" } },
            {ExtraFromServer.MaterialAssetBundle.ToString(), new string[] { ".mat"} },
            {ExtraFromServer.Texture2DAssetBundle.ToString(), new string[] { ".png", ".jpg" } },
            {ExtraFromServer.GameObjectAssetBundle.ToString(), new string[] { ".prefab" } },
        };

    public static readonly Dictionary<string, string[]> ExtraAssetBundlesNonCompressed = new Dictionary<string, string[]>()
        {
            {ExtraFromServer.VideoClipAssetBundle.ToString(), new string[] { ".mp4", ".mov" } },
        };

    public static readonly List<string> ExtraStreamingAssets = new List<string>()
        {
            {"Data" },
            {"VideoClipStreamingAsset"},
        };

    public static readonly Dictionary<string, string[]> ExtraAssetBundlesSubDirectorys = new Dictionary<string, string[]>()
        {
            {ExtraFromServer.TextAssetBundle.ToString(), new string[] { ".json" } },
        };


    public static bool IsExternalSourceAsset(string name, bool withSlash = false)
    {
        foreach (var item in ExternalSourceAssetBundles)
        {
            string newItem = item + ".unity3d";
            newItem = newItem.ToLower();

            if (withSlash == true)
            {
                newItem = "/" + newItem;
            }
            if (name.EndsWith(newItem) == true)
            {
                return true;
            }
        }
        return false;
    }


    public static bool IsGenericAsset(string name)
    {
        foreach (var item in ExtraAssetBundles)
        {
            string newItem = item.Key + ".unity3d";
            newItem = newItem.ToLower();
            if (name.EndsWith(newItem) == true)
            {
                return true;
            }
        }

        foreach (var item in ExtraAssetBundlesSubDirectorys)
        {
            string newItem = item.Key + ".unity3d";
            newItem = newItem.ToLower();
            if (name.EndsWith(newItem) == true)
            {
                return true;
            }
        }
        
        return false;
    }

    public static string GetChecksumFromFile(string file)
    {
        using (FileStream stream = File.OpenRead(file))
        {
            System.Security.Cryptography.SHA256Managed sha = new SHA256Managed();
            byte[] checksum = sha.ComputeHash(stream);
            return System.BitConverter.ToString(checksum).Replace("-", String.Empty);
        }
    }

    public static string GetChecksumFromString(string stringData)
    {
        System.Security.Cryptography.SHA256Managed sha = new SHA256Managed();
        byte[] checksum = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(stringData));
        return System.BitConverter.ToString(checksum).Replace("-", String.Empty);
    }

    public static string GetChecksum(byte[] bytes)
    {
        SHA256Managed sha = new SHA256Managed();
        byte[] checksum = sha.ComputeHash(bytes);
        return System.BitConverter.ToString(checksum).Replace("-", String.Empty);
    }

    private static readonly Dictionary<RuntimePlatform, string> platformNames = new Dictionary<RuntimePlatform, string>()
        {
            { RuntimePlatform.WindowsPlayer, "Windows" },
            { RuntimePlatform.Android, "Android" },
            { RuntimePlatform.OSXPlayer, "OSX" },
            { RuntimePlatform.IPhonePlayer, "iOS" },
            { RuntimePlatform.WebGLPlayer, "WebGL" }
        };

    public static int PlatformNamesLength => platformNames.Count;

    public static string GetPlatformName(RuntimePlatform platform)
    {
        return platformNames[platform];
    }

    public static string CurrentPlatformName()
    {
#if UNITY_ANDROID
        return platformNames[RuntimePlatform.Android];
#elif UNITY_STANDALONE_OSX
        return platformNames[RuntimePlatform.OSXPlayer];
#elif UNITY_IOS
        return platformNames[RuntimePlatform.IPhonePlayer];
#elif UNITY_WEBGL
        return platformNames[RuntimePlatform.WebGLPlayer];
#else
        return platformNames[RuntimePlatform.WindowsPlayer];
#endif
    }

    public static int CurrentPlatformIndex()
    {
#if UNITY_ANDROID
        return 1;
#elif UNITY_STANDALONE_OSX
        return 2;
#elif UNITY_IOS
        return 3;
#elif UNITY_WEBGL
        return 4;
#else
        return 0;
#endif
    }


    //Application.streamingAssetsPath jar:file:///data/app/com.musictribe.roomviewer-okK9RuB6p7T0P4NiUXAZpw==/base.apk!/assets

    //Application.persistentDataPath /storage/emulated/0/Android/data/com.musictribe.roomviewer/files

    //var persistentDataPath = CreatePersistentDataPath(bundleName);
    //string fileName = Core.Contents.FileExist(persistentDataPath, "ServerFile", false);
    // once item is stored on system its /storage/emulated/0/Android/data/com.musictribe.roomviewer/files/streamingassets/bundles/android/catalogue/prefabs/klarkteknik/klarkteknik_dn9630.unity3d

    private const string ANDROID_START = "jar:file:///";
    private const string IOS_START = "file://";
    public static PathType ConvertToAndroidPath(ref string path)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        bool pathChanged = false;
        if (path.Contains(Application.streamingAssetsPath.ToLower()) == true || path.Contains(Application.streamingAssetsPath) == true)
        {
            path = path.Replace(Application.streamingAssetsPath.ToLower(), Application.streamingAssetsPath);
            pathChanged = true;
            if (path.StartsWith(ANDROID_START) == false)
            {
                if (path[0] == '/')
                {
                    // this is not get -> jar:file:////storage/emulated/0/Android/data/com.musictribe.Roomviewer/files/streamingassets/bundles/android/catalogue/prefabs/nonproducts/lights/wall_lightpoint.json
                    // ie    jar:file:////  instead of jar:file:///
                    path = path.Remove(0, 1);
                }
                path = ANDROID_START + path;
            }
            return PathType.StreamingAssetsPath;
        }



        if (path.Contains(Application.persistentDataPath.ToLower()) == true || path.Contains(Application.persistentDataPath) == true)
        {
            path = path.Replace(Application.persistentDataPath.ToLower(), Application.persistentDataPath);
            pathChanged = true;
            return PathType.PersistentDataPath;
        }

        if (pathChanged == false)
        {
            path = path.ToLower();
            path = path.SanitiseSlashes();
        }
        return PathType.PersistentDataPath;
#endif

#if UNITY_IOS && !UNITY_EDITOR
        path = path.SanitiseSlashes();
        
        bool pathChanged = false;
        if (path.Contains(Application.streamingAssetsPath.ToLower()) == true || path.Contains(Application.streamingAssetsPath) == true)
        {
            path = path.Replace(Application.streamingAssetsPath.ToLower(), Application.streamingAssetsPath);
            pathChanged = true;
            if (path.StartsWith(IOS_START) == false)
            {
                if (path[0] == '/')
                {
                    // this is not get -> jar:file:////storage/emulated/0/Android/data/com.musictribe.Roomviewer/files/streamingassets/bundles/android/catalogue/prefabs/nonproducts/lights/wall_lightpoint.json
                    // ie    jar:file:////  instead of jar:file:///
                    path = path.Remove(0, 1);
                }
                path = IOS_START + path;
            }
            return PathType.StreamingAssetsPath;
        }


            if (path.StartsWith(IOS_START) == false)
            {
                if (path[0] == '/')
                {
                    // this is not get -> jar:file:////storage/emulated/0/Android/data/com.musictribe.Roomviewer/files/streamingassets/bundles/android/catalogue/prefabs/nonproducts/lights/wall_lightpoint.json
                    // ie    jar:file:////  instead of jar:file:///
                    path = path.Remove(0, 1);
                }
                path = IOS_START + path;
            }



        path = path.SanitiseSlashes();
        return PathType.PersistentDataPath;
#endif

#if UNITY_WEBGL
        return PathType.StreamingAssetsPath;
#endif

        path = path.ToLower();
        path = path.SanitiseSlashes();
        return PathType.PersistentDataPath;

    }

    public enum PathType
    {
        StreamingAssetsPath,
        PersistentDataPath,
    }

    [Serializable]
    public class AssetBundleVersion
    {
        [SerializeField]
        public int Version = 0;
        [SerializeField]
        public float FileSizeMB;
    }

    [System.Serializable]
    public class VFXReplaceInScene
    {
        [SerializeField]
        public string VfxName;
        [SerializeField]
        public string HierarchyPath;
        [SerializeField]
        public string HierarchyPathRoot;
        [SerializeField]
        public string Scene;
    }

    [System.Serializable]
    public class CatalogueReplaceInScene
    {
        [SerializeField]
        public string RealGUID;
        [SerializeField]
        public string Filepath;
        [SerializeField]
        public string HierarchyPath;
        [SerializeField]
        public string HierarchyPathRoot;
        [SerializeField]
        public string CatalogueGUID;
        [SerializeField]
        public string Scene;

        [SerializeField]
        public Vector3 Position = Vector3.zero;
        [SerializeField]
        public Quaternion Rotation = Quaternion.identity;
    }

    public class CatalogueData
    {
        public string GUID;
        public string ReferenceNumber;
    }

    public class GuidSafety
    {
        public string Name;
        public int GUID;
    }

    public static List<GuidSafety> m_VrInteractionGuidSafety = new List<GuidSafety>();
    public static List<GuidSafety> m_PhotonGuidSafety = new List<GuidSafety>();
}
