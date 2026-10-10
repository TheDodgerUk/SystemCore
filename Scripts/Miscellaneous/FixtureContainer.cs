using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FixtureContainer : MonoSingleton<FixtureContainer>
{
    private const string NOT_USE = "keep";
#if HouseBuilder
    private Dictionary<string, Fixtures.Fixture> m_FixtureDictionary = new Dictionary<string, Fixtures.Fixture>();
#endif
    private List<string> m_FixtureNamesList = new List<string>();
    public bool AssetsLoaded { get; private set; }

    public string Location { get; private set; }

    public void Initialise()
    {
        AssetsLoaded = false;
        Location = Core.AssetBundlesRef.TextAssetBundleRef.AssetBundleLocation;
        Core.AssetBundlesRef.TextAssetBundleRef.GetItemList(Core.Mono, (files) =>
        {
            TaskAction task = new TaskAction(files.Count, () =>
            {
#if HouseBuilder
                foreach (var item in m_FixtureDictionary.Keys)
                {
                    m_FixtureNamesList.Add(item.ToLower());
                }
#endif
                AssetsLoaded = true;
            });

            for (int i = 0; i < files.Count; i++)
            {
                int localIndex = i;
                Core.AssetBundlesRef.TextAssetBundleRef.GetItem(Core.Mono, files[localIndex], (item) =>
                {
                    if (files[localIndex] != NOT_USE)
                    {
                        TextAsset textAsset = item;
                        if (textAsset != null)
                        {
#if HouseBuilder
                            var data = Fixtures.Fixture.FromJson(textAsset.text, files[localIndex]);
                            if (data != null)
                            {
                                m_FixtureDictionary.Add(files[localIndex].ToLower(), data);
                            }
#endif
                        }
                    }
                    task.Increment();
                });
            }
        });
    }

#if HouseBuilder
    public Fixtures.Fixture GetFixture(string fixture)
    {
        fixture = fixture.ToLower();
        if (m_FixtureDictionary.ContainsKey(fixture) == true)
        {
            return m_FixtureDictionary[fixture];
        }
        else
        {
            Debug.LogError($"Cannot find {fixture}, looking in {Location}");
            return null;
        }        
    }
    public Fixtures.Fixture GetFixture(int index) => m_FixtureDictionary[m_FixtureNamesList[index].ToLower()];
    public string GetFixtureName(int index) => m_FixtureNamesList[index];
    public List<string> GetFixtureList() => m_FixtureNamesList.Clone();

    public List<string> GetModes(string fixture)
    {
        var fix = m_FixtureDictionary[fixture.ToLower()];
        List<string> modes = new List<string>();
        foreach(var item in fix.Modes)
        {
            modes.Add(item.ShortName);
        }

        return modes;
    }

    public List<string> GetModes(int index)
    {
        var fix = m_FixtureDictionary[m_FixtureNamesList[index].ToLower()];
        List<string> modes = new List<string>();
        foreach (var item in fix.Modes)
        {
            if (string.IsNullOrEmpty(item.ShortName) == false)
            {
                modes.Add(item.ShortName);
            }
            else
            {
                modes.Add(item.Name);
            }
        }

        return modes;
    }

    public List<string> GetModeChannels(int fixture, int modeIndex) => GetModeChannels(m_FixtureNamesList[fixture], modeIndex);

    public List<string> GetModeChannels(string fixture, int modeIndex)
    {
        var fix = m_FixtureDictionary[fixture.ToLower()];

        var mode = fix.Modes[modeIndex];
        List<string> channelNames = new List<string>();

        for(int i = 0; i < mode.Channels.Count; i++)
        {
            channelNames.Add(mode.Channels[i].String);
        }
       
        return channelNames;
    }
#endif
}
