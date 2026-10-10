using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

public class DigitalClock : MonoBehaviour
{
    public class CollectionData
    {
        public GameObject m_ConnectionPoint;
        public GameObject m_CreatedObject;
    }
    private List<CollectionData> m_CollectionData = new List<CollectionData>();
    private List<GameObject> m_NumberPrefabs = new List<GameObject>();
    private GameObject m_MiddleObject;

    private int m_Hour = -1;
    private int m_Minute = -1;

    private void Awake()
    {
        var numbers = this.transform.Search("Numbers");
        m_NumberPrefabs.AddRange(numbers.GetDirectChildren().Select(item => item.gameObject));
        AddItem("H1");
        AddItem("H2");
        AddItem("M1");
        AddItem("M2");

        m_MiddleObject = this.transform.Search("Colon").gameObject;
        numbers.SetActive(false);
        StartCoroutine(MiddleUpdate());
    }

    private void AddItem(string item)
    {
        CollectionData newData = new CollectionData();
        newData.m_ConnectionPoint = this.transform.Search(item).gameObject;
        var mesh = newData.m_ConnectionPoint.GetComponent<MeshRenderer>();
        if(mesh != null)
        {
            mesh.enabled = false;
        }
        m_CollectionData.Add(newData);
    }


    private IEnumerator MiddleUpdate()
    {
        while(true)
        {
            m_MiddleObject.SetActive(false);
            yield return new WaitForSeconds(0.1f);
            m_MiddleObject.SetActive(true);
            yield return new WaitForSeconds(0.4f);
        }
    }

    void Update()
    {
        DateTime time = DateTime.Now;
        if(m_Hour != time.Hour)
        {
            m_Hour = time.Hour;
            UpdateItems(m_Hour, 0, 1);
        }

        if (m_Minute != time.Minute)
        {          
            m_Minute = time.Minute;
            UpdateItems(m_Minute, 2, 3);
        }
    }


    private void UpdateItems(int time, int index1, int index2)
    {
        string result = time.ToString("00");
        int[] intArray = new int[result.Length];
        for (int i = 0; i < result.Length; i++)
        {
            intArray[i] = int.Parse($"{result[i]}");
        }
        Destroy(m_CollectionData[index1].m_CreatedObject);
        Destroy(m_CollectionData[index2].m_CreatedObject);
        m_CollectionData[index1].m_CreatedObject = Instantiate(m_NumberPrefabs[intArray[0]]);
        m_CollectionData[index1].m_CreatedObject.transform.SetParent(m_CollectionData[index1].m_ConnectionPoint.transform);
        m_CollectionData[index1].m_CreatedObject.transform.ClearLocals();

        m_CollectionData[index2].m_CreatedObject = Instantiate(m_NumberPrefabs[intArray[1]]);
        m_CollectionData[index2].m_CreatedObject.transform.SetParent(m_CollectionData[index2].m_ConnectionPoint.transform);
        m_CollectionData[index2].m_CreatedObject.transform.ClearLocals();
    }


}
