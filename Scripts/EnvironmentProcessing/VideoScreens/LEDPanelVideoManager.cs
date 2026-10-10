using System.Collections.Generic;
using UnityEngine;
using System;

public class LEDPanelVideoManager : VideoObjectManager
{
    protected override string MaterialName => "LEDVideoMaterial";

    private List<LEDPanelBlock> m_LEDPanelBlocks = new List<LEDPanelBlock>();

    private int m_BlockHeight = 0;
    private int m_BlockWidth = 0;

    public override bool Initialise()
    {
        bool success = base.Initialise();

        //Find all video panels below and process each set of blocks
        List<Transform> ledPanelBlocks = transform.SearchAll("LEDPanelBlock");

        for (int i = 0; i < ledPanelBlocks.Count; i++)
        {
            List<Transform> children = ledPanelBlocks[i].GetAllChildren();

            GetBounds(children, 0.99f, out m_BlockWidth, out m_BlockHeight);

            //Create Panel Block
            LEDPanelBlock block = ledPanelBlocks[i].AddComponent<LEDPanelBlock>();

            block.Init(m_BlockWidth, m_BlockHeight, m_VideoMaterial);
            m_LEDPanelBlocks.Add(block);            
        }

        return success;
    }

    public override void SetLayer(int layer)
    {
        foreach (var block in m_LEDPanelBlocks)
        {
            block.SetLayer(layer);
        }
    }

    protected override void ApplyEmission(float emission)
    {
        foreach (var block in m_LEDPanelBlocks)
        {
            block.UpdateEmission(emission);
        }
    }

    private void GetBounds(List<Transform> children, float threshold, out int iWidth, out int iHeight)
    {
        Vector3 localpos = children[0].localPosition;
        float minX = localpos.x;
        float maxX = localpos.x;
        float minY = localpos.y;
        float maxY = localpos.y;
        iWidth = 1;
        iHeight = 1;

        for (int i = 1; i < children.Count; i++)
        {
            localpos = children[i].localPosition;
            if (minX > localpos.x && Mathf.Abs(minX - localpos.x) > threshold)
            {
                minX = localpos.x;
                iWidth++;
            }
            else if (maxX < localpos.x && Mathf.Abs(maxX - localpos.x) > threshold)
            {
                maxX = localpos.x;
                iWidth++;
            }

            if (minY > localpos.y && Mathf.Abs(minY - localpos.y) > threshold)
            {
                minY = localpos.y;
                iHeight++;
            }
            else if (maxY < localpos.y && Mathf.Abs(localpos.y - maxY) > threshold)
            {
                maxY = localpos.y;
                iHeight++;
            }
        }
    }
}