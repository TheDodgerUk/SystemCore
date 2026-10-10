#if Photon
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DMXTESTONLY : MonoBehaviour
{
    private EnttecServer m_EnttecServer;
    private EnttecClient m_EnttecClient;

    void Start()
    {
        m_EnttecServer = new EnttecServer();
        m_EnttecClient = new EnttecClient();
        EnttecOpenDmx.start();
    }

    private void OnDisable()
    {
        m_EnttecServer.StopThread();
        m_EnttecClient.StopThread(); 
    }
    [InspectorButton]
    private void SendData1()
    {
        m_EnttecServer.SendData(1, 0);
        m_EnttecServer.SendData(3, 0);


    }

    [InspectorButton]
    private void ClearAll512()
    {
        for (int i = 0; i < 512; i++)
        {
            m_EnttecServer.SendData(i, 0);
        }
    }

    private void OnDestroy()
    {
        m_EnttecServer.StopThread();
        m_EnttecClient.StopThread();
    }
}
#endif
