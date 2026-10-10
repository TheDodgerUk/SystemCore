#if Photon
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DmxData
{
    public int DMX { get; private set; }
    public byte DmxValue { get; private set; }
    public DmxData(int dmx, byte value)
    {
        DMX = dmx;
        DmxValue = value;
    }
}


public class PhotonDMX: Singleton<PhotonDMX>
{
#if !AR_INTERACTION
    private EnttecServer m_EnttecServer;
    //private DP.ProDmx m_ProDmx;
    private PhotonDMXMono m_PhotonDMXMono;
#endif

    private bool m_IsInitilised = false;
    public bool IsInitilised() => m_IsInitilised = true;

    private GameObject m_Root;
    protected override void Construct()
    {
    }

    public void Initilise(GameObject root)
    {
        m_Root = root;
        InternalInitilise();
    }

    private void InternalInitilise()
    {
#if !AR_INTERACTION
        if (m_EnttecServer == null)
        {
            m_EnttecServer = new EnttecServer();
        }

        //m_ProDmx = root.ForceComponent<DP.ProDmx>();

        var view = m_Root.GetComponent<PhotonView>();
        if (view.IsMine == true && m_PhotonDMXMono == null)
        {
            var mono = m_Root.gameObject.GetComponentInChildren<PhotonDMXMono>(true);
            m_PhotonDMXMono = mono;
            m_IsInitilised = true;
        }
#endif
    }

    public void CollectDmxData(Action<List<DmxData>> callback) => PhotonDMXMono.DmxMessageRecivedCallback += callback;

    public void RemoveCollectDmxData(Action<List<DmxData>> callback) => PhotonDMXMono.DmxMessageRecivedCallback -= callback;

    public void SendDmxData(int dmx, byte dmxValue)
    {
#if !AR_INTERACTION
        if(m_PhotonDMXMono == null)
        {
            List<DmxData> dataList = new List<DmxData>();
            DmxData newData = new DmxData(dmx, dmxValue);
            dataList.Add(newData);
            PhotonDMXMono.DmxMessageRecivedCallback?.Invoke(dataList);
            Debug.LogError("need to  reInternalInitilise somehow");
        }
        else
        {
            m_PhotonDMXMono.SendDmxData(dmx, dmxValue);
        }
        m_EnttecServer.SendData(dmx, dmxValue);
#endif
    }
}



public class PhotonDMXMono : MonoBehaviourPun, IPunObservable
{
    private List<DmxData> m_DmxList = new List<DmxData>();

    public static Action<List<DmxData>> DmxMessageRecivedCallback;

    public void SendDmxData(int dmx, byte value) => m_DmxList.Add(new DmxData(dmx, value));


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            OnDmxDataSend(stream);
        }
        else
        {
            OnDmxDataReceived(stream);
        }
    }

    private void OnDmxDataSend(PhotonStream stream)
    {
        if (m_DmxList.Count != 0)
        {
            List<DmxData> dmxList = new List<DmxData>(m_DmxList);
            m_DmxList.Clear();


            for (int i = 0; i < dmxList.Count; i++)
            {
                stream.SendNext(dmxList[i].DMX);
                stream.SendNext(dmxList[i].DmxValue);
            }
            ConsoleExtra.Log($"InternalSendDMXData {dmxList.Count} {dmxList[0].DMX}  {dmxList[0].DmxValue} ", null, ConsoleExtraEnum.EDebugType.Photon_Sent);
           
            DmxMessageRecivedCallback?.Invoke(dmxList);                  
        }
    }

    private void OnDmxDataReceived(PhotonStream stream)
    {
        List<DmxData> newList = new List<DmxData>();

        int count = stream.ReceiveCount();
        int index = 0;
      
        if (stream.IsWriting)
        {
            index = PhotonStream.WRITE_READ_START;
        }
        else
        {
            if (count >= PhotonStream.WRITE_READ_START)
            {
                var data1 = stream.ReceiveIndex(index);
                var data2 = stream.ReceiveIndex(index + 1);
                var data3 = stream.ReceiveIndex(index + 2);
                if (data1 == null || data2 == null || data3 == null)
                {
                    index = PhotonStream.WRITE_READ_START;
                }
            }
        }

        

        bool isEven = (count - index) % 2 == 0;
        if(isEven == false)
        {
            Debug.LogError($"Invalid count {(count - index)}");
            return;
        }

        while (index < count)
        {
            object dmxObject = stream.ReceiveIndex(index);
            object valueObject = stream.ReceiveIndex(index + 1);

            if ((dmxObject is int) && (valueObject is byte))
            {
                int dmx = (int)dmxObject;
                int dmxValue = (byte)valueObject;
                DmxData newData = new DmxData(dmx, (byte)dmxValue);
                newList.Add(newData);
            }
            else
            {
                string message = $"data not valid  {count}   DMX:  {dmxObject}  {dmxObject.GetType()}    Value: {valueObject}  {valueObject.GetType()}";
                Debug.LogError(message);
                ConsoleExtra.Log(message, null, ConsoleExtraEnum.EDebugType.Photon_Receive);
            }
            index += 2;
        }
        DmxMessageRecivedCallback?.Invoke(newList);
    }

}

#endif