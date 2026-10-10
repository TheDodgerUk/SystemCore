using UnityEngine;

public class SphereLayout : MonoBehaviour//, IWebLayout
{
    [SerializeField]
    private float MinRadius = 0.05f;
    [SerializeField]
    private float Radius = 0.35f;
    [SerializeField]
    private float Base = 2;

    [SerializeField]
    private float Scale = 8;

    public void Position(Transform target, int i)
    {
        float log = Mathf.Log((i / (Scale * 0.5f)) + 2, Base);
        int ring = Mathf.FloorToInt(log);

        float countOnRing = (Scale * Mathf.Pow(Base, ring - 1));
        float k = 2 * i * Mathf.PI / countOnRing;
        float r = MinRadius + (Radius * ring);
        float x = r * Mathf.Cos(k);
        float y = r * Mathf.Sin(k);
        target.localPosition = new Vector3(x, y, 0);
    }

    public void AnimateToStartFromCentre(Transform target, int i)
    {
    }
}