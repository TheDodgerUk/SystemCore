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

public class EnttecServer
{
    private Thread m_Thread;
    private const int MAX_SEND = 50;
    private Server m_Server;
    public EnttecServer()
    {
        m_Server = new Server(GetLocalIPAddress(), 14000);
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
        UDPCore.KillPort killPort = new KillPort(14000);
#endif

        m_Thread = new Thread(new ThreadStart(m_Server.Initilise));
        m_Thread.Start();
    }


    public string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        throw new Exception("No network adapters with an IPv4 address in the system!");
    }

    public void StopThread()
    {
        m_Thread.Abort();
        m_Server.m_RunThread = false;
    }
    public void SendData(int index, byte amout) => m_Server.SendData(index, amout);
 

    class Server
    {
        public bool m_RunThread = true;
        private TcpListener m_TcpListener = null;
        private string m_Ip;
        private int m_Port;
        private List<DmxData> m_Data = new List<DmxData>();

        public Server(string ip, int port)
        {
            m_Ip = ip;  
            m_Port = port;
        }
        public void Initilise()
        {
            IPAddress localAddr = IPAddress.Parse(m_Ip);

            m_TcpListener = new TcpListener(localAddr, m_Port);
            m_TcpListener.Start();
            StartListener();
        }

        public void StartListener()
        {
            try
            {
                while (m_RunThread)
                {
                    Debug.Log("Waiting for a connection...");
                    Console.WriteLine("Waiting for a connection...");
                    TcpClient client = m_TcpListener.AcceptTcpClient();
                    Console.WriteLine("Connected!");
                    Debug.Log("Connected!");
                    Thread t = new Thread(new ParameterizedThreadStart(HandleDeivce));
                    t.Start(client);
                }               
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
                m_TcpListener.Stop();
            }
        }

        
        public void SendData(int index, byte amout) => m_Data.Add(new DmxData(index, amout));

        public void HandleDeivce(System.Object obj)
        {
            while (m_RunThread)
            {
                TcpClient client = (TcpClient)obj;
                var stream = client.GetStream();
                try
                {
                    if (m_Data.Count != 0)
                    {
                        Byte[] bytes = new Byte[100000];

                        List<DmxData> dmxList = new List<DmxData>(m_Data);
                        int count = dmxList.Count;
                        if(count< MAX_SEND)
                        {
                            m_Data.Clear();
                        }
                        else
                        {
                            m_Data.RemoveRange(0, MAX_SEND);
                            count = MAX_SEND;
                        }
                        

                        string newData = "";
                        for (int i = 0; i < count; i++)
                        {
                            newData += $":{dmxList[i].DMX}:{dmxList[i].DmxValue}:";
                        }

                        Byte[] reply = Encoding.ASCII.GetBytes(newData);

                        stream.Write(reply, 0, reply.Length);
                        Thread.Sleep(100);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception: {0}", e.ToString());
                    // not think this is needed
                    ////////client.Close();
                }
            }
        }
    }
}
#endif