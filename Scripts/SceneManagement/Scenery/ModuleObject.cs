using System;
using System.Collections.Generic;
using UnityEngine;
#if Photon
using Photon.Pun;
#endif

public enum ObjectState
{
    None,
    Invalid,
    Valid,
    Confirmed
}

public class ModuleObject : SaveableObject<SceneSaveObject>
{
    public Action<bool> OnClickAndHeld;
    public Action OnShutdown;
    public InteractableModule Interactable { get; private set; }
    public Transform RootTransform { get; private set; }

    public GameObject RootObject { get; private set; }
    public CatalogueEntry Entry { get; private set; }

#if Photon
    public PhotonView PhotonViewRef;
#endif
    public VrInteraction VrInteractionRootRef { get;  set; }

    public bool IsSelected => m_bIsModuleSelected;
    public bool IsGrouped => m_bIsGrouped;
    public bool IsHovered => m_bIsHovered;
    public bool IsGroupOwner = false;
    public ObjectState ObjectState;

    public List<ISaveable> Saveables => m_Saveables;

    private Dictionary<Type, ISaveable> m_SaveablesBySaveType;
    private Dictionary<Type, ISaveable> m_SaveablesByType;
    private Dictionary<Type, IMetaModule> m_ModulesByType;
    private List<IMetaModule> m_Modules;
    private List<ISaveable> m_Saveables;


    private ModuleObject m_SelectedModuleObject = null;
    private float m_fEnableHoldSelectionTimer = 1.0f;
    private bool m_bProcessedStack = false;
    private bool m_bIsPainting = false;

    [SerializeField]
    private bool m_bIsModuleSelected;
    [SerializeField]
    private bool m_bIsHovered;
    [SerializeField]
    private bool m_bIsGrouped;

    public bool IsValid = true;

    //Is the user currently interacting with this module object
    private bool m_bInteracting = false;

#if UNITY_EDITOR
    [InspectorButton(true, false)]
    public void CreateGroup() => ObjectGroupModule.ShowWindow(this);
    [InspectorButton(true, false)]
    public void CloneObject() => Core.Scene.CloneObject(this);
#endif

    protected override void Awake()
    {
        base.Awake();

        m_bIsModuleSelected = false;
        RootObject = gameObject;
        RootTransform = transform;

        m_ModulesByType = new Dictionary<Type, IMetaModule>();
        m_Modules = new List<IMetaModule>();

        m_SaveablesBySaveType = new Dictionary<Type, ISaveable>();
        m_SaveablesByType = new Dictionary<Type, ISaveable>();
        m_Saveables = new List<ISaveable>();

        UpdateObjectName();

        Interactable = RootObject.AddComponent<InteractableModule>();
        Interactable.Initialise(this, () => { });
        AddChild(Interactable);
    }

    public void Initialise(CatalogueEntry entry)
    {
        Entry = entry;
    }

    public void Shutdown()
    {
        OnShutdown?.Invoke();
        Core.Scene.RemoveModuleObject(this);
        CameraControllerVR.Instance.Selection.UpdateSelection(false, this);
        m_Modules.ForEach(m => m.Shutdown());
        RootObject.DestroyObject();
    }


    public void AddChild(object child)
    {
        if (child is IMetaModule)
        {
            AddModule((IMetaModule)child);
        }
        if (child is ISaveable)
        {
            AddSaveable((ISaveable)child);
        }
    }

    public T FindSaveable<T>() where T : class, ISaveable => m_Saveables.Find(s => s.GetType().IsOfType<T>()) as T;
    public T FindModule<T>() where T : class, IMetaModule => m_Modules.Find(m => m.GetType().IsOfType<T>()) as T;

    public T GetSaveable<T>() where T : class, ISaveable => m_SaveablesByType.Get(typeof(T)) as T;
    public T GetModule<T>() where T : class, IMetaModule => m_ModulesByType.Get(typeof(T)) as T;

    public void OnModulesInitialised()
    {
        m_Modules.ForEach(m => m.OnModulesInitialised(this));

        //Let parts handle the interactions not the group owner
        //if (false == IsGroupOwner)
        //{
        //    Interactable.Subscribe(this, subscription =>
        //    {
        //        subscription.Interact.Begin = OnModuleSelected;
        //        subscription.Interact.Update = OnModuleSelectUpdate;
        //        subscription.Interact.End = OnModuleSelectEnd;

        //        subscription.BtnSecondary.Begin = OnModButtonDown;
        //    });
        //}
    }

    public void OnModulesLoaded()
    {
        m_Modules.ForEach(m => m.OnModulesLoaded(this));
        UpdateObjectName();
    }

    public void OnSceneLoaded()
    {
        m_Modules.ForEach(m => m.OnSceneLoaded());
    }

    public void SetObjectState(ObjectState state)
    {
        ObjectState = state;
        OnUpdatedState();
    }

    public void SetHoverState(bool bHovered)
    {
        m_bIsHovered = bHovered;
        OnUpdatedState();
    }

    public void SetGroupState(bool bGrouped)
    {
        m_bIsGrouped = bGrouped;
        OnUpdatedState();
    }


    protected override void OnPostLoadData(SceneSaveObject data)
    {
        base.OnPostLoadData(data);

        foreach (var childData in data.Children)
        {
            GetMatchingSavable(childData)?.OnPostLoad(childData);
        }
    }

    protected override void LoadData(SceneSaveObject data, Action callback)
    {
        RootTransform.position = data.Position;
        RootTransform.rotation = data.Rotation;

        SequentialAction.List(data.Children, (childData, onTick) =>
        {
            var saveable = GetMatchingSavable(childData);
            if (saveable != null)
            {
                saveable.Load(childData, onTick);
            }
            else
            {
                onTick();
            }

        }, callback);
    }

    protected override SceneSaveObject SaveData()
    {
        if (Entry != null)
        {
            return new CatalogueSaveObject(RuntimeId, Entry.Guid, RootTransform, m_Saveables);
        }
        else
        {
            return new ModuleSaveObject(RuntimeId, RootTransform, m_Saveables);
        }
    }

    private void AddSaveable(ISaveable saveable)
    {
        var baseType = saveable.GetType().GetGenericBaseType();
        var args = baseType.GetGenericArguments().ToList();

        m_SaveablesByType[saveable.GetType()] = saveable;
        m_SaveablesBySaveType[args.Last()] = saveable;
        m_Saveables.Add(saveable);
    }

    private void AddModule(IMetaModule module)
    {
        m_ModulesByType.Add(module.GetType(), module);
        m_Modules.Add(module);
    }

    private ISaveable GetMatchingSavable(SaveData saveData)
    {
        return m_SaveablesBySaveType.Get(saveData.GetType());
    }

    private void UpdateObjectName()
    {
        string prefix = "none";
        if (Entry != null)
        {
            prefix = Entry.ShortName;
        }
        else if (m_Saveables.Count > 0)
        {
            prefix = m_Saveables.Last().GetType().Name;
        }
        else if (m_Modules.Count > 0)
        {
            prefix = m_Modules.Last().GetType().Name;
        }

        if (RootObject.name == "CatalogueEntryModule" || prefix != "none")
        {
            RootObject.name = $"[{prefix}]_{RuntimeId}";
        }
    }

    private void OnUpdatedState()
    {

    }

    private void OnModuleSelected(ControllerStateInteraction interaction, bool sendPhotonMessage = true)
    {
        if (null == interaction.Main.LockedObject)
        {
            interaction.Main.LockToObject(this);
            m_bInteracting = true;
            m_bIsModuleSelected = !m_bIsModuleSelected;

            ToggleSelectionVisual(m_bIsModuleSelected);

            //If selection is true process immediately
            if (true == m_bIsModuleSelected)
            {
                CameraControllerVR.Instance.Selection.UpdateSelection(m_bIsModuleSelected, this);
            }
            m_bIsPainting = false;
            m_bProcessedStack = false;
            m_SelectedModuleObject = null;
        }
    }

    private void OnModuleSelectUpdate(ControllerStateInteraction interaction, bool sendPhotonMessage = true)
    {
        ControllerData controller = interaction.Main;
        float time = controller.Inputs.Interact.Time.Total;

        //Allow hold selection of other ModuleObjects to toggle their states
        if (time > m_fEnableHoldSelectionTimer)
        {
            m_SelectedModuleObject = InputManagerVR.Instance.GetModuleObject(controller.HitCollider);
            if (null != m_SelectedModuleObject)
            {
                if (false == m_bIsPainting)
                {
                    if (m_SelectedModuleObject != this && false == m_SelectedModuleObject.IsGroupOwner)
                    {
                        m_bIsPainting = true;
                    }
                }

                if (false == m_bIsPainting)
                {
                    if (false == m_bProcessedStack)
                    {
                        m_bProcessedStack = true;
                        ClickAndHold(m_SelectedModuleObject);
                    }
                }
                else
                {
                    PaintSelection(m_SelectedModuleObject);
                }
            }
        }
    }

    private void OnModuleSelectEnd(ControllerStateInteraction interaction, bool sendPhotonMessage = true)
    {
        if (null != interaction.Main.LockedObject)
        {
            m_bInteracting = false;
            if (false == m_bIsModuleSelected)
            {
                CameraControllerVR.Instance.Selection.UpdateSelection(m_bIsModuleSelected, this);
            }
            interaction.Main.LockToObject(null);
        }
    }

    private void OnModButtonDown(ControllerStateInteraction interaction, bool sendPhotonMessage = true)
    {
        //If interact is currently not pressed return
        if (false == m_bInteracting)
        {
            return;
        }

    }

    public void ToggleSelectionVisual(bool bSelection)
    {
        m_bIsModuleSelected = bSelection;
    }

    private void ClickAndHold(ModuleObject module)
    {
        //Select whole group
        OnClickAndHeld?.Invoke(m_bIsModuleSelected);
    }

    private void PaintSelection(ModuleObject module)
    {
        //Toggle selection
        m_SelectedModuleObject.ToggleSelectionVisual(m_bIsModuleSelected);
    }
}
