using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class MonoSingleton<T> : MonoBehaviour where T : Component
{
    public static T Instance { get; private set; }

    public static void CreateInstance()
    {
        if (Instance == null)
        {
            var root = GetSingletonRoot();
            var go = new GameObject(typeof(T).Name);
            go.transform.SetParent(root.transform, false);
            Instance = go.AddComponent<T>();
        }
    }

    public static void DestroyInstance()
    {
        if (Instance != null)
        {
            GameObject.Destroy(Instance.gameObject);
            Instance = null;
        }
    }

    protected virtual void Awake()
    {
        if (Instance == null)
        {
            Instance = GetComponent<T>();
        }
    }

    private static GameObject GetSingletonRoot()
    {
        const string RootName = "Singletons";
        var root = GameObject.Find(RootName);
        if (root == null)
        {
            root = new GameObject(RootName);
            root.transform.Reset(true);
        }
        return root;
    }
}
