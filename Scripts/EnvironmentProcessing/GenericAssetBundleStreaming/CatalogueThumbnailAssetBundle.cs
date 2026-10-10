using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatalogueThumbnailAssetBundle : GenericAssetBundle<Texture2D>
{
    public override string AssetBundleLocation => "catalogue/cataloguethumbnail";

    protected override bool ForceDownload => true;
}
