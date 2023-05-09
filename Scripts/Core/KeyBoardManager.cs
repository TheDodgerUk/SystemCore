using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if VR_INTERACTION
public class KeyboardManager
{
    private VRKeys.Keyboard m_Keyboard;

    private bool m_Initialised = false;
    private IEnumerator m_Coroutine;

    public void CreateKeyBoard(Action callback, bool initialToggle)
    {
        var keys = Resources.Load("VRKeys");
        var inst = (GameObject)GameObject.Instantiate(keys);
        m_Keyboard = inst.GetComponent<VRKeys.Keyboard>();
        m_Keyboard.CreateKeyboard(() =>
        {
            m_Keyboard.Show(initialToggle);
            m_Initialised = true;
           callback?.Invoke();
        });
    }

    public void DestroyKeyboard()
    {
        if (m_Keyboard != null && m_Keyboard.gameObject != null)
        {
            GameObject.Destroy(m_Keyboard.gameObject);
        }
    }

    public void SetParent(GameObject parent)
    {
        m_Keyboard.transform.SetParent(parent.transform);
    }

    public void SetTransform(Vector3 position, Quaternion rotation)
    {
        m_Keyboard.transform.position = position;
        m_Keyboard.transform.rotation = rotation;
    }

    public void Show(bool toggle)
    {     
        if(m_Coroutine != null)
        {
            Core.Mono.StopCoroutine(m_Coroutine);
            m_Coroutine = null;
        }
        m_Coroutine = ToggleShow(toggle);
        Core.Mono.StartCoroutine(m_Coroutine);
    }

    private IEnumerator ToggleShow(bool toggle)
    {
        yield return new WaitUntil(() => m_Initialised == true);
        m_Keyboard.Show(toggle);
    }

    public void SetStringCallback(Action<string> callback)
    {
        m_Keyboard.SetStringCallback(callback);
    }

    public void SetStringCallback(string startString, Action<string> callback)
    {
        m_Keyboard.SetText(startString);
        m_Keyboard.SetStringCallback(callback);
    }

    public void SetStringCallbackAndClear(Action<string> callback)
    {
        m_Keyboard.SetText("");
        m_Keyboard.SetStringCallback(callback);
    }
}
#endif