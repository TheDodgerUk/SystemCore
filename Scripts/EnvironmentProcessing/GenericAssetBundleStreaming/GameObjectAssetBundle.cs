using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectAssetBundle : GenericAssetBundle<GameObject>
{
    protected override string ASSET_TYPE => nameof(GameObjectAssetBundle);

    public override void GetItem(MonoBehaviour host, string name, Action<GameObject> callback)
    {
        base.GetItem(host, name, (callbackObject) =>
        {
            callbackObject.SetActive(false);
            var newItem = GameObject.Instantiate(callbackObject);
            newItem.ReAssignChildrenRenderShaders();
            newItem.SetActive(true);
            callback?.Invoke(newItem);
        });
    }
}
                                                          