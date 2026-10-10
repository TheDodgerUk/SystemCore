using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;



public class CachedObjects : Singleton<CachedObjects>
{
    private CachedObjectsMono m_CachedObjects;
    protected override void Construct()
    {
        m_CachedObjects = Core.Mono.gameObject.ForceComponent<CachedObjectsMono>();
    }

    public void RemoveAllOptional() => m_CachedObjects.RemoveAllOptional();
    public void AddObjectsToCache(Scene scene, Transform parent, EnvironmentManager.ChunkType chunkType) => m_CachedObjects.AddObjectsToCache(scene, parent, chunkType);
    public GameObject GetPrefab(string name) => m_CachedObjects.GetPrefab(name);
    public List<string> GetItemsWhichContain(string name) => m_CachedObjects.GetItemsWhichContain(name);

    public GameObject InstantiateCachedGameObject(string name, bool removeCloneName = true) => m_CachedObjects.InstantiateCachedGameObject(name, removeCloneName);
    public GameObject InstantiateCachedGameObject(string name, Transform parent, bool removeClone = true) => m_CachedObjects.InstantiateCachedGameObject(name, parent, removeClone);


    public class CachedObjectsMono : MonoBehaviour
    {
        public const string TAG = "[CachedObjects]";

        [SerializeField]
        private Dictionary<string, GameObject> m_DictOfCachedObjects = new Dictionary<string, GameObject>();
        private SimpleDictionaryDictionary<Scene, string, GameObject> m_DictOfCachedObjectsOptional = new SimpleDictionaryDictionary<Scene, string, GameObject>();
        private bool m_HasBeenInitilised = false;

        protected void Awake()
        {
            Core.Environment.OnPreLoadEnvironment += OnPreEnvironmentLoad;
            Core.Environment.OnEnvironmentLoadingComplete += OnEnvirmonetCompleate;
        }

        private void OnEnvirmonetCompleate()
        {
            m_HasBeenInitilised = true;
        }

        private void OnPreEnvironmentLoad()
        {
            foreach (var item in m_DictOfCachedObjects)
            {
                Destroy(item.Value);
            }
            m_DictOfCachedObjects.Clear();
        }

        public void RemoveAllOptional()
        {
            foreach (var values in m_DictOfCachedObjectsOptional.Values())
            {
                m_DictOfCachedObjects.RemoveByValue(values);
            }
            m_DictOfCachedObjectsOptional.ClearAll();
        }

        public void AddObjectsToCache(Scene scene, Transform parent, EnvironmentManager.ChunkType chunkType)
        {
            parent.gameObject.ReAssignChildrenRenderShaders();
            List<Transform> childrenRef = parent.GetDirectChildren();
            for (int i = 0; i < childrenRef.Count; i++)
            {
                string name = childrenRef[i].name;
                if (false == m_DictOfCachedObjects.ContainsKey(name))
                {
                    m_DictOfCachedObjects.Add(name, childrenRef[i].gameObject);
                    childrenRef[i].gameObject.ReAssignChildrenRenderShaders();
                    parent.SetActive(false);
                    if (chunkType == EnvironmentManager.ChunkType.Optional)
                    {
                        m_DictOfCachedObjectsOptional.AddValue(scene, name, childrenRef[i].gameObject);
                    }
                }
                else
                {
                    Debug.LogError($" {name} already exists, please check your scenes");
                }
            }
        }


        public GameObject GetPrefab(string name)
        {
            if (m_HasBeenInitilised == false)
            {
                Debug.LogError($"might have to wait until     Core.Environment.OnEnvironmentLoadingComplete");
            }
            if (true == m_DictOfCachedObjects.ContainsKey(name))
            {
                return m_DictOfCachedObjects[name];
            }

            return null;
        }

        public List<string> GetItemsWhichContain(string name)
        {
            if (m_HasBeenInitilised == false)
            {
                Debug.LogError($"might have to wait until     Core.Environment.OnEnvironmentLoadingComplete");
            }
            List<string> names = new List<string>();
            foreach (string item in m_DictOfCachedObjects.Keys)
            {
                if (true == item.CaseInsensitiveContains(name))
                {
                    names.Add(item);
                }
            }
            return names;
        }



        public GameObject InstantiateCachedGameObject(string name, bool removeCloneName = true)
        {
            if (m_HasBeenInitilised == false)
            {
                Debug.LogError($"might have to wait until     Core.Environment.OnEnvironmentLoadingComplete");
            }
            if (true == m_DictOfCachedObjects.ContainsKey(name))
            {
                GameObject t = Instantiate(m_DictOfCachedObjects[name]) as GameObject;
                if (true == removeCloneName)
                {
                    t.name = name;
                }
                return t;
            }
            else
            {
                Debug.LogError($" {name} does not exist");
                return null;
            }
        }

        public GameObject InstantiateCachedGameObject(string name, Transform parent, bool removeClone = true)
        {
            if (m_HasBeenInitilised == false)
            {
                Debug.LogError($"might have to wait until     Core.Environment.OnEnvironmentLoadingComplete");
            }
            if (true == m_DictOfCachedObjects.ContainsKey(name))
            {
                GameObject t = Instantiate(m_DictOfCachedObjects[name], parent);
                if (true == removeClone)
                {
                    t.name = name;
                }
                return t;
            }
            else
            {
                Debug.LogError($" {name} does not exist");
                return null;
            }
        }
    }
}
