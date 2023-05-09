using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using System.Linq;

public class BuildDialogueBox : MonoBehaviour
{
    private DialogueBoxControl m_DialogueBoxControl;

    // Start is called before the first frame update
    public void Initialise()
    {
        m_DialogueBoxControl = gameObject.ForceComponent<DialogueBoxControl>();

        transform.GetChild(0).localPosition = new Vector3(0f, 1f, 0.25f);

        //Setup Bindings for anything unique for AB
        Dictionary<string, PlayableDirectorController> directors = TutorialManager.GetDirectorsForScene();

        //Reset bindings for all AB related nodes
        foreach (KeyValuePair<string, PlayableDirectorController> kvp in directors)
        {
            var director = kvp.Value.m_PlayableDirector;
            var playable = director.playableAsset;
            List<PlayableBinding> bindings = playable.outputs.ToList();

            for (int i = 0; i < bindings.Count; i++)
            {
                var track = bindings[i];
                string streamName = track.streamName;
                switch (streamName)
                {
                    case "Dialogue Box Track":
                        director.SetGenericBinding(track.sourceObject, m_DialogueBoxControl);
                        break;
                    default:
                        break;
                }
            }
        }
    }

    void LateUpdate()
    {
        if(null != BuildAB.ABRoot && null != CameraControllerVR.Instance &&
            null != CameraControllerVR.Instance.MainCamera)
        {
            //Set our position to match AB
            transform.position = BuildAB.ABRoot.position;

            //User head
            Transform lookTarget = CameraControllerVR.Instance.MainCamera.transform;

            Vector3 lookPosition = lookTarget.position;
            lookPosition.y = 0f;
            Vector3 position = transform.position;
            position.y = 0f;

            Vector3 direction = lookPosition - position;                    

            //Face user
            transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
        }
    }
}

public class BuildChargeStation : MonoBehaviour
{
    private Transform m_HeadLight;
    private Transform m_LeftLight;
    private Transform m_RightLight;

    private RotationControl m_HeadRotation;
    private RotationControl m_LeftLightRotation;
    private RotationControl m_RightLightControl;

    public void Initialise()
    {
        Transform headTransform = transform.Find("Charger/Charger_Dummy/MLabs_ChargingStation-Head_LOD0");
        Transform bodyTransform = headTransform.Find("Dummy - HeadPistons/MLabs_ChargingStation-HeadPistons_LOD0/Dummy - ChargingStation-Body/MLabs_ChargingStation-Body_LOD0");
        m_HeadLight = headTransform.Find("Dummy - HeadLight/MLabs_ChargingStation-LightCones");
        m_LeftLight = bodyTransform.Find("Dummy - Left Side/MLabs_ChargingStation-SidePistons (Left)_LOD0/Dummy - ChargingStation-Side (Left)/MLabs_ChargingStation-Side (Left)_LOD0/Dummy - SideLight/MLabs_ChargingStation-LightCones");
        m_RightLight = bodyTransform.Find("Dummy - Right Side/MLabs_ChargingStation-SidePistons (Right)_LOD0/Dummy - ChargingStation-Side (Right)/MLabs_ChargingStation-Side (Right)_LOD0/Dummy - SideLight/MLabs_ChargingStation-LightCones");

        m_HeadRotation = m_HeadLight.gameObject.ForceComponent<RotationControl>();
        m_LeftLightRotation = m_LeftLight.gameObject.ForceComponent<RotationControl>();
        m_RightLightControl = m_RightLight.gameObject.ForceComponent<RotationControl>();

        //Setup Bindings for anything unique for AB
        Dictionary<string, PlayableDirectorController> directors = TutorialManager.GetDirectorsForScene();

        //Reset bindings for all AB related nodes
        foreach (KeyValuePair<string, PlayableDirectorController> kvp in directors)
        {
            var director = kvp.Value.m_PlayableDirector;
            var playable = director.playableAsset;
            List<PlayableBinding> bindings = playable.outputs.ToList();

            for (int i = 0; i < bindings.Count; i++)
            {
                var track = bindings[i];
                string streamName = track.streamName;
                switch (streamName)
                {
                    case "Rotation HeadLight":
                        director.SetGenericBinding(track.sourceObject, m_HeadRotation);
                        break;
                    case "Rotation LeftLight":
                        director.SetGenericBinding(track.sourceObject, m_LeftLightRotation);
                        break;
                    case "Rotation RightLight":
                        director.SetGenericBinding(track.sourceObject, m_RightLightControl);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}