
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreenNonVR : LoadingScreenBase
{
    [SerializeField]
    private Color m_BackgroundColour = Color.grey;
    private Color m_OriginalColour = Color.black;

    //private TextMeshProUGUI m_TxtProgress;


    //private RectTransform m_CanvasTransform;
    //private Canvas m_Canvas;

    private Transform m_Transform;

    private string m_ImageName;

    List<GameObject> m_Lights = new List<GameObject>();

    private Transform m_Spinner;
    private void Awake()
    {
        m_Transform = transform;

        // fix the camera
        CameraControllerVR.Instance.MainCamera.clearFlags = CameraClearFlags.Color;
        CameraControllerVR.Instance.MainCamera.backgroundColor = Color.black;


        m_Spinner = this.gameObject.SearchComponent<Transform>("PC Camera");
        m_Spinner.SetParent(CameraControllerVR.Instance.MainCamera.transform, false);
        m_Spinner.ClearLocals();

        CameraControllerVR.Instance.MainCamera.transform.position = new Vector3(0, 2f, -10f);

        m_Lights.Add(this.gameObject.SearchComponent<Transform>("LightStage_001").gameObject);
        m_Lights.Add(this.gameObject.SearchComponent<Transform>("LightStage_002").gameObject);
        m_Lights.Add(this.gameObject.SearchComponent<Transform>("LightStage_003").gameObject);
        m_Lights.Add(this.gameObject.SearchComponent<Transform>("LightStage_004").gameObject);
        m_Lights.Add(this.gameObject.SearchComponent<Transform>("LightStage_005").gameObject);
        foreach (var item in m_Lights)
        {
            item.SetActive(false);
        }
    }

    private void Update()
    {
        this.gameObject.SetLayerRecursively(Layers.LoadingLayer);
    }
    private void OnDestroy()
    {
        Destroy(m_Spinner.gameObject);
    }

    public override void PlaceInfrontMainCamera()
    {
        // do nothing 
    }

    public override void InitialiseLoadingScene(Sprite image, bool bLoadingBar)
    {
    }

    public override void InitialiseLoadingScene(string imageName, bool bLoadingBar)
    {
    }

    public override void SetMessage(string message)
    {

    }

    public override void SetProgress(float progress)
    {
        if (progress >= 1f)
        {
            m_Lights[0].SetActive(true);
            m_Lights[1].SetActive(true);
            m_Lights[2].SetActive(true);
            m_Lights[3].SetActive(true);
            m_Lights[4].SetActive(true);
        }
        if (progress > 0.8f)
        {
            m_Lights[0].SetActive(true);
            m_Lights[1].SetActive(true);
            m_Lights[2].SetActive(true);
            m_Lights[3].SetActive(true);
        }
        else if (progress > 0.6f)
        {
            m_Lights[0].SetActive(true);
            m_Lights[1].SetActive(true);
            m_Lights[2].SetActive(true);
        }
        else if (progress > 0.4f)
        {
            m_Lights[0].SetActive(true);
            m_Lights[1].SetActive(true);
        }
        else if (progress > 0.2f)
        {
            m_Lights[0].SetActive(true);
        }
    }
}
