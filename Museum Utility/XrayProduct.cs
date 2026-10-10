using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class XrayProduct : MonoBehaviour
{
    //[SerializeField]
    //private string m_sProductCode;

    //public ArFeaturesMetaData m_ArFeaturesMetaData { get; private set; }

    // Stuff used for X-Ray mode.
    private int m_iXrayCount = 0;
    [SerializeField]
    public Renderer[] m_XrayRenderers;
    private Material[][] m_XrayStandardMaterials;
    private Material[][] m_XrayHoloMaterials;
    private Coroutine m_XrayCoroutine;
    private static float m_fXrayTime = 1.0f;
    private const string HOLOGRAM_VALUE = "_HologramValue";
    private float m_fXrayValue = 0f;

    private bool m_IsXray = false;

    public void Initialise()
    {
        //if (true == string.IsNullOrEmpty(m_sProductCode))
        //{
        //    Debug.LogError("Please enter a product code for the product you want to xray!");
        //    return;
        //}

        //// Find the AR Features Meta Data.
        //List<CatalogueEntry> catalogue = Core.Catalogue.GetCatalogue;
        //catalogue = catalogue.Where(x => x.ReferenceNumber == m_sProductCode).ToList();

        //if (catalogue.Count > 0)
        //{
        //    m_ArFeaturesMetaData = catalogue[0].GetMetaData<ArFeaturesMetaData>();
        //}
        //else
        //{
        //    Debug.LogError("Could not find product code in catalogue.");
        //    return;
        //}

        //if (null == m_ArFeaturesMetaData)
        //{
        //    Debug.LogError("No ARFeaturesMetaData on product.");
        //    return;
        //}

        // Find the X-Ray objects.
        //m_iXrayCount = m_ArFeaturesMetaData.m_XrayObjectNames.Count;
        //m_XrayRenderers = new Renderer[m_iXrayCount];

        m_iXrayCount = m_XrayRenderers.Length;

        m_XrayStandardMaterials = new Material[m_iXrayCount][];
        m_XrayHoloMaterials = new Material[m_iXrayCount][];
        Texture emissionTex;
        int iMaterials;

        for (int i = 0; i < m_iXrayCount; i++)
        {
            //m_XrayRenderers[i] = this.transform.Find(m_ArFeaturesMetaData.m_XrayObjectNames[i])?.GetComponent<Renderer>();

            if (null != m_XrayRenderers[i])
            {
                // Setup the arrays.
                iMaterials = m_XrayRenderers[i].materials.Length;
                m_XrayHoloMaterials[i] = new Material[iMaterials];
                m_XrayStandardMaterials[i] = m_XrayRenderers[i].materials;

                for (int m = 0; m < iMaterials; m++)
                {
                    // Setup the xray materials.
                    m_XrayHoloMaterials[i][m] = Instantiate(Resources.Load("X-Ray/X-Ray Material")) as Material;
                    m_XrayHoloMaterials[i][m].SetTexture("_Albedo", m_XrayStandardMaterials[i][m].GetTexture("_MainTex"));
                    m_XrayHoloMaterials[i][m].SetTexture("_Normal", m_XrayStandardMaterials[i][m].GetTexture("_BumpMap"));
                    m_XrayHoloMaterials[i][m].SetTexture("_Metallic", m_XrayStandardMaterials[i][m].GetTexture("_MetallicGlossMap"));
                    m_XrayRenderers[i].materials[m].SetFloat(HOLOGRAM_VALUE, 0f);

                    emissionTex = m_XrayStandardMaterials[i][m].GetTexture("_EmissionMap");
                    if (null != emissionTex)
                    {
                        m_XrayHoloMaterials[i][m].SetTexture("_Emission", emissionTex);
                        m_XrayHoloMaterials[i][m].SetColor("_EmissionColor", m_XrayStandardMaterials[i][m].GetColor("_EmissionColor"));
                    }
                }
            }
            //else
            //{
            //    Debug.LogError($"ProductSetup X-Ray could not find material for {m_ArFeaturesMetaData.m_XrayObjectNames[i]}", this);
            //}
        }
    }

    public void ToggleXray()
    {
        if (null != m_XrayCoroutine)
        {
            StopCoroutine(m_XrayCoroutine);
        }
        m_IsXray = !m_IsXray;
        m_XrayCoroutine = StartCoroutine(XrayBehaviour(m_IsXray));
    }

   
    [EditorButton]
    public void XrayOn()
    {
        if (null != m_XrayCoroutine)
        {
            StopCoroutine(m_XrayCoroutine);
        }

        m_XrayCoroutine = StartCoroutine(XrayBehaviour(true));
    }

    [EditorButton]
    public void XrayOff()
    {
        if (null != m_XrayCoroutine)
        {
            StopCoroutine(m_XrayCoroutine);
        }

        m_XrayCoroutine = StartCoroutine(XrayBehaviour(false));
    }


    /// <summary>
    /// This fades the X-Ray in and out.
    /// </summary>
    private IEnumerator XrayBehaviour(bool bIsXrayOn)
    {
        // Keep track of the value used to fade the X-Ray.
        Debug.LogError($"Start m_fXrayValue {m_fXrayValue} ");
        // If turning on x-ray, set the materials to their x-ray materials.

        for (int i = 0; i < m_iXrayCount; i++)
        {
            m_XrayRenderers[i].materials = m_XrayHoloMaterials[i];
        }


        while (((true == bIsXrayOn) && (m_fXrayValue < 1f)) || ((false == bIsXrayOn) && (m_fXrayValue > 0f)))
        {
            if(bIsXrayOn == true)
            {
                m_fXrayValue += (Time.deltaTime / m_fXrayTime);
            }
            else
            {
                m_fXrayValue -= (Time.deltaTime / m_fXrayTime);
            }
            m_fXrayValue = Mathf.Clamp01(m_fXrayValue);
            for (int i = 0; i < m_iXrayCount; i++)
            {
                for (int m = 0; m < m_XrayRenderers[i].materials.Length; m++)
                {
                    m_XrayRenderers[i].materials[m].SetFloat(HOLOGRAM_VALUE, m_fXrayValue);
                }
            }
            yield return null;
        }

        // If turning off x-ray, set the materials back to their standard ones.
        if (false == bIsXrayOn)
        {
            for (int i = 0; i < m_iXrayCount; i++)
            {
                m_XrayRenderers[i].materials = m_XrayStandardMaterials[i];
            }
        }
        m_fXrayValue = Mathf.Clamp01(m_fXrayValue);
        Debug.LogError($"end m_fXrayValue {m_fXrayValue} ");
        m_XrayCoroutine = null;
    }
}
