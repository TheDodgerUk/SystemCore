using UnityEngine;
using UnityEngine.UI;

public class LoadingScreenMobile : LoadingScreenBase
{
    private Slider m_BarProgress;
    private Image m_LoadingImage;

    private Transform m_Transform;
    private string m_ImageName;

    private void Awake()
    {
        m_Transform = transform;
        m_LoadingImage = m_Transform.SearchComponent<Image>("LoadingScreenImg");
        m_BarProgress = m_Transform.FindComponent<Slider>("LoadingBar");
    }


    public override  void InitialiseLoadingScene(Sprite image, bool bLoadingBar)
    {
        if (null != m_LoadingImage)
        {
            m_LoadingImage.sprite = image;
            m_LoadingImage.SetActive(m_LoadingImage.sprite != null);
        }

        if (null != m_BarProgress)
        {
            m_BarProgress.transform.SetActive(bLoadingBar);
        }
    }


    public override  void InitialiseLoadingScene(string imageName, bool bLoadingBar)
    {
        string resourceToFind = "LoadingScene/" + imageName;
        m_ImageName = imageName;
        var sprite = Resources.Load<Sprite>(resourceToFind) as Sprite;
        if (sprite == null)
        {
            if (string.IsNullOrEmpty(imageName) == false)
            {
                m_ImageName = $"{imageName} image, is missing, place it a Resources  folder 'LoadingScene/";
                Debug.LogError(m_ImageName);
            }
        }
        InitialiseLoadingScene(sprite, bLoadingBar);

    }

    public override void SetProgress(float progress) => m_BarProgress.value = progress;
}
