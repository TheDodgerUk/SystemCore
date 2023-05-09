using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LinearGroupLayout : ObjectGroupLayout
{
    public override ObjectLayoutType Type => ObjectLayoutType.Linear;
    public enum ObjectAlignment { Left, Centre, Right }

    [SerializeField]
    private ObjectAlignment m_Alignment = ObjectAlignment.Centre;
    [SerializeField]
    private float m_Distance = 0.5f;

    public override void Position(List<ModuleObject> objects)
    {
        float sign = GetSign();
        float offset = GetOffset(objects);
        for (int i = 0; i < objects.Count; ++i)
        {
            var pos = Vector3.zero;
            pos.x = (i * m_Distance * sign) + offset;
            objects[i].RootTransform.localPosition = pos;
        }
    }

    private float GetSign()
    {
        switch (m_Alignment)
        {
            case ObjectAlignment.Left:
            case ObjectAlignment.Centre:
            default:
                return 1;
            case ObjectAlignment.Right:
                return -1;
        }
    }

    private float GetOffset(List<ModuleObject> objects)
    {
        switch (m_Alignment)
        {
            case ObjectAlignment.Left:
            case ObjectAlignment.Right:
            default:
                return 0;
            case ObjectAlignment.Centre:
                return ((objects.Count - 1) * m_Distance) * -0.5f;
        }
    }
}
