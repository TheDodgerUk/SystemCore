using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnvironmentLoader : MonoBehaviour
{
    public static Action<bool> OnEnvironmentsMenuOpened;

    public event Action OnSceneLoaded;

    public event Action OnOptionalSceneLoaded;

    private GameObject m_Menu;
    private Canvas m_Canvas;

    private Transform m_ContentParent;
    private GameObject m_ContentPrefab;

    private bool m_bMenuState = true;
    private List<EnvironmentData> m_Environments;
    private bool m_MouseLock = true;
    private bool m_MouseVisible = false;
    public bool AllowMenu { get; set; }
    private bool m_AllowMenu = true;

    public void DisableMenu() => m_AllowMenu = false;

    // Use this for initialization
    public void Init(List<EnvironmentData> Environments)
    {
        if (m_Menu == null)
        {
            AllowMenu = true;
            m_Environments = Environments;
            //Create UI for selecting environment to load
            m_Menu = GameObject.Instantiate(Resources.Load<GameObject>("LoadLevelCanvas")) as GameObject;
            m_Canvas = m_Menu.GetComponent<Canvas>();
            m_Menu.transform.SetParent(transform);

            m_ContentParent = m_Menu.transform.Search("LevelsPanel");

            m_ContentPrefab = Resources.Load<GameObject>("LoadableLevel") as GameObject;

            SetState(false);

            foreach (var env in Environments)
            {
                CreateContent(env, Environments.Count);
            }
            SetMouseLockVisible(true, false);
            Core.SceneLoader.ToggleMenu();
        }
    }

    private void CreateContent(EnvironmentData data, int environmentCount)
    {
        if (environmentCount != 1)
        {
            GameObject content = GameObject.Instantiate(m_ContentPrefab) as GameObject;

            content.transform.SetParent(m_ContentParent);
            Text lbl = content.GetComponentInChildren<Text>();
            lbl.text = $"{data.EnvironmentName} - {data.VariantName}";
            Button btn = content.GetComponentInChildren<Button>();
            btn.onClick.AddListener(() =>
            {
                Debug.Log($"EnvironmentData selected  {data.EnvironmentName} - {data.VariantName}");
                InputManagerVR.Instance.ToggleInput(false);

                ToggleMenuOff();
                SetMouseLockVisible(true, false);
                Core.Environment.LoadEnvironment(data, OnSceneLoaded, (progress) =>
                {
                    Core.Environment.LoadingScreen.SetProgress(progress);
                }, null);
            });
        }
    }

    public void ToggleMenu()
    {
        if (m_AllowMenu == true)
        {
            SetState(!m_bMenuState);
        }
    }

    public void ToggleMenuOff()
    {
        SetState(false);
    }

    public void SetMouseLockVisible(bool lockMouse, bool visibleMouse)
    {
        m_MouseLock = lockMouse;
        m_MouseVisible = visibleMouse;
        MouseLockVisible(m_MouseLock, m_MouseVisible);
    }

    private void MouseLockVisible(bool lockMouse, bool visibleMouse)
    {
#if UNITY_EDITOR
        lockMouse = false;
        visibleMouse = true;
#endif
        Cursor.lockState = (lockMouse == true) ? CursorLockMode.Confined : CursorLockMode.None;
        Cursor.visible = visibleMouse;
        Debug.Log($"MouseLock: {Cursor.lockState}  visible:{Cursor.visible}");
    }

    private void SetState(bool bState)
    {
        if (m_Environments.Count == 1)
        {
            bState = false;
            Debug.Log($"m_Environments contains only {m_Environments.Count} == 1, so menu has been disabled");
        }
        else
        {
            if (bState == true)
            {
                MouseLockVisible(false, true);
            }
            else
            {
                MouseLockVisible(m_MouseLock, m_MouseVisible);
            }
        }

        m_bMenuState = bState;
        m_Menu?.SetActive(bState);

        OnEnvironmentsMenuOpened?.Invoke(bState);
    }

    public void SetInputSystemToVR(bool setToVR)
    {
        Debug.Log($"Set VR system to {setToVR}");
        UnityEngine.XR.XRSettings.enabled = setToVR;
        m_IsSystemVr = setToVR;
        VRInputModule.Instance.SetToVR(setToVR);
    }

    private bool m_IsSystemVr = false;
    public bool IsSystemVR => m_IsSystemVr;
}