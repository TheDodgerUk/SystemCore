using System.Collections.Generic;
using UnityEngine;

public class PanelMaterialData
{
    public Material Material;
    public string Name;
}

public class LEDPanel : MonoBehaviour
{
    private Dictionary<string, PanelMaterialData> m_DictOfLEDEffects = new Dictionary<string, PanelMaterialData>();

    public const string EMISSIVE = "_Emission";
    [SerializeField]
    private PanelMaterialData m_MaterialData;
    private float m_fPixelDensityPerMetre = 50f;
    [SerializeField]
    private float m_fOriginalEmissiveValue;
    private Vector4 m_PixelResolution;
    private Renderer[] m_Renderers;
    private float m_fChangeTimer = 0f;
    private const float m_fMaxTimeChange = 0.1f;
    public float m_fCurrentValue = 0f;
    public float m_fLastValue = 0f;

    public Material CurrentMaterial => m_MaterialData.Material;
    public float CurrentChangeTime
    {
        get { return m_fChangeTimer; }
        set { m_fChangeTimer = value; }
    }

    private List<string> m_KeysList = new List<string>();
    private Dictionary<string, LEDEffect> m_UpdateActions = new Dictionary<string, LEDEffect>()
    {
        { "LightingPanel07", new LEDWaveEffect() },
        { "LightingPanel08", new LEDBarEffect() },
        { "LEDAudio01", new LEDMonitorEffect() },
        { "LEDAudio02", new LEDMonitorEffect() },
        { "LEDAudio05", new LEDMonitorEffect() },
        { "LEDAudio06", new LEDMonitorEffect() }
    };    

    // Use this for initialization
    public void Init()
    {
        m_KeysList = new List<string>(m_UpdateActions.Keys);
        CreateEffectProcessors();
        CreateMaterials();
        m_MaterialData = GetRandomMaterial();
        m_Renderers = transform.GetComponentsInChildren<Renderer>();

        for (int i = 0; i < m_Renderers.Length; i++)
        {
            float pixelsX = m_Renderers[i].transform.lossyScale.x * m_fPixelDensityPerMetre;
            float pixelsY = m_Renderers[i].transform.lossyScale.y * m_fPixelDensityPerMetre;
            m_PixelResolution = new Vector4(pixelsX, pixelsY);
            m_Renderers[i].material = m_MaterialData.Material;
        }

        InvokeRepeating("ChangeMaterial", 0.0f, 30f);
	}

    private void Update()
    {
        if(true == m_UpdateActions.ContainsKey(m_MaterialData.Name))
        {
            m_UpdateActions[m_MaterialData.Name]?.UpdateEffect(this);
        }
    }

    private void CreateEffectProcessors()
    {
        foreach(KeyValuePair<string, LEDEffect> kvp in m_UpdateActions)
        {
            kvp.Value.Init();
        }
    }

    private void ChangeMaterial()
    {
       // Debug.Log("Changing material");
        SetCurrentMaterial(GetRandomMaterial());
    }

    private void SetCurrentMaterial(PanelMaterialData material)
    {
        m_MaterialData = material;
        m_MaterialData.Material.SetVector("_Tiling", m_PixelResolution);

        if(null != m_Renderers)
        {
            for(int i = 0; i < m_Renderers.Length; i++)
            {
                m_Renderers[i].material = m_MaterialData.Material;
            }
        }
    }

    public PanelMaterialData GetRandomMaterial()
    {
        int iRandom = UnityEngine.Random.Range(0, m_KeysList.Count);
        return GetMaterial(m_KeysList[iRandom]);
    }

    private void CreateMaterials()
    {
        for(int i = 0; i < m_KeysList.Count; i++)
        {
            GetMaterial(m_KeysList[i]);
        }
    }

    private PanelMaterialData GetMaterial(string sEffect)
    {
        if(false == m_DictOfLEDEffects.ContainsKey(sEffect))
        {
            Material mat = new Material(Resources.Load<Material>(sEffect)) as Material;
            PanelMaterialData data = new PanelMaterialData();
            data.Material = mat;
            data.Name = data.Material.name;
            m_DictOfLEDEffects.Add(sEffect, data);
        }

        return m_DictOfLEDEffects[sEffect];
    }
}