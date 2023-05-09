using System.Collections.Generic;
using UnityEngine;

public class SpriteColourTween : ColourTween<SpriteRenderer>
{
    protected override List<SpriteRenderer> GetObjects()
    {
        return GetComponentsInChildren<SpriteRenderer>().ToList();
    }

    protected override Color GetColour(SpriteRenderer obj)
    {
        return obj.color;
    }

    protected override void SetColour(SpriteRenderer obj, Color colour)
    {
        obj.color = colour;
    }
}
