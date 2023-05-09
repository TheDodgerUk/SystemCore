using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectLocal : GenericLocal<GameObject>
{
    protected override string ASSET_TYPE => nameof(GameObjectLocal);
    public override string FileExtention => ".prefab";

    public virtual void GetItemInstantiated(string name, Action<GameObject> callback)
    {
        GetItem(name, (item) =>
        {
            if (item != null)
            {
                var created = UnityEngine.Object.Instantiate(item);
                callback?.Invoke(created);
            }
            else
            {
                callback?.Invoke(null);
            }
        });
    }
}
