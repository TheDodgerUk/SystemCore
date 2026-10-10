#if Photon
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

////#if !UNITY_EDITOR && !UNITY_STANDALONE
////public class DmxData
////{
////    public int DMX { get; private set; }
////    public byte DmxValue { get; private set; }
////    public DmxData(int dmx, byte value)
////    {
////        DMX = dmx;
////        DmxValue = value;
////    }
////}
////#endif


public class EnttecClient
{
    private Client m_Client;
    private Thread m_Thread;
    public EnttecClient()
    {
        m_Client = new Client();
        m_Thread = new Thread(new ThreadStart(m_Client.Connect));
        m_Thread.Start();

        Console.ReadLine();
    }

    public void StopThread()
    {
        m_Thread.Abort();
        m_Client.m_Runthread = false;
    }

    public void CollectData(Action<List<DmxData>> data) => m_Client.CollectData(data);

    public class Client
    {
        public bool m_Runthread = true;
        public Client()
        {

        }
        private static Action<List<DmxData>> m_Data;

        public static string GetLocalIPAddress()
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
        public void CollectData(Action<List<DmxData>> data) => m_Data = data;

        public void Connect()
        {
            try
            {
                Int32 port = 14000;
                TcpClient client = new TcpClient(GetLocalIPAddress(), port);

                while (m_Runthread)
                {
                    NetworkStream stream = client.GetStream();


                    // Bytes Array to receive Server Response.
                    Byte[] data = new Byte[10000];
                    String response = String.Empty;

                    // Read the Tcp Server Response Bytes.
                    Int32 bytes = stream.Read(data, 0, data.Length);
                    response = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                    Console.WriteLine("Received: {0}", response);

                    var raw = response.Split(':').ToList();
                    raw.RemoveAll(e => e.Length == 0);
                    List<DmxData> newData = new List<DmxData>();
                    UnityEngine.Debug.LogError($" repose {bytes}  , raw  {raw.Count}");
                    for (int i = 0; i < raw.Count;)
                    {
                        DmxData item = new DmxData(int.Parse(raw[i]), byte.Parse(raw[i + 1]));
                        newData.Add(item);
                        i += 2;
                    }
                    m_Data?.Invoke(newData);
                }

                //stream.Close();
                client.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e);
#if UNITY_EDITOR || UNITY_STANDALONE
                UnityEngine.Debug.LogError(e);
#endif
            }

            Console.Read();
        }
    }
}
#endif
