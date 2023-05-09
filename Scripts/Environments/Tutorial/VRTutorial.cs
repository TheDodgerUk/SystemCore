using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

[Serializable]
public class VideoSeries
{
    public List<VideoSeriesData> Series = new List<VideoSeriesData>();
}

[Serializable]
public class VideoSeriesData
{
    public string VideoName;
    public int LoopCount;
}

public class VRTutorial : MonoBehaviour {

    private UIVideoScreen m_ScreenHandler;
    private Transform m_StartPanel;
    private CanvasGroup m_StartPanelCG;
    private Button m_StartBtn;

    private TutorialManager m_TutorialManager;
    [SerializeField]
    private TutorialState m_TutorialState;

    [SerializeField]
    private VideoSeries IntroSequence = new VideoSeries();
    private int m_iCurrentVideoInSequence = 0;
    private int m_iCurrentVideoLoopCount = 0;

    private enum TutorialState
    {
        PreUser,                        //No user at experience
        PreVR,                          //User detected... show greeting
        ControllerExplaination,         //Controller and HMD explaination
        VRTutorial,                     //Tutorial in VR
        VRComplete,                     //Exit screen
    }

	public void Initialise()
    {
        Core.Environment.OnEnvironmentLoadingComplete += EnvironmentLoadingComplete;

        //Setup tutorial here!!
        CreateScreenUI();
        CreateUserUI();
        CreateVideoSequence();

        m_TutorialManager = gameObject.ForceComponent<TutorialManager>();
        m_TutorialManager.Initialise();

        SetTutorialState(TutorialState.PreUser);
    }

    private void EnvironmentLoadingComplete()
    {
        Debug.Log("Applying user settings");

        InputManagerVR.Instance.ToggleInput(true);
        InputManagerVR.Instance.IterateControllers(OnControllerAdded);
        InputManagerVR.Instance.ControllerAdded -= OnControllerAdded;
        InputManagerVR.Instance.ControllerAdded += OnControllerAdded;

        m_TutorialManager.OnSceneLoaded();

        Core.Environment.OnEnvironmentLoadingComplete -= EnvironmentLoadingComplete;
    }

    private void CreateVideoSequence()
    {
        IntroSequence.Series.Clear();

        IntroSequence.Series.Add(new VideoSeriesData()
        {
            VideoName = "AB_Greeting",
            LoopCount = 1
        });

        //IntroSequence.Series.Add(new VideoSeriesData()
        //{
        //    VideoName = "Tutorial_Controller",
        //    LoopCount = 1
        //});

        //IntroSequence.Series.Add(new VideoSeriesData()
        //{
        //    VideoName = "Tutorial_HMD_01",
        //    LoopCount = 1
        //});

        //IntroSequence.Series.Add(new VideoSeriesData()
        //{
        //    VideoName = "Tutorial_HMD_02",
        //    LoopCount = 1
        //});

        IntroSequence.Series.Add(new VideoSeriesData()
        {
            VideoName = "AB_WaitingForVR",
            LoopCount = 1
        });
    }

    private void SetTutorialState(TutorialState state)
    {
        m_TutorialState = state;
        switch (m_TutorialState)
        {
            case TutorialState.PreUser:
                m_ScreenHandler.Play("Tutorial_Idle");
                break;

            case TutorialState.PreVR:
                m_StartPanel.SetActive(false);
                OnControllerTutorialStarted();                
                break;

            case TutorialState.ControllerExplaination:
                m_StartPanel.SetActive(false);
                m_TutorialManager.BeginTutorial(()=>
                {
                    m_ScreenHandler.Stop();
                    m_ScreenHandler.SetActive(false);
                    SetTutorialState(TutorialState.VRTutorial);
                });
                break;

            case TutorialState.VRTutorial:
                break;

            case TutorialState.VRComplete:
                break;
        }
    }

    private void OnControllerTutorialStarted()
    {
        m_iCurrentVideoInSequence = 0;
        m_iCurrentVideoLoopCount = 0;
        m_ScreenHandler.Play(IntroSequence.Series[m_iCurrentVideoInSequence].VideoName, () =>
        {
            OnControllerTutorialVideoCompleted();
        });
    }

    private void OnControllerTutorialVideoCompleted()
    {
        m_iCurrentVideoLoopCount++;
        VideoSeriesData data = IntroSequence.Series[m_iCurrentVideoInSequence];

        if(null != data)
        {
            if(data.LoopCount > m_iCurrentVideoLoopCount)
            {
                //Loop again
                m_ScreenHandler.Play(data.VideoName, () =>
                {
                    OnControllerTutorialVideoCompleted();
                });
                return;
            }
            else
            {
                //Progress to next video
                m_iCurrentVideoLoopCount = 0;
                m_iCurrentVideoInSequence++;

                //More videos left to play?
                if(IntroSequence.Series.Count > m_iCurrentVideoInSequence)
                {
                    data = IntroSequence.Series[m_iCurrentVideoInSequence];
                    m_ScreenHandler.Play(data.VideoName, () =>
                    {
                        OnControllerTutorialVideoCompleted();
                    });
                    return;
                }
            }

        }

        //End of all videos
        SetTutorialState(TutorialState.ControllerExplaination);
    }
	
    private void CreateScreenUI()
    {
        GameObject screenUI = Utils.Unity.Clone("VideoCanvas", transform) as GameObject;

        if(null != screenUI)
        {
            m_ScreenHandler = screenUI.ForceComponent<UIVideoScreen>();
            m_ScreenHandler?.Initialise();
        }
    }

    private void CreateUserUI()
    {
        GameObject userUI = Utils.Unity.Clone("TutorialUI", transform) as GameObject;

        if(null != userUI)
        {
            m_StartPanel = userUI.transform.Find("StartPanel");
            m_StartPanelCG = m_StartPanel.GetComponent<CanvasGroup>();
            m_StartBtn = m_StartPanel.SearchComponent<Button>("StartBtn");

            m_StartBtn.onClick.AddListener(() =>
            {
                m_StartPanel.Create<ValueTween>(1.0f, EaseType.ExpoInOut, () =>
                {
                    OnStartButtonPressed();
                }).Initialise(1f, 0f, (e) =>
                {
                    m_StartPanelCG.alpha = e;
                });                
            });
        }
    }

    private void OnStartButtonPressed()
    {
        SetTutorialState(TutorialState.PreVR);
    }

    private void OnControllerAdded(ControllerData controller)
    {
        //Chaperone.Instance.TrackObject(controller.Origin);
        float minValue, maxValue, minDist, maxDist;

        if (controller.Hand == Handedness.Left || controller.Hand == Handedness.Right)
        {
            minValue = 0.16f;
            maxValue = 0f;
            minDist = 0f;
            maxDist = 2f;
        }
        else
        {
            minValue = 0.5f;
            maxValue = 1.0f;
            minDist = 1.5f;
            maxDist = 5f;
        }
        
        BuildAB.Instance.controller.Tracker.AddTrackedObject(controller.WristTransform, minValue, maxValue, minDist, maxDist);
    }
}