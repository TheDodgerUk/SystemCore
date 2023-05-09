#if Photon
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UDPCore;
using UnityEngine;


public class TCPClient : Singleton<TCPClient>
{
    private TCPClientMono m_TCPClientMono;
    protected override void Construct()
    {
        GameObject TCPClientHolder = new GameObject("TCPClientHolder");
        TCPClientHolder.transform.SetParent(Core.Mono.transform);
        m_TCPClientMono = TCPClientHolder.ForceComponent<TCPClientMono>();
    }

    public void CollectDmxData(Action<List<DmxData>> callback) => m_TCPClientMono.DmxMessageRecivedCallback += callback;
    public void ReleaseDmxCollectData(Action<List<DmxData>> callback) => m_TCPClientMono.DmxMessageRecivedCallback -= callback;

}


public class TCPClientMono : MonoBehaviour
{
    public Action<List<DmxData>> DmxMessageRecivedCallback;
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
            m_Client.connect(m_Client.GetServerIP(), TCPServerMono.SERVER_PORT, randomPort);

            m_Client.On(TCPServer.FROM_SERVER_HEADER, OnFromServer);
            m_Client.On(TCPServer.DMX_DATA_HEADER, OnDmxDataReceved);
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
        m_Client.connect(m_Client.GetServerIP(), TCPServerMono.SERVER_PORT, randomPort);
    }


    private void OnFromServer(UDPEvent obj)
    {
        Debug.LogError("OnFromServer");
    }

    private void OnDmxDataReceved(UDPEvent obj)
    {
        List<DmxData> newList = new List<DmxData>();
        int index = 1;
        while (index < obj.pack.Length)
        {
            int dmx = int.Parse(obj.pack[index]);
            int dmxValue = int.Parse(obj.pack[index + 1]);
            DmxData newData = new DmxData(dmx, (byte)dmxValue);
            newList.Add(newData);
            index += 2;
        }
        if (newList.Count > 0)
        {
            ConsoleExtra.Log($"First Dmx: {newList[0].DMX},  dmxValue: {newList[0].DmxValue}", null, ConsoleExtraEnum.EDebugType.Photon_Receive);
        }
        //HACK    disbale  for now for testing DmxMessageRecivedCallback?.Invoke(newList);
    }


    private IEnumerator ContactServer()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();
            //m_Client.EmitToServer(TCPServer.FROM_CLIENT_HEADER, m_GeneratedID);
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