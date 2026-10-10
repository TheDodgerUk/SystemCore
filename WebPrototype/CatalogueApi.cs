using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CatalogueApi : MonoBehaviour
{
    //private const string Ip = "10.102.29.22:3000";     // server
    //private const string Ip = "10.102.50.137:8080";     // josh
    private const string Ip = "10.102.50.179:3000";     // me

    private Dictionary<string, List<WebItem>> m_ItemsByCategories = new Dictionary<string, List<WebItem>>();
    private Dictionary<string, WebItem> m_ItemsById = new Dictionary<string, WebItem>();
    private List<string> m_Categories;

    public void Categories(Action<List<string>> callback)
    {
        if (m_Categories == null)
        {
            Get("categories", r =>
            {
                m_Categories = r?.Values<string>()?.ToList();
                callback(m_Categories);
            });
        }
        else
        {
            callback(m_Categories);
        }
    }

    public void Items(string category, Action<List<WebItem>> callback)
    {
        var items = m_ItemsByCategories.Get(category);
        if (items == null)
        {
            Get($"categories/{EncodeUrl(category)}", r =>
            {
                items = Deserialise<List<WebItem>>(r);
                m_ItemsByCategories[category] = items;
                callback(items);
            });
        }
        else
        {
            callback(items);
        }
    }

    public void Item(string itemId, Action<WebItem> callback)
    {
        var item = m_ItemsById.Get(itemId);
        if (item == null)
        {
            Get($"items/{itemId}", r =>
            {
                item = Deserialise<WebItem>(r);
                m_ItemsById[itemId] = item;
                callback(item);
            });
        }
        else
        {
            callback(item);
        }
    }

    class WeightedItem { public int Weight; public string Id; }

    public void Related(string itemId, Action<List<WebItem>> callback)
    {
        Get($"items/{itemId}/related", response =>
        {
            int count = response.Count();
            var items = new List<WebItem>(count);
            var weighted = new List<WeightedItem>(count);
            for (int i = 0; i < count; ++i)
            {
                string id = (string)response[i]["item"]["id"];
                int weight = (int)response[i]["weight"];
                weighted.Add(new WeightedItem { Id = id, Weight = weight });
            }

            weighted = weighted.GroupBy(w => w.Weight).SelectMany(g => g.Take(15).ToList()).ToList();

            SequentialAction.List(weighted, (w, onTick) =>
            {
                Item(w.Id, item =>
                {
                    item.Weight = w.Weight;
                    items.Add(item);
                    onTick();
                });
            }, () => callback(items));
        });
    }

    private void Get(string urlSuffix, ResponseHandler callback)
    {
        new RestRequest(this, $"http://{Ip}/{urlSuffix}").Get(callback);
    }

    private static T Deserialise<T>(JToken json) => Json.JsonNet.ReadFromText<T>(json?.ToString());

    private static string EncodeUrl(string original) => Uri.EscapeDataString(original);
}

public class WebItem
{
    [JsonProperty("id")]
    public readonly string Guid;
    [JsonProperty("model")]
    public readonly string Model;
    [JsonProperty("brand")]
    public readonly string Brand;
    [JsonProperty("category")]
    public readonly string Category;
    [JsonProperty("skus")]
    public readonly string[] Skus;

    public int Weight { get; set; }
}
