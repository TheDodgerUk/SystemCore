using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;


public class NetworkingUsers
{
    private const string SAVE_NAME = "Users_NetworkingUsers"+GlobalConsts.JSON;
    public NetworkingUser CurrentUser { get; private set; }

    public class NetworkingUser
    {
        public string m_UserName;
        public string m_Password;
        public string m_Email;
    }

    public class NetworkingUserList
    {
        public List<NetworkingUser> m_User = new List<NetworkingUser>();
    }

    private NetworkingUserList m_NetworkingUserList;

    public NetworkingUsers()
    {
        LoadUsers((list) =>
        {
            m_NetworkingUserList = list;

            if (m_NetworkingUserList == null)
            {
                m_NetworkingUserList = new NetworkingUserList();
                m_NetworkingUserList.m_User = new List<NetworkingUser>();
                SaveUsers(null);
            }
            SetCurrentUserFromSystem();
        });
    }


    public void AddUser(NetworkingUser newUser) => m_NetworkingUserList.m_User.Add(newUser);
    public NetworkingUserList GetUsers() => m_NetworkingUserList;

    public void SaveUsers(Action<bool> callback) => Core.Network.SaveManifestJsonFile<NetworkingUserList>(SAVE_NAME, m_NetworkingUserList, callback);
    public void LoadUsers(Action<NetworkingUserList> callback) => Core.Network.LoadManifestJsonFile<NetworkingUserList>(SAVE_NAME, callback);
    public bool DoesUserExist(NetworkingUser user)
    {
        if(m_NetworkingUserList == null || m_NetworkingUserList.m_User == null || user == null)
        {
            return false;
        }
        return m_NetworkingUserList.m_User.Exists(e => e.m_UserName.ToLower() == user.m_UserName.ToLower());
    }

    public bool IsValidLoginDetails(NetworkingUser user)
    {
        if (m_NetworkingUserList == null || m_NetworkingUserList.m_User == null || user == null)
        {
            return false;
        }
        return m_NetworkingUserList.m_User.Exists(e => (e.m_UserName.ToLower() == user.m_UserName.ToLower()) && (e.m_Password == user.m_Password));
    }

    public int UserCount => m_NetworkingUserList.m_User.Count;

    public bool SetCurrentUser(NetworkingUser user)
    {
        string jsonString = Json.JsonNet.WriteToText<NetworkingUser>(user, true);
        if (DoesUserExist(user) == true)
        {
            PlayerPrefs.SetString(SAVE_NAME, jsonString);
            CurrentUser = user;
            return true;
        }
        return false;
    }

    public void ClearCurrentUser()
    {
        CurrentUser = null;
        PlayerPrefs.SetString(SAVE_NAME, "");
        DebugCurrentUser();
    }

    public void SetCurrentUserFromSystem()
    {
        string jsonString = PlayerPrefs.GetString(SAVE_NAME);
        CurrentUser = Json.JsonNet.ReadFromText<NetworkingUser>(jsonString);
        if (DoesUserExist(CurrentUser) == false)
        {
            ClearCurrentUser();
        }

        DebugCurrentUser();
    }

    public void DebugPrintUserNames()
    {
        foreach (var item in m_NetworkingUserList.m_User)
        {
            Debug.LogError($"m_UserName: {item.m_UserName}   m_Password: {item.m_Password}   m_Email:{item.m_Email}");
        }
    }

    public void DebugClearUsersNames()
    {
#if UNITY_EDITOR
        if (UnityEditor.EditorUtility.DisplayDialog("ClearUsersNames?", "Do you want to ClearUsersNames ", "OK") == true)
        {
            UnityEditor.EditorUtility.DisplayDialog("ClearUsersNames", "Code commented out for now ", "OK");
            //ClearCurrentUser();
            //m_NetworkingUserList.m_User.Clear();
            //SaveUsers(null);
        }
#endif
    }

    private void DebugCurrentUser()
    {
        if(CurrentUser == null)
        {
            ConsoleExtra.Log($"DebugCurrentUser is null", null, ConsoleExtraEnum.EDebugType.StartUp);
        }
        else
        {
            ConsoleExtra.Log($"CurrentUser  {CurrentUser.m_UserName}, IGNORE THIS", null, ConsoleExtraEnum.EDebugType.StartUp);
        }
    }
}

