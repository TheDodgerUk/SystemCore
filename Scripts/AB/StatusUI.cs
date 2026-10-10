using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class StatusUI : ScreenUI
{
    private string m_TextureName = "_StatusTex";
    private GameObject m_StatusObject;

    [SerializeField]
    private Vector2 OffsetUV = new Vector2(0f, 0f);

    // Use this for initialization
    public override void Init(Material mat)
    {
        base.Init(mat);

        CreateFace();
        SetupScreen(mat);
    }

    private void SetupScreen(Material faceMat)
    {
        faceMat.SetTexture(m_TextureName, m_RenderTexture);
        faceMat.SetTextureScale(m_TextureName, new Vector2(1f, 1f));
        faceMat.SetTextureOffset(m_TextureName, OffsetUV);
    }

    private void CreateFace()
    {
        m_StatusObject = m_CameraTransform.GetChild(0).gameObject;
        SetLayerRecursively(m_StatusObject, m_LayerMask);
        m_StatusObject.transform.localPosition = Vector3.forward * 0.2f;
    }
}
