using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class UnityExtensions
{
    public static void DestroyObject(this Object obj)
    {
        if (obj != null)
        {
            Object.Destroy(obj);
        }
    }

    public static void DestroyGameObject<T>(this T component) where T : Component
    {
        if (component != null)
        {
            Object.Destroy(component.gameObject);
        }
    }

    public static void DestroyObjectImmediate(this Object obj, bool allowDestroyAssets = false)
    {
        if (obj != null)
        {
            Object.DestroyImmediate(obj, allowDestroyAssets);
        }
    }

    public static void RebuildLayout(this RectTransform rect)
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
    }

    public static RectTransform ToRect(this Transform transform)
    {
        return transform as RectTransform;
    }

    public static RectTransform GetRect<T>(this T component) where T : Component
    {
        return component.transform.ToRect();
    }

    public static Vector2 GetCentre(this RectTransform transform)
    {
        var corners = new Vector3[4];
        transform.GetLocalCorners(corners);

        var total = Vector2.zero;
        for (int i = 0; i < corners.Length; ++i)
        {
            total += (Vector2)corners[i];
        }
        var average = (total / corners.Length);
        return transform.anchoredPosition + average;
    }

    public static Vector3 GetRelativePosition(this Transform target, Transform anchor)
    {
        if (target.IsChildOf(anchor) == false)
        {
            throw new System.Exception("Target must be a child of anchor!");
        }

        if (target.parent != anchor)
        {
            return target.parent.GetRelativePosition(anchor) + target.localPosition;
        }
        else
        {
            return target.localPosition;
        }
    }

    public static void SetStaticRecursivelyStartsWithStatic(this Transform go)
    {
        if(go.name.StartsWith("Static", System.StringComparison.CurrentCultureIgnoreCase) == true)
        {
            go.gameObject.isStatic = true;
            foreach (Transform child in go.transform)
            {
                SetStaticRecursively(child.gameObject, true);
            }
        }
    }

    public static void SetStaticRecursively(this GameObject go, bool setStatic, bool log = false)
    {       
        if(log == true)
        {
            Debug.LogError($"name starts with static, but not set as path: {go.GetGameObjectPath()}, SceneName: {go.scene.name}");
        }
        ///go.isStatic = setStatic;
        foreach (Transform child in go.transform)
        {
            SetStaticRecursively(child.gameObject, setStatic);
        }
    }

    public static void SetLayerRecursively(this GameObject go, int layer)
    {
        go.layer = layer;
        foreach (Transform child in go.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }

    public static void SetTagRecursively(this GameObject go, string tag)
    {
        go.tag = tag;
        foreach (Transform child in go.transform)
        {
            SetTagRecursively(child.gameObject, tag);
        }
    }

    public static Bounds GetEncapsulatingBounds(this GameObject obj)
    {
        Bounds totalBounds = new Bounds();
        var all = obj.GetComponentsInChildren<Renderer>(true);
        foreach (var renderer in all)
        {
            if (renderer.GetComponent<VLB.BeamGeometry>() == null)
            {
                totalBounds.Encapsulate(renderer.bounds);
                //continue; // these are used to fake lights, very quick, but messes up this function
            }
        }
        return totalBounds;
    }


    public static Color ToColour(this Vector3 v)
    {
        return new Color(v.x, v.y, v.z);
    }

    public static string ToAccurateString(this Vector3 v)
    {
        return  $"x:{v.x}    y:{v.y}    z:{v.z} ";
    }

    public static bool ContainsWithin(this Vector3 v, Vector3 v1, Vector3 v2)
    {
        if(v.x < v1.x || v.x > v2.x)
        {
            return false;
        }
        if (v.y < v1.y || v.y > v2.y)
        {
            return false;
        }
        if (v.z < v1.z || v.z > v2.z)
        {
            return false;
        }
        return true;
    }

    public static bool ContainsWithin(this int v, int v1, int v2)
    {
        return (v >= v1 && v <= v2);
    }

    public static Vector3 ToVector3(this Color c)
    {
        return new Vector3(c.r, c.g, c.b);
    }

    public static Tuple<Vector3, Vector3> GetVertexRange(this Mesh mesh)
    {
        var verts = mesh.vertices;
        var min = Vector3.one * float.MaxValue;
        var max = Vector3.one * float.MinValue;
        for (int i = 0; i < verts.Length; ++i)
        {
            min = min.Min(verts[i]);
            max = max.Max(verts[i]);
        }
        return Tuple.New(min, max);
    }

    public static float CalculateSurfaceArea(this Mesh mesh)
    {
        var triangles = mesh.triangles;
        var vertices = mesh.vertices;

        double sum = 0.0;

        for (int i = 0; i < triangles.Length; i += 3)
        {
            Vector3 corner = vertices[triangles[i]];
            Vector3 a = vertices[triangles[i + 1]] - corner;
            Vector3 b = vertices[triangles[i + 2]] - corner;

            sum += Vector3.Cross(a, b).magnitude;
        }

        float size = (float)(sum / 2.0);
        return size;
    }



    public static Vector2 GetFrustumSizeAtDistance(this Camera camera, float distance)
    {
        float fov = (camera.fieldOfView * 0.5f).ToRadians();
        float height = 2.0f * distance * Mathf.Tan(fov);
        float width = height * camera.aspect;
        return new Vector2(width, height);
    }

    public static Ray GameObjectToRay(this Camera camera, GameObject obj)
    {
        var targetScreen = camera.WorldToScreenPoint(obj.transform.position);
        targetScreen.z = 0;
        return Camera.main.ScreenPointToRay(targetScreen);
    }

    public static Ray PositionToRay(this Camera camera, Vector3 pos)
    {
        var targetScreen = camera.WorldToScreenPoint(pos);
        targetScreen.z = 0;
        return Camera.main.ScreenPointToRay(targetScreen);
    }

    public static Vector3 Abs(Vector3 vector)
    {
        vector.x = Mathf.Abs(vector.x);
        vector.y = Mathf.Abs(vector.y);
        vector.z = Mathf.Abs(vector.z);
        return vector;
    }

    public static void Play(this AudioSource audio, MonoBehaviour host, System.Action callback)
    {
        host.StopAllCoroutines();

        float length = audio.clip.length / audio.pitch;
        host.WaitFor(length + 0.5f, callback);
        audio.Stop();
        audio.time = 0;
        audio.Play();
    }

    public static void SetEmissionRate(this ParticleSystem particleSystem, float emissionRate)
    {
        var emission = particleSystem.emission;
        emission.rateOverTime = emissionRate;
    }

    public static void SetAlpha(this Graphic graphic, float alpha)
    {
        var colour = graphic.color;
        colour.a = alpha;
        graphic.color = colour;
    }

    public static void SetAlpha(this Material mat, float alpha, string property = null)
    {
        string localProperty = "_Color";
        if(string.IsNullOrEmpty(property) == false)
        {
            localProperty = property;
        }
        var colour = mat.GetColor(localProperty);
        colour.a = alpha;
        mat.SetColor(localProperty, colour);
    }

    public static void SetAlpha(this Renderer renderer, float alpha, string property = null)
    {
        renderer.material.SetAlpha(alpha, property);
    }

    public static void SetAlpha(this SpriteRenderer sprite, float alpha)
    {
        var colour = sprite.color;
        colour.a = alpha;
        sprite.color = colour;
    }

    public static void SetTexOffset(this Renderer renderer, float? x = null, float? y = null)
    {
        var m = renderer.material;
        var o = m.mainTextureOffset;
        m.mainTextureOffset = new Vector2(x ?? o.x, y ?? o.y);
    }

    public static void SetTexScale(this Renderer renderer, float? x = null, float? y = null)
    {
        var m = renderer.material;
        var s = m.mainTextureScale;
        m.mainTextureScale = new Vector2(x ?? s.x, y ?? s.y);
    }

    public static void ToggleKeyword(this Material material, string property, bool state)
    {
        if (state == true)
        {
            material.EnableKeyword(property);
        }
        else
        {
            material.DisableKeyword(property);
        }
    }

    public static Material InstantiateMaterial(this Renderer renderer)
    {
        var material = new Material(renderer.sharedMaterial);
        renderer.material = material;
        return material;
    }

    public static void CopyStandardMaterialProperties(this Material destination, Material source)
    {
        foreach (string name in new[] { "_MetallicGlossMap", "_BumpMap", "_EmissionMap", "_MainTex", "_OcclusionMap" })
        {
            if (source.HasProperty(name) && destination.HasProperty(name))
            {
                destination.SetTexture(name, source.GetTexture(name));
                destination.SetTextureScale(name, source.GetTextureScale(name));
                destination.SetTextureOffset(name, source.GetTextureOffset(name));
            }
        }
        foreach (string name in new[] { "_Metallic", "_Emission" })
        {
            if (source.HasProperty(name) && destination.HasProperty(name))
            {
                destination.SetFloat(name, source.GetFloat(name));
            }
        }
        foreach (string name in new[] { "_Color", "_EmissionColor" })
        {
            if (source.HasProperty(name) && destination.HasProperty(name))
            {
                destination.SetColor(name, source.GetColor(name));
            }
        }
    }

    public static bool IsVisible(this Renderer renderer, Transform anchor, float sqrMaxDistance, bool flipDirection = false)
    {
        // is the renderer visible to any camera?
        if (renderer.isVisible == true)
        {
            // is the renderer close to the camera?
            var camera = CameraControllerVR.Instance.CameraTransform;
            var dir = camera.position - anchor.position;
            if (dir.sqrMagnitude < sqrMaxDistance == true)
            {
                // is the renderer facing towards the camera?
                float sign = flipDirection ? 1 : -1f;
                return dir.Dot(anchor.forward * sign) > 0;
            }
        }
        return false;
    }

    public static Rect GetRect(this Texture2D texture) => new Rect(0, 0, texture.width, texture.height);

    public static void Clear(this RenderTexture renderTexture)
    {
        var currentRenderTexture = RenderTexture.active;
        RenderTexture.active = renderTexture;
        GL.Clear(true, true, Color.clear);
        RenderTexture.active = currentRenderTexture;
    }

    public static void ReadFromRenderTexture(this Texture2D texture, RenderTexture renderTexture)
    {
        var currentRenderTexture = RenderTexture.active;
        RenderTexture.active = renderTexture;
        texture.ReadPixels(texture.GetRect(), 0, 0);
        RenderTexture.active = currentRenderTexture;
    }

    public static Texture2DArray ToTexture2dArray(this List<Texture2D> textures, bool mipmap, bool linear)
    {
        var t = textures[0];
        int count = textures.Count;
        var array = new Texture2DArray(t.width, t.height, count, t.format, mipmap, linear)
        {
            wrapMode = TextureWrapMode.Clamp,
            filterMode = FilterMode.Bilinear
        };
        return FillTex2dArray(textures, count, array);
    }

    public static Texture2DArray ToTexture2dArray(this List<Texture2D> textures, Texture2DArray array)
    {
        return FillTex2dArray(textures, textures.Count, array);
    }

    private static Texture2DArray FillTex2dArray(List<Texture2D> textures, int count, Texture2DArray array)
    {
        for (int i = 0; i < count; ++i)
        {
            if (textures[i] != null)
            {
                Graphics.CopyTexture(textures[i], 0, 0, array, i, 0);
            }
        }
        return array;
    }


    public static Sprite ToSprite(this Texture2D texture2D)
    {
        return Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f));
    }

    public static ComputeBuffer CreateComputeBuffer<T>(this T[] array)
    {
        int stride = array.Length > 0 ? Marshal.SizeOf(array[0]) : Marshal.SizeOf(typeof(T));
        var buffer = new ComputeBuffer(array.Length, stride);
        buffer.SetData(array);
        return buffer;
    }

    public static Transform CreateChild(this Transform parent, string name)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent);
        go.transform.Reset(true);
        return go.transform;
    }

    public static Component CreateChild(this Transform parent, System.Type type) => parent.CreateChild(type.Name).gameObject.AddComponent(type);
    public static T CreateChild<T>(this Transform parent, string name) where T : Component => parent.CreateChild(name).AddComponent<T>();
    public static T CreateChild<T>(this Transform parent) where T : Component => parent.CreateChild<T>(typeof(T).Name);

    public static string GetPathRelativeTo<T>(this T component, Transform relative) where T : Component => component.transform.GetPathRelativeTo(relative);
    public static string GetPathRelativeTo(this Transform transform, Transform relative) => Utils.Unity.GetPathRelativeTo(transform, relative);
    public static string GetScenePath(this Transform transform) => Utils.Unity.GetScenePath(transform);

    public static void AddEventTriggerListener(this EventTrigger trigger, EventTriggerType eventType, UnityAction<BaseEventData> callback)
    {
        var triggerCallback = new EventTrigger.TriggerEvent();
        triggerCallback.AddListener(callback);

        trigger.triggers.Add(new EventTrigger.Entry
        {
            eventID = eventType,
            callback = triggerCallback,
        });
    }

    public static void RemoveEventTriggerType(this EventTrigger trigger, EventTriggerType eventType)
    {
        trigger.triggers.RemoveAll(e => e.eventID == eventType);
    }

    public static void SetAnchorAndPivot(this RectTransform transform, float x, float y)
    {
        transform.anchorMin = new Vector2(x, y);
        transform.anchorMax = new Vector2(x, y);
        transform.pivot = new Vector2(x, y);
    }

    public static void SetAnchorMin(this RectTransform transform, float? x = null, float? y = null)
    {
        transform.anchorMin = transform.anchorMin.With(x, y);
    }

    public static void SetAnchorMax(this RectTransform transform, float? x = null, float? y = null)
    {
        transform.anchorMax = transform.anchorMax.With(x, y);
    }

    public static void SetHeight(this RectTransform transform, float height)
    {
        transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
    }

    public static void SetWidth(this RectTransform transform, float width)
    {
        transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
    }

    public static void SetAnchorPos(this RectTransform transform, float? x = null, float? y = null, float? z = null)
    {
        transform.anchoredPosition3D = transform.anchoredPosition3D.With(x, y, z);
    }

    public static void SetLocalPos(this Transform transform, float? x = null, float? y = null, float? z = null)
    {
        transform.localPosition = transform.localPosition.With(x, y, z);
    }

    public static void SetWorldPos(this Transform transform, float? x = null, float? y = null, float? z = null)
    {
        transform.position = transform.position.With(x, y, z);
    }

    public static void SetLocalRot(this Transform transform, float? x = null, float? y = null, float? z = null)
    {
        transform.localEulerAngles = transform.localEulerAngles.With(x, y, z);
    }

    public static void SetScale(this Transform transform, float? x = null, float? y = null, float? z = null)
    {
        var s = transform.localScale;
        if (s.x != x || s.y != y || s.z != z)
        {
            transform.localScale = s.With(x, y, z);
        }
    }

    public static void SetScale(this Transform transform, float scale)
    {
        transform.localScale = Vector3.one * scale;
    }

    public static void ClearLocals(this Transform transform)
    {
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }

    public static void ClearLocals(this GameObject go)
    {
        go.transform.ClearLocals();
    }

    public static void ClearPosRotLocals(this Transform transform)
    {
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public static void SetParentAndClearLocals(this Transform transform, Transform parent)
    {
        transform.SetParent(parent);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }

    public static Vector2 With(this Vector2 original, float? x = null, float? y = null)
    {
        original.x = x ?? original.x;
        original.y = y ?? original.y;
        return original;
    }

    public static Vector3 With(this Vector3 original, float? x = null, float? y = null, float? z = null)
    {
        original.x = x ?? original.x;
        original.y = y ?? original.y;
        original.z = z ?? original.z;
        return original;
    }

    public static GUIContent ToGUI(this string str) => new GUIContent(str);


    // in build masks are broken , this fix's it 
    public static void FixBrokenMasks(this CanvasGroup group)
    {
        var items = group.GetComponentsInChildren<Mask>(true);
        for (int i = 0; i < items.Length; i++)
        {
            items[i].gameObject.ForceComponent<RectMask2D>();
        }
    }

    // in build masks are broken , this fix's it 
    public static void FindAndFixMasks(this Transform group)
    {
        var items = group.GetComponentsInChildren<Mask>(true);
        for (int i = 0; i < items.Length; i++)
        {
            items[i].gameObject.ForceComponent<RectMask2D>();
        }
    }


    public static void VisibleAndInteractive(this CanvasGroup group, bool visible)
    {
        group.gameObject.SetActive(visible);
        group.alpha = visible ? 1f : 0f;
        group.interactable = visible;
        group.blocksRaycasts = visible;
    }
    public static void VisibleAndInteractiveInitilise(this CanvasGroup group)
    {
        if(group.alpha == 1)
        {
            group.VisibleAndInteractive(true);
        }
        else
        {
            group.VisibleAndInteractive(false);
        }
    }


    public static Vector3 ForwardFlat(this Transform item)
    {
        var forward = item.forward;
        forward.y = 0f;
        forward.Normalize();
        return forward;
    }

    public static Vector3 RightFlat(this Transform item)
    {
        var right = item.right;
        right.y = 0f;
        right.Normalize();
        return right;
    }

    public static Vector3 LocalForwardFlat(this Transform item)
    {
        var forward = item.TransformDirection(Vector3.forward);
        forward.y = 0f;
        forward.Normalize();
        return forward;
    }
}
