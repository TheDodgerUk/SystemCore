using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Playables;

public class PlayableDirectorController : MonoBehaviour
{
    public PlayableDirector m_PlayableDirector;
    public Action OnSequenceComplete;

    private bool m_bSequenceStarted;

    public void Evaluate()
    {
        m_PlayableDirector.DeferredEvaluate();
    }

    // Start is called before the first frame update
    void Awake()
    {
        m_PlayableDirector = GetComponent<PlayableDirector>();    
    }

    public void Play()
    {
        m_bSequenceStarted = true;
        this.enabled = true;
        m_PlayableDirector.Play();
    }

    private void Update()
    {
        if(null != m_PlayableDirector)
        {
            if(true == m_bSequenceStarted &&
                m_PlayableDirector.time >= m_PlayableDirector.duration)
            {
                m_bSequenceStarted = false;
                this.enabled = false;
                OnSequenceComplete?.Invoke();
            }
        }
    }
}
