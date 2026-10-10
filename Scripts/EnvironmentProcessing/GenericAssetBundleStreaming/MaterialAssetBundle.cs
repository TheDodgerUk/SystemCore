using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialAssetBundle : GenericAssetBundle<Material>
{
    protected override  string ASSET_TYPE => nameof(MaterialAssetBundle);


    protected override void ExtraStepsOnCreatedItem(Material item) 
    {
        // FIX_SHADERS
//#if UNITY_ANDROID || UNITY_WEBGL || UNITY_EDITOR
        item.shader = Shader.Find(item.shader.name);
//#endif
    }

}
