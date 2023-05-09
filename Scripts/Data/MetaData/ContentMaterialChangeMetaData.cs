using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable, MetaData(MetaDataType.ContentMaterialChange)]
public class ContentMaterialChangeMetaData : MetaData
{
    public static readonly int TILING_ID = Shader.PropertyToID("_MainTex");
    public static readonly int TILING_ID_SECONDARY = Shader.PropertyToID("_DetailAlbedoMap");

    [System.Serializable]
    public class MaterialItemData
    {

        public enum OrientationType
        {
            Portrait,
            Landscape,
        }

        public enum MaterialType
        {
            Colour,
            Material,
            Texture
        }

        public enum TextureMap
        {
            Main,
            Secondary,
        }

        [SerializeField]
        public MaterialType m_MaterialType = MaterialType.Colour;


        [System.NonSerialized]
        public Renderer m_MaterialRenderer;

        [System.NonSerialized]
        public GameObject m_MaterialGameObject;

        [SerializeField]
        public string m_MaterialGameObjectName;

        [SerializeField]
        public int m_MaterialIndex;

        [SerializeField]
        public OrientationType m_OrientationType = OrientationType.Portrait;

        [SerializeField]
        public TextureMap m_TextureMap = TextureMap.Main;
    }

    public List<MaterialItemData> m_MaterialItemData = new List<MaterialItemData>();


    public override void CollectAllData(GameObject root)
    {
        for(int i = 0; i < m_MaterialItemData.Count; i++)
        {
            m_MaterialItemData[i].m_MaterialGameObject = StringToGameObject(root, m_MaterialItemData[i].m_MaterialGameObjectName);
            if (m_MaterialItemData[i].m_MaterialGameObject != null)
            {
                m_MaterialItemData[i].m_MaterialRenderer = m_MaterialItemData[i].m_MaterialGameObject.GetComponent<Renderer>();
            }
        }
        
    }

    public static void SetMaterial(GameObject rootGameObject, string subGameObjectName, int matIndex, string matName, Action<Material> callbackMaterial)
    {
        Core.AssetBundlesRef.MaterialAssetBundleRef.GetItem(null, matName, (mat) =>
        {
            if (mat != null)
            {
                var obj = rootGameObject.transform.Search(subGameObjectName);
                var renderer = obj.GetComponent<Renderer>();

                Material newMat = new Material(mat);

                Material[] sharedMaterialsCopy = renderer.sharedMaterials;
                sharedMaterialsCopy[matIndex] = newMat;
                renderer.sharedMaterials = sharedMaterialsCopy;
                callbackMaterial?.Invoke(mat);
            }
            else
            {
                Debug.Log("using default material");
                callbackMaterial?.Invoke(null);
            }
        });
    }

    public static string GetMaterialName(GameObject rootGameObject, string subGameObjectName, int matIndex)
    {
        var obj = rootGameObject.transform.Search(subGameObjectName);
        var renderer = obj.GetComponent<Renderer>();
        string name = renderer.materials[matIndex].name;
        if (name.CaseInsensitiveContains("instance") == true)
        {
            name = name.Split(' ')[0];
        }
        return name;
    }


    public static void SetTexture(GameObject rootGameObject, string subGameObjectName, int matIndex, string matName, MaterialItemData.TextureMap textureMap, Action<Texture2D> callbackTexture2D)
    {
        Core.AssetBundlesRef.Texture2DAssetBundleRef.GetItem(null, matName, (texture) =>
        {
            if (texture != null)
            {
                ApplyTexture(rootGameObject, subGameObjectName, matIndex, matName, textureMap, texture, callbackTexture2D);
            }
            else
            {
                Core.Network.GetCustomFileTexture(matName, (customTexture) =>
                {
                    ApplyTexture(rootGameObject, subGameObjectName, matIndex, matName, textureMap, customTexture, callbackTexture2D);
                });
            }
        });
    }

    private static void ApplyTexture(GameObject rootGameObject, string subGameObjectName, int matIndex, string matName, MaterialItemData.TextureMap textureMap, Texture texture, Action<Texture2D> callbackTexture2D)
    {
        var obj = rootGameObject.transform.Search(subGameObjectName);
        var renderer = obj.GetComponent<Renderer>();


        Material[] sharedMaterialsCopy = renderer.sharedMaterials;
        if (textureMap == MaterialItemData.TextureMap.Main)
        {
            if (texture != null)
            {
                sharedMaterialsCopy[matIndex].mainTexture = texture;
            }
        }
        else
        {
            if (texture != null)
            {
                sharedMaterialsCopy[matIndex].SetTexture(TILING_ID_SECONDARY, texture);
            }
        }
        renderer.sharedMaterials = sharedMaterialsCopy;
        callbackTexture2D?.Invoke((Texture2D)texture);
    }



    public static string GetTextureName(GameObject rootGameObject, string subGameObjectName, int matIndex, MaterialItemData.TextureMap textureMap)
    {
        var obj = rootGameObject.transform.Search(subGameObjectName);
        var renderer = obj.GetComponent<Renderer>();

        string name = "";
        if (textureMap == MaterialItemData.TextureMap.Main)
        {
            if (renderer.materials[matIndex].mainTexture != null)
            {
                name = renderer.materials[matIndex].mainTexture.name;
            }
            else
            {
                Debug.LogError($" {rootGameObject.GetGameObjectPath()}, this does not have a mainTexture");
            }
        }
        else
        {
            if (renderer.materials[matIndex].mainTexture != null)
            {
                name = renderer.materials[matIndex].GetTexture(TILING_ID_SECONDARY).name;
            }
            else
            {
                Debug.LogError($" {rootGameObject.GetGameObjectPath()}, this does not have a TILING_ID_SECONDARY");
            }
        }
        if (name.CaseInsensitiveContains("instance") == true)
        {
            name = name.Split(' ')[0];
        }
        return name;
    }


    public  void SetColourAll(Color color)
    {
        for(int  i = 0; i < m_MaterialItemData.Count; i++)
        {
            var item = m_MaterialItemData[i];
            var obj = item.m_MaterialGameObject;
            if (obj != null)
            {
                var renderer = item.m_MaterialRenderer;
                if (renderer != null)
                {
                    Material[] sharedMaterialsCopy = renderer.sharedMaterials;

                    Material newMat = new Material(sharedMaterialsCopy[item.m_MaterialIndex]);

                    sharedMaterialsCopy[item.m_MaterialIndex] = newMat;
                    sharedMaterialsCopy[item.m_MaterialIndex].color = color;
                    renderer.sharedMaterials = sharedMaterialsCopy;
                }
            }
        }
    }



    public static void SetColourStatic(GameObject rootGameObject, string subGameObjectName, int matIndex, Color color)
    {
        var obj = rootGameObject.transform.Search(subGameObjectName);
        if (obj != null)
        {
            var renderer = obj.GetComponent<Renderer>();
            if (renderer != null)
            {
                Material[] sharedMaterialsCopy = renderer.sharedMaterials;

                Material newMat = new Material(sharedMaterialsCopy[matIndex]);

                sharedMaterialsCopy[matIndex] = newMat;
                sharedMaterialsCopy[matIndex].color = color;
                renderer.sharedMaterials = sharedMaterialsCopy;
            }
            else
            {
                Debug.LogError($"Could not find the colour item on {rootGameObject}, check catalogue");
            }
        }
        else
        {
            Debug.LogError($"Could not find the colour item on {rootGameObject}, check catalogue");
        }
    }

    public static Color GetColourStatic(GameObject rootGameObject, string subGameObjectName, int matIndex)
    {
        var obj = rootGameObject.transform.Search(subGameObjectName);
        if (obj != null)
        {
            var renderer = obj.GetComponent<Renderer>();
            if (renderer != null)
            {
                Material[] sharedMaterialsCopy = renderer.sharedMaterials;
                return sharedMaterialsCopy[matIndex].color;
            }
            else
            {
                Debug.LogError($"Could not find the colour item on {rootGameObject}, check catalogue");
                return Color.white;
            }
        }
        else
        {
            Debug.LogError($"Could not find the colour item on {rootGameObject}, was looking for {subGameObjectName}  check catalogue");
            return Color.white;
        }
    }

}
