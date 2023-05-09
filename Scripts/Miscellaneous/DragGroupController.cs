using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class DragGroupController : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public enum DragType
    {
        Vertical,
        Horizontal,
    }
    private RectTransform m_CurrentTransform;
    private GameObject m_MainContent;
    private Vector3 m_CurrentPosition;
    private int m_OldIndex = 0;

    private int m_TotalChild;
    private int m_MinIndex = 0;

    private Action<int, int> m_CallbackOldIndexNewIndex;
    private DragType m_DragType = DragType.Vertical;

    public void Initilise(DragType dragType, GameObject moveRoot, Action<int, int> oldIndexNewIndex)
    {
        m_DragType = dragType;
        m_CallbackOldIndexNewIndex = oldIndexNewIndex;
        m_CurrentTransform = moveRoot.GetComponent<RectTransform>();
    }

    public void Initilise(DragType dragType, GameObject moveRoot, Action<int, int> oldIndexNewIndex, int minIndex = 0)
    {
        m_DragType = dragType;
        m_CallbackOldIndexNewIndex = oldIndexNewIndex;
        m_CurrentTransform = moveRoot.GetComponent<RectTransform>();
        m_MinIndex = minIndex;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        m_CurrentPosition = m_CurrentTransform.position;
        m_MainContent = m_CurrentTransform.parent.gameObject;
        m_TotalChild = m_MainContent.transform.childCount;
        m_OldIndex = m_CurrentTransform.GetSiblingIndex(); // record
    }

    public void OnDrag(PointerEventData eventData)
    {
        m_CurrentTransform.position = new Vector3(m_CurrentTransform.position.x, eventData.position.y, m_CurrentTransform.position.z);

        for (int i = 0; i < m_TotalChild; i++)
        {
            if (i != m_CurrentTransform.GetSiblingIndex())
            {
                Transform otherTransform = m_MainContent.transform.GetChild(i);
                Vector3 otherPosition = new Vector3(m_CurrentTransform.position.x, otherTransform.position.y, m_CurrentTransform.position.z);

                if(m_DragType == DragType.Horizontal)
                {
                    otherPosition = new Vector3(otherTransform.position.x, m_CurrentTransform.position.y, m_CurrentTransform.position.z);
                }

                int distance = (int)Vector3.Distance(m_CurrentTransform.position, otherPosition);

                if (distance <= 10)
                {
                    Vector3 otherTransformOldPosition = otherTransform.position;
                    
                    otherTransform.position = new Vector3(otherTransform.position.x, m_CurrentPosition.y, otherTransform.position.z);
                    
                    m_CurrentTransform.position = new Vector3(m_CurrentTransform.position.x, otherTransformOldPosition.y, m_CurrentTransform.position.z);
                    
                    m_CurrentTransform.SetSiblingIndex(otherTransform.GetSiblingIndex());
                    m_CurrentPosition = m_CurrentTransform.position;
                }
            }
            
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        m_CurrentTransform.position = m_CurrentPosition;
        for (int i = 0; i < m_TotalChild; i++)
        {
            if (i == m_CurrentTransform.GetSiblingIndex())
            {
                if (i >= m_MinIndex)
                {
                    m_CallbackOldIndexNewIndex?.Invoke(m_OldIndex, i);
                }
                else
                {
                    Transform otherTransform = m_MainContent.transform.GetChild(m_MinIndex);
                    m_CurrentTransform.SetSiblingIndex(otherTransform.GetSiblingIndex());
                    m_CurrentPosition = m_CurrentTransform.position;
                    m_CallbackOldIndexNewIndex?.Invoke(m_OldIndex, m_MinIndex);
                }
            }
        }
    }
}