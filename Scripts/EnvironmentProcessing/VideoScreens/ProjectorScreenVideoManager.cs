using System.Collections.Generic;
using UnityEngine;
using System;

public class ProjectorScreenVideoManager : VideoObjectManager
{
    private const string EMISSIVE = "_Emissive";

    protected override string MaterialName => "ProjectorVideoMaterial";

    private float m_OriginalEmissiveValue;
    private List<Renderer> m_Renderers = new List<Renderer>();

    public override bool Initialise()
    {
        bool success = base.Initialise();

        m_OriginalEmissiveValue = m_VideoMaterial.GetFloat(EMISSIVE);

        List<Transform> screens = transform.SearchAll("screen");
        screens.ForEach((e) =>
        {
            Renderer renderer = e.GetComponent<Renderer>();
            if(null != renderer)
            {
                m_Renderers.Add(renderer);
            }
        });

        foreach (var renderer in m_Renderers)
        {
            renderer.material = m_VideoMaterial;
        }
        this.enabled = false;
        return success;
    }

    protected override void ApplyEmission(float emission)
    {
        m_VideoMaterial.SetFloat(EMISSIVE, emission * m_OriginalEmissiveValue);
    }

    public override void SetLayer(int layer)
    {
        foreach (var renderer in m_Renderers)
        {
            renderer.gameObject.layer = layer;
        }
    }
}
