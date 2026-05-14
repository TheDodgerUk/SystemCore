#if Photon
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.Unity;
#endif

using System;
using System.Collections.Generic;
using UnityEngine;

public class PhotonMultiplayer
{
#if Photon
    private PhotonMultiplayerMono m_PhotonMultiplayerMono;
#endif

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

#if Photon
    public PhotonMultiplayer(PhotonMultiplayerMono photonMultiplayerMono)
    {
        m_PhotonMultiplayerMono = photonMultiplayerMono;
    }
#else
    public PhotonMultiplayer(object photonMultiplayerMono)
    {
    }
#endif

    public void InitialiseForApp()
    {
#if Photon
        PhotonNetwork.LocalPlayer.NickName = System.Net.Dns.GetHostName();
        m_PhotonMultiplayerMono.InitialiseForApp();
#endif
    }

    public void Initialise(NetworkType networkType)
    {
#if Photon
        m_PhotonMultiplayerMono.SetNetworkType(networkType);
#endif
    }

#if Photon
    public void ChangeRoom(string roomName, RoomOptions roomOptions = null)
    {
        m_PhotonMultiplayerMono.ChangeRoom(roomName, roomOptions);
    }
#else
    public void ChangeRoom(string roomName, object roomOptions = null)
    {
    }
#endif

    public void LeaveRoom()
    {
#if Photon
        m_PhotonMultiplayerMono.LeaveRoom();
#endif
    }

    public void JoinLobby()
    {
#if Photon
        m_PhotonMultiplayerMono.JoinLobby();
#endif
    }

#if Photon
    public void OnOwnerChanged(Action<Player> callback)
    {
        m_PhotonMultiplayerMono.m_OnOwnerChanged += callback;
    }

    public void OnRoomPlayersChanged(Action<List<Player>> callback)
    {
        m_PhotonMultiplayerMono.m_PlayersChanged += callback;
    }

    public void OnLobbyListChanged(Action<List<RoomInfo>, List<RoomInfo>> callback)
    {
        m_PhotonMultiplayerMono.m_LobbyListChanged += callback;
    }

    public RoomOptions RoomOptionsRef => m_PhotonMultiplayerMono.m_RoomOptions;
    public Photon.Realtime.Room CurrentRoom => PhotonNetwork.CurrentRoom;
    public PhotonView PhotonViewOwnerRef => m_PhotonMultiplayerMono.m_PhotonViewOwner;
    public Player MySelf => PhotonNetwork.LocalPlayer;
#else
    public void OnOwnerChanged(Action<object> callback) { }
    public void OnRoomPlayersChanged(Action<List<object>> callback) { }
    public void OnLobbyListChanged(Action<List<object>, List<object>> callback) { }

    public object RoomOptionsRef => null;
    public object CurrentRoom => null;
    public object PhotonViewOwnerRef => null;
    public object MySelf => null;
#endif

    public void OnFail(Action<NetworkFail> fail)
    {
#if Photon
        m_PhotonMultiplayerMono.m_NetworkFail += fail;
#endif
    }

    public void OwnersAvatarSendData(bool enable)
    {
#if Photon
        m_PhotonMultiplayerMono.OwnersAvatarSendData(enable);
#endif
    }

#if Photon
    public void OnOwnerLoaded(Action<Player> loaded)
    {
        m_PhotonMultiplayerMono.m_OnLoaded = loaded;
    }
#else
    public void OnOwnerLoaded(Action<object> loaded)
    {
    }
#endif

#if Photon
    public void AddItemToSync(MonoBehaviourPun view)
    {
        foreach (var item in PhotonNetwork.PhotonViewCollection)
        {
            if (item.observableSearch == PhotonView.ObservableSearch.Manual)
            {
                item.ObservedComponents.RemoveAll(e => e == null);

                if (item.ObservedComponents.Contains(view) == false)
                    item.ObservedComponents.Add(view);
            }
        }
    }

    public bool IsItemInSync(MonoBehaviourPun view)
    {
        foreach (var item in PhotonNetwork.PhotonViewCollection)
        {
            if (item.observableSearch == PhotonView.ObservableSearch.Manual)
                return item.ObservedComponents.Contains(view);
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
                item.ObservedComponents.Remove(view);
                item.ObservedComponents.RemoveAll(e => e == null);
            }
        }
    }
#else
    public void AddItemToSync(MonoBehaviour view) { }
    public bool IsItemInSync(MonoBehaviour view) => false;
    public void RemoveItemToSync(MonoBehaviour view) { }
#endif

#if Photon
    public bool IsOwner => PhotonNetwork.LocalPlayer.IsMasterClient || PhotonNetwork.CurrentRoom == null;
    public bool IsOwnerInRoom => PhotonNetwork.InRoom;
#else
    public bool IsOwner => true;
    public bool IsOwnerInRoom => false;
#endif
}

#if Photon
public class PhotonMultiplayerMono : MonoBehaviourPunCallbacks
{
    // Keep your existing PhotonMultiplayerMono implementation here.
}
#else
public class PhotonMultiplayerMono : MonoBehaviour
{
    public void SetNetworkType(PhotonMultiplayer.NetworkType networkType) { }
    public void InitialiseForApp() { }
    public void JoinLobby() { }
    public void LeaveRoom() { }
    public void ChangeRoom(string roomName, object roomOptions = null) { }
    public void OwnersAvatarSendData(bool enable) { }
}
#endif