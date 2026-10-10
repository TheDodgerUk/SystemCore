using System.Collections;
using UnityEngine;
using System;
#if Photon
using Photon.Pun;
#endif
#if UNITY_EDITOR
using UnityEditor;

public class AwakeExample : EditorWindow
{
    public void Awake()
    {

        BuildTargetGroup m_BuildTargetGroup = BuildTargetGroup.Standalone;
        var platform = BuildTarget.StandaloneWindows64;
        if (Application.streamingAssetsPath.CaseInsensitiveContains("Android") == true)
        {
            platform = BuildTarget.Android;
            m_BuildTargetGroup = BuildTargetGroup.Android;
        }
        var activeBuildTarget = EditorUserBuildSettings.activeBuildTarget;
        if (activeBuildTarget != platform)
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(m_BuildTargetGroup, platform);
        }
    }
}
#endif

public class MonoCoroutine : MonoBehaviour { }


public class Core : MonoBehaviour
{
    public static Action<Vector2> OnScreenSizeChange;
    private Vector2 m_LastScreenSize;

    public static readonly GateAction SceneLoaded = new GateAction();
    public static AssetLoader Assets { get; private set; }

    public static EnvironmentLoader SceneLoader { get; private set; }
    public static EnvironmentManager Environment { get; private set; }
    public static SaveableScene Scene { get; private set; }
    public static Catalogue Catalogue { get; private set; }
    public static AudioManager Audio { get; private set; }
    public static MonoCoroutine Mono { get; private set; }
    public static Networking Network { get; private set; }
#if VR_INTERACTION
    public static KeyboardManager Keyboard { get; private set; }
#endif
    public static StreamingAssetsContents Contents { get; private set; }
    public static LoadFromFileFBX LoadFromFileFBXRef { get; private set; }
    public static GenericMessage GenericMessageRef { get; private set; }
    public static GenericLoading GenericLoadingRef { get; private set; }

    public static VisualLogger VisualLoggerRef { get; private set; }

    public static AssetBundles AssetBundlesRef { get; private set; }
    public static AssetsLocal AssetsLocalRef { get; private set; }
#if Photon
    public static PhotonGeneric PhotonGenericRef { get; private set; }
    public static PhotonMultiplayer PhotonMultiplayerRef { get; private set; }
#endif


    [SerializeField, ReadOnly]
    private CoreProgress m_SetupProgress = (CoreProgress)(-1);
    [SerializeField]
    private bool m_SetupLogging = false;
    [SerializeField]
    private bool m_EnableVr = true;
    [SerializeField]
    private string m_EnvironmentVariantName = GlobalConsts.STARTING_ENVIRONMENT;

    private bool m_VideosEnabled = true;

    private enum CoreProgress
    {
        StartingSetup,
        CreatingCameraInputAndLocalisation,
        CreatingEnvironmentManager,
        CreatingArbiter,
        LoadingAssetDatabase,
        LoadingLanguages,
        EnablingVR,
        InitialisingInputManager,
        LoadingConstructScene,
        LoadingCatalogue,
        LoadingEnvironmentData,
        LoadingEnvironmentChunks,
        LoadingEnvironment,
        LoadingScene,
        Complete,
    }

    private void Awake()
    {
#if UNITY_EDITOR
        StreamingAssetsContents.WriteData();
        StreamingAssetsContents.WriteDataLocal();
#endif
#if !UNITY_WEBGL // Unity WebGL hasn't got the Caching class.
        Caching.ClearCache();
#endif
        Device.ResetVrType();
        Application.backgroundLoadingPriority = ThreadPriority.Low;
        Application.runInBackground = true;

        Mono = this.gameObject.ForceComponent<MonoCoroutine>();
        AssetsLocalRef = new AssetsLocal();
        Contents = new StreamingAssetsContents();
        Network = new Networking(this.gameObject.ForceComponent<NetworkingMono>());
        Scene = GetComponentInChildren<SaveableScene>();
        Audio = GetComponentInChildren<AudioManager>();
        LoadFromFileFBXRef = new LoadFromFileFBX(GetComponentInChildren<LoadFromFileFBXMono>());
        GenericMessageRef = new GenericMessage(GetComponentInChildren<GenericMessageMono>());
        GenericLoadingRef = new GenericLoading(GetComponentInChildren<GenericLoadingMono>());
        VisualLoggerRef = new VisualLogger(GetComponentInChildren<VisualLoggerMono>(true));
        VisualLoggerRef.Show(false);
        AssetBundlesRef = new AssetBundles();
        

#if Photon
        if (PhotonDMX.Instance == null)
        {
            var dmx = GetComponentInChildren<PhotonDMXMono>().gameObject;
            PhotonDMX.CreateInstance();
            PhotonDMX.Instance.Initilise(dmx);
        }

        PhotonGenericRef = new PhotonGeneric(GetComponentInChildren<PhotonGenericMono>());
        PhotonMultiplayerRef = new PhotonMultiplayer(this.transform.gameObject.ForceComponent<PhotonMultiplayerMono>());
#endif

#if UNITY_ANDROID
        var camera = GetComponentInChildren<Camera>();
        if (camera != null)
        {
            camera.SetActive(false);
        }

#endif

#if VR_INTERACTION
        Keyboard = new KeyboardManager();
        if(LayerMask.NameToLayer(Autohand.Hand.grabbableLayerNameDefault) != Layers.VrInteractionLayer)
        {
            for (int i = 0; i < 10; i++)
            {
                Debug.LogError("Autohand.Hand.grabbableLayerNameDefault needs to be set to VrInteraction other wise you not pick up objects correctly");
            }
        }

        if (LayerMask.NameToLayer(Autohand.Hand.rightHandLayerName) != Layers.ControllerRightLayer)
        {
            for (int i = 0; i < 10; i++)
            {
                Debug.LogError("Autohand.Hand.rightHandLayerName needs to be set to ControllerRightLayer other wise you not pick up objects correctly");
            }
        }

        if (LayerMask.NameToLayer(Autohand.Hand.leftHandLayerName) != Layers.ControllerLeftLayer)
        {
            for (int i = 0; i < 10; i++)
            {
                Debug.LogError("Autohand.Hand.leftHandLayerName needs to be set to ControllerLeftLayer other wise you not pick up objects correctly");
            }
        }

#if VR_INTERACTION_AVATAR
        //if get error here, them OvrAvatarCustomHandPose has been overwritten , fix it 
        int test = Oculus.Avatar2.OvrAvatarCustomHandPose.JOINT_TRANSFORMS;

        // if this breaks, then it means you need to fix the file thats been overwritten
        var fngerTest = Autohand.HandPoseData.VALID_FINGERS;

        // if this breaks, then it means you need to fix the file thats been overwritten
        var otherTest = Oculus.Avatar2.OvrAvatarCustomHandPose.LEFT_SCALE_MAIN;
#endif


#endif
        m_LastScreenSize = new Vector2(Screen.width, Screen.height);

#if UNITY_STANDALONE
        if (Screen.fullScreenMode == FullScreenMode.Windowed || Screen.fullScreenMode == FullScreenMode.MaximizedWindow)
        {
            if(m_LastScreenSize.x < 1920)
            {
                Screen.SetResolution(1920, 1080, false);
            }
        }
#endif

#if VR_INTERACTION
        if (Application.isEditor == false)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
#endif

    }

    private void Start()
    {
#if UNITY_ANDROID
        if (UnityEngine.Android.Permission.HasUserAuthorizedPermission(UnityEngine.Android.Permission.FineLocation) == false)
        {
            UnityEngine.Android.Permission.RequestUserPermission(UnityEngine.Android.Permission.FineLocation);
        }


        if (UnityEngine.Android.Permission.HasUserAuthorizedPermission(UnityEngine.Android.Permission.ExternalStorageRead) == false)
        {
            UnityEngine.Android.Permission.RequestUserPermission(UnityEngine.Android.Permission.ExternalStorageRead);
        }

        if (UnityEngine.Android.Permission.HasUserAuthorizedPermission(UnityEngine.Android.Permission.ExternalStorageWrite) == false)
        {
            UnityEngine.Android.Permission.RequestUserPermission(UnityEngine.Android.Permission.ExternalStorageWrite);
        }
#endif

#if UNITY_IOS
      IOSNetworkPermission.TriggerDialog();
#endif

        Progress(CoreProgress.StartingSetup);

        // warm up guid generator & json library
        Utils.Code.GenerateGuid();
        Json.ResetLibrary();

        StartCoroutine(StartupProcedure());
    }

    [InspectorButton]
    private void DisplayNumberOfObjects()
    {
        Debug.LogError($"DisplayNumberOfObjectsDisplayNumberOfObjects   {GameObject.FindObjectsOfType(typeof(MonoBehaviour)).Length}");
    }

    [InspectorButton]
    private void DisplayNumberOfRenderers()
    {
        Debug.LogError($"DisplayNumberOfObjectsDisplayNumberOfObjects   {GameObject.FindObjectsOfType(typeof(Renderer)).Length}");
    }


    private void Update()
    {
        // Space [Core]: reset user
        // F     [FPSCounter]: shows fps
        // F4    [SaveableScene]: save scene
        // F5    [SaveableScene]: load scene
        // F7    [InputManager]: unlocks controllers
        // F8    [VideoObjectManager]: disables videos
        // F9    [Screenshot]: capture screenshot

        if (Input.GetKeyDown(KeyCode.BackQuote) && SceneLoader.AllowMenu == true)
        {
            SceneLoader.ToggleMenu();
        }


        if (Input.GetKeyDown(KeyCode.F8) == true)
        {
            m_VideosEnabled = !m_VideosEnabled;
            foreach (var video in Environment.GetFx<VideoObjectManager>())
            {
                video.GetComponent<UnityEngine.Video.VideoPlayer>().enabled = m_VideosEnabled;
            }
        }

        Vector2 currentScreenSize = new Vector2(Screen.width, Screen.height);
        if (m_LastScreenSize != currentScreenSize)
        {
            m_LastScreenSize = currentScreenSize;
            OnScreenSizeChange?.Invoke(m_LastScreenSize);
        }
    }

    private IEnumerator StartupProcedure()
    {
        // create camera, input + localisation
        Progress(CoreProgress.CreatingCameraInputAndLocalisation);
        CameraControllerVR.CreateInstance();
        InputManagerVR.CreateInstance();
        Localisation.CreateInstance();
#if !AR_INTERACTION
        VideoLoader.CreateInstance();
        VideoLoader.Instance.Init();
#endif
        SceneDataManager.CreateInstance();
        Brand.CreateInstance();

        Catalogue = transform.CreateChild<Catalogue>();

        yield return null;

        // setup environment manager
        Progress(CoreProgress.CreatingEnvironmentManager);
        Environment = new EnvironmentManager();

        CachedObjects.CreateInstance();

        Layers.Initialise();
        PhysicMaterials.Initialise();

        yield return null;

        Scene.Initialise();

        SceneLoader = GetComponentInChildren<EnvironmentLoader>();
        SceneLoader.OnSceneLoaded += OnEnvironmentLoaded;


        // create arbiter
        Progress(CoreProgress.CreatingArbiter);
        transform.CreateChild<Arbiter>();
        yield return null;

        // load asset database
        Progress(CoreProgress.LoadingAssetDatabase);
        Assets = new AssetLoader();
        Assets.Initialise(this, () =>
        {
            // load video data
            LoadLanguages();
        });

        //Commented out as prop not need this any more 
        //var apply = this.gameObject.ForceComponent<ReApplyShaders>();
        //apply.Initialise(this);

    }

    private void LoadLanguages()
    {
        Progress(CoreProgress.LoadingLanguages);
        Localisation.Instance.LoadLanguage(this, Localisation.Language.English, () =>
        {
            this.WaitFor(0.5f, () =>
            {
                if (m_EnableVr == true)
                {
                    Progress(CoreProgress.EnablingVR);
                    Device.EnableVr(this, InitialiseInputSystems);
                }
                else
                {
                    InitialiseInputSystems();
                }
            });
        });
    }

    private void InitialiseInputSystems()
    {
        Progress(CoreProgress.InitialisingInputManager);
        Debug.Log("InitialiseInputSystems");
        InputManagerVR.Instance.Initialise(InitialiseCamera);
    }

    private void InitialiseCamera()
    {
        CameraControllerVR.Instance.Initialise();
        //CameraController.Instance.ToggleUiCam(false);
        InputManagerVR.Instance.ToggleInput(false);

        Progress(CoreProgress.LoadingConstructScene);
        Environment.LoadConstructScene(() =>
        {
            Environment.LoadingScreen.PlaceInfrontMainCamera();
            Environment.LoadingScreen.InitialiseLoadingScene("", true);
            Progress(CoreProgress.LoadingCatalogue);
            LoadEnvironment();
        });

    }

    private void LoadEnvironment()
    {
        Core.SceneLoader.SetInputSystemToVR(false);

        Progress(CoreProgress.LoadingEnvironmentData);
        Environment.LoadEnvironmentData(() =>
        {
            SceneLoader.Init(Environment.Environments);
            Progress(CoreProgress.LoadingEnvironment);
            ConsoleExtra.Log("ToDo Change from index to name, file is system_settings,   m_StartingEnvironmentIndex", null, ConsoleExtraEnum.EDebugType.Todo);
            EnvironmentData environment = null;

            if (Environment.Environments.Count == 1)
            {
                environment = Environment.Environments.Get(0);
                Debug.Log($"VariantName {environment.VariantName}   EnvironmentName {environment.EnvironmentName}");
            }
            else
            {
                environment = Environment.Environments.FindLast(e => e.VariantName == m_EnvironmentVariantName);
            }

            if (environment != null)
            {
                Environment.LoadingScreen.PlaceInfrontMainCamera();

                Environment.LoadingScreen.InitialiseLoadingScene(environment.EnvironmentName + "_" + environment.VariantName, true);
                Progress(CoreProgress.LoadingCatalogue);
                Environment.LoadEnvironment(environment, OnEnvironmentLoaded, p =>
                {
                    // use 40% of bar plus 40%
                    Environment.LoadingScreen.SetProgress(p * 0.4f + 0.4f);
                }, () => Progress(CoreProgress.LoadingEnvironmentChunks));
            }
            else
            {
                ConsoleExtra.Log($"Could not find VariantName in : {m_EnvironmentVariantName}", null, ConsoleExtraEnum.EDebugType.StartUp);
                OnEnvironmentLoaded();
            }
        }, p =>
        {
            // use 10% of bar + 30%
            // we not care about the menu screen at the beginning 
           ////// Environment.LoadingScreen.SetProgress(p * 0.1f + 0.3f);
        });
    }

    private void OnEnvironmentLoaded()
    {
        Debug.Log("Environment loaded\n");

        Audio.OnEnvironmentLoaded();

        Progress(CoreProgress.LoadingScene);

        Scene.LoadDefaultScene(() =>
        {
#if VR_INTERACTION
            CameraControllerVR.Instance.ToggleBlink(true);
#endif
            // CameraController.Instance.ToggleUiCam(true);

            OnSceneLoaded();
        }, p =>
        {
            // use 20% of bar + 80%
            Environment.LoadingScreen.SetProgress(p * 0.2f + 0.8f);
        });
    }

    private void OnSceneLoaded()
    {

        this.WaitForFrame(() =>
        {
#if !PLATFORM_WEBGL
            InputManagerVR.Instance.ToggleInput(true);
#endif
            
            Progress(CoreProgress.Complete);
            if (Environment.IsCurrentEnvironmentValid == true)
            {
                Environment.UnloadLoadingScene(null);
            }
            SceneLoaded.Invoke();

        });
    }

    private void Progress(CoreProgress setupProgress)
    {
        m_SetupProgress = setupProgress;
        Debug.Log($"CoreProgress: {m_SetupProgress}\n");

    }
}
