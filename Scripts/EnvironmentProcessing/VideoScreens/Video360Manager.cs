using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Video;

public class Video360Manager : VideoObjectManager
{
    public const string TAG = "[360Video]";
    protected override string MaterialName => "LEDVideoMaterial";

    private List<LEDPanelBlock> m_LEDPanelBlocks = new List<LEDPanelBlock>();

    private int m_BlockHeight = 0;
    private int m_BlockWidth = 0;

    protected override void ApplyEmission(float emission)
    {
        
    }
    public override bool Initialise()
    {
        bool success = base.Initialise();
        if(false == success)
        {
            return false;
        }

        string videoName = EnvironmentEffect.GetObjectNameExtensionArguments(this.gameObject, 0);

        MeshRenderer MeshRendererRef = this.GetComponentInChildren<MeshRenderer>();
        m_VideoMaterial.SetTexture("_MainTex", m_VideoTexture);
        MeshRendererRef.material = m_VideoMaterial;

        m_VideoPlayer.audioOutputMode =  VideoAudioOutputMode.None;
        VideoAudioOutputMode mode = m_VideoPlayer.audioOutputMode;
        if (true == Enum.TryParse(EnvironmentEffect.GetObjectNameExtensionArguments(this.gameObject, 1), out mode))
        {
            m_VideoPlayer.audioOutputMode = mode;
        }

        this.WaitFor(1f, () =>
        {
            if (false == string.IsNullOrEmpty(videoName))
            {
                Play(videoName);
            }
            else
            {
                Debug.LogWarning($"{ScreenName} does not have a default video");
            }
        });


        return success;
    }


}