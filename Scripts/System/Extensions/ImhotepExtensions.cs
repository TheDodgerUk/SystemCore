using UnityEngine;

public static class ImhotepExtensions
{
    public static void ToggleVisibility(this GameObject gameObject, bool state)
    {
        gameObject.SetLayerRecursively(state ? Layers.DefaultLayer : Layers.HiddenLayer);
    }
}
