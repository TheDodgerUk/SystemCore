using UnityEngine;

public static class Layers
{
    public static void Initialise()
    {
        // this is to Initialise all the readonly data below
        int temp = DefaultLayer;
    }

    public static readonly string MainCameraTag = "MainCamera";
    public static readonly string UntaggedTag = "Untagged";
    public static readonly string RootCatTag = "RootCatalogueItem";
    public static readonly string PC_Only = "PC_Only";

    public static readonly int DefaultLayer = LayerMask.NameToLayer("Default");
    private static readonly int TransparentFXLayer = LayerMask.NameToLayer("TransparentFX");
    private static readonly int IgnoreRaycastLayer = LayerMask.NameToLayer("Ignore Raycast");
    private static readonly int WaterLayer = LayerMask.NameToLayer("Water");
    private static readonly int UILayer = LayerMask.NameToLayer("UI");

    public static readonly int LoadingLayer = LayerMask.NameToLayer("Loading");
 
    public static readonly int VrInteractionLayer = LayerMask.NameToLayer("VrInteraction");

    public static readonly int ControllerLeftLayer = LayerMask.NameToLayer("ControllerLeft");
    public static readonly int ControllerRightLayer = LayerMask.NameToLayer("ControllerRight");
    public static readonly int ControllerUILayer = LayerMask.NameToLayer("ControllerUI");

    public static readonly int TeleportLayer = LayerMask.NameToLayer("Teleport");
    public static readonly int HiddenLayer = LayerMask.NameToLayer("HiddenLayer");
    public static readonly int GrabbleLayer = LayerMask.NameToLayer("Grabbable");


    public static readonly int AllMask = ~0;

    public static readonly int RaycastMask = (1 << VrInteractionLayer); // night have to put UI in , not not sure at all 
    public static readonly int CameraRuntimeMask = ~(1 << LoadingLayer); // all BUT LoadingLayer
    public static readonly int CameraLoadingMask = (1 << LoadingLayer) | (1 << ControllerLeftLayer) | (1 << ControllerRightLayer);
    public static readonly int NavigationMask = (1 << DefaultLayer) |(1 << VrInteractionLayer) | (1 << TeleportLayer);
    public static readonly int TeleportMask = (1 << TeleportLayer);
    public static readonly int UIMask = (1 << ControllerUILayer);
    public static readonly int ControllerMask = (1 << ControllerLeftLayer) | (1 << ControllerRightLayer);
    public static readonly int VrInteractionMask = (1 << VrInteractionLayer); // night have to put UI in , not not sure at all 
    public static readonly int IgnoreVrInteractionMask =~ (1 << VrInteractionLayer); // night have to put UI in , not not sure at all 


    public static bool IsTeleport(this Collider collider) => collider.IsLayer(TeleportLayer);

    public static bool IsEnvironment(this Collider collider) => (collider.IsLayer(DefaultLayer)); // the mouse click interaction uses RaycastMask // nothing on RaycastMask can be in IsEnvironment as it breaks it 
    private static bool IsLayer(this Collider collider, int layer)
    {
        if (collider != null)
        {
            return (collider.gameObject.layer == layer);
        }
        return false;
    }

    private static bool IsLayer(this GameObject item, int layer)
    {
        if (item != null)
        {
            return (item.layer == layer);
        }
        return false;
    }

    public static bool IsVrController(this GameObject item)
    {
        bool found = (item.IsLayer(ControllerLeftLayer) ||item.IsLayer(ControllerRightLayer));
        return found;
    }

    public static bool IsVrInteractionLayerLayer(this GameObject item)
    {
        bool found = item.IsLayer(VrInteractionLayer);
        return found;
    }


    public static bool IsValidLayer(this GameObject item)
    {
        bool found = (item.IsLayer(DefaultLayer) ||
            item.IsLayer(TransparentFXLayer) ||
            item.IsLayer(IgnoreRaycastLayer) ||
            item.IsLayer(WaterLayer) ||
            item.IsLayer(UILayer) ||
            item.IsLayer(LoadingLayer) ||
            item.IsLayer(VrInteractionLayer) ||
            item.IsLayer(ControllerLeftLayer) ||
            item.IsLayer(ControllerRightLayer) ||
            item.IsLayer(ControllerUILayer) ||
            item.IsLayer(TeleportLayer) ||
            item.IsLayer(HiddenLayer));
        return found;
    }
}
