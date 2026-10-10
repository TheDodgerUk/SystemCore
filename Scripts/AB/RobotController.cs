using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotController : MonoBehaviour
{
    [SerializeField]
    private Transform m_LookTarget;
    [SerializeField]
    private Transform m_EyeTarget;
    private Material faceMat;
    private FaceUI faceUI;

    private PriorityTracker m_PriorityTracker;
    public PriorityTracker Tracker => m_PriorityTracker;

    // Use this for initialization
    public void Init()
    {
        m_LookTarget = transform.parent.Find("LookTarget");
        m_EyeTarget = transform.parent.Find("EyeTarget");
        CreateFace();

        m_PriorityTracker = gameObject.ForceComponent<PriorityTracker>();
        Transform faceDirection = transform.Search("ForwardDir");
        m_PriorityTracker.Init(faceDirection);
        m_PriorityTracker.OnTargetChange += (e) => faceUI.SetPriorityTarget(e);
    }

    private void CreateFace()
    {
        Transform screen = transform.Find("Bounce/ABBot/facescreen_Game001");
        faceMat = screen.GetComponent<Renderer>().sharedMaterial;

        faceUI = GetComponent<FaceUI>();
        faceUI.Init(faceMat);
        faceUI.SetLookTarget(m_LookTarget, m_EyeTarget);

        GetComponent<StatusUI>().Init(faceMat);

        Transform bodyScreen = transform.Find("Bounce/ABBot/chestscreen_Game001");
        Material chestScreenMat = bodyScreen.GetComponent<Renderer>().sharedMaterial;

        bodyScreen.GetComponent<StatusUI>().Init(chestScreenMat);
    }
}