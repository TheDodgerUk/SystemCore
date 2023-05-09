using EnvironmentHelpers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/*
[Posters]_posters1
    [Materials]
        gameObject with materail on 
        gameObject with materail on 
        gameObject with materail on 
        gameObject with materail on 

    [PosterName]_Poster2    <- name in the Dictionary
        [Poster]            <- gameobject materail it swapps it out 
    [PosterName]_Main
        [Poster] 
 */
public class PosterGroup
{
    private Dictionary<string, Material> m_Materials = new Dictionary<string, Material>();
    private Dictionary<string, Renderer> m_Renderers = new Dictionary<string, Renderer>();

    public List<string> MaterailList => m_Materials.Keys.ToList();
    public List<string> RendererList => m_Renderers.Keys.ToList();

    public void SetMaterail(string frameName, string materialName)
    {
        m_Renderers[frameName].material = m_Materials[materialName];
    }

    public void SetMaterail(string frameName, Material materialRef)
    {
        m_Renderers[frameName].material = materialRef;
    }


    public PosterGroup(Dictionary<string, Material> materailDictonary, Dictionary<string, Renderer> renderDictonary)
    {
        m_Materials = materailDictonary;
        m_Renderers = renderDictonary;
    }

    public void Randomize()
    {
        List<Material> materialList = m_Materials.Values.ToList();
        foreach (var rendererRef in m_Renderers.Keys)
        {
            SetMaterail(rendererRef, materialList.GetRandom());
        }
    }
}



public class EnvironmentPosterManager : EnvironmentEffect
{
    public const string TAG = "[Posters]";
    public static PosterGroup GetPosterManager(string sUID)
    {
        if (true == m_DictOfPosterManagers.ContainsKey(sUID))
        {
            return m_DictOfPosterManagers[sUID];
        }

        return null;
    }

    public override bool Initialise()
    {
        string posterGroupName = GetObjectName();
        if (true == string.IsNullOrEmpty(posterGroupName))
        {
            return false;
        }
        Dictionary<string, Material> materailDictonary = new Dictionary<string, Material>();
        Dictionary<string, Renderer> framesDictonary = new Dictionary<string, Renderer>();

        Transform baseMaterial = this.transform.Search("[Materials]");
        List<Transform> materailList = baseMaterial.GetDirectChildren();
        foreach (var item in materailList)
        {
            Renderer rendererRef = item.GetComponent<Renderer>();
            if (null != rendererRef)
            {
                materailDictonary.Add(rendererRef.material.name, rendererRef.material);
                item.SetActive(false);
            }
            else
            {
                Debug.LogError($"Cannot find Renderer on {item.gameObject.GetGameObjectPath()}", this);
            }
        }

        List<Transform> posterNameList = this.transform.SearchChildrenWithNameContainsList("[PosterName]");
        foreach (var posterNameRef in posterNameList)
        {
            Transform posterRef = posterNameRef.SearchChildrenWithNameContains("[Poster]");
            Renderer rendererRef = posterRef.GetComponent<Renderer>();
            if (null != rendererRef)
            {
                framesDictonary.Add(GetObjectName(posterNameRef.gameObject), rendererRef);
            }
            else
            {
                Debug.LogError($"Cannot find Renderer on {posterRef.gameObject.GetGameObjectPath()}", this);
            }
        }

        PosterGroup m_PosterGroup = new PosterGroup(materailDictonary, framesDictonary);
        m_DictOfPosterManagers[posterGroupName] = m_PosterGroup;
        m_PosterGroup.Randomize();
        return true;
    }

    private void OnDestroy() => m_DictOfPosterManagers.Remove(GetObjectName());
    public override void OnSplashReset() { }
    public override void OnSplashComplete() { }
    private static Dictionary<string, PosterGroup> m_DictOfPosterManagers = new Dictionary<string, PosterGroup>();
    public static List<string> Keys => m_DictOfPosterManagers.Keys.ToList();
}
