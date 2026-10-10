using System;
using TMPro;
using UnityEngine;

public class WebEntry : MonoBehaviour
{
    public event Action<WebEntry> Pressed = null;

    public Transform Transform => m_Transform;
    public string Category { get; private set; }
    public WebItem Item { get; private set; }

    private InteractableModule m_Interactable;
    private Transform m_Transform;
    private TextMeshPro m_TxtLabel;

    private void Awake()
    {
        m_Transform = transform;
        m_TxtLabel = m_Transform.FindComponent<TextMeshPro>("Txt");
        m_Interactable = GetComponent<InteractableModule>();
    }

    private void Start()
    {
        m_Interactable.SetColliders(GetComponentsInChildren<Collider>().ToList());
        m_Interactable.Subscribe(this, subscription =>
        {
            subscription.Interact.Begin += OnInteractBegin;
            subscription.Interact.End += OnInteractEnd;
        });
    }

    public void Clone(WebEntry clone)
    {
        if (Category != null)
        {
            clone.InitialiseCategory(Category);
        }
        else if (Item != null)
        {
            clone.InitialiseItem(Item);
        }
        else
        {
            clone.InitialiseAsHome();
        }
        clone.Pressed = Pressed;
        clone.m_Transform.OrientTo(m_Transform, true);
    }

    public void InitialiseAsHome() => Initialise("Home");

    public void InitialiseCategory(string category)
    {
        Initialise(category);
        Category = category;
    }

    public void InitialiseItem(WebItem item)
    {
        Initialise(item.Model);
        Item = item;
    }

    private void Initialise(string label)
    {
        Category = null;
        Item = null;
        ClearCallback();

        m_TxtLabel.text = label;
        this.name = label;
    }

    private void OnInteractBegin(ControllerStateInteraction interaction, bool sendPhotonMessage = true)
    {
        interaction.Main.LockToObject(this);
    }

    private void OnInteractEnd(ControllerStateInteraction interaction, bool sendPhotonMessage = true)
    {
        if (interaction.Main.CurrentCollider == interaction.Main.HitCollider)
        {
            Pressed?.Invoke(this);
        }
        interaction.Main.UnlockFromObject();
    }

    public void ClearCallback() => Pressed = null;
}