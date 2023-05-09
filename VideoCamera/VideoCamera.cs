using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VideoCamera : MonoBehaviour
{

    List<Transform> m_CameraPositions = new List<Transform>();
    private int m_TransformIndex;
    private Camera m_Camera;
    // Start is called before the first frame update
    void Start()
    {
        m_Camera = this.GetComponentInChildren<Camera>();
        m_CameraPositions = this.GetComponentsInChildren<Transform>().ToList();
        var cameraTransform = m_CameraPositions.Find(e => e.transform == m_Camera.transform);
        m_CameraPositions.Remove(cameraTransform);
        m_Camera.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            if(m_Camera.isActiveAndEnabled == false)
            {
                m_Camera.SetActive(true);
            }
            else 
            {
                m_TransformIndex = m_TransformIndex.WrapIncrement(m_CameraPositions);
                m_Camera.transform.SetParent(m_CameraPositions[m_TransformIndex]);
                m_Camera.transform.ClearLocals();
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (m_Camera.isActiveAndEnabled == false)
            {
                m_Camera.SetActive(true);
            }
            else
            {
                m_TransformIndex = m_TransformIndex.WrapDecrement(m_CameraPositions);
                m_Camera.transform.SetParent(m_CameraPositions[m_TransformIndex]);
                m_Camera.transform.ClearLocals();
            }
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            m_Camera.SetActive(false);
        }
    }
}
