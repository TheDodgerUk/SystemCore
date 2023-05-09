#if Photon
#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MonitorTrainer;

using UnityEngine.UI;
using System.IO;

[CustomEditor(typeof(PhotonMultiplayerMono))]
public class PhotonMultiplayerMonoEditor : UnityEditor.Editor
{

    public override void OnInspectorGUI()
    {
        PhotonMultiplayerMono helper = (PhotonMultiplayerMono)target;

        if (GUILayout.Button("OwnersAvatarSendData TRUE"))
        {
            helper.OwnersAvatarSendData(true);
        }

        if (GUILayout.Button("OwnersAvatarSendData FALSE"))
        {
            helper.OwnersAvatarSendData(false);
        }

    }


}
#endif
#endif