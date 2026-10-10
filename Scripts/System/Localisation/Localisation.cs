using System;
using System.Collections.Generic;

public class Localisation : Singleton<Localisation>
{
    private static UnityEngine.MonoBehaviour m_Mono;
    private Dictionary<string, string> m_Dictionary;

    public string this[string key]
    {
        get { return Get(key); }
        set { Set(key, value); }
    }

    public static Dictionary<string, string> LoadLanguage(Language language)
    {
        string path = GetLanguagePath(language);
        return Json.JsonNet.ReadFromFile<Dictionary<string, string>>(path);
    }

    public void LoadLanguage(UnityEngine.MonoBehaviour mono, Language language, Action callback)
    {
        m_Mono = mono;
        string path = GetLanguagePath(language);

#if UNITY_ANDROID || UNITY_IOS || UNITY_WEBGL
        Json.AndroidNet.ReadFromFileAsync<Dictionary<string, string>>(m_Mono, showError: false, path, JsonLibraryType.FullSerializer, (dictionary) =>
#else
        Json.JsonNet.ReadFromFileAsync<Dictionary<string, string>>(path, dictionary =>
#endif
        {
            m_Dictionary = dictionary;
            callback();
        });
    }

    public void SaveLanguage(Language language)
    {
        string path = GetLanguagePath(language);
        Json.Library.WriteToFile(m_Dictionary, path, true);
    }

    public void PurgeKeys(string startsWith)
    {
        var keys = new List<string>(m_Dictionary.Keys);
        for (int i = 0; i < keys.Count; ++i)
        {
            if (keys[i].ToLower().StartsWith(startsWith.ToLower()) == true)
            {
                m_Dictionary.Remove(keys[i]);
                keys.RemoveAt(i);
                --i;
            }
        }
    }

    public string Get(string key) => m_Dictionary.Get(key) ?? string.Empty;

    public void Set(string key, string value)
    {
        m_Dictionary[key] = value;
    }

    private static string GetLanguagePath(Language language)
    {
        return $"{GlobalConsts.BUNDLE_STREAMING_LANGUAGES}{language.ToString().ToLower()}";
    }

    public enum Language { English };
}
