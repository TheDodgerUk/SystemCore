using System;
using UnityEngine;


[SerializeField]
public class GameObjectData
{
    [System.NonSerialized]
    public GameObject GameObjectRef;
    [SerializeField]
    public string GameObjectName;

    public virtual void CollectAllData(GameObject root)
    {
        if (string.IsNullOrEmpty(GameObjectName) == false)
        {
            GameObjectRef = MetaData.StringToGameObject(root, GameObjectName, false);
        }
    }
}

[SerializeField]
public class AnimationGameObjectData : GameObjectData
{
    [SerializeField]
    public string AnimationName;
}
[SerializeField]
public class RendererGameObjectData : GameObjectData
{
    [System.NonSerialized]
    public Renderer RendererObj;

    public override void CollectAllData(GameObject root)
    {
        base.CollectAllData(root);
        if (GameObjectRef != null)
        {
            RendererObj = GameObjectRef.GetComponent<Renderer>();
        }
    }
}

[SerializeField]
public class ColliderGameObjectData : GameObjectData
{
    [System.NonSerialized]
    public Collider ColliderObj;

    public override void CollectAllData(GameObject root)
    {
        base.CollectAllData(root);
        if (GameObjectRef != null)
        {
            ColliderObj = GameObjectRef.GetComponent<Collider>();
        }
    }
}

public enum MetaDataType
{
    NOTHING,
    Catalogue,
    Render,
    ContentLight,
    ContentPlacement,
    ContentPivot,
    ContentAudio,
    ContentAnimation,
    ContentMaterialChange,
    ContentScale,
    ContentAdaptive,
    ContentGameObjectCycle,
    ContentScript,
    ContentSteamAudio,
    ContentModular,
    ContentLighting,
    ContentSlider,
    ContentButton,
    ContentDial,
    ContentClick,
    ArFeatures,
    ContentStaging,
    ContentVideoPlayer,
    ContentPickUp,
    ContentParticleSystem,
    ContentDrink,
    ContentFood,
    ContentPickUpSocket,
    ContentPickUpCable,
    ContentHinge,
    ContentHasRigidBody,
    ContentCollision,
}

public enum ExternalMetaDataType
{
    MlfLoudspeaker,
    MlfMechanical,
}

public enum MetaAttribute
{
    Moveable,
}

public abstract class MetaData
{
    public virtual void Initialise(MonoBehaviour host, CatalogueEntry entry, Action callback) => callback();

    public MetaDataType GetMetaDataType() => GetType().GetMetaDataType().Value;
    public bool IsGizmoType() => GetMetaDataType().ToString().ToLower().Contains("gizmo");

    public virtual void CollectAllData(UnityEngine.GameObject root) { }

    public static GameObject StringToGameObject(GameObject root, string name, bool showError = true)
    {
        if(string.IsNullOrEmpty(name) == true)
        {
            if (showError == true)
            {
                Debug.LogError($"StringToGameObject name was null, on  {root.name}");
            }
            return null;
        }
        if (string.Equals(root.name, name, StringComparison.CurrentCultureIgnoreCase) == true)
        {
            return root;
        }
        var found = root.transform.Search(name);
        if(found != null)
        {
            return found.gameObject;
        }
        else
        {
            if (showError == true)
            {
                Debug.LogError($"Cannot find name: {name}, on GameObject: {root.name}", root);
            }
            return null;
        }
    }
}

[System.Serializable]
public class MetaDataAttribute : Attribute
{
    public MetaDataType MetaType { get; }
    public MetaDataAttribute(MetaDataType enumType)
    {
        MetaType = enumType;
    }
}