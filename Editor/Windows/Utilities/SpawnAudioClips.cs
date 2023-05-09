using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SpawnAudioClipWindow : EditorWindow
{
    public static void ShowWindow() => GetWindow<SpawnAudioClipWindow>("AudioClips").ShowPopup();

    private List<AudioClip> m_Clips;

    private void OnGUI()
    {
        if (m_Clips == null)
        {
            m_Clips = Selection.GetFiltered<AudioClip>(SelectionMode.DeepAssets).ToList();
            m_Clips.Sort(Comparisons.NaturalByName);

            int controlID = GUIUtility.GetControlID(FocusType.Passive);
            EditorGUIUtility.ShowObjectPicker<AudioSource>(null, true, "", controlID);
        }

        EditorGUILayout.LabelField("Select an AudioSource to clone");

        string commandName = Event.current.commandName;
        switch (Event.current.commandName)
        {
            case "ObjectSelectorClosed":
                var go = EditorGUIUtility.GetObjectPickerObject() as GameObject;
                var audioSource = go?.GetComponent<AudioSource>();
                if (audioSource != null)
                {
                    SpawnAudioClips(audioSource, m_Clips);
                }
                Close();
                break;
            default:
                break;
        }
    }

    private static void SpawnAudioClips(AudioSource source, List<AudioClip> clips)
    {
        foreach (var clip in clips)
        {
            var clone = Utils.Unity.Clone<AudioSource>(source.gameObject, source.transform.parent);
            clone.name = clip.name;
            clone.clip = clip;
        }
        Debug.Log($"Spawned {clips.Count} AudioClips\n");
    }
}
