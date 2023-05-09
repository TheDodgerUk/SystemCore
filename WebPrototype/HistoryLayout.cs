using System;
using System.Collections.Generic;
using UnityEngine;

public class HistoryLayout : MonoBehaviour
{
    [Header("Move to Centre")]
    [SerializeField]
    private EaseType m_ScaleToCentre = EaseType.BounceOut;
    [SerializeField]
    private EaseType m_MoveToCentre = EaseType.BounceOut;
    [SerializeField]
    private float m_TimeToCentre = 1f;
    [SerializeField]
    private float m_CentreScale = 1.2f;

    [Header("Move Along Line")]
    [SerializeField]
    private EaseType m_ScaleInLine = EaseType.BounceOut;
    [SerializeField]
    private EaseType m_MoveInLine = EaseType.BounceOut;
    [SerializeField]
    private float m_TimeInLine = 1f;
    [SerializeField]
    private float m_SeperationDepth = 0.4f;
    [SerializeField]
    private float m_DepthInLine = 0.4f;
    [SerializeField]
    private float m_HeightInLinePower = 2f;
    [SerializeField]
    private float m_HeightInLineScale = 0.1f;
    [SerializeField]
    private float m_LeanInLine = 0.2f;

    [Header("Move Away")]
    [SerializeField]
    private EaseType m_ScaleAway = EaseType.SineOut;
    [SerializeField]
    private EaseType m_MoveAway = EaseType.ExpoOut;
    [SerializeField]
    private float m_TimeAway = 0.5f;
    [SerializeField]
    private float m_HeightAway = 1f;

    public void AnimateLine(List<WebEntry> entries)
    {
        for (int i = 1; i < entries.Count; ++i)
        {
            AnimateInLine(entries[i], i);
        }
    }

    public void AddToHistory(WebEntry entry)
    {
        entry.Transform.StopAllTweens();
        entry.Transform.Create<ScaleTween>(m_TimeToCentre, m_ScaleToCentre).Initialise(m_CentreScale);
        entry.Transform.Create<MoveTween>(m_TimeToCentre, m_MoveToCentre).Initialise(Vector3.zero, MoveType.Local);
    }

    public void AnimateAway(WebEntry entry, Action<WebEntry> callback)
    {
        var task = new TaskAction(2, () => callback(entry));
        float y = entry.transform.localPosition.y - m_HeightAway;
        entry.Transform.StopAllTweens();
        entry.Transform.Create<ScaleTween>(m_TimeAway, m_ScaleAway, task.Increment).Initialise(0f);
        entry.Transform.Create<MoveTween>(m_TimeAway, m_MoveAway, task.Increment).Initialise(y: y, type: MoveType.Local);
    }

    public void Position(Transform target, int i, int count) => target.localPosition = GetPosition(i);

    private void AnimateInLine(WebEntry entry, int i)
    {
        entry.Transform.StopAllTweens();
        entry.Transform.Create<ScaleTween>(m_TimeInLine, m_ScaleInLine).Initialise(1f);
        entry.Transform.Create<MoveTween>(m_TimeInLine, m_MoveInLine).Initialise(GetPosition(i), type: MoveType.Local);
    }

    private Vector3 GetPosition(int i)
    {
        float lean = i * m_LeanInLine;
        float depth = i * m_DepthInLine + m_SeperationDepth;
        float height = i.Pow(m_HeightInLinePower) * m_HeightInLineScale;
        return new Vector3(lean, height, depth);
    }
}
