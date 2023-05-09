using System;
using UnityEngine;

public class SteamVrPlayArea : PlayArea
{
    protected override void RequestPlayAreaSize(Action<Vector3> callback)
    {
        //var size = SteamVR_PlayArea.Size.Calibrated;
        //var rect = new Valve.VR.HmdQuad_t();

        //this.WaitUntil(20, () => SteamVR_PlayArea.GetBounds(size, ref rect), () =>
        //{
        //    var min = Vector3.one * float.MaxValue;
        //    var max = Vector3.one * float.MinValue;
        //    var corners = new[] { rect.vCorners0, rect.vCorners1, rect.vCorners2, rect.vCorners3 };
        //    foreach (var c in corners)
        //    {
        //        var corner = new Vector3(c.v0, c.v1, c.v2);
        //        min = Vector3.Min(min, corner);
        //        max = Vector3.Max(max, corner);
        //    }
        //    callback(max - min);
        //});
    }
}