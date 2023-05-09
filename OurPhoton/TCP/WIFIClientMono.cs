#if Photon
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UDPCore;
using UnityEngine;


public class WIFIClient : Singleton<WIFIClient>
{
    private WIFIClientMono m_WIFIClientMono;
    protected override void Construct()
    {
        GameObject TCPClientHolder = new GameObject("TCPClientHolder");
        TCPClientHolder.transform.SetParent(Core.Mono.transform);
        m_WIFIClientMono = TCPClientHolder.ForceComponent<WIFIClientMono>();
    }

    public void CollectAudioData(Action<List<AudioData>> callback) => m_WIFIClientMono.AudioMessageRecivedCallback += callback;
    public void ReleaseAudioCollectData(Action<List<AudioData>> callback) => m_WIFIClientMono.AudioMessageRecivedCallback -= callback;



    public void CollectGenericHeader(string header, Action callback)
    {
        InternalCollectGenericHeader((data) =>
        {
            if (header == data)
            {
                callback?.Invoke();
            }
        });
    }

    private void InternalCollectGenericHeader(Action<string> callback)
    {
        m_WIFIClientMono.GenericHeaderCallback += callback;
    }




    public void CollectGenericHeaderAndData(string header, Action<string> callback)
    {
        InternalCollectGenericHeaderAndData((internalSubHeader, data) =>
        {
            if (header == internalSubHeader)
            {
                callback?.Invoke(data);
            }
        });
    }

    public void CollectGenericHeaderAndDataAndData(string header, Action<string, string> callback)
    {
        InternalCollectGenericHeaderAndDataAndData((internalSubHeader, data1, data2) =>
        {
            if (header == internalSubHeader)
            {
                callback?.Invoke(data1, data2);
            }
        });
    }



    private void InternalCollectGenericHeaderAndData(Action<string, string> callback)
    {
        m_WIFIClientMono.GenericHeaderAndDataCallback += callback;
    }


    private void InternalCollectGenericHeaderAndDataAndData(Action<string, string, string> callback)
    {
        m_WIFIClientMono.GenericHeaderAndDataAndDataCallback += callback;
    }

}


public class WIFIClientMono : MonoBehaviour
{
    public Action<List<AudioData>> AudioMessageRecivedCallback;
    public Action<string, string, string> GenericHeaderAndDataAndDataCallback;
    public Action<string, string> GenericHeaderAndDataCallback;
    public Action<string> GenericHeaderCallback;

    private UDPComponent m_Client;
    private string m_GeneratedID;

    private void Awake()
    {
        m_Client = this.gameObject.ForceComponent<UDPComponent>();
        ConnectToUDPServer();
        m_GeneratedID = GenerateID();
    }

    public void ConnectToUDPServer()
    {
        if (m_Client.GetServerIP() != string.Empty)
        {
            // from Tom code review 
            // Ahh ok cool, if it's been working it's probably fine, just feel like it would be better with a set port in a config or something. Could TODO it?
            int randomPort = UnityEngine.Random.Range(3001, 3310);
            m_Client.connect(m_Client.GetServerIP(), WIFIServerMono.SERVER_PORT, randomPort);

            m_Client.On(WIFIServer.FROM_SERVER_HEADER, OnFromServer);
            m_Client.On(WIFIServer.AUDIO_DATA_HEADER, OnAudioDataReceved);
            ////m_Client.On(TCPServer.GENERIC_HEADER, OnGenericHeaderReceved);
            ////m_Client.On(TCPServer.GENERIC_HEADER_AND_DATA, OnGenericHeaderAndDataReceved);
            ////m_Client.On(TCPServer.GENERIC_HEADER_AND_DATA_AND_DATA, OnGenericHeaderAndDataAndDataReceved);
        }
        else
        {
            Debug.LogError("TCPClient the server IP is null");
        }
        StartCoroutine(ContactServer());
    }


    [InspectorButton]
    private void DEBUG_Connect()
    {
        int randomPort = UnityEngine.Random.Range(3001, 3310);
        m_Client.connect(m_Client.GetServerIP(), WIFIServerMono.SERVER_PORT, randomPort);
    }


    private void OnFromServer(UDPEvent obj)
    {
        Debug.LogError("OnFromServer");
    }

    private void OnAudioDataReceved(UDPEvent obj)
    {
        List<AudioData> newList = new List<AudioData>();
        int index = 1;
        while (index < obj.pack.Length)
        {
            bool found =  float.TryParse(obj.pack[index], out float data);
            AudioData newData = new AudioData();
            newData.m_Data = data;
            newList.Add(newData);
            index++;
        }

        AudioMessageRecivedCallback?.Invoke(newList);
    }

    private void OnGenericHeaderReceved(UDPEvent obj)
    {
        string subHeader = obj.pack[1];
        GenericHeaderCallback?.Invoke(subHeader);
    }


    private void OnGenericHeaderAndDataReceved(UDPEvent obj)
    {
        string subHeader = obj.pack[1];
        string message = obj.pack[2];
        GenericHeaderAndDataCallback?.Invoke(subHeader, message);
    }

    private void OnGenericHeaderAndDataAndDataReceved(UDPEvent obj)
    {
        string subHeader = obj.pack[1];
        string data1 = obj.pack[2];
        string data2 = obj.pack[3];
        GenericHeaderAndDataAndDataCallback?.Invoke(subHeader, data1, data2);
    }


    private IEnumerator ContactServer()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();
            m_Client.EmitToServer(WIFIServer.FROM_CLIENT_HEADER, m_GeneratedID);
            yield return new WaitForSeconds(3);
            if (m_Client.noNetwork == true)
            {
                Debug.LogError("Please Connect to Wifi Hotspot");
            }
        }
    }

    private string GenerateID()
    {
        string id = Guid.NewGuid().ToString("N");

        //reduces the size of the id
        id = id.Remove(id.Length - 15);

        return id;
    }

    private void OnDestroy()
    {
        m_Client.disconnect();
    }

}
#endif