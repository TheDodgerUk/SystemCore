using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System;

public class UIVideoScreen : MonoBehaviour
{
    private Action VideoEndedCallback;
    private RawImage videoImage = null;
    private VideoPlayer m_VideoPlayer;
    private RenderTexture m_VideoTexture;

    public void Initialise()
    {
        videoImage = transform.SearchComponent<RawImage>("VideoImage");

        CreateTexture(new Vector2(512, 512));
        SetupVideoPlayer();
        videoImage.texture = m_VideoTexture;
    }

    private void CreateTexture(Vector2 res)
    {
        m_VideoTexture = new RenderTexture((int)res.x, (int)res.y, 0, RenderTextureFormat.ARGB32)
        {
            filterMode = FilterMode.Trilinear,
            wrapMode = TextureWrapMode.Clamp,
            autoGenerateMips = true,
            useMipMap = true,
            name = gameObject.name,
        };
    }

    private void SetupVideoPlayer()
    {
        m_VideoPlayer = gameObject.ForceComponent<VideoPlayer>();
        m_VideoPlayer.audioOutputMode = VideoAudioOutputMode.None;
        m_VideoPlayer.renderMode = VideoRenderMode.RenderTexture;
        m_VideoPlayer.aspectRatio = VideoAspectRatio.Stretch;
        m_VideoPlayer.source = VideoSource.Url;

        m_VideoPlayer.prepareCompleted += OnVideoPrepared;
        m_VideoPlayer.loopPointReached += OnVideoEnded;
        m_VideoPlayer.targetTexture = m_VideoTexture;
    }

    private void SetVideoClip(string url)
    {
        if (false == string.IsNullOrEmpty(url))
        {
            m_VideoPlayer.url = url;
            m_VideoPlayer.source = VideoSource.Url;
            m_VideoPlayer.Prepare();
        }
    }

    public void Play(string sFileName, Action callbackOnEnd = null)
    {
        VideoEndedCallback = callbackOnEnd;
        SetVideoClip(VideoLoader.Instance.GetVideo(sFileName));
    }

    public void Play(FakeDictionaryStrings.FakeDictionaryItem fakeDictonaryItemRef, Action callbackOnEnd = null)
    {
        VideoEndedCallback = callbackOnEnd;
        SetVideoClip(VideoLoader.Instance.GetVideo(fakeDictonaryItemRef.m_Value));
    }

    public void Stop()
    {
        m_VideoPlayer.Stop();
    }

    protected virtual void OnVideoPrepared(VideoPlayer player)
    {
        m_VideoPlayer.Play();
    }

    protected virtual void OnVideoEnded(VideoPlayer player)
    {
        if (null != VideoEndedCallback)
        {
            VideoEndedCallback?.Invoke();
        }
        else
        {
            m_VideoPlayer.Play();
        }

    }
}
