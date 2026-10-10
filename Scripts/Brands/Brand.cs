using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Brand : MonoSingleton<Brand>
{
    private const string AssetBundleLocation = "brands/logos";

    [SerializeField]
    private BrandData m_BrandData;
    private List<string> m_BrandNames = new List<string>();

    protected override void Awake()
    {
        base.Awake();
        string fileName = $"{GlobalConsts.BUNDLE_STREAMING_BRANDS}BrandData";


        Json.AndroidNet.ReadFromFileAsync<BrandData>(this, showError: false, fileName, JsonLibraryType.JsonNet, (data) =>
        {
            if (data == null)
            {
                Core.Network.LoadAssetJsonFile<BrandData>(fileName, false, (networkData) =>
                {
                    Initialise(networkData);
                });
            }
            else
            {
                Initialise(data);
            }
        });
    }

    private void Initialise(BrandData data)
    {
        m_BrandData = data;
        m_BrandData.BrandInfo.ForEach((b) => m_BrandNames.Add(b.BrandName));
    }

    public List<string> GetBrands => m_BrandNames.Clone();

    public void GetBrandLogo(MonoBehaviour host, string brand, Action<Texture2D> callback)
    {
        Core.Assets.LoadAsset(host, brand, AssetBundleLocation, callback, 0);
    }
}
