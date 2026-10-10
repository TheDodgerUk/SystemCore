using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class AudioManager : MonoBehaviour
{
    private Dictionary<string, GameObject> m_AudioSources;
    private List<List<AudioSource>> m_SpeakersCurves;
    private List<AudioSource> m_SpeakersSpl;
    private List<AudioSource> m_Lights;
    private Transform m_Transform;
    public AudioSimulationSetup AudioSimulationSetupRef { get; private set; }
#if HouseBuilder
    public LoadSaveGeneratedSound LoadSaveGeneratedSoundRef { get; private set; }
#endif
#if Steam_Audio
    public SteamAudio.SteamAudioManager SteamAudioManagerRef { get; private set; }
#endif

    private void Awake()
    {
        m_Transform = transform;

        AudioSimulationSetupRef = GetComponentInChildren<AudioSimulationSetup>();
#if HouseBuilder
        LoadSaveGeneratedSoundRef = new LoadSaveGeneratedSound();
#endif
#if Steam_Audio
        SteamAudioManagerRef = GetComponentInChildren<SteamAudio.SteamAudioManager>(true);
#endif

        var audioSources = GetComponentsInChildren<AudioSource>(true).ToList();
        m_AudioSources = audioSources.Extract(a => a.name, a => a.gameObject);

        foreach (var sfx in audioSources)
        {
            if (sfx.playOnAwake == false)
            {
                ToggleSfx(sfx, false);
            }
        }

        Transform trans = m_Transform.Find("SpeakersCurves");
        if (trans != null)
        {
            var speakersCurves = m_Transform.Find("SpeakersCurves").GetDirectChildren();
            m_SpeakersCurves = speakersCurves.Extract(s => s.FindComponents<AudioSource>());

            m_SpeakersSpl = m_Transform.FindComponents<AudioSource>("SpeakersSpl");
            m_Lights = m_Transform.FindComponents<AudioSource>("Lights");
        }
    }

    public void PlayDetached(AudioSource source, Transform parent, Vector3? position = null)
    {
        if (source == null)
        {
            return;
        }

        source.SetActive(true);

        // parent to manager and reposition if requested
        var transform = source.transform;
        transform.SetParent(m_Transform, true);
        transform.position = position ?? parent.position;

        // play clip
        source.Play(this, () =>
        {
            // reparent to original
            transform.SetParent(parent);
            transform.Reset(true);
            source.SetActive(false);
        });
    }

    public void OnLightsToggled(bool state) => PlaySfx(m_Lights);

    public void OnSplToggled(bool state) => PlaySfx(m_SpeakersSpl);

    public void OnCurvesChanged(int index)
    {
        PlaySfx(m_SpeakersCurves.Get(0));
    }


    public void OnEnvironmentLoaded()
    {

    }

    private void PlaySfx(List<AudioSource> soundEffects)
    {
        foreach (var sfx in soundEffects)
        {
            ToggleSfx(sfx, true);
            sfx.Play(this, () =>
            {
                if (sfx.isPlaying == false)
                {
                    ToggleSfx(sfx, false);
                }
            });
        }
    }

    private static void ToggleSfx(AudioSource sfx, bool state)
    {
#if Steam_Audio
        var steam = sfx.GetComponent<SteamAudio.SteamAudioSource>();
        if (steam != null)
        {
            steam.enabled = state;
        }
        sfx.enabled = state;
#endif
    }

    public class AudioClipData
    {
        public AudioClip m_AudioClip;
        public string m_FullName;
    }

    public void PromptAndLoadAudioFile(Action<AudioClipData> audioClipData)
    {
#if !PLATFORM_WEBGL
        string folderPath = Core.LoadFromFileFBXRef.GetSearchDirectory();
        string file = Crosstales.FB.FileBrowser.OpenSingleFile("Open single file", folderPath, new Crosstales.FB.ExtensionFilter("Items", "mp3", "wav"));
        if(string.IsNullOrEmpty(file) == false)
        {
            LoadAudioFile(file, (clip) =>
            {
                AudioClipData data = new AudioClipData();
                data.m_AudioClip = clip;
                data.m_FullName = file;
                audioClipData?.Invoke(data);
            });
        }
        else
        {
            audioClipData?.Invoke(null);
        }
#endif
    }
    public void LoadAudioFile(string filename, Action<AudioClip> audioClip)
    {
        if(File.Exists(filename) == true)
        {
            StartCoroutine(InternalLoadAudioFile(filename, audioClip));
        }
        else
        {
            audioClip?.Invoke(null);
        }
    }
    private IEnumerator InternalLoadAudioFile(string filename, Action<AudioClip> audioClip)
    {

        yield return null;
        UnityWebRequest AudioFiles = UnityWebRequestMultimedia.GetAudioClip(filename, AudioType.UNKNOWN);
        yield return AudioFiles.SendWebRequest();

        AudioClip clip = DownloadHandlerAudioClip.GetContent(AudioFiles);
        audioClip?.Invoke(clip);
    }

}
