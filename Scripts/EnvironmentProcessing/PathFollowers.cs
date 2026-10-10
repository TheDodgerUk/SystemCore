using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace EnvironmentHelpers
{
    public class PathFollowers : MonoBehaviour
    {
        [SerializeField]
        private List<GameObject> m_PathModels = new List<GameObject>();
        [SerializeField]
        private List<Paths> m_Paths = new List<Paths>();

        // Start is called before the first frame update
        public void Initialise()
        {
            Transform sourceModels = transform.Search("SourceModels");
            for (int i = 0; i < sourceModels.childCount; i++)
            {
                var model = sourceModels.GetChild(i).gameObject;
                model.SetActive(false);
                m_PathModels.Add(model);
            }

            Transform pathRoot = transform.Search("Paths");

            for(int i = 0; i < pathRoot.childCount; i++)
            {
                Transform child = pathRoot.GetChild(i).transform;
                var pathScript = child.AddComponent<Paths>();
                pathScript.Init(m_PathModels);
                m_Paths.Add(pathScript);
            }
        }

        private void Update()
        {
            for(int i = 0; i < m_Paths.Count; i++)
            {
                m_Paths[i].ManualUpdate();
            }
        }
    }
}