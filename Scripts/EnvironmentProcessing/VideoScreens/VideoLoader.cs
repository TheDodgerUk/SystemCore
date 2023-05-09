using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class VideoObjectData
{
    public readonly string ScreenName;
    public readonly List<string> VideoNames;
    public readonly string RemappedScreen;   //After the intro should this screen use the video for another screen?
    public readonly bool ChangeVideoOnCompleted;
    public readonly bool MirrorImage;
    public readonly string StartVideo;
    public readonly Vector2 Resolution;
}

public class SequenceData
{
    public readonly string TargetScreen;
    public readonly string VideoName;
}

public class VideoSequence
{
    //The list of possible changes
    public List<SequenceData> Sequence = new List<SequenceData>();
}

public class VideoSequenceData
{
    //The screen that will trigger the sequence change
    public string ControllingScreen;

    //Data for a particular sequence
    public List<VideoSequence> SequenceList = new List<VideoSequence>();
}

public class VideoScreenConfiguration
{
    public Dictionary<string, VideoObjectData> m_EnvironmentVideoData;

    public VideoScreenConfiguration(Dictionary<string, VideoObjectData> data)
    {
        m_EnvironmentVideoData = data;
    }
}

public class VideoLoader : MonoSingleton<VideoLoader>
{
    private List<string> VideoExtension = new List<string>() { "*.mov", "*mp4" };

    protected string Location => GlobalConsts.LocalBundleStreamingAssetsDirectoryData;
    public string CurrentSequenceDataFolder => $"{Location}/VideoSequenceData";
    public string CurrentDataFolder => $"{Location}/VideoData";


    private List<VideoUrlData> m_UrlsByType;

    private Dictionary<string, string> m_DictOfVideoLocations = new Dictionary<string, string>();
    [SerializeField]
    private VideoScreenConfiguration m_VideoConfiguration;
    [SerializeField]
    private VideoSequenceData m_VideoSequenceData;

    private string m_currentEnvironment = "";

    public List<string> FileNames => m_UrlsByType.ConvertAll(x => x.FileName);
    public List<string> Urls => m_UrlsByType.ConvertAll(x => x.Url);

    public void Init()
    {
        m_UrlsByType = new List<VideoUrlData>();
    }

    public void LoadVideoData()
    {
        m_VideoSequenceData = null;
        if (Core.Contents.FileExist(CurrentSequenceDataFolder, "", true) == StreamingAssetsContents.ContentsEnum.InBuild)
        {
#if UNITY_ANDROID
            Json.AndroidNet.ReadFromFileAsync<VideoSequenceData>(this, showError: false, CurrentSequenceDataFolder, JsonLibraryType.FullSerializer, (data) =>
            {
                m_VideoSequenceData = data;
            });
#else
            var videoSequenceData = Json.FullSerialiser.ReadFromFile<VideoSequenceData>(CurrentSequenceDataFolder);
            m_VideoSequenceData = videoSequenceData;
#endif
        }
        else
        {
            ConsoleExtra.Log($"not found {CurrentSequenceDataFolder}", null, ConsoleExtraEnum.EDebugType.StartUp);
        }



        m_VideoConfiguration = null;

        if (Core.Contents.FileExist(CurrentDataFolder, "", true) == StreamingAssetsContents.ContentsEnum.InBuild)
        {
#if UNITY_ANDROID
            Json.AndroidNet.ReadFromFileAsync<Dictionary<string, VideoObjectData>>(this, showError: false, CurrentDataFolder, JsonLibraryType.FullSerializer, (data) =>
            {
                m_VideoConfiguration = new VideoScreenConfiguration(data);
            });
#else
            var videoObjectData = Json.JsonNet.ReadFromFile<Dictionary<string, VideoObjectData>>(CurrentDataFolder);
            if (null != videoObjectData)
            {
                m_VideoConfiguration = new VideoScreenConfiguration(videoObjectData);
            }
#endif



        }
        else
        {
            ConsoleExtra.Log($"not found {CurrentDataFolder}", null, ConsoleExtraEnum.EDebugType.StartUp);
        }

        OnVideoSequenceLoaded(null);
    }

    public VideoObjectData GetScreenData(string screenName)
    {
        if (null != m_VideoConfiguration)
        {
            return m_VideoConfiguration.m_EnvironmentVideoData.Get(screenName);
        }

        return null;
    }

    public VideoSequenceData GetVideoSequenceData(string screenName)
    {
        if (m_VideoSequenceData != null && screenName == m_VideoSequenceData.ControllingScreen)
        {
            return m_VideoSequenceData;
        }

        return null;
    }

    public string GetRandomVideo(string type) => m_UrlsByType.GetRandom()?.Url;

    public string GetVideo(string name)
    {
        if (true == m_DictOfVideoLocations.ContainsKey(name))
        {
            return m_DictOfVideoLocations[name];
        }
        Debug.LogWarning($"Could not find video: {name}, in folder: {CurrentDataFolder}");
        return string.Empty;
    }

    public void OnVideoSequenceLoaded(Action callback)
    {
        m_DictOfVideoLocations = new Dictionary<string, string>();
        m_UrlsByType = new List<VideoUrlData>();

        List<string> videos = DirSearch(GlobalConsts.LocalBundleStreamingAssetsDirectoryVideo, VideoExtension);

        for (int i = 0; i < videos.Count; i++)
        {
            string file = Path.GetFileNameWithoutExtension(videos[i]);
            string URL = videos[i];
            var videoData = new VideoUrlData()
            {
                FileName = file,
                Url = URL,
            };

            if (m_DictOfVideoLocations.ContainsKey(file) == false)
            {
                m_DictOfVideoLocations.Add(file, URL);
            }
            else
            {
                Debug.LogError($"OnVideoSequenceLoaded is tying to add same again : {file}");
            }

            m_UrlsByType.Add(videoData);
        }

        callback?.Invoke();
    }

    private List<String> DirSearch(string sDir, List<string> searchPtn)
    {
        List<String> files = new List<String>();
        try
        {
            foreach (string ptn in searchPtn)
            {
                foreach (string f in Directory.GetFiles(sDir, ptn))
                {
                    files.Add(f);
                }
                foreach (string d in Directory.GetDirectories(sDir))
                {
                    files.AddRange(DirSearch(d, searchPtn));
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning($"VideoLoader Message: {e.Message}");
        }

        return files;
    }

    public class VideoUrlData
    {
        public string Url;
        public string FileName;
    }
}
