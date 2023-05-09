using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GenericLocal<T> where T : UnityEngine.Object
{
    protected virtual string ASSET_TYPE => nameof(DecalsLocal);
    public virtual string FileExtention => ".mat";

    protected Dictionary<string, string> m_Items = new Dictionary<string, string>();
    protected const string RESOURCES = "resources/";


    public void SetList(List<string> items)
    {
        m_Items.Clear();
        foreach (var item in items)
        {
            if(item.CaseInsensitiveContains($"/{ASSET_TYPE}/") && item.EndsWith(FileExtention, StringComparison.CurrentCultureIgnoreCase) == true)
            {
                string fileName = Path.GetFileNameWithoutExtension(item);

                int index = item.IndexOf(RESOURCES) + RESOURCES.Length;
                string fullPath = item.Substring(index);

                fullPath = Path.Combine(Path.GetDirectoryName(fullPath), Path.GetFileNameWithoutExtension(fullPath));
                m_Items.Add(fileName.ToLower(), fullPath.ToLower());
            }
        }
    }




    public void GetItemList(Action<List<string>> callbackList)
    {
        List<string> keyList = new List<string>(m_Items.Keys);
        callbackList?.Invoke(keyList);
    }

    public void GetItemList(string startsWith, Action<List<string>> callbackList)
    {
        List<string> items = new List<string>();
        List<string> keyList = new List<string>(m_Items.Keys);
        foreach (var item in keyList)
        {
            if(item.StartsWith(startsWith, StringComparison.CurrentCultureIgnoreCase) == true)
            {
                items.Add(item);
            }
        }
        callbackList?.Invoke(items);
    }

    public virtual void GetItem(string name, Action<T> callback)
    {
        if (string.IsNullOrEmpty(name) == true)
        {
            callback?.Invoke(null);           
        }
        else
        {
            name = name.ToLower();
            string fileName = "";
            if (m_Items.ContainsKey(name) == true)
            {
                fileName = m_Items[name];
            }
            else
            {
                string newName = $"{name}{FileExtention}";
                if (m_Items.ContainsKey(newName) == true)
                {
                    fileName = m_Items[newName];
                }
                else
                {
                    Debug.LogError($"Coonot find: {name}");
                }
            }


            var item = Resources.Load(fileName, typeof(T)) as T;
            if (item == default(T))
            {
                Debug.LogError("Item is null");
            }
            callback.Invoke(item);
        }
    }
}