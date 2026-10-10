using SFB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TMPro;
using TriLibCore;
using UnityEngine;
using UnityEngine.UI;


public class LoadFromFileFBX
{
    private LoadFromFileFBXMono m_LoadFromFileFBX;
    public LoadFromFileFBX(LoadFromFileFBXMono loadFromFileFBX) => m_LoadFromFileFBX = loadFromFileFBX;
    public string GetSearchDirectory() => m_LoadFromFileFBX.GetSearchDirectory();
    public void SetSearchDirectory(string filename) => m_LoadFromFileFBX.SetSearchDirectory(filename);
    public void CreateGameobject(string path, Action<GameObject> callback) => m_LoadFromFileFBX.CreateGameobject(path, callback);
    public CatalogueEntry CreateCatalogueEntry(string path) => m_LoadFromFileFBX.CreateCatalogueEntry(path);
}

public class LoadFromFileFBXMono :MonoBehaviour
{
    private const string FBX_DIRECTORY = "FBX_DIRECTORY";
    private SimpleDictionaryList<string, Action<GameObject>> m_LoadedItemsCount = new SimpleDictionaryList<string, Action<GameObject>>();
    private Dictionary<string, GameObject> m_LoadedItems = new Dictionary<string, GameObject>();
    private Canvas m_Canvas;
    private Slider m_Slider;
    private TextMeshProUGUI m_Message;
    private GameObject m_LoadFromFileFBXHolder;
    private Button m_ErrorButton;
    private TextMeshProUGUI m_ErrorText;


    // load from URL , NOTES
    //var assetLoaderOptions = AssetLoader.CreateDefaultLoaderOptions();
    //var webRequest = AssetDownloader.CreateWebRequest("https://ricardoreis.net/trilib/demos/sample/TriLibSampleModel.zip");
    //AssetDownloader.LoadModelFromUri(webRequest, OnLoad, OnMaterialsLoad, OnProgress, OnError, null, assetLoaderOptions);


    private void Awake()
    {
        m_Canvas = this.GetComponentInChildren<Canvas>(true);
        m_Slider = this.GetComponentInChildren<Slider>(true);
        m_Message = this.transform.Search("Message").GetComponent<TextMeshProUGUI>();
        m_ErrorButton = this.GetComponentInChildren<Button>(true);
        m_ErrorText = this.transform.Search("txtButton").GetComponent<TextMeshProUGUI>();

        m_Canvas.SetActive(false);
        m_LoadFromFileFBXHolder = new GameObject("m_LoadFromFileFBXHolder");
        m_LoadFromFileFBXHolder.transform.SetParent(this.transform);
    }

    public string GetSearchDirectory()
    {
        string defaultPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        string path =  PlayerPrefs.GetString(FBX_DIRECTORY, defaultPath);
        if(Directory.Exists(path) == true)
        {
            return path;
        }
        else
        {
            return defaultPath;
        }
    }

    public void SetSearchDirectory(string filename)
    {
        string path = Path.GetDirectoryName(filename);
        PlayerPrefs.SetString(FBX_DIRECTORY, path);
    }

    public CatalogueEntry CreateCatalogueEntry(string path)
    {
        var entry = new CatalogueEntry();
        if (entry.Attributes == null)
        {
            entry.Attributes = new System.Collections.Generic.List<MetaAttribute>();
        }
        entry.Attributes.Add(MetaAttribute.Moveable);

        RenderMetaData renderMeta = new RenderMetaData();
        renderMeta.RuntimeDirectory = path;
        entry.AddMetaData(MetaDataType.Render, renderMeta);
        return entry;
    }

    public void CreateGameobject(string path, Action<GameObject> callback)
    {
        Core.Mono.StartCoroutine(InternalCreateGameobject(path, callback));
    }

    private IEnumerator InternalCreateGameobject(string path, Action<GameObject> callback)
    {
        string fullFileName = path;
        GameObject created = null;
        if (m_LoadedItems.ContainsKey(fullFileName) == false)
        {
            m_LoadedItemsCount.AddToList(fullFileName, callback);


            string fileName = Path.GetFileNameWithoutExtension(fullFileName);

            m_Canvas.SetActive(true);
            m_ErrorButton.SetActive(false);
            m_Message.text = $"Loading in model {fileName}";
            SetMessageInfront();

            if (m_LoadedItemsCount[fullFileName].Count == 1)
            {
               var timer =  Stopwatch.Diagnostics();

                // 2 tasks , model loaded, textures applied
                TaskAction task = new TaskAction(2, () =>
                {
                    GameObject modelRootObject = created;

                    if (modelRootObject != null)
                    {
#if HouseBuilder
                        float scale = HouseBuilder.HouseBuilderConstants.GetTilingAmountFromTextureName(fullFileName);
                        modelRootObject.transform.SetScale(scale);
#endif

                        var renders = modelRootObject.GetComponentsInChildren<Renderer>().ToList();
                        for (int i = 0; i < renders.Count; i++)
                        {
                            SetColour(renders[i]);
                        }

                        var rootObject = new GameObject();
                        rootObject.name = fileName;

                        GameObject collisionRootObject = new GameObject(GlobalConsts.CollisionObj);
                        GameObject collisionChildObject = new GameObject(GlobalConsts.ColliderName);

                        modelRootObject.name = GlobalConsts.RootModel;
                        collisionRootObject.transform.SetParent(rootObject.transform);
                        collisionChildObject.transform.SetParent(collisionRootObject.transform);

                        modelRootObject.transform.SetParent(rootObject.transform);


                        // if facing not game direction
                        if (fileName.CaseInsensitiveContains("FacingY") == true)
                        {
                            modelRootObject.transform.rotation = Quaternion.Euler(90, 0, 0);
                        }
                        // if facing not game direction
                        if (fileName.CaseInsensitiveContains("FacingZ") == true)
                        {
                            modelRootObject.transform.rotation = Quaternion.Euler(-90, 0, 0);
                        }

                        var box = collisionChildObject.AddComponent<BoxCollider>();
                        box.size = modelRootObject.gameObject.GetEncapsulatingBounds().size;

                        // compensate for differnt items, pivot point
                        float centreY = modelRootObject.gameObject.GetEncapsulatingBounds().center.y;
                        float offset = box.size.y / 2;
                        collisionRootObject.transform.localPosition = new Vector3(0, offset, 0);
                        modelRootObject.transform.localPosition = new Vector3(0, offset + -centreY, 0);


                        var entry = new CatalogueEntry();
                        if (entry.Attributes == null)
                        {
                            entry.Attributes = new System.Collections.Generic.List<MetaAttribute>();
                        }
                        entry.Attributes.Add(MetaAttribute.Moveable);


                        RenderMetaData renderMeta = new RenderMetaData();
                        renderMeta.RuntimeDirectory = path;
                        entry.AddMetaData(MetaDataType.Render, renderMeta);

                        rootObject.SetActive(false);

                        m_LoadedItems.Add(fullFileName, rootObject);
                        rootObject.transform.SetParent(m_LoadFromFileFBXHolder.transform);

                        foreach (var item in m_LoadedItemsCount[fullFileName])
                        {
                            Core.Mono.StartCoroutine(ReturnItem(fullFileName, item));
                        }
                        m_LoadedItemsCount[fullFileName].Clear();

                        DebugLogPolys(fileName, timer, rootObject);
                    }
                    else
                    {
                        // model not loaded
                        m_LoadedItems.Add(fullFileName, null);

                        foreach (var item in m_LoadedItemsCount[fullFileName])
                        {
                            Core.Mono.StartCoroutine(ReturnItem(fullFileName, item));
                        }
                        m_LoadedItemsCount[fullFileName].Clear();
                    }
                });

                TriLibCore.AssetLoader.LoadModelFromFile(fullFileName, (data) =>
                {
                    // model loaded
                    created = data.RootGameObject;
                    task.Increment();
                }, (materailsLoaded) =>
                {
                    // texturesApplied
                    task.Increment();
                }, OnProgress, (error) => { OnError(task, fileName); }, null, null, null);
            }
        }
        else
        {
            Core.Mono.StartCoroutine(ReturnItem(fullFileName, callback));
        }

        yield return new WaitForEndOfFrame();
    }

    private void DebugLogPolys(string fileName, Stopwatch timer, GameObject rootObject)
    {
        int count = 0;
        var meshs = rootObject.transform.GetComponentsInChildren<MeshFilter>();
        foreach (var mesh in meshs)
        {
            count += mesh.mesh.triangles.Length / 3;
        }

        timer.Stop();
        Debug.LogError($"{fileName}   {timer.ToReadable()}  polycount {count.ToString("#,##0")}");
    }

    private IEnumerator ReturnItem(string fullFileName, Action<GameObject> callback)
    {
        m_Canvas.SetActive(false);
        GameObject newObj = null;
        if (m_LoadedItems[fullFileName] != null)
        {
            newObj = GameObject.Instantiate(m_LoadedItems[fullFileName]);
            yield return new WaitForEndOfFrame();                               // delays needed other wise it breaks 
            newObj.SetActive(true);
            yield return new WaitForEndOfFrame();                               // delays needed other wise it breaks 
        }

        callback?.Invoke(newObj);
    }

    private void OnProgress(AssetLoaderContext arg1, float arg2)
    {
        SetMessageInfront();
        m_Slider.value = arg2;
    }


    private void SetMessageInfront()
    {
        if (Core.SceneLoader.IsSystemVR == true)
        {
          ////////TODO   m_Message.gameObject.PlaceInfrontMainCamera(1);
        }
    }

    private void OnError(TaskAction task, string fileName)
    {
        m_ErrorButton.SetActive(true);
        m_ErrorText.text = $"File would not load: {fileName}";
        m_ErrorButton.onClick.RemoveAllListeners();
        m_ErrorButton.onClick.AddListener(() =>
        {
            task.Increment();
            task.Increment();
            m_Canvas.SetActive(false);
            Debug.LogError(m_ErrorText.text);
        });
    }

    public void SetColour(Renderer renderer)
    {
        const string NAME_SPLITTER = "_";
        const int NAME_SPLITTER_NUMBER = 5;

        if (renderer != null)
        {
            Material[] sharedMaterialsCopy = renderer.sharedMaterials;

            for (int matIndex = 0; matIndex < sharedMaterialsCopy.Length; matIndex++)
            {
                var nameSplit = sharedMaterialsCopy[matIndex].name.SplitList(NAME_SPLITTER);
                if (nameSplit.Count == NAME_SPLITTER_NUMBER)
                {
                    // some items import with materail names     0.30984_0.20442_0.54226_0.00000_0.00000 (instance)  . this converts to colours 
                    if (float.TryParse(nameSplit[0], out float colour1) == true && float.TryParse(nameSplit[1], out float colour2) == true && float.TryParse(nameSplit[2], out float colour3) == true && float.TryParse(nameSplit[3], out float colour4) == true)
                    {
                        Material newMat = new Material(sharedMaterialsCopy[matIndex]);
                        sharedMaterialsCopy[matIndex] = newMat;
                        sharedMaterialsCopy[matIndex].color = new Color(colour1, colour2, colour3, colour4);
                        renderer.sharedMaterials = sharedMaterialsCopy;
                    }
                }
            }

        }

    }


}
