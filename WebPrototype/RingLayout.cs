using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RingLayout : MonoBehaviour
{
    [Header("Ring Parameters")]
    [SerializeField]
    private float m_RingHeight = 0f;
    [SerializeField]
    private float m_MinRadius = 0f;
    [SerializeField]
    private float m_Radius = 0.15f;
    [SerializeField, Range(1, 360)]
    private float m_AngleSize = 360f;
    [SerializeField, Range(0, 360)]
    private float m_AngleOffset = 90f;

    [Space]
    [SerializeField]
    private int m_RingSize = 5;
    [SerializeField]
    private float m_RingDepthScale = 0.1f;
    [SerializeField]
    private float m_RingDepthPower = 2f;
    [SerializeField]
    private float m_AnimateOutDepth = 0.5f;

    [Header("Home")]
    [SerializeField]
    private EaseType m_ScaleHome = EaseType.SineIn;
    [SerializeField]
    private EaseType m_MoveHome = EaseType.BounceOut;
    [SerializeField]
    private float m_TimeHome = 1f;
    [SerializeField]
    private float m_HomeHeight = 0.5f;
    [SerializeField]
    private float m_HomeScale = 0.5f;

    [Header("Move In")]
    [SerializeField]
    private EaseType m_ScaleIn = EaseType.SineIn;
    [SerializeField]
    private EaseType m_MoveIn = EaseType.BounceOut;
    [SerializeField]
    private float m_TimeIn = 1f;
    [SerializeField]
    private float m_DelayIn = 0.015f;

    [Header("Move Away")]
    [SerializeField]
    private EaseType m_MoveOut = EaseType.SineIn;
    [SerializeField]
    private EaseType m_ScaleOut = EaseType.SineOut;
    [SerializeField]
    private float m_TimeOut = 1f;

    public void AnimateHome(WebEntry entry)
    {
        entry.Transform.Create<ScaleTween>(m_TimeHome, m_ScaleHome).Initialise(m_HomeScale);
        entry.Transform.Create<MoveTween>(m_TimeHome, m_MoveHome).Initialise(0, m_HomeHeight, 0, MoveType.Local);
    }

    public void AnimateToStart(List<WebEntry> entries)
    {
        int n = entries.Count;
        for (int i = 0; i < entries.Count; ++i)
        {
            AnimateToStart(Vector3.zero, GetPosition(i, n), entries[i], i, n);
        }
    }

    public void AnimateToStart(WebEntry selected, List<WebEntry> entries, List<WebItem> items, bool organiseByWeight)
    {
        var startPos = selected.Transform.localPosition;
        var groups = items.GroupBy(i => i.Weight).ToDictionary(g => g.Key, g => g.ToList());
        for (int i = 0; i < entries.Count; ++i)
        {
            var position = GetPosition(i, groups, items, organiseByWeight);
            AnimateToStart(startPos, position, entries[i], i, items.Count);
        }
    }

    public void AnimateAway(List<WebEntry> entries, Action<WebEntry> onComplete)
    {
        entries.ForEach(e => AnimateAway(e, () => onComplete(e)));
    }

    public void Position(Transform target, int i, int n) => target.localPosition = GetPosition(i, n);

    private void AnimateToStart(Vector3 start, Vector3 destination, WebEntry entry, int i, int n)
    {
        float activationTime = (n - i) * m_DelayIn;
        entry.Transform.StopAllTweens();
        entry.Transform.localScale = Vector3.zero;
        entry.Transform.localPosition = start;
        entry.WaitFor(activationTime, () =>
        {
            entry.Transform.Create<ScaleTween>(m_TimeIn * 0.5f, m_ScaleIn).Initialise(1f);
            entry.Transform.Create<MoveTween>(m_TimeIn, m_MoveIn).Initialise(destination, MoveType.Local);
        });
    }

    private void AnimateAway(WebEntry entry, Action callback)
    {
        var task = new TaskAction(2, callback);
        entry.Transform.StopAllTweens();
        entry.Transform.Create<MoveTween>(m_TimeOut, m_MoveOut, task.Increment).Initialise(0, 0, m_AnimateOutDepth, MoveType.Local);
        entry.Transform.Create<ScaleTween>(m_TimeOut, m_ScaleOut, task.Increment).Initialise(Vector3.zero);
    }

    private Vector3 GetPosition(int i, Dictionary<int, List<WebItem>> groups, List<WebItem> items, bool organiseByWeight)
    {
        if (organiseByWeight == false)
        {
            return GetPosition(i, items.Count);
        }
        else
        {
            var item = items[i];
            var group = groups[item.Weight];
            i = group.IndexOf(item);
            int ring = (groups.Count - item.Weight) + 1;
            return GetPosition(i, ring, group.Count);
        }
    }

    private Vector3 GetPosition(int i, int n)
    {
        int r = GetRing(i);
        int c = GetRingCount(r);
        int max = GetRing(n - 1);
        if (r == max) { c = n; }
        int i0 = i;
        for (int x = r - 1; x > 0; --x)
        {
            int count = GetRingCount(x);
            if (r == max) { c -= count; }
            i0 -= count;
        }
        return GetPosition(i0, r, c);
    }

    private Vector3 GetPosition(int i, int ring, float countOnRing)
    {
        if (m_AngleSize != 360 && countOnRing > 1)
        {
            countOnRing--;
        }

        float offset = 90f + ((360f - m_AngleSize) * 0.5f) + (m_AngleOffset);
        float k = m_AngleSize * (i / countOnRing) + offset;
        float r = m_MinRadius + (m_Radius * ring);
        float x = r * Mathf.Cos(k.ToRadians());
        float y = r * Mathf.Sin(k.ToRadians());
        float z = (ring.Pow(m_RingDepthPower) - 1) * m_RingDepthScale;
        return new Vector3(x, y + m_RingHeight, z);
    }

    private int GetRingCount(int r) => r * m_RingSize;

    private int GetRing(int i) => 1 + ((Mathf.Sqrt(1 + (i / m_RingSize) * 8) - 1) / 2).Floor();
}
