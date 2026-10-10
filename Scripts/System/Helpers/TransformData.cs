using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum TransformDataEnum
{
    AllLocal,
    AllButParentLocal,
}

public class TransformData
{
    public Transform Parent;
    public Vector3 Position;
    public Quaternion Rotation;
    public Vector3 Scale;
    public int SiblingIndex;
}

public static class TransformDataUtils
{
    public static TransformData GetTransformData(this Transform self) => GetTransformData(self.gameObject);
    public static TransformData GetTransformData(this GameObject self)
    {
        TransformData data = new TransformData
        {
            Parent = self.transform.parent,
            Position = self.transform.position,
            Rotation = self.transform.rotation,
            Scale = self.transform.localScale,
            SiblingIndex = self.transform.GetSiblingIndex()
        };
        return data;
    }

    public static void ApplyTransformData(this Transform self, TransformData data, TransformDataEnum type) => ApplyTransformData(self.gameObject, data, type);
    public static void ApplyTransformData(this GameObject self, TransformData data, TransformDataEnum type)
    {
        switch (type)
        {
            case TransformDataEnum.AllLocal:
                self.transform.parent = data.Parent;
                self.transform.position = data.Position;
                self.transform.rotation = data.Rotation;
                self.transform.localScale = data.Scale;
                self.transform.SetSiblingIndex(data.SiblingIndex);
                break;
            case TransformDataEnum.AllButParentLocal:
                self.transform.position = data.Position;
                self.transform.rotation = data.Rotation;
                self.transform.localScale = data.Scale;
                self.transform.SetSiblingIndex(data.SiblingIndex);
                break;
            default:
                break;
        }
    }

}

