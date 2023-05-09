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




public class AudioData
{
    public float m_Data;
}

public class WIFIServer : Singleton<WIFIServer>
{
    public class Client
    {
        public string m_Id;
        public IPEndPoint m_RemoteEP;
    }

    public static int AUDIO_DATA_NUMBER = 512;
    public const string FROM_SERVER_HEADER = "FromServer";
    public const string FROM_CLIENT_HEADER = "FromClient";
    public const string AUDIO_DATA_HEADER = "Audio_Data";
    public const string GENERIC_HEADER = "GenericHeader";
    public const string GENERIC_HEADER_AND_DATA = "GenericHeaderAndData";
    public const string GENERIC_HEADER_AND_DATA_AND_DATA = "GenericHeaderAndDataAndData";


    private WIFIServerMono m_WIFIServerMono;



    protected override void Construct()
    {
        GameObject TCPServerHolder = new GameObject("TCPServerHolder");
        TCPServerHolder.transform.SetParent(Core.Mono.transform);
        m_WIFIServerMono = TCPServerHolder.ForceComponent<WIFIServerMono>();
    }

    public void OnConnected(Action<bool> callback) => m_WIFIServerMono.OnClientConnected += callback;


    public void SendAudioData(List<AudioData> audioList)
    {
        m_WIFIServerMono.SendAudioData(audioList);
    }

    public void SendGenericHeaderAndData(string header, string data)
    {
        m_WIFIServerMono.SendMessageGenericHeaderAndData(header, data);
    }

    public void SendGenericHeaderAndDataAndData(string header, string data1, string data2)
    {
        m_WIFIServerMono.SendMessageGenericHeaderAndDataAndData(header, data1, data2);
    }

    public void SendGenericHeader(string header)
    {
        m_WIFIServerMono.SendGenericHeader(header);
    }
}

public class WIFIServerMono : MonoBehaviour
{
    public const int SERVER_PORT = 3310;
    public Action<bool> OnClientConnected = null;

    private List<AudioData> m_AudioDataList = new List<AudioData>();
    private Client m_Client;
    private UDPComponent m_UDPComponent;

    private void Awake()
    {
        m_UDPComponent = this.gameObject.ForceComponent<UDPComponent>();
        m_UDPComponent.serverPort = SERVER_PORT;
        m_UDPComponent.StartServer();

        m_UDPComponent.On(WIFIServer.FROM_CLIENT_HEADER, OnReceiveConectNotice);

        StartCoroutine(ContactClient());
        StartCoroutine(InternalSendAudioData());

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
    public void SendAudioData(List<AudioData> data) => m_AudioDataList.AddRange(data);

    private IEnumerator InternalSendAudioData()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();

            if (m_AudioDataList.Count != 0)
            {
                List<AudioData> newdata = m_AudioDataList.Clone();
                m_AudioDataList.Clear();

                //Build message to client 
                byte[] msg = CreateAudioMesssage(WIFIServer.AUDIO_DATA_HEADER, newdata);
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
                            msg = CreateAudioMesssage(WIFIServer.AUDIO_DATA_HEADER, batch);
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


    private byte[] CreateAudioMesssage(string header, List<AudioData> data)
    {
        string response = $"{header}";
        foreach (var item in data)
        {
            response += $":{item.m_Data}";
        }
        return Encoding.ASCII.GetBytes(response);
    }
    #endregion DMX

    #region GENERIC
    public void SendMessageGenericHeaderAndData(string subHeader, string data)
    {
        //byte[] msg = CreateGenericHeaderAndDataMesssage(TCPServer.GENERIC_HEADER_AND_DATA, subHeader, data);
        //if (m_Client != null)
        //{
        //    m_UDPComponent.EmitToClient(msg, m_Client.m_RemoteEP);
        //    ConsoleExtra.Log($"sending message", null, ConsoleExtraEnum.EDebugType.OSC_Sent);
        //}
        //else
        //{
        //    ConsoleExtra.Log($"m_Client not connected", null, ConsoleExtraEnum.EDebugType.StartUp);
        //}
    }

    public void SendMessageGenericHeaderAndDataAndData(string subHeader, string data1, string data2)
    {
        //byte[] msg = CreateGenericHeaderAndDataAndDataMesssage(TCPServer.GENERIC_HEADER_AND_DATA_AND_DATA, subHeader, data1, data2);
        //if (m_Client != null)
        //{
        //    m_UDPComponent.EmitToClient(msg, m_Client.m_RemoteEP);
        //    ConsoleExtra.Log($"sending message", null, ConsoleExtraEnum.EDebugType.OSC_Sent);
        //}
        //else
        //{
        //    ConsoleExtra.Log($"m_Client not connected", null, ConsoleExtraEnum.EDebugType.StartUp);
        //}
    }



    public void SendGenericHeader(string subHeader)
    {
        //byte[] msg = CreateGenericHeaderMesssage(TCPServer.GENERIC_HEADER, subHeader);
        //if (m_Client != null)
        //{
        //    m_UDPComponent.EmitToClient(msg, m_Client.m_RemoteEP);
        //}
        //else
        //{
        //    ConsoleExtra.Log($"m_Client not connected", null, ConsoleExtraEnum.EDebugType.StartUp);
        //}
    }


    private byte[] CreateGenericHeaderAndDataMesssage(string header, string subHeader, string data)
    {
        string response = $"{header}:{subHeader}:{data}";
        return Encoding.ASCII.GetBytes(response);
    }

    private byte[] CreateGenericHeaderAndDataAndDataMesssage(string header, string subHeader, string data1, string data2)
    {
        string response = $"{header}:{subHeader}:{data1}:{data2}";
        return Encoding.ASCII.GetBytes(response);
    }

    private byte[] CreateGenericHeaderMesssage(string header, string subHeader)
    {
        string response = $"{header}:{subHeader}";
        return Encoding.ASCII.GetBytes(response);
    }

    #endregion GENERIC

    private void OnDestroy()
    {
        m_UDPComponent.disconnect();
    }

}
#endif