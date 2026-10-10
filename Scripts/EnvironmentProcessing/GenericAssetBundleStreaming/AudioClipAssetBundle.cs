using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioClipAssetBundle : GenericAssetBundle<AudioClip>
{
    protected override string ASSET_TYPE => nameof(AudioClipAssetBundle);
}

