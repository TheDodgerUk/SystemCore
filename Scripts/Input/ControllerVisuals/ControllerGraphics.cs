using UnityEngine;

public enum Handedness
{
    Left,
    Right
}

public class ControllerGraphics : MonoBehaviour
{
    public Transform RaycastRoot { get; protected set; }

    protected Transform m_Transform;

    private void Awake()
    {
        m_Transform = transform;
    }
    public void BaseInitilise()
    {
        Awake();
    }

    public virtual void Initialise(Handedness handedness) { }

    public virtual void UpdateState(ControllerData controller) { }

    public virtual void ToggleGraphics(bool state) { }

    public virtual void SetLayer(int layer) { }

    public virtual void SplashReset() { }

    public virtual void OnNewPoses() { }
}
