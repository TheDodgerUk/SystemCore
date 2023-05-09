#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGroupModule : SaveableObject<ObjectGroupSaveData>
{
    [SerializeField, Expandable]
    private ObjectGroupLayout m_ObjectLayout;

    private List<ModuleObject> m_Objects;
    private Transform m_Transform;

    protected override void Awake()
    {
        base.Awake();

        m_ObjectLayout = ScriptableObject.CreateInstance<StaticGroupLayout>();
        m_Objects = new List<ModuleObject>();
        m_Transform = transform;
    }

    public void AddObject(ModuleObject moduleObject)
    {
        AddModuleObject(moduleObject);
        UpdatePosition();
    }

    public void UpdatePosition()
    {
        m_ObjectLayout.Position(m_Objects);
    }

    protected override void LoadData(ObjectGroupSaveData data, Action callback)
    {
        m_ObjectLayout = data.Layout ?? ScriptableObject.CreateInstance<StaticGroupLayout>();

        Core.Scene.LoadObjects(data.Objects, moduleObjects =>
        {
            moduleObjects.ForEach(AddModuleObject);
            UpdatePosition();
            callback();
        });
    }

    protected override ObjectGroupSaveData SaveData()
    {
        return new ObjectGroupSaveData(RuntimeId, m_Objects, m_ObjectLayout);
    }

    private void AddModuleObject(ModuleObject moduleObject)
    {
        moduleObject.RootTransform.SetParent(m_Transform, true);

        Core.Scene.RemoveModuleObject(moduleObject);
        m_Objects.Add(moduleObject);
    }

#if UNITY_EDITOR
    public static void ShowWindow(ModuleObject moduleObject)
    {
        ObjectGroupModuleWindow.ShowWindow(moduleObject);
    }
#endif
}

#if UNITY_EDITOR
public class ObjectGroupModuleWindow : EditorWindow
{
    public static void ShowWindow(ModuleObject moduleObject)
    {
        var window = GetWindow<ObjectGroupModuleWindow>();
        window.m_Target = moduleObject;
        window.ShowAuxWindow();
    }

    private List<ObjectGroupModule> m_Groups;
    private ModuleObject m_Target;
    private Vector2 m_ScrollPos;

    private void Awake()
    {
        m_Groups = FindObjectsOfType<ObjectGroupModule>().ToList();
    }

    private void OnGUI()
    {
        EditorGUILayout.ObjectField(m_Target, m_Target.GetType(), true);

        using (var s = new EditorGUILayout.ScrollViewScope(m_ScrollPos))
        {
            foreach (var group in m_Groups)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.ObjectField(group, group.GetType(), true);
                    if (GUILayout.Button("Select", GUILayout.Width(150)) == true)
                    {
                        group.AddObject(m_Target);
                        Close();
                    }
                }
            }
            m_ScrollPos = s.scrollPosition;
        }
        if (GUILayout.Button("Create New") == true)
        {
            var newObject = Core.Scene.CreateModuleObject(typeof(ObjectGroupModule));
            var group = newObject.GetSaveable<ObjectGroupModule>();
            group.AddObject(m_Target);
            Close();
        }
    }
}
#endif