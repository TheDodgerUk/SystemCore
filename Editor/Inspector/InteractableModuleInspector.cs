using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(InteractableModule), true)]
public class InteractableModuleInspector : Editor
{
    private Dictionary<InputButtonStateHandler, bool> m_StateFoldouts = new Dictionary<InputButtonStateHandler, bool>();

    private void SetFoldout(InputButtonStateHandler handler, bool state) => m_StateFoldouts[handler] = state;
    private bool GetFoldout(InputButtonStateHandler handler) => m_StateFoldouts.TryGet(handler) ?? false;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var interactable = target as InteractableModule;
        if (interactable == null || interactable.Subscriptions == null)
        {
            return;
        }

        DrawSubscriptions(interactable.Subscriptions.SubscriptionsByPair);
        DrawModules(interactable.Subscriptions.ModulesByCollider);
    }

    private void DrawModules(Dictionary<Collider, List<MonoBehaviour>> modules)
    {
        if ((modules?.Count ?? 0) == 0)
        {
            return;
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Modules", EditorStyles.boldLabel);
        foreach (var item in modules)
        {
            EditorHelper.ObjectField(item.Key);
            using (new GuiIndentScope())
            {
                foreach (var module in item.Value)
                {
                    EditorHelper.ObjectField(module);
                }
            }
            EditorGUILayout.Space();
        }
    }

    private void DrawSubscriptions(Dictionary<ColliderModulePair, InteractionSubscription> subscriptions)
    {
        if ((subscriptions?.Count ?? 0) == 0)
        {
            return;
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Subscriptions", EditorStyles.boldLabel);

        var grouped = subscriptions.GroupBy(kvp => kvp.Value);
        foreach (var group in grouped)
        {
            using (new EditorGUILayout.VerticalScope(GUI.skin.box))
            {
                var pairs = group.Select(kvp => kvp.Key);
                foreach (var module in pairs.Select(p => p.Module).Distinct())
                {
                    EditorHelper.ObjectField(module);
                }
                foreach (var collider in pairs.Select(p => p.Collider).Distinct())
                {
                    EditorHelper.ObjectField(collider);
                }

                using (new GuiIndentScope())
                {
                    var subscription = group.Key;
                    DrawStateHandler("Hover", subscription.Hover);
                    DrawStateHandler("Grab", subscription.Grab);
                    DrawStateHandler("Interact", subscription.Interact);
                    DrawStateHandler("BtnPrimary", subscription.BtnPrimary);
                    DrawStateHandler("BtnSecondary", subscription.BtnSecondary);
                }
            }
        }
    }

    private void DrawStateHandler(string name, InputButtonStateHandler stateHandler)
    {
        using (new GuiIndentScope())
        {
            if (stateHandler.Begin != null || stateHandler.Update != null || stateHandler.End != null)
            {
                GUILayout.BeginHorizontal();
                bool foldout = EditorGUILayout.Foldout(GetFoldout(stateHandler), name, true);
                if (foldout == false)
                {
                    if (GUILayout.Button("Begin") == true)
                    {
                        InvokeHandler(stateHandler.Begin);
                    }
                    if (GUILayout.RepeatButton("Update") == true)
                    {
                        InvokeHandler(stateHandler.Update);
                    }
                    if (GUILayout.Button("End") == true)
                    {
                        InvokeHandler(stateHandler.End);
                    }
                }
                GUILayout.EndHorizontal();

                SetFoldout(stateHandler, foldout);
                if (foldout == true)
                {
                    DrawHandler("Begin ->", stateHandler.Begin);
                    DrawHandler("Update ->", stateHandler.Update);
                    DrawHandler("End ->", stateHandler.End);
                }
            }
        }
    }

    private void DrawHandler(string name, InputButtonHandler handler)
    {
        using (new GuiIndentScope())
        {
            if (handler != null)
            {
                if (GUILayout.Button("Invoke") == true)
                {
                    InvokeHandler(handler);
                }
                foreach (var d in handler.GetInvocationList())
                {
                    EditorGUILayout.LabelField(name, $"{d.Target.GetType().Name}.{d.Method.Name}()");
                }
            }
            else
            {
                EditorGUILayout.LabelField(name, "(null)");
            }
        }
    }

    private static void InvokeHandler(InputButtonHandler handler)
    {
        Debug.LogError("Warning Inspector Debug, ControllerStateInteraction is null");
        handler.Invoke(new ControllerStateInteraction());
    }
}
