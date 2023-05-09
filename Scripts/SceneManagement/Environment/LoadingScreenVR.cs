
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreenVR : LoadingScreenBase
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

    private Transform m_LoadingWristLeft;
    private Transform m_LoadingWristRight;


    private void Start()
    {
        m_Transform = transform;

        // fix the camera
        CameraControllerVR.Instance.MainCamera.clearFlags = CameraClearFlags.Color;
        CameraControllerVR.Instance.MainCamera.backgroundColor = Color.black;

        m_Spinner = this.gameObject.SearchComponent<Transform>("PC Camera");

#if VR_INTERACTION
        CameraControllerVR.Instance.TeleportAvatar(this.gameObject.scene, m_Spinner.transform.position, m_Spinner.transform.rotation, () =>
        {
            m_Spinner.SetParent(CameraControllerVR.Instance.MainCamera.transform, false);
            m_Spinner.ClearLocals();
            m_Spinner.transform.localPosition = new Vector3(-1, 0, 0); //  this is so it can be seen
            PlaceInfrontMainCamera();
        });
#endif

        var lefthand = InputManagerVR.Instance.GetController(Handedness.Left);
        if (lefthand != null)
        {
            m_LoadingWristLeft = this.gameObject.SearchComponent<Transform>("LoadingWristLeft");
            if (m_LoadingWristLeft != null)
            {
                lefthand.AddModelAttachmentToRaycastRoot(m_LoadingWristLeft.gameObject, Vector3.zero, Quaternion.identity);
            }
            else
            {
                Debug.LogError("Tom this is were you attach the round counter percenatge to the hand");
            }
        }


        var righthand = InputManagerVR.Instance.GetController(Handedness.Right);
        if (righthand != null)
        {
            m_LoadingWristRight = this.gameObject.SearchComponent<Transform>("LoadingWristRight");
            if (m_LoadingWristRight != null)
            {
                righthand.AddModelAttachmentToRaycastRoot(m_LoadingWristRight.gameObject, Vector3.zero, Quaternion.identity);
            }
            else
            {
                Debug.LogError("Tom this is were you attach the round counter percenatge to the hand");
            }
        }

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

    private void OnDestroy()
    {
        Destroy(m_Spinner.gameObject);
        Destroy(m_LoadingWristLeft.gameObject);
        Destroy(m_LoadingWristRight.gameObject);
    }

    public override void PlaceInfrontMainCamera()
    {
        //////m_Transform.gameObject.PlaceInfrontMainCamera(2f);
        //////m_Transform.LookAt(CameraControllerVR.Instance.MainCamera.transform);
        //////m_Transform.rotation = Quaternion.Euler(0, m_Transform.rotation.eulerAngles.y -180, 0);
        //////var pos = m_Transform.transform.position;
        ////////pos.y = 0;
        //////m_Transform.transform.position = pos;
    }

    public override  void InitialiseLoadingScene(Sprite image, bool bLoadingBar)
    {
    }

    public override  void InitialiseLoadingScene(string imageName, bool bLoadingBar)
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



    private void Update()
    {
        // for some reason, if i put in awake or start , nothing happens
        // i checked the hieracky and all the layers are set to LoadingLayer
        // this is here to force it 
        this.gameObject.SetLayerRecursively(Layers.LoadingLayer);
    }

}
