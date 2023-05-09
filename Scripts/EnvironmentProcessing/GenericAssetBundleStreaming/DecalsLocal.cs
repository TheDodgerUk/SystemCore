using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecalsLocal : GenericLocal<Material>
{
    protected override string ASSET_TYPE => nameof(DecalsLocal);
    public override string FileExtention => ".mat";
}