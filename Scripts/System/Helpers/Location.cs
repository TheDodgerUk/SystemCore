using UnityEngine;

[System.Serializable]
public class Location
{
    public Vector3 Position;
    public Quaternion Rotation;

    public Location(Vector3 position, Vector3 rotation) : this(position, Quaternion.Euler(rotation)) { }
    public Location() : this(Vector3.zero, Quaternion.identity) { }

    public Location(Vector3 position, Quaternion rotation)
    {
        Position = position;
        Rotation = rotation;
    }
}
