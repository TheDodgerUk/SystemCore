using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace EnvironmentHelpers
{
    public static class FastTravel
    {
        private static Dictionary<string, FastTravelPoint> m_DictOfFastTravelPoints = new Dictionary<string, FastTravelPoint>();

        public static void OnLoadEnvironment()
        {
            m_DictOfFastTravelPoints.Clear();
        }

        public static void AddFastTravelPoint(string name, FastTravelPoint ftp)
        {
            m_DictOfFastTravelPoints[name] = ftp;
        }

        public static FastTravelPoint GetPoint(string name)
        {
            if (true == m_DictOfFastTravelPoints.ContainsKey(name))
            {
                return m_DictOfFastTravelPoints[name];
            }

            return null;
        }
    }
}

namespace EnvironmentHelpers
{
    public class FastTravelPoint : MonoBehaviour
    {
        public Action<bool> OnFastTravelStateChange;
        [SerializeField] private Transform m_Orientation;
        [SerializeField] private Transform m_Marker;
        [SerializeField] private Transform m_Zone;

        private GameObject m_UIGameObject;
        private Transform m_UITransform;

        [SerializeField]
        private Button m_FastTravelBtn;

        private bool bUserInSpace = false;
        [SerializeField]
        private float m_fScale = 280f;

        private Vector3 m_InitialScale;

        // Start is called before the first frame update
        public void Initialise()
        {
            //Save with name
            FastTravel.AddFastTravelPoint(gameObject.name, this);

            m_Orientation = transform.Search("Orientation");
            m_Marker = transform.Search("Marker");
            m_Zone = transform.Search("Zone");
            
            m_InitialScale = m_Marker.localScale;

            SetupVisuals();
            SetupUI();
            SetupTrigger();
        }

        private void SetupUI()
        {
            if(null != m_Marker)
            {
                //Setup UI
                Canvas canvas = m_Marker.GetComponentInChildren<Canvas>();
                m_UIGameObject = canvas.gameObject;
                m_UITransform = canvas.transform;
                m_UIGameObject.ForceComponent<VRUICanvas>();

                if (null != m_UIGameObject)
                {
                    m_FastTravelBtn = m_UIGameObject.GetComponentInChildren<Button>();
                    m_FastTravelBtn.onClick.AddListener(() => OnClicked());
                }
            }
        }

        private void SetupVisuals()
        {

        }

        private void SetupTrigger()
        {
            if(null != m_Zone)
            {
                gameObject.layer = Layers.TeleportLayer;
                Rigidbody rb = gameObject.ForceComponent<Rigidbody>();
                rb.isKinematic = true;
                
                BoxCollider box = gameObject.AddComponent<BoxCollider>();
                Vector3 zoneScale = m_Zone.localScale;
                box.center = new Vector3(0f, zoneScale.y * 0.5f, 0f);
                box.size = zoneScale;
                box.isTrigger = true;
            }
        }

        private void OnTriggerEnter()
        {
            UpdateVisual(true);
        }

        private void OnTriggerExit()
        {
            UpdateVisual(false);
        }

        private void UpdateVisual(bool inSpace)
        {
            if(inSpace != bUserInSpace)
            {
                bUserInSpace = inSpace;

                //
                m_UIGameObject.SetActive(!bUserInSpace);

                OnFastTravelStateChange?.Invoke(bUserInSpace);
            }
        }

        private void Update()
        {
            if (null != m_UITransform && false == bUserInSpace)
            {
                var cameraTransform = CameraControllerVR.Instance.CameraTransform;
                m_UITransform.LookAt(cameraTransform, Vector3.up);

                //Within interaction distance of UI?
                if ((m_Marker.position - cameraTransform.position).magnitude < VRUIPointer.PointerLength)
                {
                    Camera cam = CameraControllerVR.Instance.MainCamera;
                    Vector3 a = cam.WorldToScreenPoint(transform.position);
                    Vector3 b = new Vector3(a.x, a.y + m_fScale, a.z);

                    Vector3 aa = cam.ScreenToWorldPoint(a);
                    Vector3 bb = cam.ScreenToWorldPoint(b);

                    Vector3 newScale = Vector3.Max(Vector3.one, Vector3.one * (aa - bb).magnitude);
                    m_Marker.localScale = newScale;
                }
                else
                {
                    m_Marker.localScale = Vector3.zero;
                }
            }
        }

        [InspectorButton]
        private void OnClicked()
        {
            //Teleport here
            Debug.Log("Fast Travel activated");
            var camera = CameraControllerVR.Instance.CameraTransform;
            ////CameraControllerVR.Instance.TeleporterLeft.Teleport(m_Orientation.position, onBlink: () =>
            ////{
            ////    var rig = CameraControllerVR.Instance.RigRoot;
            ////    var camForward = camera.forward.With(y: 0);
            ////    float angle = camForward.AngleSigned(m_Orientation.forward, Vector3.up);
            ////    rig.Rotate(0, angle, 0);
            ////});
        }
    }
}