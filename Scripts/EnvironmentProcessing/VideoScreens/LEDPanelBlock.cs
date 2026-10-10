using UnityEngine;

public class LEDPanelBlock : MonoBehaviour
{
    private const string EMISSIVE = "_Emissive";

    private MeshRenderer[] m_MeshRenderer;
    private Material m_Material;
    private float m_OriginalEmissiveValue;

    MaterialPropertyBlock props;
    public void Init(int width, int height, Material videoMaterial)
    {
        props = new MaterialPropertyBlock();
        m_MeshRenderer = gameObject.GetComponentsInChildren<MeshRenderer>();
        m_Material = videoMaterial;
        SetMaterialProperties(width, height);

        if (null != m_Material)
        {
            m_OriginalEmissiveValue = m_Material.GetFloat(EMISSIVE);
            UpdateEmission(1f);
        }
    }

    public void SetLayer(int layer)
    {
        foreach (var renderer in m_MeshRenderer)
        {
            renderer.gameObject.layer = layer;
        }
    }

    public void UpdateEmission(float emission)
    {
        emission *= m_OriginalEmissiveValue;

        foreach (var renderer in m_MeshRenderer)
        {
            renderer.GetPropertyBlock(props);
            props.SetFloat(EMISSIVE, emission);
            renderer.SetPropertyBlock(props);
        }
    }

    private void SetMaterialProperties(int iWidth, int iHeight)
    {
        for (int i = 0; i < m_MeshRenderer.Length; i++)
        {
            MeshRenderer renderer = m_MeshRenderer[i];
            renderer.gameObject.layer = Layers.DefaultLayer;
            int iScreenNumber;
            int.TryParse(renderer.gameObject.name, out iScreenNumber);
            renderer.material = m_Material;

            renderer.GetPropertyBlock(props);
            props.SetFloat("_Width", iWidth);
            props.SetFloat("_Height", iHeight);
            props.SetFloat("_Tile", iScreenNumber);

            renderer.SetPropertyBlock(props);
        }
    }
}
