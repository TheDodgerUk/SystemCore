#if Photon

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.IO;
using System;

using Photon.Voice.Unity;
using static PhotonMultiplayer;
using Oculus.Platform.Models;
using Photon.Voice.Unity.UtilityScripts;
#if UNITY_ANDROID && VR_INTERACTION
using Oculus.Avatar2;
#endif

public class PhotonMultiplayer
{
    public class UserData
    {
        public bool IsOwner = false;
        public int Actor;
        public Color? Colour;
        public int ColourIndex = -1;
    }

    public class SetUserData
    {
        public bool IsOwner = false;
        public int Actor;
        public Color Colour = Color.white;
    }
    public enum NetworkType
    {
        FullPlayer,
        FullPlayerNoVoice
    }

    public enum NetworkFail
    {
        OnJoinRoomFailed,
        OnCreateRoomFailed,
        Lobby
    }

    private PhotonMultiplayerMono m_PhotonMultiplayerMono;
    private bool m_IsInitilised = false;

    public PhotonMultiplayer(PhotonMultiplayerMono PhotonMultiplayerMono)
    {
        m_PhotonMultiplayerMono = PhotonMultiplayerMono;
    }

    public void InitialiseForApp()
    {
        if(Core.Environment.HasOnEnvironmentLoadingComplete == false)
        {
            DebugBeep.LogError($"PhotonSetup need to be done After  Core.Environment.OnEnvironmentLoadingComplete, when all messages recived is done", DebugBeep.MessageLevel.High);

        }

        PhotonNetwork.LocalPlayer.NickName = System.Net.Dns.GetHostName();
#if UNITY_ANDROID && VR_INTERACTION && !UNITY_EDITOR 
        Oculus.Platform.Users.GetLoggedInUser().OnComplete(GetLoggedInUserCallback);
#endif
        m_PhotonMultiplayerMono.InitialiseForApp();
    }

    private void GetLoggedInUserCallback(Oculus.Platform.Message<User> message)
    {
        if (message.IsError == false)
        {
            PhotonNetwork.LocalPlayer.NickName = string.IsNullOrEmpty(message.Data.DisplayName) ? message.Data.OculusID : message.Data.DisplayName; // oculus user display name}
        }
    }

    public void Initialise(NetworkType networkType)
    {
        m_PhotonMultiplayerMono.SetNetworkType(networkType);
        m_IsInitilised = true;
    }

    public void ChangeRoom(string roomName,RoomOptions roomOptions = null)
    {
        m_PhotonMultiplayerMono.ChangeRoom(roomName, roomOptions = null);
    }

    public void LeaveRoom()
    {
        m_PhotonMultiplayerMono.LeaveRoom();
    }

    public void JoinLobby()
    {
        m_PhotonMultiplayerMono.JoinLobby();
    }

    public void OnOwnerChanged(Action<Player> callback)
    {
        m_PhotonMultiplayerMono.m_OnOwnerChanged += callback;
    }


    public void OnRoomPlayersChanged(Action<List<Player>> callback)
    {
        m_PhotonMultiplayerMono.m_PlayersChanged += callback;
    }

    public void OnLobbyListChanged(Action<List<RoomInfo>,List<RoomInfo>> allroomsAndUseable)
    {
        m_PhotonMultiplayerMono.m_LobbyListChanged += allroomsAndUseable;
    }

    public void OnFail(Action<NetworkFail> fail)
    {
        m_PhotonMultiplayerMono.m_NetworkFail += fail;
    }

    public RoomOptions RoomOptionsRef => m_PhotonMultiplayerMono.m_RoomOptions;

#if Photon
    

    public void OwnersAvatarSendData(bool enable) => m_PhotonMultiplayerMono.OwnersAvatarSendData(enable);

    
    public void OnOwnerLoaded(Action<Player> loaded) => m_PhotonMultiplayerMono.m_OnLoaded = loaded;
    // needed when teleporting 
#endif

    public void AddItemToSync(MonoBehaviourPun view)
    {
        foreach (var item in PhotonNetwork.PhotonViewCollection)
        {
            if (item.observableSearch == PhotonView.ObservableSearch.Manual)
            {
                item.ObservedComponents.RemoveAll(e => e == null);
                if (item.ObservedComponents.Contains(view) == false)
                {
                    item.ObservedComponents.Add(view);
                }
            }
        }
    }

    public bool IsItemInSync(MonoBehaviourPun view)
    {
        foreach (var item in PhotonNetwork.PhotonViewCollection)
        {
            if (item.observableSearch == PhotonView.ObservableSearch.Manual)
            {
                return (item.ObservedComponents.Contains(view) == true);
            }
        }
        return false;
    }

    public void RemoveItemToSync(MonoBehaviourPun view)
    {
        foreach (var item in PhotonNetwork.PhotonViewCollection)
        {
            if (item.observableSearch == PhotonView.ObservableSearch.Manual)
            {
                item.ObservedComponents.RemoveAll(e => e == null);
                if (item.ObservedComponents.Contains(view) == true)
                {
                    item.ObservedComponents.Remove(view);
                }
                item.ObservedComponents.RemoveAll(e => e == null);
            }
        }

    }
    public Photon.Realtime.Room CurrentRoom => PhotonNetwork.CurrentRoom;
    public PhotonView PhotonViewOwnerRef => m_PhotonMultiplayerMono.m_PhotonViewOwner;


    public bool IsOwner => (PhotonNetwork.LocalPlayer.IsMasterClient) ||( PhotonNetwork.CurrentRoom == null);

    public bool IsOwnerInRoom => PhotonNetwork.InRoom;
    public Player MySelf => PhotonNetwork.LocalPlayer;
}

public class PhotonMultiplayerMono : MonoBehaviourPunCallbacks
{
    public enum RoomEnum
    {
        ChangeRoom, 
        ToLobby, 
    }

    // Start is called before the first frame update
    private bool m_LobbyJoined = false;

    NetworkType? m_NetworkType = null;

    private string m_ChangeRoom = "";
    private Recorder m_Recorder;

    public Action<Player> m_OnOwnerChanged;
    public Action<List<Player>> m_PlayersChanged;
    public Action<List<RoomInfo>, List<RoomInfo>> m_LobbyListChanged;
    public Action<NetworkFail> m_NetworkFail;

    public RoomOptions m_RoomOptions = new RoomOptions();

    private RoomEnum m_RoomEnumRef = RoomEnum.ToLobby;
    private bool m_IsConnected = false;

    public Action<Player> m_OnLoaded;

    public void SetNetworkType(NetworkType networkType) => m_NetworkType = networkType;

#if VR_INTERACTION
    private NetworkPlayerGloves m_OwnerNetworkPlayer;
    private NetworkPlayerAvatar m_OwnerNetworkPlayerAvatar;
#endif
    public PhotonView m_PhotonViewOwner;
    private InputSystem m_InputSystem;

    public void OwnersAvatarSendData(bool enable)
    {
        bool isDone = false;
#if VR_INTERACTION
        if (m_OwnerNetworkPlayer != null)
        {
            isDone = true;
            m_OwnerNetworkPlayer.SetUpdateOwnerData(enable);
        }

        if (m_OwnerNetworkPlayerAvatar != null)
        {
            isDone = true;
            m_OwnerNetworkPlayerAvatar.SetUpdateOwnerData(enable);
        }

        if (isDone == false && m_InputSystem != null)
        {
            Debug.LogError($"OwnersAvatarSendData cannot find the correct script");
        }
#endif
    }


    public void InitialiseForApp()
    {
        m_Recorder = Core.Mono.GetComponentInChildren<Recorder>(true);
        PhotonNetwork.ConnectUsingSettings();
        //nConnectedToMaster gets called  when its all connected
    }

    public void JoinLobby() => PhotonNetwork.JoinLobby();
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("OnJoinedLobby");
        if (m_IsConnected == false)
        {
            m_IsConnected = true;
            return;
        }
        
        m_LobbyJoined = true;
        if (m_RoomEnumRef == RoomEnum.ChangeRoom)
        {
            PhotonNetwork.JoinOrCreateRoom(m_ChangeRoom, m_RoomOptions, null);
        }
        else
        {
            m_PlayersChanged?.Invoke(PhotonNetwork.PlayerList.ToList());
        }
    }

    public override void OnErrorInfo(ErrorInfo errorInfo)
    {
        Debug.LogError($"OnErrorInfo {errorInfo}");
    }

    public override void OnCreatedRoom()
    {
        Debug.LogError($"OnCreatedRoom   {PhotonNetwork.CurrentRoom.Name}");

        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            Debug.Log($"OnCreatedRoom PhotonNetwork.PlayerList   {PhotonNetwork.PlayerList[i].UserId}");
        }
    }

    public override void OnJoinedRoom()
    {
        if(m_InputSystem == null)
        {
            
            m_InputSystem = InputManagerVR.Instance.GetComponentInChildren<InputSystem>();
        }
        string networkStringType = "";

        if(m_NetworkType == null)
        {
            Debug.LogError($"networkType has not been initilised  PhotonSetup.Instance.Initialise(null, PhotonSetup.NetworkType.FullPlayerNoVoice);");
        }

        switch (m_NetworkType)
        {
            case NetworkType.FullPlayer:
                networkStringType = m_InputSystem.PHOTON_NETWORK_PLAYER;
                m_Recorder.SetActive(true);
                break;
            case NetworkType.FullPlayerNoVoice:
                networkStringType = m_InputSystem.PHOTON_NETWORK_PLAYER;
                m_Recorder.SetActive(false);
                break;
            default:
                break;
        }

        GameObject created = PhotonNetwork.Instantiate(networkStringType, Vector3.zero, Quaternion.identity, 0);
#if UNITY_ANDROID && VR_INTERACTION
    
        var fingers = created.GetComponentsInChildren<PhotonTransformFingers>(true);
        if(fingers.Length != 2)
        {
            Debug.LogError($"PhotonTransformFingers there should be 2, can only find number: {fingers.Length}");
            foreach (var item in fingers)
            {
                Debug.LogError($"PhotonTransformView {item.name}  {item.gameObject.GetGameObjectPath()}");
            }
        }

        var views = created.GetComponentsInChildren<PhotonTransformView>(true);
        if (views.Length != 3)
        {
            Debug.LogError($"PhotonTransformView there should be 3, can only find number: {views.Length}");
            foreach (var item in views)
            {
                Debug.LogError($"PhotonTransformView {item.name}  {item.gameObject.GetGameObjectPath()}");
            }
        }


        var entity = created.GetComponent <SampleAvatarEntity>();
        if (entity != null)
        {
            if(entity._deferLoading == false)
            {
                Debug.LogError("entity._deferLoading must be true");
            }

            if (entity._autoCdnRetry == false)
            {
                Debug.LogError("entity._autoCdnRetry must be true");
            }
            if (entity._autoCheckChanges == false)
            {
                Debug.LogError("entity._autoCheckChanges must be true");
            }

            if (entity.IsLocal == true)
            {
                Debug.LogError("entity.IsLocal must be false");
            }           
        }

        var rigidbodys = created.GetComponentsInChildren<Rigidbody>(true).ToList();
        foreach (var item in rigidbodys)
        {
            item.useGravity = false;
            item.isKinematic = true;
            item.transform.ClearLocals();
        }

        var view = created.GetComponent<PhotonView>();
        if (view != null)
        {
            if (view.observableSearch != PhotonView.ObservableSearch.Manual)
            {
                Debug.LogError("view.observableSearch != PhotonView.ObservableSearch.Manual  , must be set to manual");
            }
        }

        created.name = $"{networkStringType}: OWNER:{view.Owner.IsMasterClient}, ActorNumber {view.Owner.ActorNumber} ";
        if (view.AmOwner == true)
        {
            m_PhotonViewOwner = view;
            m_OwnerNetworkPlayer = view.GetComponentInChildren<NetworkPlayerGloves>(true);
            m_OwnerNetworkPlayerAvatar = view.GetComponentInChildren<NetworkPlayerAvatar>(true);

            if (m_OwnerNetworkPlayer != null && m_OwnerNetworkPlayerAvatar != null)
            {
                Debug.LogError("Cannot have BOTH NetworkPlayerGloves && NetworkPlayerAvatar, on same MultiplayerSync");
            }
            if (m_OwnerNetworkPlayer == null && m_OwnerNetworkPlayerAvatar == null)
            {
                Debug.LogError($"Cannot have NON NetworkPlayerGloves && NetworkPlayerAvatar, on MultiplayerSync Look at this Prefab NOW: {networkStringType}");
            }
            m_OnLoaded?.Invoke(view.Owner);

            var head = created.transform.Find("Head");
            VrInteractionFood.AttachFoodCollider(head.gameObject);
        }


        for(int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            Debug.Log($"OnJoinedRoom PhotonNetwork.PlayerList   {PhotonNetwork.PlayerList[i].UserId}");
            Core.VisualLoggerRef.SetInformation(VisualLogger.InformationType.Avatars, VisualLogger.Information.Information_5, $"PlayerList count {PhotonNetwork.PlayerList.Length}");
        }
        m_PlayersChanged?.Invoke(PhotonNetwork.PlayerList.ToList());

        Debug.LogError($"OnJoinedRoom   {PhotonNetwork.CurrentRoom.Name}");

        if (m_OwnerNetworkPlayer != null)
        {
            Debug.LogError("Head change colour here");
            var list = PhotonNetwork.PlayerList.ToList();
            int colourIndex = list.FindIndex(e => e.ActorNumber == Core.PhotonMultiplayerRef.MySelf.ActorNumber);

            int correctIndex = colourIndex.Wrap(GlobalConsts.GLOVE_COLOURS);
            Color newColour = GlobalConsts.GLOVE_COLOURS[correctIndex];
            var render = m_OwnerNetworkPlayer.gameObject.SearchComponent<Renderer>("Sphere");
            render.material.color = newColour;
        }
#endif
    }




    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError("OnJoinRoomFailed");
        base.OnJoinRoomFailed(returnCode, message);
        m_NetworkFail?.Invoke(NetworkFail.OnJoinRoomFailed);
    }


    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
        Debug.LogError("OnCreateRoomFailed");
        m_NetworkFail?.Invoke(NetworkFail.OnJoinRoomFailed);
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.LogError($"OnPlayerEnteredRoom   {PhotonNetwork.CurrentRoom.Name}");

        base.OnPlayerEnteredRoom(newPlayer);
        m_PlayersChanged?.Invoke(PhotonNetwork.PlayerList.ToList());
    }


    public override void OnPlayerLeftRoom(Player newPlayer)
    {
        Debug.Log($"OnPlayerLeftRoom   {PhotonNetwork.CurrentRoom.Name}");

        base.OnPlayerLeftRoom(newPlayer);
        m_PlayersChanged?.Invoke(PhotonNetwork.PlayerList.ToList());

    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        Debug.LogError("OnLeftRoom");
        m_PlayersChanged?.Invoke(PhotonNetwork.PlayerList.ToList());
        JoinLobby();
    }


    public void LeaveRoom()
    {
        m_RoomEnumRef = RoomEnum.ToLobby;
        PhotonNetwork.LeaveRoom(true);
    }

    public void ChangeRoom(string roomName, RoomOptions roomOptions = null)
    {
        m_ChangeRoom = roomName;
        m_RoomEnumRef = RoomEnum.ChangeRoom;
        if(PhotonNetwork.InLobby == false)
        {
            Debug.LogError($"Not in lobby, force join");
            PhotonNetwork.JoinLobby();
        }
        this.WaitUntil(5, () => IsReady(), () =>
        {
            Debug.LogError($"ChangeRoom {roomName} ");
            if (PhotonNetwork.CurrentRoom != null)
            {
                Debug.LogError($"PhotonNetwork.LeaveRoom()");
                PhotonNetwork.LeaveRoom(true);
            }
            else
            {
                Debug.LogError($"JoinOrCreateRoom");
                if (roomOptions == null)
                {
                    var findRoom = m_CurrentRoomList.Find(e => e.Name == m_ChangeRoom);
                    if(findRoom != null)
                    {
                        PhotonNetwork.JoinRoom(m_ChangeRoom);
                    }
                    else
                    {
                        PhotonNetwork.CreateRoom(m_ChangeRoom, m_RoomOptions, null);
                    }
                    
                }
                else
                {
                    var findRoom = m_CurrentRoomList.Find(e => e.Name == m_ChangeRoom);
                    if (findRoom != null)
                    {
                        PhotonNetwork.JoinRoom(m_ChangeRoom);
                    }
                    else
                    {
                        PhotonNetwork.CreateRoom(m_ChangeRoom, roomOptions, null);
                    }
                }
                
            }
        });
    }


    private bool IsReady()
    {
        bool ss = PhotonNetwork.InLobby;
        bool dd = PhotonNetwork.IsConnectedAndReady;
        return (PhotonNetwork.IsConnectedAndReady == true && PhotonNetwork.InLobby == true);
    }
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        base.OnMasterClientSwitched(newMasterClient);
        m_OnOwnerChanged?.Invoke(newMasterClient);
    }

    private List<RoomInfo> m_CurrentRoomList = new List<RoomInfo>();
    private List<RoomInfo> m_CurrentSeeableRoomList = new List<RoomInfo>();
    public override void OnRoomListUpdate(List<RoomInfo> roomlist)
    {
        base.OnRoomListUpdate(roomlist);
        m_CurrentRoomList = roomlist;
        m_CurrentSeeableRoomList = roomlist;
        m_CurrentSeeableRoomList.RemoveAll(e => e.RemovedFromList == true);
        m_LobbyListChanged?.Invoke(roomlist, m_CurrentSeeableRoomList);
    }



    private void OnDestroy()
    {
        PhotonNetwork.LeaveRoom(true);
    }

}
#endif