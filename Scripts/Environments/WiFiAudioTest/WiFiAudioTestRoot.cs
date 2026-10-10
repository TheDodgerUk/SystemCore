using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WiFiAudioTest
{
    public class WiFiAudioTestRoot : MonoBehaviour
    {
#if Photon
        private float[] m_Samples = new float[WIFIServer.AUDIO_DATA_NUMBER];
        private AudioSource m_AudioSource;
        private List<GameObject> m_Cubes = new List<GameObject>();

        public void Initialise()
        {
            m_AudioSource = this.gameObject.ForceComponent<AudioSource>();
            m_AudioSource.loop = true;
            Core.Environment.OnEnvironmentLoadingComplete += OnEnvironmentLoadingComplete;

        }

        private void OnEnvironmentLoadingComplete()
        {
            WIFIServer.CreateInstance();

            this.WaitFor(2, () =>
            {
                WIFIClient.CreateInstance();

                WIFIClient.Instance.CollectAudioData((data) =>
                {
                    Visualize(data);
                });
            });
            
            
            for(int i = 0; i < WIFIServer.AUDIO_DATA_NUMBER; i++)
            {
                var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.localScale = new Vector3(1f, 0.1f, 1f);
                cube.transform.eulerAngles = new Vector3(0f, -0.703125f * i, 0f);
                cube.transform.position = cube.transform.forward * 100;
                m_Cubes.Add(cube);
            }

            Camera.main.transform.position = new Vector3(0f, 50f, -200f);
            Camera.main.transform.LookAt(Vector3.zero);


        }

        private void Visualize(List<AudioData> data)
        {
            for(int i = 0; i < data.Count; i++)
            {
                m_Cubes[i].transform.localScale = new Vector3(1f, data[i].m_Data * 1000f, 1f);
            }
        }

        
        private void Update()
        {
            if(WIFIServer.Instance != null)
            {
                if (m_AudioSource.clip != null)
                {
                    m_AudioSource.GetSpectrumData(m_Samples, 0, FFTWindow.Blackman);

                    for (int i = 0; i < m_Samples.Length; ++i)
                    {
                        m_Samples[i] = m_Samples[i] * 0.5f;
                    }
                    List<AudioData> newDataList = new List<AudioData>();
                    for (int i = 0; i < m_Samples.Length; ++i)
                    {
                        AudioData newData = new AudioData();
                        newData.m_Data = m_Samples[i];
                        newDataList.Add(newData);
                    }
                    WIFIServer.Instance.SendAudioData(newDataList);
                }
            }
        }
#else
        public void Initialise()
        {
        }
#endif
        }
}
