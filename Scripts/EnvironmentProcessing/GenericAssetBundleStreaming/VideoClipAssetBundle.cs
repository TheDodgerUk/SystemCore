using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoClipAssetBundle : GenericAssetBundle<VideoClip>
{
    protected override string ASSET_TYPE => nameof(VideoClipAssetBundle);
}
