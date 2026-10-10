using System;
using System.Collections.Generic;
using UnityEngine;

public class ControllerSelection : MonoBehaviour
{
    public event Action SelectionChanged = null;

    [SerializeField]
    private List<ModuleObject> m_SelectedObjects = new List<ModuleObject>();

    public void UpdateSelection(bool add, ModuleObject module)
    {

        if (true == add)
        {
            OnObjectSelected(module);
        }
        else
        {
            OnObjectDeselected(module);
        }
    }

    private void OnObjectSelected(ModuleObject module)
    {
        if (false == m_SelectedObjects.Contains(module))
        {
            m_SelectedObjects.Add(module);
            SelectionChanged?.Invoke();
        }
    }

    private void OnObjectDeselected(ModuleObject module)
    {
        if (m_SelectedObjects.Remove(module) == true)
        {
            SelectionChanged?.Invoke();
        }
    }

    public void ClearSelection()
    {
        m_SelectedObjects.Clear();
    }

    public List<ModuleObject> GetSelection()
    {
        return m_SelectedObjects;
    }
}