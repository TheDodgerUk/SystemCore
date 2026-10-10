using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class ComponentList<T> : List<T> where T : Component
{
    public void DestroyAll()
    {
        for (int i = 0; i < base.Count; i++)
        {
            UnityEngine.Object.Destroy(base[i].gameObject);
        }
        base.Clear();
    }
}
