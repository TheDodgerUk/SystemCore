using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Playables;

public class TutorialManager : MonoBehaviour
{
    public enum TutorialStage
    {
        None,
        Intro,
        Chaperone,
        PlaySpace,
        Trigger,
        Grip,
        ClickStick,
        TouchStick,
        Menu,
        Outro
    }

    private List<TutorialStage> TutorialSequence = new List<TutorialStage>()
    {
        TutorialStage.Intro, TutorialStage.Chaperone, TutorialStage.PlaySpace, TutorialStage.Trigger,
        TutorialStage.Grip, TutorialStage.ClickStick, TutorialStage.TouchStick,
        TutorialStage.Menu, TutorialStage.Outro
    };

    private Dictionary<TutorialStage, TutorialStep> m_DictOfTutorialSteps = new Dictionary<TutorialStage, TutorialStep>();
    private static Dictionary<string, PlayableDirectorController> m_DictOfDirectors = new Dictionary<string, PlayableDirectorController>();

    public static Dictionary<string, PlayableDirectorController> GetDirectorsForScene()
    {
        return m_DictOfDirectors;
    }

    [SerializeField]
    private TutorialStage m_TutorialStage;
    private int m_iTutorialStep = 0;
    private TutorialStep m_CurrentStep = null;

    private float m_fWalkTriggerRadius = 0.25f;

    public void Initialise()
    {
        m_CurrentStep = null;
        m_iTutorialStep = 0;

        FindSequenceParent();
    }

    public void OnSceneLoaded()
    {        
        //Create classes to allow tutorial to take place
        CreateAllSteps();

        //User has not started the tutorial 
        SetTutorialStage(TutorialStage.None);
    }

    private void FindSequenceParent()
    {
        List<UnityEngine.SceneManagement.Scene> scenes = Core.Environment.GetLoadedScenes();

        for (int i = 0; i < scenes.Count; i++)
        {
            GameObject[] rootObjects = scenes[i].GetRootGameObjects();

            for(int x =0; x < rootObjects.Length; x++)
            {
                if(rootObjects[x].name == "Sequences")
                {
                    var playableDirectors = rootObjects[x].GetComponentsInChildren<PlayableDirector>();

                    foreach(var director in playableDirectors)
                    {
                        var controller = director.gameObject.ForceComponent<PlayableDirectorController>();
                        if(false == m_DictOfDirectors.ContainsKey(director.name))
                        {
                            m_DictOfDirectors.Add(director.name, null);
                        }

                        m_DictOfDirectors[director.name] = controller;
                    }
                }
            }
        }
    }

    private void CreateAllSteps()
    {
        foreach(var step in TutorialSequence)
        {
            var stage = GetStep(step);
            if (null != stage)
            {
                m_DictOfTutorialSteps.Add(step, stage);
            }
        }
    }

    private TutorialStep GetStep(TutorialStage stage)
    {
        TutorialStep step = null;
        switch (stage)
        {
            case TutorialStage.Intro:
                SetupIntro(ref step);
                break;
            case TutorialStage.Chaperone:
                SetupChaperone(ref step);
                break;
            case TutorialStage.PlaySpace:
                SetupPlayspace(ref step);
                break;
            case TutorialStage.Trigger:
                SetupTrigger(ref step);
                break;
            case TutorialStage.Grip:
                SetupGrip(ref step);
                break;
            case TutorialStage.ClickStick:
                SetupClickStick(ref step);
                break;
            case TutorialStage.TouchStick:
                SetupTouchStick(ref step);
                break;
            case TutorialStage.Menu:
                SetupMenu(ref step);
                break;
            case TutorialStage.Outro:
                break;
        }

        return step;
    }

    private void SetupIntro(ref TutorialStep step)
    {
        //First step is look tutorial
        LookTutorialStep lookStep = CreateChild(gameObject, "IntroLook").ForceComponent<LookTutorialStep>();
        step = lookStep;

        //Get look transform for first step
        Transform introLookTarget = transform.Find("IntroSequenceTrigger");

        if (null != introLookTarget)
        {
            var director = m_DictOfDirectors["IntroSequence"];

            director.Evaluate();
            lookStep.Initialise(introLookTarget);

            //When user looks at first location... perform the following
            lookStep.OnCompleteCallback += () =>
            {                    
                director.Play();
                director.OnSequenceComplete += () =>
                {
                    var welcomeSequence = m_DictOfDirectors["WelcomeToVR"];
                    welcomeSequence.Play();
                    welcomeSequence.OnSequenceComplete += () =>
                    {
                        Debug.Log("Intro seq finished");
                        //Play space intro
                        var chaperoneInro = m_DictOfDirectors["ChaperoneIntro"];
                        chaperoneInro.Play();
                        chaperoneInro.OnSequenceComplete += () =>
                        {
                            ProgressTutorialManually();
                        };
                    };
                };
            };        
        }
    }

    private void SetupChaperone(ref TutorialStep step)
    {
        Transform chaparoneMarker = transform.Find("ChaperoneMarker");

        var chaperoneTutorial = CreateChild(gameObject, "Chaperone").ForceComponent<MoveTutorialStep>();
        step = chaperoneTutorial;

        Transform cameraTransform = CameraControllerVR.Instance.MainCamera.transform;
        chaperoneTutorial.Initialise(chaparoneMarker, cameraTransform, m_fWalkTriggerRadius);

        var walkStep = SetupEffectController<ParticleEffectController>(chaperoneTutorial.gameObject, "WalkMarker", chaparoneMarker, chaperoneTutorial);

        var chaperoneEnd = m_DictOfDirectors["ChaperoneEnd"];
        var playspace01 = m_DictOfDirectors["Playspace01"];

        chaperoneTutorial.OnCompleteCallback += () =>
        {
            walkStep.OnDeactivated();
            chaperoneEnd.Play();
            chaperoneEnd.OnSequenceComplete += () =>
            {
                playspace01.Play();
                playspace01.OnSequenceComplete += () =>
                {
                    ProgressTutorialManually();
                };
            };
        };
     }

    private void SetupPlayspace(ref TutorialStep step)
    {
        Transform walkMarker01 = transform.Find("WalkMarker01");
        Transform walkMarker02 = transform.Find("WalkMarker02");

        var movementTutorial01 = CreateChild(gameObject, "PlaySpace_01").ForceComponent<MoveTutorialStep>();
        var movementTutorial02 = CreateChild(gameObject, "Playspace_02").ForceComponent<MoveTutorialStep>();
        step = movementTutorial01;

        Transform cameraTransform = CameraControllerVR.Instance.MainCamera.transform;
        movementTutorial01.Initialise(walkMarker01, cameraTransform, m_fWalkTriggerRadius);
        movementTutorial02.Initialise(walkMarker02, cameraTransform, m_fWalkTriggerRadius);

        var walkStep01 = SetupEffectController<ParticleEffectController>(movementTutorial01.gameObject, "WalkMarker", walkMarker01, movementTutorial01);
        var walkStep02 = SetupEffectController<ParticleEffectController>(movementTutorial02.gameObject, "WalkMarker", walkMarker02, movementTutorial02);

        var director_playspace02 = m_DictOfDirectors["Playspace02"];
        var director_playspaceEnd = m_DictOfDirectors["PlayspaceEnd"];
        var controllerIntro = m_DictOfDirectors["Controllers"];
        var trigger = m_DictOfDirectors["Trigger"];

        movementTutorial01.OnCompleteCallback += () =>
        {
            walkStep01.OnDeactivated();
            director_playspace02.Play();
            director_playspace02.OnSequenceComplete += () =>
            {
                movementTutorial02.ActivateStep();
                movementTutorial02.OnCompleteCallback += () =>
                {
                    walkStep02.OnDeactivated();
                    //Play end sequence for movement tutorial
                    director_playspaceEnd.Play();
                    director_playspaceEnd.OnSequenceComplete += () =>
                    {
                        controllerIntro.Play();
                        controllerIntro.OnSequenceComplete += () =>
                        {
                            trigger.Play();
                            trigger.OnSequenceComplete += () =>
                            {
                                ProgressTutorialManually();
                            };
                        };
                    };
                };
            };
        };
    }

    private void SetupTrigger(ref TutorialStep step)
    {
        //Setup the press trigger for controller
        var tutorialStep = CreateChild(gameObject, "Trigger").ForceComponent<PressTutorialStep>();
        
        //Setup effects
        var laserEmitter = SetupEffectController<ParticleEffectController>(tutorialStep.gameObject, "LaserEmitter");
        var laserImpact = SetupEffectController<ParticleEffectController>(tutorialStep.gameObject, "LaserImpact");
        var laserBeam = SetupEffectController<LineRenderEffectController>(tutorialStep.gameObject, "LaserBeam");

        step = tutorialStep;

        //Setup callbacks for all effects on this laser
        tutorialStep.OnDown += (e) => laserEmitter.OnEffectStart(e.Reticle);
        tutorialStep.OnHeld += (e) => laserEmitter.OnEffectUpdate(e.Reticle);
        tutorialStep.OnUp += (e) => laserEmitter.OnEffectEnd(e.Reticle);
        tutorialStep.OnDown += (e) => laserImpact.OnEffectStart(e.Raycaster.GetEndPoint(), -e.Raycaster.Ray.direction);
        tutorialStep.OnHeld += (e) => laserImpact.OnEffectUpdate(e.Raycaster.GetEndPoint(), -e.Raycaster.Ray.direction);
        tutorialStep.OnUp += (e) => laserImpact.OnEffectEnd(e.Raycaster.GetEndPoint(), -e.Raycaster.Ray.direction);
        tutorialStep.OnDown += (e) => laserBeam.OnEffectStart(e);
        tutorialStep.OnHeld += (e) => laserBeam.OnEffectUpdate(e);
        tutorialStep.OnUp += (e) => laserBeam.OnEffectEnd(e);

        var triggerPressed = m_DictOfDirectors["TriggerEnd"];
        var grip = m_DictOfDirectors["Grip"];
        tutorialStep.Initialise(TriggerState.Down, PressTutorialStep.ButtonType.Trigger);
        tutorialStep.OnActivateCallback += laserEmitter.OnActivated;
        tutorialStep.OnActivateCallback += laserImpact.OnActivated;
        tutorialStep.OnActivateCallback += laserBeam.OnActivated;

        //Process order at which events occur from completing controller input
        tutorialStep.OnCompleteCallback += () =>
        {
            this.WaitFor(8.0f, () =>
            {
                triggerPressed.Play();
                triggerPressed.OnSequenceComplete += () =>
                {
                    laserEmitter.OnDeactivated();
                    laserImpact.OnDeactivated();
                    laserBeam.OnDeactivated();

                    //Activate environment
                    grip.Play();
                    //Do something like wait
                    grip.OnSequenceComplete += () =>
                    {
                        //Deactivate controllers and progress
                        tutorialStep.DeInitialise();
                        ProgressTutorialManually();
                    };
                };
            });
        };
    }

    private void SetupGrip(ref TutorialStep step)
    {
        var tutorialStep = CreateChild(gameObject, "Grip").ForceComponent<PressTutorialStep>();
        ParticleEffectController bubbleEffect = SetupEffectController<ParticleEffectController>(tutorialStep.gameObject, "Bubbles");

        BuildAB.Instance.controller.Tracker.AddTrackedParticleSystem(bubbleEffect.GetParticleSystem, 0.05f, 0.1f, 0f, 0.25f);
        step = tutorialStep;

        var gripPressed = m_DictOfDirectors["GripEnd"];
        var clickPad = m_DictOfDirectors["TrackPad"];

        tutorialStep.Initialise(TriggerState.Down, PressTutorialStep.ButtonType.Grip);
        tutorialStep.OnDown += (e) => bubbleEffect.OnEffectStart(e.WristTransform);
        tutorialStep.OnHeld += (e) => bubbleEffect.OnEffectUpdate(e.WristTransform);
        tutorialStep.OnUp += (e) => bubbleEffect.OnEffectEnd(e.WristTransform);

        //Activate bubbles particle effect
        tutorialStep.OnActivateCallback += bubbleEffect.OnActivated;
        tutorialStep.OnCompleteCallback += () =>
        {
            gripPressed.Play();
            gripPressed.OnSequenceComplete += () =>
            {
                this.WaitFor(8f, () =>
                {
                    bubbleEffect.OnDeactivated();
                    //Start click pad tutorial sequence
                    clickPad.Play();
                    clickPad.OnSequenceComplete += () =>
                    {
                        //Deactivate controllers and progress
                        tutorialStep.DeInitialise();
                        ProgressTutorialManually();
                    };
                });
            };
        };
    }

    private void SetupClickStick(ref TutorialStep step)
    {
        var tutorialStep = CreateChild(gameObject, "ClickStick").ForceComponent<PressTutorialStep>();
        step = tutorialStep;

        var clickPadPressed = m_DictOfDirectors["TrackPadEnd"];
        var touchPad = m_DictOfDirectors["TrackPadDance"];
        var openSpeakers = m_DictOfDirectors["Speakers_OpenandRise"];

        tutorialStep.Initialise(TriggerState.Down, PressTutorialStep.ButtonType.ClickStick);
        tutorialStep.OnCompleteCallback += () =>
        {
            openSpeakers.Play();
            clickPadPressed.Play();
            clickPadPressed.OnSequenceComplete += () =>
            {
                //Do something like wait
                touchPad.Play();
                touchPad.OnSequenceComplete += () =>
                {
                    //Deactivate controllers and progress
                    tutorialStep.DeInitialise();
                    ProgressTutorialManually();
                };
            };
        };
    }

    private void SetupTouchStick(ref TutorialStep step)
    {
        var tutorialStep = CreateChild(gameObject, "TouchStick").ForceComponent<PressTutorialStep>();
        step = tutorialStep;

        var touchPadPressed = m_DictOfDirectors["TrackPadDanceEnd"];
        var menu = m_DictOfDirectors["Menu"];

        tutorialStep.Initialise(TriggerState.Down, PressTutorialStep.ButtonType.TouchStick);
        tutorialStep.OnCompleteCallback += () =>
        {
            //Activate environment
            touchPadPressed.Play();
            touchPadPressed.OnSequenceComplete += () =>
            {
                //Do something like wait
                this.WaitFor(10.0f, () =>
                {
                    //Deactivate controllers and progress
                    tutorialStep.DeInitialise();
                    menu.Play();
                    menu.OnSequenceComplete += () =>
                    {
                        ProgressTutorialManually();
                    };
                });
            };
        };
    }

    private void SetupMenu(ref TutorialStep step)
    {
        var tutorialStep = CreateChild(gameObject, "MenuBtn").ForceComponent<PressTutorialStep>();
        step = tutorialStep;

        ParticleEffectController confettiEffect = SetupEffectController<ParticleEffectController>(tutorialStep.gameObject, "Confetti");

        var menuPressed = m_DictOfDirectors["MenuEnd"];
        var exitSequence = m_DictOfDirectors["ExitSquence"];

        tutorialStep.Initialise(TriggerState.Down, PressTutorialStep.ButtonType.Menu);
        tutorialStep.OnDown += (e) => confettiEffect.OnEffectStart(e.WristTransform);
        tutorialStep.OnHeld += (e) => confettiEffect.OnEffectUpdate(e.WristTransform);
        tutorialStep.OnUp += (e) => confettiEffect.OnEffectEnd(e.WristTransform);

        tutorialStep.OnActivateCallback += confettiEffect.OnActivated;
        tutorialStep.OnCompleteCallback += () =>
        {
            menuPressed.Play();
            menuPressed.OnSequenceComplete += () =>
            {
                this.WaitFor(5.0f, () =>
                {
                    confettiEffect.OnDeactivated();
                    exitSequence.Play();
                    exitSequence.OnSequenceComplete += () =>
                    {
                        ProgressTutorialManually();
                    };
                });
            };
        };
    }

    private T SetupEffectController<T>(GameObject obj, string sEffect) where T : EffectController
    {
        var controller = obj.AddComponent<T>();
        controller.Init(sEffect);
        return controller;
    }

    private T SetupEffectController<T>(GameObject obj, string sEffect, Transform marker, TutorialStep step) where T : EffectController
    {
        var controller = SetupEffectController<T>(obj, sEffect);

        step.OnActivateCallback += () =>
        {
            controller.OnActivated();
            controller.OnEffectStart(marker);
        };

        return controller;
    }

    private GameObject CreateChild(GameObject parent, string name)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent.transform);
        obj.transform.Reset();

        return obj;
    }

    public void BeginTutorial(Action OnTutorialStartTrigger)
    {
        //When this first stage is completed... inform Tutorial Manager
        var step = GetTutorialStage(TutorialSequence[m_iTutorialStep]);
        step.OnCompleteCallback += () =>
        {
            OnTutorialStartTrigger?.Invoke();
        };

        SetTutorialStage(TutorialSequence[m_iTutorialStep]);
    }

    private void SetTutorialStage(TutorialStage stage)
    {
        m_TutorialStage = stage;

        var step = GetTutorialStage(stage);
        if (null != step)
        {
            Debug.Log($"Starting Step {stage}");
            //Get step so we can track and increment progress
            m_CurrentStep = step;
            m_CurrentStep.ActivateStep();
        }
    }

    private TutorialStep GetTutorialStage(TutorialStage stage)
    {
        if (true == m_DictOfTutorialSteps.ContainsKey(stage))
        {
            return m_DictOfTutorialSteps[stage];
        }

        return null;
    }

    private void ProgressTutorialManually()
    {
        m_iTutorialStep = Mathf.Clamp(m_iTutorialStep + 1, 0, TutorialSequence.Count - 1);
        SetTutorialStage(TutorialSequence[m_iTutorialStep]);
    }    

    [InspectorButton]
    public void ProgressTutorial() => ProgressTutorialManually();
}