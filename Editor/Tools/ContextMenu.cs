using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public static class ContextMenu
{
    [MenuItem("CONTEXT/Component/Move to GameObject", false, 0)]
    public static void MoveComponentToGameObject(MenuCommand command)
    {
        var component = command.context as Component;
        if (component != null)
        {
            if (ComponentUtility.CopyComponent(component) == true)
            {
                var go = new GameObject(component.GetType().Name);
                Undo.RegisterCreatedObjectUndo(go, $"Undo Create {go.name}");
                go.transform.SnapToParent(component.transform);
                ComponentUtility.PasteComponentAsNew(go);
                Undo.DestroyObjectImmediate(component);
            }
            else
            {
                Debug.LogError($"Failed to copy component {component.name}\n", component);
            }
        }
        else
        {
            Debug.LogError($"This is not a Component!\n", command.context);
        }
    }

    [MenuItem("CONTEXT/BoxCollider/Centre Transform to Collider", false, 0)]
    public static void CentreTransformToCollider_BoxCollider(MenuCommand command)
    {
        var boxCollider = command.context as BoxCollider;
        RectUtility.CentreTransformToCollider(boxCollider);
    }

    [MenuItem("CONTEXT/BoxCollider/Resize To Rect", false, 0)]
    public static void ResizeToRect_BoxCollider(MenuCommand command)
    {
        var boxCollider = command.context as BoxCollider;
        RectUtility.AlignColliderToRect(boxCollider);
    }

    [MenuItem("CONTEXT/BoxCollider/Resize To Rect", true, 0)]
    public static bool ResizeToRect_BoxCollider_Validation(MenuCommand command)
    {
        var boxCollider = command.context as BoxCollider;
        return boxCollider.GetRect() != null;
    }

    [MenuItem("CONTEXT/Transform/Resize To Rect", false, 0)]
    public static void ResizeToRect_Transform(MenuCommand command)
    {
        var transform = command.context as Transform;
        RectUtility.AlignTransformToRect(transform);
    }

    [MenuItem("CONTEXT/Transform/Resize To Rect", true, 0)]
    public static bool ResizeToRect_Transform_Validation(MenuCommand command)
    {
        var transform = command.context as Transform;
        return (transform.parent is RectTransform);
    }

    private static string MenuPath<T>(string menu) => $"CONTEXT/{typeof(T).Name}/{menu}";
}
