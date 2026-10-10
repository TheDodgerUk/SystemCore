using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using System.Linq;

public class BuildAB : MonoBehaviour
{
    public static BuildAB Instance;
    public static Transform ABRoot;
    private const string LOOK_TARGET = "LookTarget";
    private const string FACE_UI = "Bounce/ABBot/FaceCam";
    private const string FACE_STATUS_UI = "Bounce/ABBot/StatusCam";
    private const string BODY_TRANSFORM = "Bounce/ABBot/chestscreen_Game001";
    private const string HEAD_BONE = "Bounce/ABBot/BN_Hub/BN_Neck/BN_Head";
    private const string AB_BOT = "Bounce/ABBot";

    private const string BODY_STATUS_CAM = "StatusCam";
    private const string UI_FACE_LAYER = "Face";
    private const string UI_STATUS_LAYER = "Status";
    private const string UI_BODY_LAYER = "Body";

    public RobotController controller;

    // Start is called before the first frame update
    public void Initialise()
    {
        Instance = this;
        ABRoot = transform;

        Transform LookTarget = transform.parent.Find(LOOK_TARGET);

        Transform bounceTransform = transform.Find("Bounce");

        BounceHeight bounceHeight = null;
        if (null != bounceTransform)
        {
            bounceHeight = bounceTransform.gameObject.ForceComponent<BounceHeight>();
            bounceHeight.Init(0.05f);
        }

        FaceUI face = gameObject.ForceComponent<FaceUI>();
        face.Construct(FACE_UI, UI_FACE_LAYER, 0.15f);

        StatusUI ui = gameObject.ForceComponent<StatusUI>();
        ui.Construct(FACE_STATUS_UI, UI_STATUS_LAYER, 0.1f);

        Transform body = transform.Find(BODY_TRANSFORM);
        if(null != body)
        {
            StatusUI chest = body.gameObject.ForceComponent<StatusUI>();
            chest.Construct(BODY_STATUS_CAM, UI_BODY_LAYER, 0.1f);
        }

        controller = gameObject.ForceComponent<RobotController>();
        controller.Init();

        //Add physics
        Transform headBone = transform.Find(HEAD_BONE);
        var arialBezierCable = headBone.gameObject.ForceComponent<BezierCable>();

        Transform pointA = headBone.Find("BN_Aerial01");
        Transform pointB = headBone.Find("BN_Aerial12");
        Transform anchorA = headBone.Find("ArialAnchorA");
        Transform anchorB = headBone.Find("ArialAnchorB");
        Transform[] parts = FindAll(headBone, "BN_Aerial", 13, "00");
        arialBezierCable.Initialise(pointA, pointA, anchorA, anchorB, parts);

        Transform abBot = transform.Find(AB_BOT);
        var bodyBezierCable = abBot.gameObject.ForceComponent<BezierCable>();

        Transform bodyPointA = abBot.Search("CATRigTail1");
        Transform bodyPointB = abBot.Search("CATRigTail15");
        Transform bodyAnchorA = abBot.Search("CableAnchorA");
        Transform bodyAnchorB = abBot.Search("CableAnchorB");
        Transform[] bodyParts = FindAll(abBot, "CATRigTail", 16, "0");
        bodyBezierCable.Initialise(bodyPointA, bodyPointB, bodyAnchorA, bodyAnchorB, bodyParts);

        //Setup Bindings for anything unique for AB
        Dictionary<string, PlayableDirectorController> directors = TutorialManager.GetDirectorsForScene();

        //Reset bindings for all AB related nodes
        foreach (KeyValuePair<string, PlayableDirectorController> kvp in directors)
        {
            var director = kvp.Value.m_PlayableDirector;
            var playable = director.playableAsset;
            List<PlayableBinding> bindings = playable.outputs.ToList();

            for(int i = 0; i < bindings.Count; i++)
            {
                var track = bindings[i];
                string streamName = track.streamName;
                switch(streamName)
                {
                    case "Emotion Control Track":
                        director.SetGenericBinding(track.sourceObject, face.GetFacialEmotion);
                        break;
                    case "Bounce Height Track":
                        director.SetGenericBinding(track.sourceObject, bounceHeight);
                        break;
                }
            }
        }
    }

    private Transform[] FindAll(Transform parent, string sName, int iMaxNumber, string format)
    {
        List<Transform> list = new List<Transform>();

        for(int i = 1; i < iMaxNumber; i++)
        {
            string childName = sName + i.ToString(format);
            var child = parent.Search(childName);
            if(null != child)
            {
                list.Add(child);
            }
        }

        return list.ToArray();
    }

    [InspectorButton]
    public void ManualInitialise() => Initialise();
}