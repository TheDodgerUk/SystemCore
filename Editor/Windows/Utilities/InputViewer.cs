using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class InputViewer : EditorWindow, ISerializationCallbackReceiver
{
    [MenuItem("Window/Utility/Input Viewer")]
    public static void ShowWindow() => GetWindow<InputViewer>("Input Viewer");

    private enum Mode { InputSystem, Raw };

    private Dictionary<InputState, EditorInputState> m_EditorData;
    private List<InputSystem> m_ActiveSystems;
    private Vector2 m_ScrollPos;
    private bool m_RunSetup = true;
    private Mode m_Mode = Mode.InputSystem;

    public void OnBeforeSerialize() { }
    public void OnAfterDeserialize() => m_RunSetup = true;

    private void OnEnable()
    {
        if (EditorApplication.isPlaying == true && InputManagerVR.Instance != null)
        {
            m_EditorData = new Dictionary<InputState, EditorInputState>();
            InputManagerVR.Instance.ControllerUpdate -= OnControllerUpdate;
            InputManagerVR.Instance.ControllerUpdate += OnControllerUpdate;
            m_RunSetup = false;
        }
    }

    private void OnDestroy()
    {
        if (EditorApplication.isPlaying == true && InputManagerVR.Instance != null)
        {
            InputManagerVR.Instance.ControllerUpdate -= OnControllerUpdate;
        }
    }

    private void OnControllerUpdate(List<InputSystem> inputSystems)
    {
        m_ActiveSystems = inputSystems;
        Repaint();
    }

    private void OnGUI()
    {
        if (m_RunSetup == true)
        {
            OnEnable();
        }

        if (EditorApplication.isPlaying == false)
        {
            m_ActiveSystems = null;
        }

        EditorGUIUtility.wideMode = true;
        m_Mode = EditorHelper.EnumToolbar(m_Mode);
        if (m_Mode == Mode.InputSystem)
        {
            if (m_ActiveSystems != null)
            {
                using (var s = new EditorGUILayout.ScrollViewScope(m_ScrollPos))
                {
                    m_ActiveSystems.ForEach(i => DrawInputSystem(i));
                    m_ScrollPos = s.scrollPosition;
                }
            }
        }
        else if (m_Mode == Mode.Raw)
        {
            using (var s = new EditorGUILayout.ScrollViewScope(m_ScrollPos))
            {
                switch (Device.VrType)
                {
                    case VRDevices.Oculus:
                        DrawOculusRawInput();
                        break;
                    case VRDevices.OpenVR:
                        DrawOpenVrInput();
                        break;
                    case VRDevices.None:
                    default:
                        break;
                }
                m_ScrollPos = s.scrollPosition;
            }
        }
    }

    private void DrawOculusRawInput()
    {
        #if VR_INTERACTION && UNITY_ANDROID
        var buttons = new[]
        {
            OVRInput.Button.PrimaryIndexTrigger,
            OVRInput.Button.PrimaryHandTrigger,
            OVRInput.Button.Two,
            OVRInput.Button.One,
        };

        var ids = new[]
        {
            OVRInput.Controller.LTouch,
            OVRInput.Controller.RTouch,
        };

        foreach (var id in ids)
        {
            EditorGUILayout.LabelField(id.ToString(), EditorStyles.boldLabel);

            foreach (var button in buttons)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    bool down = OVRInput.GetDown(button, id);
                    bool held = OVRInput.Get(button, id);
                    bool up = OVRInput.GetUp(button, id);

                    using (new LabelWidthScope(180))
                    {
                        EditorGUILayout.PrefixLabel(button.ToString());
                        EditorGUILayout.Toggle(down, GUILayout.Width(18));
                        EditorGUILayout.Toggle(held, GUILayout.Width(18));
                        EditorGUILayout.Toggle(up, GUILayout.Width(18));
                    }
                }
            }
            EditorGUILayout.Space();
        }
#endif
    }

    private void DrawOpenVrInput()
    {
        //if (SteamVR.active == false || SteamVR.enabled == false)
        //{
        //    EditorGUILayout.LabelField("SteamVR is not active and enabled");
        //    return;
        //}

        //var controllerManager = FindObjectOfType<SteamVR_ControllerManager>();
        //if (controllerManager == null)
        //{
        //    EditorGUILayout.LabelField("Cannot find SteamVR_ControllerManager");
        //    return;
        //}

        //var buttons = new[]
        //{
        //    EVRButtonId.k_EButton_SteamVR_Trigger,
        //    EVRButtonId.k_EButton_Grip,
        //    EVRButtonId.k_EButton_SteamVR_Touchpad,
        //    EVRButtonId.k_EButton_ApplicationMenu,
        //};

        //foreach (var obj in controllerManager.objects)
        //{
        //    EditorHelper.ObjectField(obj);
        //    var trackedObject = obj.GetComponent<SteamVR_TrackedObject>();
        //    if (trackedObject.index == SteamVR_TrackedObject.EIndex.None)
        //    {
        //        EditorGUILayout.LabelField("Tracked Index is none");
        //    }
        //    else
        //    {
        //        var device = SteamVR_Controller.Input((int)trackedObject.index);
        //        foreach (var button in buttons)
        //        {
        //            using (new EditorGUILayout.HorizontalScope())
        //            {
        //                bool down = device.GetPressDown(button);
        //                bool held = device.GetPress(button);
        //                bool up = device.GetPressUp(button);

        //                using (new LabelWidthScope(180))
        //                {
        //                    EditorGUILayout.PrefixLabel(button.ToString());
        //                    EditorGUILayout.Toggle(down, GUILayout.Width(18));
        //                    EditorGUILayout.Toggle(held, GUILayout.Width(18));
        //                    EditorGUILayout.Toggle(up, GUILayout.Width(18));
        //                }
        //            }
        //        }
        //    }
        //    EditorGUILayout.Space();
        //}
    }

    private void DrawInputSystem(InputSystem inputSystem)
    {
        using (new EditorGUILayout.HorizontalScope())
        {
            foreach (var controller in inputSystem?.GetControllers() ?? new List<ControllerData>())
            {
                using (new EditorGUILayout.VerticalScope(GUI.skin.box))
                {
                    EditorGUILayout.LabelField(controller.WristTransform.name);
                    EditorHelper.ObjectField(controller.WristTransform);

                    EditorGUILayout.Toggle("Locked", controller.IsLocked);
                    if (controller.IsLocked == true)
                    {
                        using (new GuiIndentScope())
                        {
                            if (GUILayout.Button("Disable Lock") == true)
                            {
                                controller.UnlockFromObject();
                            }
                            EditorHelper.ObjectField("Locked Object", controller.LockedObject);
                        }
                    }

                    EditorHelper.ObjectField("Current", controller.CurrentCollider);
                    EditorHelper.ObjectField("Previous", controller.PreviousCollider);

                    EditorGUILayout.Vector3Field("Position", controller.WristTransform.position.Round(2));
                    EditorGUILayout.Vector3Field("Rotation", controller.WristTransform.rotation.eulerAngles.Round(2));

                    DrawInputState("Start", controller.Inputs.BtnStart);
                    DrawInputState("Hover", controller.Inputs.Hover);
                    DrawInputState("Grab", controller.Inputs.BtnGrab);
                    DrawInputState("Interact", controller.Inputs.Interact);
                    DrawInputState("BtnPrimary", controller.Inputs.BtnPrimary);
                    DrawInputState("BtnSecondary", controller.Inputs.BtnSecondary);
                    DrawInputState("(Touch) Thumbstick", controller.Inputs.Stick);
                    DrawInputState("(Click) Thumbstick", controller.Inputs.BtnStick);
                }
            }
        }
    }

    private void DrawInputState(string name, InputState state)
    {
        string label = $"{name} ({state.State})";
        var editorData = m_EditorData.Get(state) ?? new EditorInputState();
        editorData.Foldout = EditorGUILayout.Foldout(editorData.Foldout, label, true);
        if (editorData.Foldout == true)
        {
            using (new GuiIndentScope())
            {
                var newState = EditorHelper.EnumPopup("State", state.State);
                if (state.State != newState)
                {
                    state.State = newState;
                    state.AutoUpdate = false;
                }
                state.AutoUpdate = EditorGUILayout.Toggle("Update", state.AutoUpdate);
                                
                Vector2State thumbstick = state as Vector2State;
                if(null != thumbstick)
                {
                    EditorGUIUtility.wideMode = true;
                    EditorGUILayout.Vector2Field("Axis", thumbstick.Value2);
                }

                EditorGUILayout.FloatField("Start Time", state.Time.Start.Round(4));
                EditorGUILayout.FloatField("Delta Time", state.Time.Delta.Round(2));
                DrawSpatialState("World", state.Position, state.Rotation);
            }
        }
        m_EditorData[state] = editorData;
    }

    private static void DrawSpatialState(string label, DeltaComputer<Vector3> position, DeltaComputer<Quaternion> rotation)
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
        EditorGUILayout.Vector3Field("Start Position", position.Start.Round(2));
        EditorGUILayout.Vector3Field("Delta Position", position.Delta.Round(2));
        EditorGUILayout.Vector3Field("Total Position", position.Total.Round(2));

        EditorGUILayout.Space();
        EditorGUILayout.Vector3Field("Start Rotation", rotation.Start.eulerAngles.Round(2));
        EditorGUILayout.Vector3Field("Delta Rotation", rotation.Delta.eulerAngles.WrapTo180().Round(3));
        EditorGUILayout.Vector3Field("Total Rotation", rotation.Total.eulerAngles.WrapTo180().Round(3));

        EditorGUILayout.Space();
        EditorGUILayout.FloatField("Mag Velocity", position.Delta.magnitude.Round(4));
        EditorGUILayout.FloatField("Sqr Velocity", position.Delta.sqrMagnitude.Round(6));
    }

    public class EditorInputState
    {
        public bool Foldout;
    }
}
