using System;
using System.Collections.Generic;
using UnityEngine;

public class Materialiser : MonoBehaviour
{
    private const float DissolveTime = 1.8f;

    private List<MaterialisedRenderer> m_Renderers;
    private ValueTween m_CurrentTween;
    private float m_DissolveAmount;

    public void InitialiseRenderers(Material materialiserMaterial)
    {
        var renderers = GetComponentsInChildren<MeshRenderer>().ToList();
        m_Renderers = GetMaterialisedRenderers(renderers, materialiserMaterial);
    }

    public void Initialise(bool showModel, float dissolveAmount)
    {
        ToggleShowModel(showModel);
        SetDissolveAmount(dissolveAmount);
    }

    public void ToggleShowModel(bool state)
    {
        foreach (var renderer in m_Renderers)
        {
            renderer.ToggleShowModel(state);
        }
    }

    public void ModifyMaterial(Action<Material> modifier)
    {
        foreach (var renderer in m_Renderers)
        {
            modifier(renderer.Materialised);
        }
    }

    public void Materialise(Action callback = null) => Materialise(DissolveTime, callback);
    public void Materialise(float duration, Action callback = null)
    {
        Fade(1f, duration, () =>
        {
            Initialise(true, m_DissolveAmount);
            Fade(0f, duration, () =>
            {
                ToggleMaterialised(false);
                callback?.Invoke();
            });
        });
    }

    public void Fade(float targetValue, Action callback = null) => Fade(targetValue, DissolveTime, callback);
    public void Fade(float targetValue, float duration, Action callback = null)
    {
        ToggleMaterialised(true);

        m_CurrentTween?.Stop();
        m_CurrentTween = this.Create<ValueTween>(duration, EaseType.SineInOut, callback);
        m_CurrentTween.Initialise(m_DissolveAmount, targetValue, SetDissolveAmount);
    }

    public void SetDissolveAmount(float amount)
    {
        foreach (var renderer in m_Renderers)
        {
            renderer.SetDissolveAmount(amount);
        }
        m_DissolveAmount = amount;
    }

    public void ToggleMaterialised(bool state)
    {
        foreach (var renderer in m_Renderers)
        {
            renderer.ToggleMaterialised(state);
        }
    }

    private List<MaterialisedRenderer> GetMaterialisedRenderers(List<MeshRenderer> meshRenderers, Material materialiserMaterial)
    {
        // group renderers by material
        var groupedRenderers = new Dictionary<Material, List<Renderer>>();
        foreach (var renderer in meshRenderers)
        {
            foreach (var material in renderer.sharedMaterials)
            {
                if (material != null)
                {
                    if (false == groupedRenderers.ContainsKey(material))
                    {
                        groupedRenderers.Add(material, new List<Renderer>());
                    }
                    groupedRenderers[material].Add(renderer);
                }
                else
                {
                    Debug.LogError($"material is null: {gameObject.GetGameObjectPath()}", this);
                }
            }
        }

        // create materialised renderers from groups
        var renderers = new List<MaterialisedRenderer>();
        foreach (var group in groupedRenderers)
        {
            renderers.Add(new MaterialisedRenderer(group.Value, group.Key, materialiserMaterial, this.gameObject));
        }
        return renderers;
    }

    private class MaterialisedRenderer
    {
        private static readonly List<string> ColourProperties = new List<string>
        {
            "_Color",
            "_EmissionColor",
        };
        private static readonly List<string> FloatProperties = new List<string>
        {
            "_BumpScale",
            "_Cutoff",
            "_DetailNormalMapScale",
            "_DstBlend",
            "_GlossMapScale",
            "_Glossiness",
            "_GlossyReflections",
            "_Metallic",
            "_Mode",
            "_OcclusionStrength",
            "_Parallax",
            "_SmoothnessTextureChannel",
            "_SpecularHighlights",
            "_SrcBlend",
            "_UVSec",
            "_ZWrite",
        };
        private static readonly List<string> TexProperties = new List<string>
        {
            "_BumpMap",
            "_DetailAlbedoMap",
            "_DetailMask",
            "_DetailNormalMap",
            "_EmissionMap",
            "_MainTex",
            "_MetallicGlossMap",
            "_OcclusionMap",
            "_ParallaxMap"
        };

        private const string DissolveAmount = "_Dissolve";
        private const string ShowModel = "_ShowModel";

        public Material Materialised => m_Materialised;

        private List<Renderer> m_Renderers;

        private Material m_Materialised;
        private Material m_Original;
        private GameObject m_GameObject;

        public MaterialisedRenderer(List<Renderer> renderers, Material original, Material materialised, GameObject gameObject)
        {
            m_GameObject = gameObject;
            m_Materialised = new Material(materialised);
            m_Original = new Material(original);
            m_Renderers = renderers;

            CopyMaterialProperties(m_Original, m_Materialised);
        }

        public void ToggleMaterialised(bool state)
        {
            for (int i = 0; i < m_Renderers.Count; i++)
            {
                m_Renderers[i].material = state ? m_Materialised : m_Original;
            }
        }

        public void ToggleShowModel(bool state)
        {
            m_Materialised.SetFloat(ShowModel, state ? 1f : 0f);
        }

        public void SetDissolveAmount(float amount)
        {
            m_Materialised.SetFloat(DissolveAmount, amount);
        }

        private void CopyMaterialProperties(Material source, Material destination)
        {
            foreach (string property in TexProperties)
            {
                if (source.HasProperty(property) == true)
                    destination.SetTexture(property, source.GetTexture(property));
                ////// too many errors   else Debug.LogError($"{m_GameObject.GetGameObjectPath()} does not have: {property}", m_GameObject);
            }
            foreach (string property in FloatProperties)
            {
                if (source.HasProperty(property) == true)
                    destination.SetFloat(property, source.GetFloat(property));
            }
            foreach (string property in ColourProperties)
            {
                if (source.HasProperty(property) == true)
                    destination.SetColor(property, source.GetColor(property));
            }

            foreach (var keyword in source.shaderKeywords)
            {
                destination.EnableKeyword(keyword);
            }
        }
    }
}