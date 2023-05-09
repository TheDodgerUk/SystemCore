using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public static class ComponentExtensions
{
    public static void SetActive<T>(this T component, bool state) where T : Component
    {
        var go = component.gameObject;
        if (go.activeSelf != state)
        {
            go.SetActive(state);
        }
    }

    // massive safety in editor mode
    public static T SafetyGetComponentInChildren<T>(this Transform transform, bool safetyOnlyInEditor = false) where T : Component
    {
#if UNITY_EDITOR
        var all = transform.transform.GetComponentsInChildren<T>();
        if (all.Length > 1)
        {
            Debug.LogError($"Found more than 1, count {all.Length}");
        }
        if(all.Length > 0)
        {
            return all[0];
        }
        return null;
#else
        if(safetyOnlyInEditor == false)
        {
            return transform.transform.GetComponentInChildren<T>();
        }
        else 
        {
            var all = transform.transform.GetComponentsInChildren<T>();
            if (all.Length > 1)
            {
                Debug.LogError($"Found more than 1, count {all.Length}");
            }
            if (all.Length > 0)
            {
                return all[0];
            }
            return null;
        }
#endif

    }



    public static GameObject FindGameObject(this Transform transform, string name)
    {
        return transform?.Find(name)?.gameObject;
    }

    public static RectTransform FindRect(this Transform transform, string name)
    {
        return transform?.Find(name) as RectTransform;
    }

    public static T GetComponentInSibling<T>(this Transform transform, bool includeInactive = false) where T : Component
    {
        return transform.parent?.GetComponentInChildren<T>(includeInactive);
    }

    public static List<T> GetComponentsInSibling<T>(this Transform transform) where T : Component
    {
        return transform.parent?.GetComponentsInChildren<T>().ToList();
    }

    public static T FindComponent<T>(this List<GameObject> objs) where T : Component
    {
        foreach (var go in objs)
        {
            var component = go.GetComponent<T>();
            if (component != null)
            {
                return component;
            }
        }
        return null;
    }
    public static T FindComponentInChildren<T>(this List<GameObject> objs) where T : Component
    {
        foreach (var go in objs)
        {
            var component = go.GetComponentInChildren<T>();
            if (component != null)
            {
                return component;
            }
        }
        return null;
    }




    public static T FindComponent<T>(this Transform transform, string name) where T : Component
    {
        return transform?.Find(name)?.GetComponent<T>();
    }

    public static List<T> FindComponents<T>(this Transform transform, string name) where T : Component
    {
        var child = transform?.Find(name);
        if (child != null)
        {
            return child.FindComponents<T>();
        }
        return new List<T>();
    }

    public static List<T> FindComponents<T>(this Transform transform) where T : Component
    {
        var list = new List<T>();
        foreach (Transform child in transform)
        {
            list.AddIfNotNull(child.GetComponent<T>());
        }
        return list;
    }

    public static T FindComponentInSibling<T>(this Transform transform, string name) where T : Component
    {
        return transform.parent.FindComponent<T>(name);
    }

    public static List<T> FindComponentsInSibling<T>(this Transform transform, string name) where T : Component
    {
        return transform.parent.FindComponents<T>(name);
    }

    public static List<T> FindComponentsInChildren<T>(this Transform transform) where T : Component
    {
        return transform.GetComponentsInChildren<T>(true).ToList();
    }

    public static List<T> FindComponentsInChildren<T>(this Transform transform, string name) where T : Component
    {
        return transform.Find(name)?.FindComponentsInChildren<T>();
    }


    public static T SearchComponent<T>(this Transform transform, string name, bool showError = true) where T : Component
    {
        var all = transform.GetComponentsInChildren<T>(true).ToList();

        var item = all.FindLast(e => e.name.ToLower() == name.ToLower());
        //var item = transform.Search(name)?.GetComponent<T>();
        if (item == null && showError == true)
        {
            DebugBeep.LogError($"Could not find :  {name}", DebugBeep.MessageLevel.Medium);
            string allItems = "";
            foreach (var trans in all)
            {
                allItems += $"{trans} \n";
            }
        }
        return item;
    }

    public static T SearchComponent<T>(this GameObject obj, string name, bool showError = true) where T : Component
    {
        var all = obj.transform.GetComponentsInChildren<T>(true).ToList();

        var item = all.FindLast(e => e.name == name);
        //var item = obj.transform.Search(name)?.GetComponent<T>();
        if (item == null && showError == true)
        {
            Debug.LogError($"Could not find :  {name}");
        }
        return item;
    }





    private static Transform SearchInternal(this Transform transform, string name)
    {
        var child = transform.Find(name);
        if (child != null)
        {
            return child;
        }
        else
        {
            foreach (Transform newChild in transform)
            {
                child = newChild.Search(name);
                if (child != null)
                {
                    return child;
                }
            }
        }
        return null;
    }

    public static Transform Search(this Transform transform, string name)
    {
        if (true == string.IsNullOrEmpty(name))
        {
            Debug.LogError($"Name cannot be null or empty,  gameObject: {transform.gameObject.GetGameObjectPath()}");
        }

        Transform returnTransform = transform.SearchInternal(name);
        if ((null == returnTransform))
        {
            // Debug.LogError($"Cannot find {name} in gameObject: {transform.gameObject.GetGameObjectPath()}");
        }
        return returnTransform;
    }

    public static GameObject Search(this GameObject gameObject, string name)
    {
        if (true == string.IsNullOrEmpty(name))
        {
            Debug.LogError($"Name cannot be null or empty,  gameObject: {gameObject.GetGameObjectPath()}");
        }

        Transform returnTransform = gameObject.transform.SearchInternal(name);
        if ((null == returnTransform))
        {
            return null;
        }

        return returnTransform.gameObject;
    }


    // searchs for name plus number eg    fred   ->  fred, fred0, fred1, fred2, fred3
    public static List<Transform> SearchListWithIncrementNumber(this Transform transform, string name)
    {
        //if (true == string.IsNullOrEmpty(name))
        //{
        //    Debug.LogError($"Name cannot be null or empty,  gameObject: {transform.gameObject.GetGameObjectPath()}");
        //}
        List<Transform> newList = new List<Transform>();
        Transform returnTransform = transform.SearchInternal(name);

        int count = 0;
        if (returnTransform != null)
        {
            newList.Add(returnTransform);
            while (true)
            {
                string nameNumber = name + count;
                Transform numberTransform = transform.SearchInternal(nameNumber);
                if (numberTransform != null)
                {
                    newList.Add(numberTransform);
                    count++;
                }
                else
                {
                    break;
                }
            }
        }

        return newList;
    }



    public static List<Transform> SearchAll(this Transform transform, string name)
    {
        var children = transform.GetDirectChildren();
        var found = children.FindAll(t => t.name == name);
        var foundChildren = children.Extract(t => t.SearchAll(name));
        found.AddRange(foundChildren.Flatten());
        return found;
    }



    public static Transform FindSibling(this Transform transform, string name)
    {
        return transform.parent.Find(name);
    }

    public static int SiblingCount(this Transform transform)
    {
        return transform.parent.childCount;
    }

    public static List<Transform> GetDirectChildren(this Transform transform)
    {
        var children = new List<Transform>();
        foreach (Transform child in transform)
        {
            children.Add(child);
        }
        return children;
    }

    public static T GetSelfOrDirectChildren<T>(this Transform transform) where T : Behaviour
    {
        T item = transform.GetComponent<T>();
        if (item == null)
        {
            var list = transform.GetDirectChildren();
            foreach (var item1 in list)
            {
                item = item1.GetComponent<T>();
                if (item != null)
                {
                    return item;
                }
            }
        }
        return item;
    }

    public static List<Transform> GetAllChildren(this Transform transform)
    {
        var children = new List<Transform>();
        foreach (Transform child in transform)
        {
            children.AddRange(child.GetAllChildren());
            children.Add(child);
        }
        return children;
    }

    public static Dictionary<string, Transform> SearchChildren(this Transform transform)
    {
        var children = new Dictionary<string, Transform>();
        foreach (Transform child in transform)
        {
            children.Add(child.name, child);

            var grandChildren = child.SearchChildren();
            foreach (var grandChild in grandChildren)
            {
                children.Add(grandChild.Key, grandChild.Value);
            }
        }
        return children;
    }

    public static List<Transform> SearchChildren(this Transform transform, string name)
    {
        var children = GetAllChildren(transform);
        var matching = new List<Transform>();
        foreach (Transform child in children)
        {
            if (child.name == name)
            {
                matching.Add(child);
            }
        }

        return matching;
    }

    public static List<Transform> SearchChildrenWithNameContainsList(this Transform transform, string name)
    {
        var children = GetAllChildren(transform);
        var matching = new List<Transform>();
        foreach (Transform child in children)
        {
            if (true == child.name.CaseInsensitiveContains(name))
            {
                matching.Add(child);
            }
        }

        return matching;
    }

    public static Transform SearchChildrenWithNameContains(this Transform transform, string name)
    {
        if (transform.name.CaseInsensitiveContains(name))
        {
            // this object starts with the string passed in "start":
            // do whatever you want with it...
            return transform;
        }
        // now search in its children, grandchildren etc.
        foreach (Transform newChild in transform)
        {
            Transform returnTransform = SearchChildrenWithNameContains(newChild, name);
            if (null != returnTransform)
            {
                return returnTransform;
            }
        }
        return null;
    }

    public static Transform FindParent(this Transform transform, string name)
    {
        if (transform.parent == null)
        {
            return null;
        }
        else if (transform.parent.name == name)
        {
            return transform.parent;
        }
        else
        {
            return transform.parent.FindParent(name);
        }
    }

    public static void Reset(this Transform transform, bool withScale = true)
    {
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        if (withScale == true)
        {
            transform.localScale = Vector3.one;
        }
    }

    public static void OrientTo(this Transform transform, Transform target, bool withScale = true)
    {
        transform.position = target.position;
        transform.rotation = target.rotation;

        if (withScale == true)
        {
            transform.localScale = target.localScale;
        }
    }

    public static void SnapToParent(this Transform transform, Transform parent, bool withScale = true)
    {
        if (transform.parent != parent)
        {
            transform.SetParent(parent, false);
        }
        transform.Reset(withScale);
    }

    public static List<Transform> GetOtherChildren(this Transform transform, params Component[] components)
    {
        var children = transform.FindComponents<Transform>();
        foreach (var component in components)
        {
            children.Remove(component.transform);
        }
        return children;
    }

    public static T ForceComponent<T>(this GameObject go) where T : Component
    {
        var component = go?.GetComponent<T>();
        if (component == null)
        {
            return go?.AddComponent<T>();
        }
        return component;
    }


    public static T FindAndForceComponent<T>(this GameObject go) where T : Component
    {
        var fff = typeof(T);
        var nameToSearch = fff.Name;
        var trans = go.transform.Search(nameToSearch);
        var component = trans?.GetComponent<T>();
        if (component == null)
        {
            return trans?.AddComponent<T>();
        }
        return component;
    }

    /// <summary>
    /// From    SliderExtraPanel SliderExtraPanelRef = new SliderExtraPanel(this.transform.Search("SliderExtraPanel").gameObject.ForceComponent<SliderExtraPanelMono>());
    /// To      SliderExtraPanel SliderExtraPanelRef = this.gameObject.FindAndForceComponentMono<SliderExtraPanel, SliderExtraPanelMono>();
    /// </summary>
    public static NonMono FindAndForceComponentMono<NonMono, Mono>(this GameObject go) where Mono : Component
    {
        var nonMono = typeof(NonMono);
        var nameToSearch = nonMono.Name;
        var trans = go.transform.Search(nameToSearch);
        var component = trans?.GetComponent<Mono>();
        if (component == null)
        {
            component = trans?.AddComponent<Mono>();
        }

        ConstructorInfo ctor = nonMono.GetConstructor(new[] { typeof(Mono) });
        object instance = ctor.Invoke(new object[] { component });
        return (NonMono)instance;
    }

    public static void ReAssignChildrenRenderShaders(this GameObject go)
    {
        // FIX_SHADERS
//#if UNITY_ANDROID || UNITY_WEBGL|| UNITY_EDITOR
        Renderer[] renderers = go.GetComponentsInChildren<Renderer>(true);
        for (int i = 0; i < renderers.Length; i++)
        {
            for (int j = 0; j < renderers[i].materials.Length; j++)
            {
                Shader shader = Shader.Find(renderers[i].materials[j].shader.name);
                if (null != shader)
                {
                    renderers[i].materials[j].shader = shader;
                }
                else
                {
                    Debug.LogError($"Cannot find shader {renderers[i].materials[j].shader.name}");
                }
            }
        }
//#endif
    }


    public static void DestroyComponent<T>(this Component component) where T : Component
    {
        component.GetComponent<T>().DestroyObject();
    }

    public static void DestroyComponent<T>(this GameObject go) where T : Component
    {
        go.GetComponent<T>().DestroyObject();
    }

    public static T AddComponent<T>(this Component component) where T : Component
    {
        return component.gameObject.AddComponent<T>();
    }

    public static string GetGameObjectPath(this Transform transform)
    {
        string path = transform.name;
        while (transform.parent != null)
        {
            transform = transform.parent;
            path = transform.name + "/" + path;
        }
        return path;
    }

    public static string GetGameObjectPath(this GameObject gameObject)
    {
        return GetGameObjectPath(gameObject.transform);
    }


    public static T GetComponentInParentNotSelf<T>(this GameObject gameObject) where T : Component
    {
        var all = gameObject.GetComponentsInParent<T>(true).ToList();
        var self = gameObject.GetComponent<T>();

        foreach (var item in all)
        {
            if (item.gameObject != self.gameObject) // checks gameObject in case you have multiply of the same Component 
            {
                return item;
            }
        }

        return null;
    }



    public static int GetGameObjectParentCount(this Transform transform)
    {
        int count = 0;
        while (transform.parent != null)
        {
            transform = transform.parent;
            count++;
        }
        return count;
    }

    public static int GetGameObjectParentCount(this GameObject gameObject)
    {
        return GetGameObjectParentCount(gameObject.transform);
    }


    /// <summary>
    /// Places object in a flat plane in front of the camera
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="distance"></param>
    public static void PlaceInfrontMainCamera(this GameObject obj, float distance)
    {
        if (Camera.main != null)
        {
            var trans = obj.transform;
            trans.position = Camera.main.transform.position;
            trans.rotation = Camera.main.transform.rotation;

            // move flat amount in front
            var forward = Camera.main.transform.forward;
            forward.y = 0;
            forward.Normalize();
            trans.position += forward * distance;
            trans.rotation = Quaternion.LookRotation(trans.position - Camera.main.transform.position);
        }
    }

    /// <summary>
    /// its a look at with out changing the rotation,
    /// but instead changes to position so it looks at
    /// </summary>
    /// <param name="objToMove"></param>
    /// <param name="target"></param>
    public static void MoveToLookAtRealive(this GameObject objToMove, GameObject target)
    {
        float distance = Vector3.Distance(objToMove.transform.position, target.transform.position);

        objToMove.transform.position = (target.transform.position);
        objToMove.transform.position += objToMove.transform.forward * -distance;
    }


    public static void CopyMainSettings(this Camera dest, Camera source)
    {
        dest.backgroundColor = source.backgroundColor;

        dest.useOcclusionCulling = source.useOcclusionCulling;
        dest.depthTextureMode = source.depthTextureMode;
        dest.clearFlags = source.clearFlags;
        dest.cullingMask = source.cullingMask;

        dest.orthographic = source.orthographic;
        dest.orthographicSize = source.orthographicSize;
        dest.allowDynamicResolution = source.allowDynamicResolution;
        dest.allowMSAA = source.allowMSAA;
        dest.allowHDR = source.allowHDR;
        dest.eventMask = source.eventMask;
        dest.rect = source.rect;

        dest.stereoTargetEye = source.stereoTargetEye;
        dest.stereoConvergence = source.stereoConvergence;
        dest.stereoSeparation = source.stereoSeparation;

        dest.pixelRect = source.pixelRect;

        dest.targetDisplay = source.targetDisplay;
        dest.targetTexture = source.targetTexture;

        dest.renderingPath = source.renderingPath;
        dest.farClipPlane = source.farClipPlane;
        dest.nearClipPlane = source.nearClipPlane;

        if (source.stereoTargetEye == StereoTargetEyeMask.None)
        {
            dest.fieldOfView = source.fieldOfView;
        }
    }

    public static void CopySettings(this Camera dest, Camera source)
    {
        dest.clearStencilAfterLightingPass = source.clearStencilAfterLightingPass;

        dest.usePhysicalProperties = source.usePhysicalProperties;
        dest.backgroundColor = source.backgroundColor;
        dest.sensorSize = source.sensorSize;

        dest.lensShift = source.lensShift;
        dest.cullingMatrix = source.cullingMatrix;
        dest.useOcclusionCulling = source.useOcclusionCulling;
        dest.layerCullDistances = source.layerCullDistances;
        dest.cameraType = source.cameraType;
        dest.layerCullSpherical = source.layerCullSpherical;
        dest.depthTextureMode = source.depthTextureMode;
        dest.clearFlags = source.clearFlags;
        dest.cullingMask = source.cullingMask;

        dest.focalLength = source.focalLength;
        dest.aspect = source.aspect;
        dest.depth = source.depth;

        dest.transparencySortAxis = source.transparencySortAxis;
        dest.transparencySortMode = source.transparencySortMode;
        dest.opaqueSortMode = source.opaqueSortMode;
        dest.orthographic = source.orthographic;
        dest.orthographicSize = source.orthographicSize;
        dest.forceIntoRenderTexture = source.forceIntoRenderTexture;
        dest.allowDynamicResolution = source.allowDynamicResolution;
        dest.allowMSAA = source.allowMSAA;
        dest.allowHDR = source.allowHDR;
        dest.eventMask = source.eventMask;
        dest.rect = source.rect;

        dest.stereoTargetEye = source.stereoTargetEye;

        dest.stereoConvergence = source.stereoConvergence;
        dest.stereoSeparation = source.stereoSeparation;
        dest.pixelRect = source.pixelRect;
        dest.useJitteredProjectionMatrixForTransparentRendering = source.useJitteredProjectionMatrixForTransparentRendering;
        dest.nonJitteredProjectionMatrix = source.nonJitteredProjectionMatrix;
        dest.projectionMatrix = source.projectionMatrix;
        dest.worldToCameraMatrix = source.worldToCameraMatrix;
        dest.targetDisplay = source.targetDisplay;
        dest.targetTexture = source.targetTexture;

        dest.scene = source.scene;
        dest.renderingPath = source.renderingPath;
        dest.farClipPlane = source.farClipPlane;
        dest.nearClipPlane = source.nearClipPlane;

        if (source.stereoTargetEye == StereoTargetEyeMask.None)
        {
            dest.fieldOfView = source.fieldOfView;
        }
    }
}
