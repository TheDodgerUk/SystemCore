using System;

public interface ISaveable
{
    string RuntimeId { get; }

    void GenerateNewGuid();

    void OnPostLoad(SaveData saveData);

    void Load(SaveData saveData, Action callback);

    SaveData Save();
}
