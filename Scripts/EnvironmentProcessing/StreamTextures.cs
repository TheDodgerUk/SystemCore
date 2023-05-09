using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EnvironmentHelpers
{
    /// <summary>
    /// Allow the loading of textures from streamingfiles onto a UI surface (should be edited to allow
    /// other surfaces if required). 
    /// 
    /// To use tag the top level game object with [StreamTextures] and any child transform which contains
    /// a texture tag them with [StreamTexture]_TEXTURENAME.
    /// 
    /// Avoid using Underscores as part of the texture name
    /// </summary>
    public class StreamTextures : MonoBehaviour
    {
        public const string TAG = "[StreamTextures]";
        public const string INTERNAL_TAG = "[StreamTexture]";
        [SerializeField]
        private List<RawImage> m_ImagesToProcess = new List<RawImage>();

        public void Init()
        {
            var children = transform.GetAllChildren();

            for(int i = 0; i < children.Count; i++)
            {
                string name = children[i].name;
                if(true == name.Contains(INTERNAL_TAG))
                {
                    //Can only add a Image if this object has a RawImage component
                    //For other alternatives (Material application etc...) edit this to also
                    //include other components and process accordingly
                    RawImage img = children[i].gameObject.GetComponent<RawImage>();
                    if (null != img)
                    {
                        m_ImagesToProcess.Add(img);
                        //Second half of data contains the texture name to display
                        string[] data = name.Split('_');
                        if (data.Length > 1)
                        {
                            //Add image to this texture
                            Core.AssetBundlesRef.MaterialAssetBundleRef.GetItem(this, data[1], (t) =>
                            {
                                img.texture = t.mainTexture;
                            });
                        }
                    }
                }
            }
        }
    }
}