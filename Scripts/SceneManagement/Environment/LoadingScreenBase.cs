
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreenBase : MonoBehaviour
{

    public virtual void PlaceInfrontMainCamera() { }
    public virtual void InitialiseLoadingScene(Sprite image, bool bLoadingBar) { }

    public virtual void InitialiseLoadingScene(string imageName, bool bLoadingBar) { }

    public virtual void SetMessage(string message) { }
    public virtual void SetProgress(float progress) { }

    public virtual void OnSceneUnloading() { }
    public void Awake()
    {
        Camera.main.enabled = true;
    }

}
