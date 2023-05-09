using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class VrInteraction : MonoBehaviour
{
    public enum StateEnum
    {
        On,                     //On VrInteraction callbacks,  On  Colliders, On  Outline, On renderers  
        OffWithColliderOff,     //Off VrInteraction callbacks, Off Colliders, Off Outline, On renderers  
        OffWithColliderOn,      //Off VrInteraction callbacks, On  Colliders, Off Outline, On renderers  
        OffIncludingRenderer,   //Off VrInteraction callbacks, Off Colliders, Off Outline, Off renderers  
    }

    protected bool m_InteractionsAllowed = true;
    protected InteractableModule m_InteractableModule;
    protected ModuleObject m_ModuleObject;
    private Collider[] m_Colliders;


    public List<Collider> ColliderList => m_Colliders.ToList();

    [SerializeField]
    private string m_OriginalName = "";
    public string OriginalName => m_OriginalName;

    [SerializeField]
    private string m_OriginalFullPath = "";
    public string OriginalFullPath => m_OriginalFullPath;

    public TransformData m_TransformData;

    public int GuidRef { get; private set; }

    private EPOOutline.Outlinable m_Outline;
    public bool EnableOutlineCanBeUsed = false;
    public bool EnableOutlineOnHover = true;
    private InteractionSubscription m_Subscription;

    [SerializeField]
    private StateEnum m_StateEnum = StateEnum.On;


    public CatalogueEntry CatalogueEntryRef => m_ModuleObject?.Entry;


    // These are used for the AR instruction mode and VR Tooltips.
    public string FriendlyName { get; protected set; }
    public string ItemDescription { get; protected set; }

    public string ActorNickNameTouched { get; set; } = "";
    public Vector3 ActorNickNamePosition { get; set; } = Vector3.zero;

    public virtual VrInteraction GetRealVrInteraction() => this;

    public bool IsRootGameObject { get; private set; }


    protected Action<float> m_PercentageCallback;

    public bool HasGameStarted { get; protected set; } = false;

    public void ClearAddCallback(Action<float> callback)
    {
        m_PercentageCallback = null;
        m_PercentageCallback += callback;
    }
    public void AddCallback( Action<float> callback)=>  m_PercentageCallback += callback;
    public virtual void SetPercentageCallbackAmount(float amount) { }
    public virtual void SetPercentageNoCallbackAmount(float amount) { }

    public SimpleDictionaryList<MetaDataType, MetaData> BaseVrMetaDatas { get; set; } = new SimpleDictionaryList<MetaDataType, MetaData>();

    public SimpleDictionaryList<MetaDataType, VrInteraction> BaseVrInteractions { get; set; } = new SimpleDictionaryList<MetaDataType, VrInteraction>();
    public VrInteraction GetVrInteractionFromRoot(MetaDataType metaDataType, string OriginalName)
    {
        var returnValue = BaseVrInteractions.GetList(metaDataType).Find(e => e.OriginalName.ToLower() == OriginalName.ToLower());
        if(returnValue == null)
        {
            Debug.LogError($"cannot find: {metaDataType}, OriginalName: {OriginalName}, in root {this.OriginalName}", this.gameObject);
            var items = BaseVrInteractions.GetList(metaDataType);
            string allItems = "Only got these : \n";
            foreach (var item in items)
            {
                allItems += item.OriginalName + "\n";
            }
            Debug.LogError($"Only Have: {allItems} ");
        }
        return returnValue;
    }


    public List<VrInteraction> GetVrInteractionFromRoot(MetaDataType metaDataType)
    {
        var returnValue = BaseVrInteractions.GetList(metaDataType);
        if (returnValue == null)
        {
            Debug.LogError($"cannot find: {metaDataType}, OriginalName: {OriginalName}");
        }
        return returnValue;
    }


    public List<MetaData> GetMetaDataTypeFromRoot(MetaDataType metaDataType) 
    {
        var returnValue = BaseVrMetaDatas.GetList(metaDataType);
        if (returnValue == null)
        {
            Debug.LogError($"cannot find: {metaDataType}, OriginalName: {OriginalName}");
        }
        return returnValue;
    }


    public VrInteraction GetVrInteractionFromRoot(string OriginalName) => BaseVrInteractions.GetListAll().Find(e => e.OriginalName == OriginalName);

    public void SetOutLine(bool enable)
    {
        if ((null != m_Outline) && (true == EnableOutlineCanBeUsed))
        {
            m_Outline.SetOutLine(enable);
        }
    }


    public void SetState(StateEnum state)
    {
        m_StateEnum = state;
        switch (state)
        {
            case StateEnum.On:
                On();
                break;
            case StateEnum.OffWithColliderOff:
                OffWithColliderOff();
                break;
            case StateEnum.OffWithColliderOn:
                OffWithColliderOn();
                break;
            case StateEnum.OffIncludingRenderer:
                OffIncludingRenderer();
                break;
            default:
                break;
        }
    }

    [InspectorButton]
    private void On()
    {
        gameObject.SetActive(true);
        OnEnable();
        SetCollider(true);
    }

    [InspectorButton]
    private void OffWithColliderOff()
    {
        gameObject.SetActive(true);
        OnDisable();
        SetCollider(false);
    }

    [InspectorButton]
    private void OffWithColliderOn()
    {
        gameObject.SetActive(true);
        OnDisable();
        SetCollider(true);
    }

    [InspectorButton]
    private void OffIncludingRenderer()
    {
        gameObject.SetActive(false);
        OnDisable();
        SetCollider(false);
    }

    private void SetCollider(bool state)
    {
        if (ColliderList != null)
        {
            ColliderList.ForEach(e => e.enabled = state);
        }
        else
        {
            Debug.LogError("Looks like you trying to setState before everything is set up, delay it for a frame" +
                " this.WaitForFrames(3, () => { SetState() });", this);
        }
    }


    public void OnEnable()
    {
        if (m_InteractableModule == null)
        {
            Debug.LogError($"m_InteractableModule == null,  gameObject: {gameObject.GetGameObjectPath()}", gameObject);
        }
        else
        {
            m_InteractableModule.IsEnabled = true;
            m_Outline.enabled = true;
        }
    }

    public void OnDisable()
    {
        if (m_InteractableModule == null || m_Outline == null)
        {
            Debug.LogError("Looks like you trying to setState before everything is set up, delay it for a frame" +
                " this.WaitForFrames(3, () => { SetState() });" + $"{this.name}", this);
        }
        else
        {
            m_InteractableModule.IsEnabled = false;
            m_Outline.enabled = false;
        }
    }


    public void SetOutlineColour(EPOOutline.Outliner.EnumItemSelected selected) => m_Outline.ItemSelected = selected;



    private const string HASH_CODE_TEST = "HASH_CODE_TEST/test if same /freeeme_Prefab 134";
    private const int HASH_CODE_TEST_RESULT = -1245388548;
    private static int m_HashCodeResult;

    public void ForceCreateGuid()
    {
        m_OriginalName = this.name;
        m_OriginalFullPath = this.transform.gameObject.GetGameObjectPath();
        GuidRef = m_OriginalFullPath.GetHashCode();
        m_TransformData = this.gameObject.GetTransformData();
    }

    private void OnDestroy()
    {
        GlobalConsts.m_VrInteractionGuidSafety.RemoveAll(e => e.GUID == this.GuidRef);
    }

    protected virtual void Awake()
    {
        if(m_HashCodeResult == 0)
        {
            m_HashCodeResult = HASH_CODE_TEST.GetHashCode();
            if(m_HashCodeResult != HASH_CODE_TEST_RESULT)
            {
                Debug.LogError($"m_HashCodeResult result has changed new :{m_HashCodeResult}, old: {HASH_CODE_TEST_RESULT}");
            }
        }

        IsRootGameObject = (this.CompareTag(GlobalConsts.RootCatalogueItemTag) == true);
        m_OriginalName = this.name;
        m_OriginalFullPath = this.transform.gameObject.GetGameObjectPath();
        GuidRef = m_OriginalFullPath.GetHashCode();
        m_TransformData = this.gameObject.GetTransformData();

       

        var foundName = GlobalConsts.m_VrInteractionGuidSafety.Find(e => e.Name == m_OriginalFullPath);
        var foundGuid = GlobalConsts.m_VrInteractionGuidSafety.Find(e => e.GUID == GuidRef);
        if (foundName != null)
        {
            Debug.LogError($"Duplicate of: {m_OriginalFullPath}, tring to enter {m_OriginalFullPath}, {GuidRef}, FOUND {foundName.Name}, {foundName.GUID}");
        }

        if (foundGuid != null)
        {
            Debug.LogError($"Duplicate of: {GuidRef}, tring to enter {m_OriginalFullPath}, {GuidRef}, FOUND {foundGuid.Name}, {foundGuid.GUID}");
        }

        if (foundName == null && foundGuid == null)
        {
            GlobalConsts.GuidSafety newItem = new GlobalConsts.GuidSafety();
            newItem.Name = m_OriginalFullPath;
            newItem.GUID = GuidRef;
            GlobalConsts.m_VrInteractionGuidSafety.Add(newItem);
        }




        m_ModuleObject = gameObject.ForceComponent<ModuleObject>();
        m_InteractableModule = m_ModuleObject.Interactable;
        if(m_ModuleObject == null)
        {
            Debug.LogError("m_ModuleObject is null,", gameObject);
        }

        if (m_ModuleObject.Interactable == null)
        {
            Debug.LogError("m_ModuleObject.Interactable is null,", gameObject);
        }

        if (IsRootGameObject == true)
        {
            var placementCollisionRoot = this.transform.Find(GlobalConsts.CollisionObj);
            var placementCollisions = placementCollisionRoot.GetComponentsInChildren<Collider>(true);

            var physicsCollisionRoot = this.transform.Find(GlobalConsts.RootModel);
            var physicsCollisions = physicsCollisionRoot.GetComponentsInChildren<Collider>(true);

            if (Core.Environment.CurrentEnvironment.InteractionType == GlobalConsts.InteractionType.Placement)
            {
                for (int i = 0; i < physicsCollisions.Length; i++)
                {
                    physicsCollisions[i].enabled = false;
                }
                m_Colliders = placementCollisions;
            }
            else
            {
                for (int i = 0; i < placementCollisions.Length; i++)
                {
                    placementCollisions[i].enabled = false;
                }
                m_Colliders = physicsCollisions;
            }

            if (Core.Environment.CurrentEnvironment.InteractionType == GlobalConsts.InteractionType.Physics)
            {
                for (int i = 0; i < m_Colliders.Length; i++)
                {
                    if (m_Colliders[i] is MeshCollider)
                    {
                        var mesh = (MeshCollider)m_Colliders[i];
                        mesh.convex = true;
                    }
                }
            }

        }
        else
        {
            m_Colliders = this.GetComponentsInChildren<Collider>();
        }



        // remove colliders which should not be part of this VrInteraction trigger 
        List<Collider> removeColliders = new List<Collider>();
        foreach (var item in m_Colliders)
        {
            var parent = item.GetComponentInParent<VrInteraction>();
            if(parent != null)
            {
                if (parent != this)
                {
                    removeColliders.Add(item);
                }
            }
        }

        var listOfColliders = m_Colliders.ToList();
        foreach (var toRemove in removeColliders)
        {
            listOfColliders.Remove(toRemove);
            ConsoleExtra.Log($"Collider needs to be removed, as its being used in another VrInteraction", toRemove.gameObject, ConsoleExtraEnum.EDebugType.Generic);
        }
        m_Colliders = listOfColliders.ToArray();



        CheckAll();
        m_InteractableModule.InitialiseWithColliders(m_Colliders.ToList());
        Subscribe();

        m_Outline = this.gameObject.ForceComponent<EPOOutline.Outlinable>();

        this.gameObject.SetLayerRecursively(Layers.VrInteractionLayer);
        
        Core.Environment.OnEnvironmentLoadingComplete += OnEnvironmentLoadingComplete;
        if (Core.Environment.HasOnEnvironmentLoadingComplete == true)
        {
            // this is so the overide AWake will kick in before GameStarted
            Core.Mono.WaitForEndOfFrame(() =>
            {
                OnEnvironmentLoadingComplete();
            });
        }
    }

    private void OnEnvironmentLoadingComplete()
    {
        Core.Environment.OnEnvironmentLoadingComplete -= OnEnvironmentLoadingComplete;
        GameStarted();
    }
    public void GameStarted() 
    {
        HasGameStarted = true;
        InternalGameStarted();
    }
    protected virtual void InternalGameStarted()
    {

    }

    public void ReInitiliseOutline()
    {
        if (m_Outline != null)
        {
            m_Outline.InitialiseActiveRenderers();
        }
        else
        {
            if (this.gameObject.activeInHierarchy == true)
            {
                Debug.LogError($"m_Outline == null", gameObject);
            }
        }
    }

    public void ReInitialiseWithEnabledColliders()
    {
        var cols = this.GetComponentsInChildren<Collider>(true).ToList();
        List<Collider> enabled = new List<Collider>();
        foreach (var item in cols)
        {
            if (item.enabled && item.gameObject.activeSelf == true)
            {
                enabled.Add(item);
            }
        }

        m_InteractableModule.RemoveSubscription(this, enabled, m_Subscription);

        m_InteractableModule.InitialiseWithColliders(enabled);
        Subscribe();
    }


    private void Subscribe()
    {
        var all = this.GetComponents<VrInteraction>();

        m_InteractableModule.Subscribe(this, (subs) =>
        {
            m_Subscription = subs;
            subs.Interact.Begin = null;
            subs.Interact.Update = null;
            subs.Interact.End = null;

            subs.ClickStick.Begin = null;
            subs.ClickStick.Update = null;
            subs.ClickStick.End = null;

            subs.Hover.Begin = null;
            subs.Hover.End = null;

            // this is changed so that i can have more than 1 vrInteraction on an object
            for (int i = 0; i < all.Length; i++)
            {
                subs.Interact.Begin += all[i].BeginLaser;
                subs.Interact.Update += all[i].OnUpdateLaser;
                subs.Interact.End += all[i].EndLaser;

                subs.ClickStick.Begin += all[i].BeginClickStick;
                subs.ClickStick.Update += all[i].OnUpdateClickStick;
                subs.ClickStick.End += all[i].EndClickStick;

                subs.Hover.Begin += all[i].OnHoverBegin;
                subs.Hover.End += all[i].OnHoverEnd;
            }
        });
    }



    public virtual void OnHoverBegin(ControllerStateInteraction interaction, bool sendPhotonMessage = true)
    {

    }

    public virtual void OnHoverEnd(ControllerStateInteraction interaction, bool sendPhotonMessage = true)
    {

    }

    private void CheckAll()
    {
#if UNITY_EDITOR
        foreach (var item in m_Colliders)
        {
            if (item == gameObject)
            {
                Debug.LogWarning($"VrInteraction: There cannot be a collider on the mainGameObject, on originalName: {m_OriginalName}", item);
            }
        }

        if ((null == m_Colliders) || (m_Colliders.Length == 0))
        {
            Debug.LogError($"VrInteraction: There are no colliders on this, on originalName: {m_OriginalName} ", this.gameObject);
        }
#endif
    }



    public virtual void BeginFingerTouch(GameObject fingerTransform)
    {
    }


    public virtual void OnUpdateFingerTouch(GameObject fingerTransform)
    {
    }

    public virtual void EndFingerTouch(GameObject fingerTransform)
    {
    }



    public virtual void BeginClickStick(ControllerStateInteraction interaction, bool sendPhotonMessage = true)
    {
    }


    public virtual void OnUpdateClickStick(ControllerStateInteraction interaction, bool sendPhotonMessage = true)
    {
    }

    public virtual void EndClickStick(ControllerStateInteraction interaction, bool sendPhotonMessage = true)
    {
    }

    public virtual void BeginLaser(ControllerStateInteraction interaction, bool sendPhotonMessage = true)
    {
    }


    public virtual void OnUpdateLaser(ControllerStateInteraction interaction, bool sendPhotonMessage = true)
    {
    }

    public virtual void EndLaser(ControllerStateInteraction interaction, bool sendPhotonMessage = true)
    {
    }

    public virtual void ResetToOriginalState()
    {
    }

    public static UnityEngine.GameObject StringToGameObject(string item, UnityEngine.GameObject root)
    {
        if (root == null)
        {
            UnityEngine.Debug.LogError($"PrefabManager.LoadPrefab came back as null");
            return null;
        }
        if (string.IsNullOrEmpty(item) == false)
        {
            UnityEngine.Transform trans = root.transform.Search(item);
            if (trans != null)
            {
                return trans.gameObject;
            }
        }
        return null;
    }

    public static T StringToComponent<T>(string item, UnityEngine.GameObject root) where T : UnityEngine.Component
    {
        if (root == null)
        {
            UnityEngine.Debug.LogError($"PrefabManager.LoadPrefab came back as null");
            return null;
        }
        if (string.IsNullOrEmpty(item) == false)
        {
            UnityEngine.Transform trans = root.transform.Search(item);
            if (trans != null)
            {
                return trans.gameObject.GetComponent<T>();
            }
        }
        return null;
    }
}

