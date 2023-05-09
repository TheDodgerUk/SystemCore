#if Photon
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UDPCore;
using UnityEngine;


public class Client
{
    public string m_Id;
    public IPEndPoint m_RemoteEP;
}


//////public class DmxData
//////{
//////    public int DMX { get; private set; }
//////    public byte DmxValue { get; private set; }
//////    public DmxData(int dmx, byte value)
//////    {
//////        DMX = dmx;
//////        DmxValue = value;
//////    }
//////}

public class TCPServer : Singleton<TCPServer>
{
    public const string FROM_SERVER_HEADER = "FromServer";
    public const string FROM_CLIENT_HEADER = "FromClient";
    public const string DMX_DATA_HEADER = "DMX_Data";
    private TCPServerMono m_TCPServerMono;



    protected override void Construct()
    {
        GameObject TCPServerHolder = new GameObject("TCPServerHolder");
        TCPServerHolder.transform.SetParent(Core.Mono.transform);
        m_TCPServerMono = TCPServerHolder.ForceComponent<TCPServerMono>();
    }

    public void OnConnected(Action<bool> callback) => m_TCPServerMono.OnClientConnected += callback;
    public void SendDmxData(int dmx, byte dmxValue)
    {
        ConsoleExtra.Log($"Dmx: {dmx},  dmxValue: {dmxValue}", null, ConsoleExtraEnum.EDebugType.Photon_Sent);
        m_TCPServerMono.SendDmxData(dmx, dmxValue);
    }

    public void SendDmxData(List<DmxData> dmxList)
    {
        foreach (var item in dmxList)
        {
            m_TCPServerMono.SendDmxData(item.DMX, item.DmxValue);
        }
    }
}

public class TCPServerMono : MonoBehaviour
{
    public const int SERVER_PORT = 3310;
    public Action<bool> OnClientConnected = null;

    private List<DmxData> m_DmxList = new List<DmxData>();
    private Client m_Client;
    private UDPComponent m_UDPComponent;

    private void Awake()
    {
        m_UDPComponent = this.gameObject.ForceComponent<UDPComponent>();
        m_UDPComponent.serverPort = SERVER_PORT;
        m_UDPComponent.StartServer();

        // m_UDPComponent.On(TCPServer.FROM_CLIENT_HEADER, OnReceiveConectNotice);

        StartCoroutine(ContactClient());
        StartCoroutine(InternalSendDMXData());

    }

    private IEnumerator ContactClient()
    {
        yield return new WaitForSeconds(3);
    }

    private void OnReceiveConectNotice(UDPEvent data)
    {
        if (m_Client == null)
        {
            Debug.LogError("Connect m_Client to server");
            ConsoleExtra.Log($"Connected to client", null, ConsoleExtraEnum.EDebugType.StartUp);
            m_Client = new Client();
            m_Client.m_Id = data.pack[1];//set client id

            //set  clients's port and ip address
            m_Client.m_RemoteEP = data.anyIP;
            OnClientConnected?.Invoke(true);
        }
    }

    #region DMX
    public void SendDmxData(int dmx, byte value) => m_DmxList.Add(new DmxData(dmx, value));

    private IEnumerator InternalSendDMXData()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();
            if (m_DmxList.Count != 0)
            {
                List<DmxData> newdata = m_DmxList.Clone();
                m_DmxList.Clear();

                //Build message to client 
                byte[] msg = CreateDmxMesssage(TCPServer.DMX_DATA_HEADER, newdata);
                if (m_Client != null)
                {
                    if (msg.Length < UDPComponent.MAX_BUF_SIZE)
                    {
                        m_UDPComponent.EmitToClient(msg, m_Client.m_RemoteEP);
                    }
                    else
                    {
                        int number = msg.Length / UDPComponent.MAX_BUF_SIZE;
                        number++;
                        var splitLists = newdata.SplitList(msg.Length / number);
                        Debug.Log($"Size was too big, number of batchs {splitLists.Count}   number {number}   msg.Length  {msg.Length}");
                        foreach (var batch in splitLists)
                        {
                            Debug.Log($"batch size {batch.Count}");
                            msg = CreateDmxMesssage(TCPServer.DMX_DATA_HEADER, batch);
                            m_UDPComponent.EmitToClient(msg, m_Client.m_RemoteEP);
                        }

                    }
                }
                else
                {
                    ConsoleExtra.Log($"m_Client not connected", null, ConsoleExtraEnum.EDebugType.StartUp);
                }
            }
        }
    }


    private byte[] CreateDmxMesssage(string header, List<DmxData> data)
    {
        string response = $"{header}";
        foreach (var item in data)
        {
            response += $":{item.DMX}:{item.DmxValue}";
        }
        return Encoding.ASCII.GetBytes(response);
    }
    #endregion DMX


    private void OnDestroy()
    {
        m_UDPComponent.disconnect();
    }

}
#endif