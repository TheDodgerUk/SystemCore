using System.Collections.Generic;
using UnityEngine;

public class EnvironmentEffects : MonoBehaviour
{
    public static EnvironmentEffects Instance;
    private Dictionary<string, GameObject> m_SceneEffects = new Dictionary<string, GameObject>();

    public void Init()
    {
        Instance = this;
        List<Transform> children = transform.GetDirectChildren();

        foreach(var child in children)
        {
            //Assumes unique naming and simple particle system per instance for now
            m_SceneEffects.Add(child.name, child.gameObject);
            child.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// The returned particle is a reference to and effect and should be instantiate and duplicated as needed
    /// </summary>
    /// <param name="sEffectName"></param>
    /// <returns></returns>
    public GameObject GetEffect(string sEffectName)
    {
        if(true == m_SceneEffects.ContainsKey(sEffectName))
        {
            return m_SceneEffects[sEffectName];
        }

        return null;
    }

    private void OnDestroy()
    {
        Instance = null;
    }
}