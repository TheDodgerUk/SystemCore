#if Photon
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;


public class PhotonGeneric
{

    public class VrCreationData
    {
        public int ActorNumber;
        public string CatGuid;
        public string BaseName;
        public string NewName;
        public float SyncTime;
        public UnityEngine.Vector3 Position;
        public UnityEngine.Quaternion Rotation;
    }
    public abstract class VrInteractionMessageData
    {
        public int GuidRef;
        /// not needed as it only sends to the room you are in //public string RoomName;
    }

    public class VrInteractionMessageIntData : VrInteractionMessageData
    {
        public int Message;
    }

    public class VrInteractionMessageStringData : VrInteractionMessageData
    {
        public string Message;
    }

    private PhotonGenericMono m_PhotonGenericMono;
    private bool m_IsInitilised = false;
    public bool IsInitilised() => m_IsInitilised = true;

    public PhotonGeneric(PhotonGenericMono photonGenericMono)
    {
        m_PhotonGenericMono = photonGenericMono;

        InternalCollectVrCreation((data) =>
        {
            Core.Scene.SpawnObject(data.CatGuid, (created) =>
            {
                var vr = created.gameObject.GetComponent<VrInteraction>();
                vr.name = data.NewName;
                vr.transform.position = data.Position;
                vr.transform.rotation = data.Rotation;
                vr.ForceCreateGuid();
                PhotonGenericMono.VRAllCreationDataCallback?.Invoke(data.BaseName, vr);
            });
        });
    }


    public void SendGenericHeaderAndData<T>(string subHeader, T data, ReceiverGroup group = ReceiverGroup.All) where T : class
    {
        ConsoleExtra.Log($"SendGenericHeaderAndData : {subHeader}", null, ConsoleExtraEnum.EDebugType.Photon_Sent);
        var rawData = Json.JsonNet.WriteToText(data, true);
        m_PhotonGenericMono.SendMessageGenericHeaderAndData(subHeader, rawData, group);
    }

    public void CollectGenericHeaderAndData<T>(string header, Action<T> callback)
    {
        InternalCollectGenericHeaderAndData((internalSubHeader, data) =>
        {
            if (header == internalSubHeader)
            {
                ConsoleExtra.Log($"CollectGenericHeaderAndData : {header}", null, ConsoleExtraEnum.EDebugType.Photon_Receive);
                var convertedData = Json.JsonNet.ReadFromText<T>(data);
                if(convertedData == null)
                {
                    UnityEngine.Debug.LogError($"bad json header: {header}");
                }
                callback?.Invoke(convertedData);
            }
        });
    }



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

    public void SendVrCreation(VrCreationData data)
    {
        var rawData = Json.JsonNet.WriteToText(data, true);
        m_PhotonGenericMono.SendVrCreation(rawData);

    }

    private void InternalCollectVrCreation(Action<VrCreationData> callback)
    {
        InternalCollectVrCreationData((data) =>
        {
            var convertedData = Json.JsonNet.ReadFromText<VrCreationData>(data);
            callback?.Invoke(convertedData);
        });
    }

    public void CollectVrCreation(Action<string,  VrInteraction> callback)
    {
        PhotonGenericMono.VRAllCreationDataCallback += callback;
    }

    private void InternalCollectGenericHeader(Action<string> callback)
    {
        PhotonGenericMono.GenericHeaderCallback += callback;
    }

    private void InternalCollectGenericHeaderAndData(Action<string, string> callback)
    {
        PhotonGenericMono.GenericHeaderAndDataCallback += callback;
    }

    private void InternalCollectVrInteractionMessageData(Action<int, string, string> callback)
    {
        PhotonGenericMono.VrInteractionMessageTypeAndMessageCallback += callback;
    }

    private void InternalCollectVrInteractionIntData(Action<string> callback)
    {
        PhotonGenericMono.VrInteractionDataIntCallback += callback;
    }

    private void InternalCollectDebugIntData(Action<int> callback)
    {
        PhotonGenericMono.DebugIntCallback += callback;
    }

    private void InternalCollectDebugIntDataData(Action<int, string> callback)
    {
        PhotonGenericMono.DebugDataIntCallback += callback;
    }

    private void InternalCollectAchievmentData(Action<string, string> callback)
    {
        PhotonGenericMono.AchievmentDataCallback += callback;
    }

    private void InternalCollectLeaderBoardData(Action<string, string> callback)
    {
        PhotonGenericMono.LeaderBoardDataCallback += callback;
    }
    private void InternalCollectUserData(Action<string, string> callback)
    {
        PhotonGenericMono.UserDataCallback += callback;
    }

    public void SendGenericHeader(string subHeader, ReceiverGroup group = ReceiverGroup.All)
    {
        m_PhotonGenericMono.SendGenericHeader(subHeader, group);
    }

    private void InternalCollectVrCreationData(Action<string> callback)
    {
        PhotonGenericMono.VRCreationDataCallback += callback;
    }

    public void SendDebugInt(int data)
    {
        m_PhotonGenericMono.SendMessageDebugInt(data);
    }

    public void SendDebugIntData<T>(int data1, T data2)
    {
        var rawData = Json.JsonNet.WriteToText(data2, true);
        m_PhotonGenericMono.SendMessageDebugIntData(data1, rawData);
    }

    public void SendAchievmentData<T>(T data)
    {
        var rawData = Json.JsonNet.WriteToText(data, true);
        m_PhotonGenericMono.SendAchievmentData(rawData, typeof(T).Name);
    }

    public void SendLeaderBoardData<T>(T data)
    {
        var rawData = Json.JsonNet.WriteToText(data, true);
        m_PhotonGenericMono.SendLeaderBoardData(rawData, typeof(T).Name);
    }

    public void SendUserData<T>(T data)
    {
        var rawData = Json.JsonNet.WriteToText(data, true);
        m_PhotonGenericMono.SendUserData(rawData, typeof(T).Name);
    }



    public void SendGenericVrInterationMessage<T>(VrInteraction interaction, T Message, ReceiverGroup group = ReceiverGroup.Others)
    {
        if (Core.PhotonMultiplayerRef.CurrentRoom != null)
        {
            VrInteractionMessageStringData messageData = new VrInteractionMessageStringData();
            messageData.GuidRef = interaction.GuidRef;
            var rawMessageData = Json.JsonNet.WriteToText(Message, true);
            messageData.Message = rawMessageData;

            var rawData = Json.JsonNet.WriteToText(messageData, true);
            m_PhotonGenericMono.SendMessageVrInteractionMessageData(messageData.GuidRef, typeof(T).Name, rawData, group);
        }
    }

    public void CollectGenericVrInterationMessage<T>(VrInteraction interaction, Action<T> callback)
    {
        var className = typeof(T).Name;
        var classGUID = className.GetHashCode();
        var foundValidCopy = GlobalConsts.m_PhotonGuidSafety.Find(e => e.Name == className && e.GUID == classGUID);

        var foundCopyName = GlobalConsts.m_PhotonGuidSafety.Find(e => e.Name == className && e.GUID != classGUID);
        var foundCopyGUID = GlobalConsts.m_PhotonGuidSafety.Find(e => e.Name != className && e.GUID == classGUID);

        if(foundCopyName != null)
        {
            UnityEngine.Debug.LogError($"Duplicate of: {className}, {classGUID}, FOUND {foundCopyName.Name}, {foundCopyGUID.GUID}");
        }

        if (foundCopyGUID != null)
        {
            UnityEngine.Debug.LogError($"Duplicate of: {className}, {classGUID}, FOUND {foundCopyGUID.Name}, {foundCopyGUID.GUID}");
        }

        if(foundValidCopy == null && foundCopyName == null && foundCopyName == null)
        {
            GlobalConsts.GuidSafety newSafety = new GlobalConsts.GuidSafety();
            newSafety.Name = className;
            newSafety.GUID = classGUID;
            GlobalConsts.m_PhotonGuidSafety.Add(newSafety);
        }

        InternalCollectVrInteractionMessageData((itemGuidRef, messageDataType, messageData) =>
        {
            if (Core.PhotonMultiplayerRef.CurrentRoom != null)
            {
                if (interaction.GuidRef == itemGuidRef)// this to optimise the hell out of it 
                {
                    if (messageDataType == typeof(T).Name)
                    {
                        var convertedData = Json.JsonNet.ReadFromText<VrInteractionMessageStringData>(messageData);
                        if (interaction.GuidRef == convertedData.GuidRef)
                        {
                            var convertedMessageData = Json.JsonNet.ReadFromText<T>(convertedData.Message);
                            callback?.Invoke(convertedMessageData);
                        }
                    }
                }
            }
        });
    }




    public void SendGenericVrInterationIntMessage(VrInteraction interaction, int Message, ReceiverGroup group = ReceiverGroup.Others)
    {
        if (Core.PhotonMultiplayerRef.CurrentRoom != null)
        {
            VrInteractionMessageIntData messageData = new VrInteractionMessageIntData();
            messageData.GuidRef = interaction.GuidRef;
            messageData.Message = Message;

            var rawData = Json.JsonNet.WriteToText(messageData, true);
            m_PhotonGenericMono.SendMessageVrInteractionIntData(rawData, group);
        }
    }

    public void CollectGenericVrInterationIntMessage(VrInteraction interaction, int intMessage, Action callback)
    {
        InternalCollectVrInteractionIntData((messageData) =>
        {
            if (Core.PhotonMultiplayerRef.CurrentRoom != null)
            {
                    var convertedData = Json.JsonNet.ReadFromText<VrInteractionMessageIntData>(messageData);
                    if (interaction.GuidRef == convertedData.GuidRef)
                    {
                        if (convertedData.Message == intMessage)
                        {
                            callback?.Invoke();
                        }
                    }
            }
        });
    }

    public void CollectDebugIntMessage(int intMessage, Action callback)
    {
        InternalCollectDebugIntData((messageData) =>
        {
            if (messageData == intMessage)
            {
                callback?.Invoke();
            }
        });
    }
    public void CollectDebugIntDataMessage<T>(int intMessage, Action<T> callback)
    {
        InternalCollectDebugIntDataData((messageData, intCallback) =>
        {
            if (messageData == intMessage)
            {
                var convertedData = Json.JsonNet.ReadFromText<T>(intCallback);
                callback?.Invoke(convertedData);
            }
        });
    }

    public void CollectAchievmentDataMessage<T>(Action<T> callback)
    {
        InternalCollectAchievmentData((messageData, messageType) =>
        {
            if (messageType == typeof(T).Name)
            {
                var convertedData = Json.JsonNet.ReadFromText<T>(messageData);
                callback?.Invoke(convertedData);
            }
        });
    }

    public void CollectLeaderBoardDataMessage<T>(Action<T> callback)
    {
        InternalCollectLeaderBoardData((messageData, messageType) =>
        {
            if (messageType == typeof(T).Name)
            {
                var convertedData = Json.JsonNet.ReadFromText<T>(messageData);
                callback?.Invoke(convertedData);
            }
        });
    }
    public void CollectUserDataMessage<T>(Action<T> callback)
    {
        InternalCollectUserData((messageData, messageType) =>
        {
            if (messageType == typeof(T).Name)
            {
                var convertedData = Json.JsonNet.ReadFromText<T>(messageData);
                callback?.Invoke(convertedData);
            }
        });
    }

}


public class PhotonGenericMono : MonoBehaviourPun, IOnEventCallback
{
    public static Action<string> GenericHeaderCallback;
    public static Action<string, string> GenericHeaderAndDataCallback;
    public static Action<int, string, string> VrInteractionMessageTypeAndMessageCallback;
    public static Action<string> VrInteractionDataIntCallback;
    public static Action<int> DebugIntCallback;
    public static Action<int, string> DebugDataIntCallback;

    public static Action<string, string> AchievmentDataCallback;
    public static Action<string, string> LeaderBoardDataCallback;
    public static Action<string, string> UserDataCallback;
    public static Action<string> VRCreationDataCallback;

    public static Action<string, VrInteraction> VRAllCreationDataCallback;
    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }


    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        switch(eventCode)
        {
            case (byte)PhotonVrInteraction.EventIDEnum.GenericHeader:
                {
                    object[] data = (object[])photonEvent.CustomData;
                    string subHeader = (string)data[0];
                    GenericHeaderCallback?.Invoke(subHeader);
                }
                break;

            case (byte)PhotonVrInteraction.EventIDEnum.GenericHeaderAndData:
                {
                    object[] data = (object[])photonEvent.CustomData;
                    string subHeader = (string)data[0];
                    string subdata = (string)data[1];
                    GenericHeaderAndDataCallback?.Invoke(subHeader, subdata);
                }
                break;
            case (byte)PhotonVrInteraction.EventIDEnum.VrInteractionMessage:
                {
                    object[] data = (object[])photonEvent.CustomData;
                    int itemGuid = (int)data[0];
                    string messageType = (string)data[1];
                    string subdata = (string)data[2];
                    VrInteractionMessageTypeAndMessageCallback?.Invoke(itemGuid, messageType, subdata);
                }
                break;
            case (byte)PhotonVrInteraction.EventIDEnum.VrInteractionInt:
                {
                    object[] data = (object[])photonEvent.CustomData;
                    string subdata = (string)data[0];
                    VrInteractionDataIntCallback?.Invoke(subdata);
                }
                break;
            case (byte)PhotonVrInteraction.EventIDEnum.DebugInt:
                {
                    object[] data = (object[])photonEvent.CustomData;
                    int subdata = (int)data[0];
                    DebugIntCallback?.Invoke(subdata);
                }
                break;
            case (byte)PhotonVrInteraction.EventIDEnum.DebugIntData:
                {
                    object[] data = (object[])photonEvent.CustomData;
                    int subdata = (int)data[0];
                    string subdata1 = (string)data[1];
                    DebugDataIntCallback?.Invoke(subdata, subdata1);
                }
                break;
            case (byte)PhotonVrInteraction.EventIDEnum.AchievmentData:
                {
                    object[] data = (object[])photonEvent.CustomData;
                    string subdata = (string)data[0];
                    string messageType = (string)data[1];
                    AchievmentDataCallback?.Invoke(subdata, messageType);
                }
                break;
            case (byte)PhotonVrInteraction.EventIDEnum.LeaderBoardData:
                {
                    object[] data = (object[])photonEvent.CustomData;
                    string subdata = (string)data[0];
                    string messageType = (string)data[1];
                    LeaderBoardDataCallback?.Invoke(subdata, messageType);
                }
                break;
            case (byte)PhotonVrInteraction.EventIDEnum.UserData:
                {
                    object[] data = (object[])photonEvent.CustomData;
                    string subdata = (string)data[0];
                    string messageType = (string)data[1];
                    UserDataCallback?.Invoke(subdata, messageType);
                }
                break;
            case (byte)PhotonVrInteraction.EventIDEnum.VRCreation:
                {
                    object[] data = (object[])photonEvent.CustomData;
                    string subdata = (string)data[0];
                    VRCreationDataCallback?.Invoke(subdata);
                }
                break;

        }
    }




    public void SendGenericHeader(string subHeader, ReceiverGroup group = ReceiverGroup.All)
    {
        object[] content = new object[] { subHeader }; // Array contains the target position and the IDs of the selected units
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = group }; // You would have to set the Receivers to All in order to receive this event on the local client as well
        PhotonNetwork.RaiseEvent((byte)PhotonVrInteraction.EventIDEnum.GenericHeader, content, raiseEventOptions, SendOptions.SendReliable);
    }

    public void SendMessageVrInteractionIntData(string data, ReceiverGroup group = ReceiverGroup.Others)
    {
        object[] content = new object[] {data }; // Array contains the target position and the IDs of the selected units
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = group }; // You would have to set the Receivers to All in order to receive this event on the local client as well
        PhotonNetwork.RaiseEvent((byte)PhotonVrInteraction.EventIDEnum.VrInteractionInt, content, raiseEventOptions, SendOptions.SendReliable);
    }

    public void SendMessageVrInteractionMessageData(int itemGuid, string messageType, string data, ReceiverGroup group = ReceiverGroup.Others)
    {
        object[] content = new object[] { itemGuid, messageType, data }; // Array contains the target position and the IDs of the selected units
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = group }; // You would have to set the Receivers to All in order to receive this event on the local client as well
        PhotonNetwork.RaiseEvent((byte)PhotonVrInteraction.EventIDEnum.VrInteractionMessage, content, raiseEventOptions, SendOptions.SendReliable);
    }

    public void SendMessageGenericHeaderAndData(string subHeader, string data, ReceiverGroup group = ReceiverGroup.Others)
    {
        object[] content = new object[] { subHeader, data }; // Array contains the target position and the IDs of the selected units
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = group }; // You would have to set the Receivers to All in order to receive this event on the local client as well
        PhotonNetwork.RaiseEvent((byte)PhotonVrInteraction.EventIDEnum.GenericHeaderAndData, content, raiseEventOptions, SendOptions.SendReliable);
    }

    public void SendMessageDebugInt(int data)
    {
        object[] content = new object[] {data }; // Array contains the target position and the IDs of the selected units
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
        PhotonNetwork.RaiseEvent((byte)PhotonVrInteraction.EventIDEnum.DebugInt, content, raiseEventOptions, SendOptions.SendReliable);
    }

    public void SendMessageDebugIntData(int data1, string data2)
    {
        object[] content = new object[] { data1, data2 }; // Array contains the target position and the IDs of the selected units
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
        PhotonNetwork.RaiseEvent((byte)PhotonVrInteraction.EventIDEnum.DebugIntData, content, raiseEventOptions, SendOptions.SendReliable);
    }


    public void SendAchievmentData(string data, string messageType)
    {
        object[] content = new object[] { data, messageType }; // Array contains the target position and the IDs of the selected units
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
        PhotonNetwork.RaiseEvent((byte)PhotonVrInteraction.EventIDEnum.AchievmentData, content, raiseEventOptions, SendOptions.SendReliable);
    }
    public void SendLeaderBoardData(string data,string messageType)
    {
        object[] content = new object[] { data, messageType }; // Array contains the target position and the IDs of the selected units
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
        PhotonNetwork.RaiseEvent((byte)PhotonVrInteraction.EventIDEnum.LeaderBoardData, content, raiseEventOptions, SendOptions.SendReliable);
    }

    public void SendUserData(string data, string messageType)
    {
        object[] content = new object[] { data, messageType }; // Array contains the target position and the IDs of the selected units
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
        PhotonNetwork.RaiseEvent((byte)PhotonVrInteraction.EventIDEnum.UserData, content, raiseEventOptions, SendOptions.SendReliable);
    }

    public void SendVrCreation(string data)
    {
        object[] content = new object[] { data}; // Array contains the target position and the IDs of the selected units
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others }; // You would have to set the Receivers to All in order to receive this event on the local client as well
        PhotonNetwork.RaiseEvent((byte)PhotonVrInteraction.EventIDEnum.VRCreation, content, raiseEventOptions, SendOptions.SendReliable);
    }
}
#endif
