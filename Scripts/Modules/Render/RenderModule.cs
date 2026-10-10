using System;
using System.Collections.Generic;
using UnityEngine;

public class RenderModule : MetaDataModule<RenderMetaData, RenderModuleSaveData>
{
    public Transform CollisionParent => m_CollisionRoot;
    public Transform ModelParent => m_ModelRoot;

    private List<Collider> m_NavigationColliders;
    private List<Collider> m_ObjectColliders;
    private List<Collider> m_MainColliders;
    private List<Collider> m_AllColliders;

    private Materialiser m_Materialiser;

    private GameObject m_RootObject;
    private Transform m_CollisionRoot;
    private Transform m_ModelRoot;

    private RenderMetaData m_Data;

    public override void Initialise(ModuleObject moduleObject, RenderMetaData metaData, Action callback)
    {
        m_RootObject = gameObject;
        m_ModelRoot = transform;

        m_Data = metaData;
        if (m_Data.RuntimeDirectory == null)
        {
            metaData.LoadMainPrefab(this, prefab =>
            {
                Create(moduleObject, callback, prefab);
            });
        }
        else
        {
            Core.LoadFromFileFBXRef.CreateGameobject(metaData.RuntimeDirectory, (item) =>
            {
                Create(moduleObject, callback, item, true);
            });
        }
    }

    private void Create(ModuleObject moduleObject, Action callback, GameObject prefab, bool deletePrefab = false)
    {
        if (prefab == null)
        {
            moduleObject.IsValid = false;
            Debug.LogError($"Prefab was null! {moduleObject.Entry.ShortName}\n");
            callback();
            return;
        }

        // instantiate collision and model
        m_CollisionRoot = InstantiateChild(prefab, GlobalConsts.CollisionObj, m_ModelRoot.parent, false, deletePrefab);

        if (m_CollisionRoot == null)
        {
            Debug.LogError($"Check the catalogue side, make sure the FBX and prefab are not the same name, change the prefab name ");
            Debug.LogError($"RenderMetaData, prefab, see its correct or not then check the file created via the GUID");
            callback();
            return;
        }
        InstantiateChild(prefab, name, m_ModelRoot, true, deletePrefab);

        // grab colliders and send to interactable
        m_AllColliders = m_CollisionRoot.FindComponentsInChildren<Collider>();
        moduleObject.Interactable?.SetColliders(m_AllColliders);

        SortCollisions();

        var materialiserMaterial = Resources.Load<Material>("Materials/Materialiser");
        m_Materialiser = m_RootObject.AddComponent<Materialiser>();
        m_Materialiser.InitialiseRenderers(materialiserMaterial);

        if(deletePrefab == true)
        {
            this.WaitForFrames(5, () =>
            {
                Destroy(prefab);
            });
        }
        callback();
    }

    public override void OnModulesLoaded(ModuleObject moduleObject)
    {
        base.OnModulesLoaded(moduleObject);
    }

    public void LoadPartPrefab(int iPartNumber, Action<GameObject> callback)
    {
        LoadPartPrefab(this, iPartNumber, callback);
    }

    public void LoadPartPrefab(MonoBehaviour host, int iPartNumber, Action<GameObject> callback)
    {
        m_Data.LoadPartPrefab(host, iPartNumber, callback);
    }

    public void Toggle(bool state)
    {
        m_CollisionRoot.SetActive(state);
        m_ModelRoot.SetActive(state);
    }

    public void ToggleRenderer(bool state)
    {
        m_ModelRoot.SetActive(state);
    }

    public void ToggleCollision(bool state)
    {
        foreach (var collider in m_AllColliders)
        {
            collider.SetActive(state);
        }
    }

    public void ToggleCollisionLayer(bool state)
    {
        int layer = Layers.DefaultLayer;
        foreach (var collider in m_ObjectColliders)
        {
            collider.gameObject.layer = layer;
        }
    }

    public void SetRotation(Quaternion rotation)
    {
        m_CollisionRoot.localRotation = rotation;
        m_ModelRoot.localRotation = rotation;
    }

    public void MaterialiseModel()
    {
        m_Materialiser.Initialise(true, 1f);
        m_Materialiser.Fade(0f, () =>
        {
            m_Materialiser.ToggleMaterialised(false);
        });
    }

    protected override void LoadData(RenderModuleSaveData data, Action callback)
    {
        Toggle(!data.HideModel);
        callback();
    }

    protected override RenderModuleSaveData SaveData()
    {
        if (m_ModelRoot.gameObject.activeSelf == false)
        {
            return new RenderModuleSaveData(RuntimeId, true);
        }
        return null;
    }

    private void SortCollisions()
    {
        m_NavigationColliders = new List<Collider>();
        m_ObjectColliders = new List<Collider>();

        foreach (var collider in m_AllColliders)
        {
            if (collider.gameObject.layer == Layers.TeleportLayer)
            {
                m_NavigationColliders.Add(collider);
            }
            else
            {
                m_ObjectColliders.Add(collider);
            }
        }
    }

    private static Transform InstantiateChild(GameObject prefab, string name, Transform parent, bool ontoParent, bool deletePrefab)
    {
        GameObject child = prefab.transform.Find(name)?.gameObject;
        if (child != null)
        {
            if (ontoParent == true)
            {
                if (deletePrefab == false)
                {
                    Utils.Unity.CloneOntoParent(child, parent, true);
                    return parent;
                }
                else
                {
                    child.transform.SetParent(parent);
                    return parent;
                }
            }
            else
            {
                if (deletePrefab == false)
                {
                    GameObject clone = Utils.Unity.Clone(child, parent, true);
                    clone.transform.SetAsFirstSibling();
                    return clone.transform;
                }
                else
                {
                    child.transform.SetParent(parent);
                    child.transform.SetAsFirstSibling();
                    return child.transform;
                }
            }
        }
        return null;
    }
}