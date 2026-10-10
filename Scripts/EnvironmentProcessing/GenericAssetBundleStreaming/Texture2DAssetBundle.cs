using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Texture2DAssetBundle : GenericAssetBundle<Texture2D>
{
    public const string THUMBNAIL_MATERIAL_PREFIX = "ThumbnailMaterial_";
    public const string THUMBNAIL_VIDEO_PREFIX = "ThumbnailVideo_";

    protected override string ASSET_TYPE => nameof(Texture2DAssetBundle);


    public void GetItemSprite(MonoBehaviour host, string name, Action<Sprite> callback)
    {
        GetItem(host, name, (text2d) =>
        {
            if (text2d != null)
            {
                Sprite sp = Sprite.Create((Texture2D)text2d, new Rect(0, 0, text2d.width, text2d.height), Vector2.zero);
                sp.name = name;
                callback.Invoke(sp);
            }
            else
            {
                callback.Invoke(null);
            }
        });
    }
}
