using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable, MetaData(MetaDataType.ContentScript)]
public class ContentScriptMetaData : MetaData
{
    public enum AddScript
    {
        AnalogueClock,
        DigitalClock,
        DeskFan,
        NurfGun,
        SuperSoaker,
        FireExtinguisher,
        EnvironmentSpecials,
    }

    [SerializeField]
    public AddScript m_AddScript = AddScript.AnalogueClock;


    List<EnvironmentEffect> m_EnvironmentEffect = new List<EnvironmentEffect>();

#if !CATALOG_PROGRAM
    public void AddItem(GameObject root)
    {

        switch (m_AddScript)
        {
            case AddScript.AnalogueClock:
                root.ForceComponent<AnalogueClock>();
                break;

            case AddScript.DigitalClock:
                root.ForceComponent<DigitalClock>();
                break;
            case AddScript.DeskFan:
                root.ForceComponent<DeskFan>();
                break;
            case AddScript.NurfGun:
                root.ForceComponent<NurfGun>();
                break;
            case AddScript.SuperSoaker:
                root.ForceComponent<SuperSoaker>();
                break;              
            case AddScript.FireExtinguisher:
                root.ForceComponent<FireExtinguisher>();
                break;

            case AddScript.EnvironmentSpecials:
                Transform[] trans = root.GetComponentsInChildren<Transform>();
                for(int i = 0; i < trans.Length; i++)
                {
                    EnvironmentSpecials(trans[i]);
                }
                break;

            default:
                Debug.LogError("Cannot find item");
                break;
        }
    }

    private void EnvironmentSpecials(Transform obj)
    {
        if (obj.name.StartsWith("[SeatingDummies]") == true)
        {
            var entry = Core.Catalogue.GetEntry("5af4a7c4-abd4-4098-b764-9ac600db1610");
            Core.Scene.SpawnObject(entry, (created) =>
            {
                obj.AddComponent<DrawMeshSeating>().Init(created.gameObject, created.gameObject.GetComponentInChildren<Renderer>().material);
                created.gameObject.SetActive(false);
            });
        }
    }

#endif
}