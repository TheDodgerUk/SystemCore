using AOT;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ContentSteamAudioMetaData;
#if Steam_Audio
using SteamAudio;
using Vector3 = SteamAudio.Vector3;
#endif
#if HouseBuilder
using HouseBuilder;
#endif

public class AudioSimulationSetup : MonoBehaviour
{
    public static AudioSimulationSetup s_Singleton;

    [Header("FOR DEBUG: Baking progress:")]
    [SerializeField]
    private float m_fBakingProgress = 0.0f;

    [SerializeField]
    private int m_iBakingProbe = 0;

    private AudioListener m_AudioListener;

    [Header("Assign the audio source, and settings related to it:")]
    [SerializeField]
    private AudioSource[] m_AudioSources;

    private int m_iAudioSourcesLength;

    [Header("Bake audio, or Load pre-baked data:")]
    [SerializeField]
    private bool m_bBakeAudio = true;

    [Header("Play audio from mic input, or play the assigned audio clip:")]
    [SerializeField]
    private bool m_bUseMic = false;

    [Header("If this is true, there can be no audio processing, but it currently sounds better:")]
    [SerializeField]
    private bool m_bStreamDirectFromMic = false;

    [Header("If you aren't using mic input, assign an audio clip to play:")]
    [SerializeField]
    private AudioClip m_ClipToPlay;

#if Steam_Audio
    [Header("If no material is already setup, use this:")]
    [SerializeField]
    private ContentSteamAudioMetaData.MaterialPreset m_DefaultMaterialPreset = ContentSteamAudioMetaData.MaterialPreset.Concrete;

    [Header("Assign the Steam Audio Manager")]
    [SerializeField]
    private SteamAudioManager m_SteamAudioManager;

    private PhononCore.BakeProgressCallback m_BakeCallback;

    public SteamAudioProbeBox SteamAudioProbeBoxRef { get; private set; }
#endif

    private GameObject m_ProbeGroup;

    private Coroutine m_LoadingCoroutine = null;

    private IEnumerator m_CreatingMapsCoroutine = null;

    private List<IEnumerator> m_CreatingMapsSubCoroutine = null;

    public UnityEngine.Vector3 m_ProbBoxSize = UnityEngine.Vector3.one;
    public UnityEngine.Vector3 m_ProbBoxCenter = UnityEngine.Vector3.one;
    //[SerializeField]
    //private float fPitchMod = 0.0f; // This is a test to see if we can mess with the data from the mic.


    private void Start()
    {
#if !UNITY_IOS && Steam_Audio
        s_Singleton = this;

        SteamAudioProbeBoxRef = this.GetComponentInChildren<SteamAudioProbeBox>();

        m_SteamAudioManager.SetActive(true);

        // If it's Android, ask for file system read/write permissions.
#if UNITY_ANDROID
        UnityEngine.Android.Permission.RequestUserPermission(UnityEngine.Android.Permission.ExternalStorageRead);
        UnityEngine.Android.Permission.RequestUserPermission(UnityEngine.Android.Permission.ExternalStorageWrite);
#endif
#endif
    }


    public string GetFinalPhononSceneName()
    {
#if Steam_Audio
        AssignAudioName();
        return Scene.SceneFileName(m_SteamAudioManager.LoadSaveNameOverride);
#else
        return "";
#endif
    }


    public string GetFinalProbesSceneName()
    {
#if Steam_Audio
        AssignAudioName();
        return SteamAudioProbeBoxRef.DataFileName();
#else
        return "";
#endif
    }


    public void AssignAudioName()
    {
#if Steam_Audio
        string user = "NonPreson";
        if (Core.Network.Users.CurrentUser != null)
        {
            user = Core.Network.Users.CurrentUser.m_UserName;
        }

        m_SteamAudioManager.LoadSaveNameOverride = $"{m_SteamAudioManager.StartOfFileName}_{user.ToLower()}_{m_SteamAudioManager.CurrentFileNameAppend}";
#endif
    }


    public bool LoadBakedData(Action<string> onHeader, Action<string> onBody, Action<float> onProgress, Action compleate)
    {
        if (null == m_LoadingCoroutine)
        {
            GC.Collect();

            m_LoadingCoroutine = StartCoroutine(LoadBakedDataCoroutine(onHeader, onBody, onProgress, compleate));
            return true;
        }
        else
        {
            Debug.LogError("There is already an Audio Simulation Coroutine running!", this);
            return false;
        }
    }


    /// <summary>
    /// This loads data that has been previously baked, then sets up SteamAudio.
    /// </summary>
    private IEnumerator LoadBakedDataCoroutine(Action<string> onHeader, Action<string> onBody, Action<float> onProgress, Action complete)
    {
#if Steam_Audio
        SteamAudioSource.m_RunUpdate = false;

        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        AssignAudioName();

        yield return new WaitForEndOfFrame();

        Debug.Log("Loading Data.", this);
        onHeader?.Invoke("Loading Audio Data...");

        SetupComponents();

        // Clear SteamAudio.
        ResetSteamAudio();

        Debug.Log("REMOVED FOR NOW");
        //m_SteamAudioManager.Initialize(GameEngineStateInitReason.Baking);

        //IntPtr probeBoxPtr = IntPtr.Zero;
        //IntPtr context = m_SteamAudioManager.GameEngineState().Context();
        //byte[] probeBoxData = SteamAudioProbeBoxRef.LoadData();

       
        //try
        //{
        //    PhononCore.iplLoadProbeBox(context, probeBoxData, probeBoxData.Length, ref probeBoxPtr);
        //}
        //catch (Exception e)
        //{
        //    Debug.LogError(e.Message);
        //    yield break;
        //}

        //yield return new WaitForSeconds(1.0f);

        //PhononCore.iplSaveProbeBox(probeBoxPtr, probeBoxData);

        //onProgress?.Invoke(1.0f);
        //onBody?.Invoke("Reinitialising Audio.");

        yield return StartCoroutine(ResetAudioSystem());

        onProgress?.Invoke(1.0f);

        m_LoadingCoroutine = null;
        SteamAudioSource.m_RunUpdate = true;

        var all = GameObject.FindObjectsOfType<SteamAudioSource>();
        for(int i = 0; i < all.Length; i++) 
        {
            all[i].SplTextureMapRef.LoadAudioTextureMapData(i);
        }
        complete?.Invoke();
#endif
        yield return new WaitForEndOfFrame();
    }


    public void DeleteVisualProbes()
    {
        if (null != m_ProbeGroup)
        {
            Debug.Log("delete DeleteVisualProbes");
            Destroy(m_ProbeGroup);
        }
    }


    public void CreateVisualProbes(Action<int> callback, bool generateMeshes = true)
    {
        StartCoroutine(CreateVisualProbeCoroutine(callback, generateMeshes));
    }


    private IEnumerator CreateVisualProbeCoroutine(Action<int> callback, bool generateMeshes)
    {
        DeleteVisualProbes();
        yield return new WaitForEndOfFrame();

#if Steam_Audio
        SteamAudioProbeBox[] probeBoxes = FindObjectsOfType<SteamAudioProbeBox>();
        Debug.Log($"probeBoxes   {probeBoxes.Length}");
        int ilength = probeBoxes.Length;
        List<Sphere> listSphere = new List<Sphere>();
        int count = 0;
        m_ProbeGroup = new GameObject($"Probes");

        for (int i = 0; i < ilength; i++)
        {
            Sphere[] items = probeBoxes[i].GenerateProbes();
            if (items == null || items.Length == 0)
            {
                Debug.LogError("GenerateProbes is null or empty");
                m_LoadingCoroutine = null;
                SteamAudioSource.m_RunUpdate = true;
                callback.Invoke(0);
                yield break;
            }
            else
            {
                listSphere.AddRange(items);
            }

        }

        List<UnityEngine.Vector3> positionList = new List<UnityEngine.Vector3>();

        if (true == generateMeshes)
        {
            foreach (Sphere sphere in listSphere)
            {
                UnityEngine.Vector3 pos = new UnityEngine.Vector3(sphere.centerx, sphere.centery, -sphere.centerz); // SteamAudioProbeBox.cs  OnDrawGizmosSelected  minus on the z

                GameObject sphereMesh = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                MeshRenderer meshRenderer = sphereMesh.GetComponent<MeshRenderer>();
                meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                meshRenderer.receiveShadows = false;
                sphereMesh.transform.SetScale(0.1f, 0.1f, 0.1f);
                sphereMesh.transform.position = pos;
                sphereMesh.transform.SetParent(m_ProbeGroup.transform);
                positionList.Add(new UnityEngine.Vector3(pos.x, sphere.centery, pos.z));
                count++;
            }
        }

        count = listSphere.Count;

        Debug.Log($"Probes created {count}");
        callback?.Invoke(count);
#endif
    }


    public void CreateTextureMaps(int numberPerMetre, float maxHeight, Action<string> eta, Action<float> percenatge, Action onFinish)
    {
#if HouseBuilder
        var interactions = FindObjectsOfType<InteractionGameObjectMove>().ToList();
        interactions.RemoveAll(e => e.SteamSoundRef.MetaDataRef == null);
        var heatMapGameObject = interactions.FindLast(e => e.SteamSoundRef.MetaDataRef.m_SplGameObject != null);

        if(heatMapGameObject != null)
        {
            heatMapGameObject.SteamSoundRef.MetaDataRef.m_SplGameObject.SetActive(true);
        }
#endif

        Debug.LogError("See if we can move this whole thing to a sperate script");
        StopCreateTextureMaps();
        m_CreatingMapsCoroutine = CreateTextureMapsCoroutine(numberPerMetre, maxHeight, eta, percenatge, () =>
        {
            onFinish?.Invoke();
        });
        StartCoroutine(m_CreatingMapsCoroutine);
    }

    public void StopCreateTextureMaps()
    {
        if (m_CreatingMapsSubCoroutine != null)
        {
            foreach (var item in m_CreatingMapsSubCoroutine)
            {
                StopCoroutine(item);
            }
        }
        m_CreatingMapsSubCoroutine = null;

        if (m_CreatingMapsCoroutine != null)
        {
            StopCoroutine(m_CreatingMapsCoroutine);
        }
        m_CreatingMapsCoroutine = null;
    }


#if UNITY_DEBUG

    public UnityEngine.Vector3 m_DebugMinVector = new UnityEngine.Vector3(-3f, -3f, -3f);
    public UnityEngine.Vector3 m_DebugMaxVector = new UnityEngine.Vector3(3f, 3f, 3f);

    [InspectorButton]
    private void TestCreateTextureMaps()
    {
        var sources = FindObjectsOfType<SteamAudioSource>().ToList();

        var interactions = FindObjectsOfType<Interaction>().ToList();
        var heatMapGameObject = interactions.FindLast(e => e.m_HeatMapMeshRenderer != null);

        m_CreatingMapsSubCoroutine = new List<IEnumerator>();
        m_CreatingMapsSubCoroutine.Add(sources[0].CreateTextureMaps(heatMapGameObject, 0.5f, 3f, 0f, m_DebugMinVector, m_DebugMaxVector, null, null, null));
        StartCoroutine(m_CreatingMapsSubCoroutine[0]);
    }
#endif

    private IEnumerator CreateTextureMapsCoroutine(int numberPerMetre, float maxHeight, Action<string> eta, Action<float> onPercentage, Action onFinish)
    {
#if Steam_Audio
        DeleteVisualProbes();
        yield return new WaitForEndOfFrame();


        var interactions = FindObjectsOfType<InteractionGameObjectMove>().ToList();
        interactions.RemoveAll(e => e.SteamSoundRef.MetaDataRef == null);
        Interaction heatMapGameObject =  interactions.FindLast(e => e.SteamSoundRef.MetaDataRef.m_SplGameObject != null);
        SteamAudioProbeBox[] probeBoxes = FindObjectsOfType<SteamAudioProbeBox>();

        List<Sphere> listSphere = new List<Sphere>();

        for (int i = 0; i < probeBoxes.Length; i++)
        {
            Sphere[] probes = probeBoxes[i].GenerateProbes();
            if (probes != null)
            {
                listSphere.AddRange(probes.ToList());
            }
            else
            {
                Debug.LogError("probes is null , why");
            }
            
        }

        List<UnityEngine.Vector3> positionList = new List<UnityEngine.Vector3>();

        foreach (Sphere sphere in listSphere)
        {
            UnityEngine.Vector3 pos = new UnityEngine.Vector3(sphere.centerx, sphere.centery, -sphere.centerz); // SteamAudioProbeBox.cs  OnDrawGizmosSelected  minus on the z
            positionList.Add(new UnityEngine.Vector3(pos.x, sphere.centery, pos.z));
        }

        // find min and max
        UnityEngine.Vector3 minVector = UnityEngine.Vector3.positiveInfinity;
        UnityEngine.Vector3 maxVector = UnityEngine.Vector3.negativeInfinity;
        for (int i = 0; i < positionList.Count; i++)
        {
            minVector = UnityEngine.Vector3.Min(minVector, positionList[i]);
            maxVector = UnityEngine.Vector3.Max(maxVector, positionList[i]);
        }

        var sources = FindObjectsOfType<SteamAudioSource>().ToList();


        TaskAction compleated = new TaskAction(sources.Count, () =>
        {
            SteamAudioSource.m_RunUpdate = true;
            onFinish?.Invoke();
        });

        if (sources.Count == 0)
        {
            Debug.LogError("No audio sources detected");
        }
        SteamAudioSource.m_RunUpdate = false;

        yield return new WaitForEndOfFrame();
        m_CreatingMapsSubCoroutine = new List<IEnumerator>();
        for (int index = 0; index < sources.Count; index++)
        {
            if (index == 0)
            {
                m_CreatingMapsSubCoroutine.Add(sources[index].SplTextureMapRef.CreateTextureMaps(heatMapGameObject, numberPerMetre, maxHeight, index, minVector, maxVector, (outEta) =>
                {
                    eta?.Invoke(outEta);
                }, (percentage) =>
                {
                    onPercentage?.Invoke(percentage);
                }, compleated));

                StartCoroutine(m_CreatingMapsSubCoroutine[index]);
            }
            else
            {
                m_CreatingMapsSubCoroutine.Add(sources[index].SplTextureMapRef.CreateTextureMaps(heatMapGameObject, numberPerMetre, maxHeight, index, minVector, maxVector, null, null, compleated));
                StartCoroutine(m_CreatingMapsSubCoroutine[index]);
            }
        }
#endif
        yield return new WaitForEndOfFrame();
    }

    public SplTextureMap.SplTextureData GetTextureDataFirst()
    {
#if Steam_Audio
        var sources = FindObjectsOfType<SteamAudioSource>().ToList();
        if(sources.Count > 0)
        {
            return sources[0].SplTextureMapRef.m_TextureData;
        }
        else
        {
            return null;
        }
#else
        return null;
#endif
    }

    public List<SplTextureMap.SplTextureData> GetTextureDataAll()
    {
#if Steam_Audio
        var sources = FindObjectsOfType<SteamAudioSource>().ToList();

        List<SplTextureMap.SplTextureData> all = new List<SplTextureMap.SplTextureData>();
        foreach (var item in sources)
        {
            all.Add(item.SplTextureMapRef.m_TextureData);
        }
        return all;
#else
        return null;
#endif
    }


    /// <summary>
    /// This bakes how the sound will travel around the room you just built.
    /// </summary>
    public bool CreateBakeAudio(Action<string> onHeader, Action<string> onBody, Action<float> onProgress, Action compleate)
    {
        if (null == m_LoadingCoroutine)
        {
            m_LoadingCoroutine = StartCoroutine(CalculateSoundSimulation(onHeader, onBody, onProgress, compleate));
            return true;
        }
        else
        {
            Debug.LogError("There is already an Audio Simulation Coroutine running!", this);
            return false;
        }
    }

    /// <summary>
    /// Cancels audio baking.
    /// </summary>
    public void CancelAudioBake()
    {
        if (null != m_LoadingCoroutine)
        {
            StopCoroutine(m_LoadingCoroutine);
            m_LoadingCoroutine = null;
#if Steam_Audio
            m_SteamAudioManager.phononBaker.CancelBake();
            SteamAudioSource.m_RunUpdate = true;
            // Re-apply the old baked data, if there is any.
            Core.Audio.LoadSaveGeneratedSoundRef.ConfirmUpdateGeneratedSound(null, false);
#endif
        }
    }

    private IEnumerator CalculateSoundSimulation(Action<string> onHeader, Action<string> onBody, Action<float> onProgress, Action complete)
    {
#if Steam_Audio
        SteamAudioSource.m_RunUpdate = false;
        onHeader?.Invoke("Baking Audio:");
        onProgress?.Invoke(0.0f);
        onBody?.Invoke("Setting up objects.");
        m_iBakingProbe = 0;
        yield return null;
        Debug.Log("Setting up objects.", this);

        // Clean up any existing references to Steam Audio, as it defaults to play mode and we don't want that.
        ResetSteamAudio();

        // Export the environment to SteamAudio.
        Debug.Log("Exporting mesh.", this);

        onProgress?.Invoke(0.0f);
        onBody?.Invoke("Exporting mesh.");
        yield return null;
#if UNITY_EDITOR
        Debug.LogError("Extra build times in editor as we saving the mesh as well");
        yield return new WaitForEndOfFrame();
        m_SteamAudioManager.ExportScene(true);
        yield return new WaitForSeconds(5.0f);
        m_SteamAudioManager.ExportScene(false);
        // Wait for the export to happen as it's using an external DLL.
        yield return new WaitForSeconds(5.0f); // TODO: Make this actually wait for it to finish.
#else
        m_SteamAudioManager.ExportScene(false);
        // Wait for the export to happen as it's using an external DLL.
        yield return new WaitForSeconds(1.0f); // TODO: Make this actually wait for it to finish.

#endif
        SetupComponents();

        // Tell the probe box to generate the probes. (use config or quality level to dictate probe spacing).
        Debug.Log("Generating probes.", this);
        onProgress?.Invoke(0.0f);
        onBody?.Invoke("Generating probes.");
        yield return null;
        SteamAudioProbeBox[] probeBoxes = FindObjectsOfType<SteamAudioProbeBox>();
        Debug.Log($"probeBoxes   {probeBoxes.Length}");
        int ilength = probeBoxes.Length;

        List<Sphere> count = new List<Sphere>();
        for (int i = 0; i < ilength; i++)
        {
            Sphere[] items = probeBoxes[i].GenerateProbes();
            if(items == null ||items.Length == 0)
            {
                Debug.LogError("GenerateProbes is null or empty");
                m_LoadingCoroutine = null;
                SteamAudioSource.m_RunUpdate = true;
                complete?.Invoke();
                yield break;
            }
            else
            {
                count.AddRange(items);
            }
            
        }

        // Wait for the export to happen as it's using an external DLL.
        yield return new WaitForSeconds(1.0f);

        // Bake everything.
        onBody?.Invoke("Setting items for baking.");
        SelectAllForBaking();

        onBody?.Invoke("Beginning bake.");
        yield return null;
        System.Diagnostics.Stopwatch stopwatchb = new System.Diagnostics.Stopwatch();
        stopwatchb.Start();
        BakeSound();
        stopwatchb.Stop();
        Debug.Log($"pre bake {stopwatchb.Elapsed.TotalSeconds} ");

        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();
        // Grab the baking callback so that we can display the bake progress on the UI.
        m_BakeCallback = new PhononCore.BakeProgressCallback(AdvanceProgress);
        m_SteamAudioManager.phononBaker.bakeCallback = m_BakeCallback;
        // I had to change this in SteamAudio, I needed steamAudioManager.phononBaker.bakeCallback to be public.

        // Add the cancel button to the audio baking progress screen.
        AssetSyncMenu.Instance.ShowCancel(CancelAudioBake);

        int iTotalBakes = (m_iAudioSourcesLength * ilength) + 2; // + 1 because 0, and + 1 because of baking reverb.
        // Wait for the bake, and display progress on UI.
        while (BakeStatus.InProgress == m_SteamAudioManager.phononBaker.GetBakeStatus())
        {
            yield return new WaitForEndOfFrame();

            if (m_fBakingProgress > 0.0f)
            {
                onProgress?.Invoke(Mathf.Clamp(m_fBakingProgress, 0.0f, 0.9999f));
                onBody?.Invoke((m_iBakingProbe < iTotalBakes) ? ($"Baking Audio Source: {m_iBakingProbe}/{iTotalBakes - 1}") : ("Baking Reverb"));
            }
        }

        // Turn off the cancel button again now that baking is complete.
        AssetSyncMenu.Instance.HideCancel();

        stopwatch.Stop();

        Debug.Log($"Audio Sim Bake Time: {stopwatch.Elapsed.TotalSeconds}  probes: {count.Count} per probe: {stopwatch.Elapsed.TotalSeconds / count.Count}");
        Debug.Log("AUDIO SIMULATION SETUP: BAKING COMPLETE!!!");

        onProgress?.Invoke(0.9999f);
        onBody?.Invoke("Baking Complete. Reinitialising Audio...");
        yield return StartCoroutine(ResetAudioSystem());


        Core.Audio.LoadSaveGeneratedSoundRef.CreateZipFiles(() =>
        {
            Debug.Log("Create linking json version file");
            m_LoadingCoroutine = null;
            SteamAudioSource.m_RunUpdate = true;
            complete?.Invoke();
        });
#else
        yield return new WaitForEndOfFrame();
#endif
    }


    /// <summary>
    /// Find and setup the camera and the audio sources.
    /// </summary>
    private void SetupComponents()
    {
#if Steam_Audio
        // find AudioListener every time, as the Camera.main can be different if it's non-VR or VR
        m_AudioListener = Camera.main.GetComponent<AudioListener>();

        // Setup the audio sources in the scene using the settings from the test scene. Some of these will be driven by what speaker we are using.
        m_AudioSources = FindObjectsOfType<AudioSource>();
        m_iAudioSourcesLength = m_AudioSources.Length;
        SteamAudioSource currentSource;
        int iIdentifier = 0;
        for (int i = 0; i < m_iAudioSourcesLength; i++)
        {
            // TURN OFF DOPPLER!!!
            m_AudioSources[i].dopplerLevel = 0.0f;

            // Add a steam audio source and ensure it's set to baked source.
            currentSource = m_AudioSources[i].gameObject.GetComponent<SteamAudioSource>();

            // only want items were SteamAudioSource has been placed correctly on to use SteamAudioSource
            if (currentSource != null)
            {
                currentSource.simulationType = SourceSimulationType.BakedStaticSource;

                // Setup the identifier.
                currentSource.uniqueIdentifier = currentSource.name;
                iIdentifier++;
                currentSource.bakedDataIdentifier = new BakedDataIdentifier { identifier = Common.HashForIdentifier(currentSource.name), type = BakedDataType.StaticSource };
            }
        }
#endif
    }


    /// <summary>
    /// Cleans up steam audio so that we can get it to do things in runtime that it shouldn't be doing.
    /// </summary>
    private void ResetSteamAudio()
    {
#if Steam_Audio
        while (0 < m_SteamAudioManager.ManagerData().referenceCount)
        {
            m_SteamAudioManager.Destroy();
        }
#endif
    }


    /// <summary>
    /// Select all objects ready for baking. (Mostly stolen from SteamAudio).
    /// </summary>
    private void SelectAllForBaking()
    {
#if Steam_Audio
        Debug.Log("Setting items for baking.", this);

        SteamAudioSource[] bakedSources = GameObject.FindObjectsOfType<SteamAudioSource>();
        foreach (SteamAudioSource bakedSource in bakedSources)
        {
            if (bakedSource.uniqueIdentifier.Length != 0)
            {
                bakedSource.bakeToggle = true;
            }
        }

        SteamAudioBakedStaticListenerNode[] bakedStaticNodes = GameObject.FindObjectsOfType<SteamAudioBakedStaticListenerNode>();
        foreach (SteamAudioBakedStaticListenerNode bakedStaticNode in bakedStaticNodes)
        {
            if (bakedStaticNode.uniqueIdentifier.Length != 0)
            {
                bakedStaticNode.bakeToggle = true;
            }
        }

        SteamAudioListener bakedReverb = GameObject.FindObjectOfType<SteamAudioListener>();
        if (!(bakedReverb == null))
        {
            bakedReverb.bakeToggle = true;
        }
#endif
    }


    /// <summary>
    /// Triggers baking the audio for the current scene. (Mostly stolen from SteamAudio).
    /// </summary>
    private void BakeSound()
    {
#if Steam_Audio
        Debug.Log("Beginning bake.", this);

        List<GameObject> gameObjects = new List<GameObject>();
        List<BakedDataIdentifier> identifers = new List<BakedDataIdentifier>();
        List<string> names = new List<string>();
        List<Sphere> influenceSpheres = new List<Sphere>();
        List<SteamAudioProbeBox[]> probeBoxes = new List<SteamAudioProbeBox[]>();

        SteamAudioSource[] bakedSources = GameObject.FindObjectsOfType<SteamAudioSource>();
        foreach (SteamAudioSource bakedSource in bakedSources)
        {
            if (bakedSource.uniqueIdentifier.Length != 0 && bakedSource.bakeToggle)
            {
                gameObjects.Add(bakedSource.gameObject);
                identifers.Add(bakedSource.bakedDataIdentifier);
                names.Add(bakedSource.uniqueIdentifier);

                Sphere bakeSphere;
                Vector3 sphereCenter = Common.ConvertVector(bakedSource.transform.position);
                bakeSphere.centerx = sphereCenter.x;
                bakeSphere.centery = sphereCenter.y;
                bakeSphere.centerz = sphereCenter.z;
                bakeSphere.radius = bakedSource.bakingRadius;
                influenceSpheres.Add(bakeSphere);

                // Find all probe boxes, this doesn't get populated by UI because they're added at runtime, so just grab them all.
                probeBoxes.Add(FindObjectsOfType<SteamAudioProbeBox>() as SteamAudioProbeBox[]);
            }
        }

        SteamAudioBakedStaticListenerNode[] bakedStaticNodes = GameObject.FindObjectsOfType<SteamAudioBakedStaticListenerNode>();
        foreach (SteamAudioBakedStaticListenerNode bakedStaticNode in bakedStaticNodes)
        {
            if (bakedStaticNode.uniqueIdentifier.Length != 0 && bakedStaticNode.bakeToggle)
            {
                gameObjects.Add(bakedStaticNode.gameObject);
                identifers.Add(bakedStaticNode.bakedDataIdentifier);
                names.Add(bakedStaticNode.name);

                Sphere bakeSphere;
                Vector3 sphereCenter = Common.ConvertVector(bakedStaticNode.transform.position);
                bakeSphere.centerx = sphereCenter.x;
                bakeSphere.centery = sphereCenter.y;
                bakeSphere.centerz = sphereCenter.z;
                bakeSphere.radius = bakedStaticNode.bakingRadius;
                influenceSpheres.Add(bakeSphere);

                // Find all probe boxes, this doesn't get populated by UI because they're added at runtime, so just grab them all.
                probeBoxes.Add(FindObjectsOfType<SteamAudioProbeBox>() as SteamAudioProbeBox[]);
            }
        }

        SteamAudioListener bakedReverb = GameObject.FindObjectOfType<SteamAudioListener>();
        if (!(bakedReverb == null) && bakedReverb.bakeToggle)
        {
            gameObjects.Add(bakedReverb.gameObject);
            identifers.Add(bakedReverb.Identifier);
            names.Add("reverb");
            influenceSpheres.Add(new Sphere());

            // Find all probe boxes, this doesn't get populated by UI because they're added at runtime, so just grab them all.
            probeBoxes.Add(FindObjectsOfType<SteamAudioProbeBox>() as SteamAudioProbeBox[]);
        }

        if (gameObjects.Count > 0)
        {
            SteamAudioManager phononManager = m_SteamAudioManager;
            phononManager.phononBaker.BeginBake(gameObjects.ToArray(), identifers.ToArray(), names.ToArray(),
                influenceSpheres.ToArray(), probeBoxes.ToArray());
        }
        else
        {
            Debug.LogWarning("No game object selected for baking.");
        }
#endif
    }


    /// <summary>
    /// The plan for this is to gather the input from the mic, so that we can do stuff with it before passing it to the audio source.
    /// </summary>
    private IEnumerator PlayMicSound()
    {
#if !UNITY_WEBGL // Unity WebGL hasn't got the microphone class.
        // TODO: Add mic selection.
        // Figure out the frequency of the recording device.
        Microphone.GetDeviceCaps("", out int iMin, out int iMax);
        // Loop the audio source, this is especially important for streaming direct from Microphone.
        for (int i = 0; i < m_iAudioSourcesLength; i++)
        {
            m_AudioSources[i].loop = true;
        }

        // This one works best at the moment, but we can't mess with the sound.
        if (true == m_bStreamDirectFromMic)
        {
            for (int i = 0; i < m_iAudioSourcesLength; i++)
            {
                m_AudioSources[i].clip = Microphone.Start(null, true, 1, iMax);
                m_AudioSources[i].Play();
            }
        }
        // This almost works, it's still a tiny bit choppy but other than that sounds the same as direct from mic.
        else
        {
            AudioClip micClip = Microphone.Start(null, true, 5, iMax);

            // Figure out how many samples we want as a buffer, 0.1 seconds should be enough.
            int iBufferSamples = (iMax / 10);

            // Wait for a second so that we've got the buffer, plus some extra to play.
            yield return new WaitForSeconds(1.0f);

            // Cache as much as we can outside the while loop.
            int readHead = 0;
            int writeHead;
            int newSamplesLength;
            AudioClip newClip;

            // DEBUG STUFF
            // Use this to see how consistent fixed update is.
            //DateTime PreviousFrameStart;
            //DateTime CurrentFrameStart = DateTime.Now;
            //float fPreviousAudioTime = 0.0f;

            // Running in fixed update as in theory the time between each one should be the same, should reduce stuttering.
            while (true)
            {
                // DEBUG STUFF
                //PreviousFrameStart = CurrentFrameStart;
                //CurrentFrameStart = DateTime.Now;
                //Debug.LogError($"Timestep = {(CurrentFrameStart - PreviousFrameStart).TotalMilliseconds} ms. Last Audio: {fPreviousAudioTime} ms");

                // Figure out how many samples have been recorded, and how many we need to play.
                writeHead = Microphone.GetPosition(null);
                newSamplesLength = (micClip.samples + writeHead - readHead) % micClip.samples;

                // If there's enough samples to at least fill the buffer.
                if (newSamplesLength > iBufferSamples)
                {
                    // Get the samples from the audio clip the mic is streaming into.
                    float[] data = new float[newSamplesLength]; // If these stayed the same length we could re-use this.
                    micClip.GetData(data, readHead);

                    // DEBUG STUFF
                    //fPreviousAudioTime = newSamplesLength / iMax;
                    //Debug.LogError(data.Length);

                    // Here is where we could mess with the data.
                    for (int i = 0; i < data.Length; i++)
                    {
                        data[i] = Mathf.Clamp(data[i], -1.0f, 1.0f);
                    }

                    // Pass the data to a new audio clip, and make the audio sources play it.
                    // Mic recording is always mono, so play as one channel.
                    newClip = AudioClip.Create($"Mic Output", data.Length, 1, iMax, false); //: {newSamplesLength.ToString("f2")} // DEBUG STUFF
                    newClip.SetData(data, 0);

                    for (int i = 0; i < m_iAudioSourcesLength; i++)
                    {
                        m_AudioSources[i].clip = newClip;
                        m_AudioSources[i].Play();
                    }            

                    // Figure out the starting point for reading samples for the next frame.
                    readHead = ((readHead + newSamplesLength) - iBufferSamples) % micClip.samples;

                    yield return new WaitForFixedUpdate();
                }
            }
        }
#else
        yield return new WaitForFixedUpdate();
#endif
    }


    /// <summary>
    /// Resets Steam Audio, and the camera, and the audio listeners.
    /// </summary>
    private IEnumerator ResetAudioSystem()
    {     
        yield return new WaitForSeconds(1.0f);

#if Steam_Audio
        ResetSteamAudio();

        yield return new WaitForSeconds(1.0f);

        Debug.Log("Reinitialising Audio.");

        // Reinitialise SteamAudio as playing so that it now works with the baked data.
        m_SteamAudioManager.Initialize(GameEngineStateInitReason.Playing);

        // Wait a second, then turn off the baking UI.
        // Also, turn SteamAudioManager off and on again to trigger it's OnEnable again now that there's audio sources setup.
        yield return new WaitForSeconds(1.0f);

        m_SteamAudioManager.gameObject.SetActive(false);
        m_AudioListener.enabled = false;

        for (int i = 0; i < m_iAudioSourcesLength; i++)
        {
            m_AudioSources[i].SetActive(false);
        }

        yield return new WaitForSeconds(1.0f);

        m_SteamAudioManager.gameObject.SetActive(true);
        m_AudioListener.enabled = true;

        for (int i = 0; i < m_iAudioSourcesLength; i++)
        {
            m_AudioSources[i].SetActive(true);
        }
#endif
    }

#if Steam_Audio
    /// <summary>
    /// The baking DLL calls this to report baking progress, we kinda hijack the callback.
    /// </summary>
    [MonoPInvokeCallback(typeof(PhononCore.BakeProgressCallback))]
    static void AdvanceProgress(float bakeProgressFraction)
    {
        float dif = Mathf.Abs(bakeProgressFraction - s_Singleton.m_fBakingProgress);
        if (s_Singleton.m_fBakingProgress == 0 || dif > 0.5f)
        {
            s_Singleton.m_iBakingProbe++;
        }
        s_Singleton.m_fBakingProgress = bakeProgressFraction;

    }
#endif
}
