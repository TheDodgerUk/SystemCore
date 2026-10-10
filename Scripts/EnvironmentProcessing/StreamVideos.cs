using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnvironmentHelpers
{
    public class StreamVideos : MonoBehaviour
    {
        private ScreenVideoPlayer m_VideoPlayer;

        public void Init()
        {
            var children = transform.GetAllChildren();
            for (int i = 0; i < children.Count; i++)
            {
                string name = children[i].name;
                if (true == name.Contains("[StreamVideo]"))
                {
                    string[] data = name.Split('_');
                    if (data.Length > 1)
                    {
                        GameObject child = children[i].gameObject;
                        var videoPlayer = child.ForceComponent<ScreenVideoPlayer>();
                        videoPlayer.Init(new List<GameObject>() { child });
                        videoPlayer.PlayVideo(data[1], null, null);
                    }
                }
            }
        }
    }
}