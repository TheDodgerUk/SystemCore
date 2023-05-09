using System.Collections.Generic;
using UnityEngine;

public class WebPrototype : MonoBehaviour
{
    private const int MaxHistory = 10;

    private ReuseablePool<WebEntry> m_EntryPool;
    private List<WebEntry> m_Entries;
    private List<WebEntry> m_History;
    private WebEntry m_HomeEntry;
    private CatalogueApi m_Api;

    private HistoryLayout m_HistoryLayout;
    private RingLayout m_RingLayout;

    private Transform m_HistoryParent;
    private Transform m_RingParent;

    private void Awake()
    {
        var parent = transform.CreateChild("Categories");
        var prefab = GetComponentInChildren<WebEntry>();
        prefab.SetActive(false);

        m_EntryPool = new ReuseablePool<WebEntry>(prefab, parent);
        m_Entries = new List<WebEntry>();
        m_History = new List<WebEntry>();

        m_HistoryLayout = GetComponentInChildren<HistoryLayout>();
        m_RingLayout = GetComponentInChildren<RingLayout>();

        m_HistoryParent = m_HistoryLayout.transform;
        m_RingParent = m_RingLayout.transform;

        m_Api = gameObject.AddComponent<CatalogueApi>();
    }

    private void Start()
    {
        m_Api.Categories(OnCategoriesReceived);
    }

    [InspectorButton]
    private void Reposition()
    {
        for (int i = 0; i < m_Entries.Count; ++i)
        {
            m_RingLayout.Position(m_Entries[i].Transform, i, m_Entries.Count);
        }
        for (int i = 1; i < m_History.Count; ++i)
        {
            m_HistoryLayout.Position(m_History[i].Transform, i, m_History.Count);
        }
    }

    [InspectorButton]
    private void AutomaticallyReposition() => this.LoopUntil(() => false, Reposition);

    [InspectorButton]
    private void StopPositioning() => this.StopAllCoroutines();

    private void AnimateEntriesAndHistory(WebEntry selected)
    {
        if (selected != null)
        {
            // animate selected
            m_Entries.Remove(selected);
            m_History.Insert(0, selected);
            m_HistoryLayout.AddToHistory(selected);
            selected.Transform.SetParent(m_HistoryParent);

            // set callback ready to go back
            selected.ClearCallback();
            selected.Pressed += OnHistoryItemSelected;

            while (m_History.Count > MaxHistory)
            {
                var last = m_History.Last();
                m_History.Remove(last);
                m_HistoryLayout.AnimateAway(last, m_EntryPool.Return);
            }
            m_HistoryLayout.AnimateLine(m_History);
        }

        m_RingLayout.AnimateAway(m_Entries, m_EntryPool.Return);

        // clear entries
        m_Entries.Clear();
    }

    private void OnCategoriesReceived(List<string> categories)
    {
        for (int i = 0; i < categories.Count; ++i)
        {
            var entry = RequestWebEntry(m_RingParent, m_Entries);
            entry.InitialiseCategory(categories[i]);
            entry.Pressed += OnCategorySelected;
        }
        m_RingLayout.AnimateToStart(m_Entries);
    }

    private void OnCategorySelected(WebEntry selected)
    {
        if (m_HomeEntry == null)
        {
            m_HomeEntry = RequestWebEntry(transform);
            m_HomeEntry.InitialiseAsHome();
            m_HomeEntry.Pressed += OnHomeSelected;
            m_RingLayout.AnimateHome(m_HomeEntry);
        }
        AnimateEntriesAndHistory(selected);
        m_Api.Items(selected.Category, items => OnItemsReceived(selected, items, false));
    }

    private void OnItemsReceived(WebEntry selected, List<WebItem> items, bool organiseByWeight)
    {
        for (int i = 0; i < items.Count; ++i)
        {
            var entry = RequestWebEntry(m_RingParent, m_Entries);
            entry.InitialiseItem(items[i]);
            entry.Pressed += OnItemSelected;
        }
        m_RingLayout.AnimateToStart(selected, m_Entries, items, organiseByWeight);
    }

    private void OnItemSelected(WebEntry selected)
    {
        AnimateEntriesAndHistory(selected);
        m_Api.Related(selected.Item.Guid, items => OnItemsReceived(selected, items, true));
    }

    private void OnHomeSelected(WebEntry selected)
    {
        AnimateEntriesAndHistory(selected);
        m_Api.Categories(OnCategoriesReceived);
        m_HomeEntry = null;
    }

    private void OnHistoryItemSelected(WebEntry selected)
    {
        var clone = RequestWebEntry(m_HistoryParent);
        selected.Clone(clone);

        if (clone.Category != null)
        {
            OnCategorySelected(clone);
        }
        else if (clone.Item != null)
        {
            OnItemSelected(clone);
        }
        else
        {
            OnHomeSelected(clone);
        }
    }

    private WebEntry RequestWebEntry(Transform parent, List<WebEntry> list = null)
    {
        var entry = m_EntryPool.GetNext();
        entry.Transform.SetParent(parent, true);
        if (list != null)
        {
            list.Add(entry);
        }

        return entry;
    }
}
