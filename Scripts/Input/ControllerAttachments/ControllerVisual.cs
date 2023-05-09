using UnityEngine;

public class ControllerVisual : MonoBehaviour
{
    protected const string EMISSION_COLOR = "_EmissionColor";
    protected const string EMISSION_KEYWORD = "_EMISSION";
    protected const string TRIGGER_BUTTON = "Trigger";
    protected const string GRIP_BUTTON = "Grip";
    protected const string BASE_MODEL = "Base";
    protected const string DISSOLVE = "_Dissolve";
    protected const string FADE_IN = "_FadeIn";

    private const float m_TransitionTime = 0.5f;

    protected ControllerGraphics m_Graphics;
    protected GameObject m_GameObject;
    protected Transform m_Transform;
    protected Transform m_BaseController;

    protected bool m_bVisible;

    private ValueTween m_TweenValue;

    public Transform GetTransform()
    {
        return m_Transform;
    }

    public virtual void Init(ControllerGraphics gfx)
    {
        m_GameObject = gameObject;
        m_Transform = transform;
        m_Graphics = gfx;
        m_BaseController = m_Transform.Search(BASE_MODEL);

    }

    public virtual void Visible(bool bShow)
    {
        if (null != m_TweenValue)
        {
            m_TweenValue.Stop();
        }

        m_bVisible = bShow;

        //Fade In!
        if (true == m_bVisible)
        {
            FadeIn();
        }
        else
        {
            FadeOut();
        }
    }

    private void FadeIn()
    {
        m_GameObject.SetActive(m_bVisible);

    }

    private void FadeOut()
    {
        m_GameObject.SetActive(m_bVisible);
    }
    public virtual void ToggleGraphics(bool state) { }

    public virtual void OnNewPoses() { }

    public virtual void SplashReset() { }

    public virtual void SetLayer(int layer) { }

    protected virtual void OnTransitionUpdate(float value)
    {

    }

    public virtual void UpdateControllerState(ControllerData controller) { }
    public virtual void OnGrabBegin(ControllerData controller) { }
    public virtual void OnGrabUpdate(ControllerData controller) { }
    public virtual void OnGrabEnd(ControllerData controller) { }
    public virtual void OnInteractBegin(ControllerData controller) { }
    public virtual void OnInteractUpdate(ControllerData controller) { }
    public virtual void OnInteractEnd(ControllerData controller) { }
    public virtual void OnTouchstickBegin(ControllerData controller) { }
    public virtual void OnTouchstickUpdate(ControllerData controller) { }
    public virtual void OnTouchstickEnd(ControllerData controller) { }

    protected void UpdateTransitionPosition(float value, Transform obj, Vector3 targetPosition)
    {
        obj.localPosition = Vector3.Lerp(obj.localPosition, targetPosition, value);
    }

    protected void UpdateTransitionRotate(float value, Transform obj, Quaternion targetRotation)
    {
        obj.localRotation = Quaternion.Slerp(obj.localRotation, targetRotation, value);
    }

    protected void UpdateTransitionScale(float value, Transform obj, Vector3 targetScale)
    {
        obj.localScale = Vector3.Lerp(obj.localScale, targetScale, value);
    }

    protected void UpdateMaterialColor(float value, Material material, string ID, Color original, Color color)
    {
        Color col = Color.Lerp(original, color, value);
        material.SetColor(ID, col);
    }

    protected void UpdateMaterialFloat(float value, Material material, string ID)
    {
        material.SetFloat(ID, value);
    }

    protected Material GetMaterial(Transform obj)
    {
        Renderer renderer = obj.GetComponent<Renderer>();
        if (null != renderer)
        {
            if (renderer.sharedMaterial != null)
            {
                Material mat = new Material(renderer.sharedMaterial);
                renderer.material = mat;
                return mat;
            }
            else
            {
                Debug.LogError($"cannot find sharedMaterial on {obj.gameObject.GetGameObjectPath()}");
                return null;
            }
        }

        return null;
    }
}
