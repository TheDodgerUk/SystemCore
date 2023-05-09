using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class MaterialEmission : EditorWindow
{
    [MenuItem("Window/Utility/Material Emission")]
    public static void ShowWindow()
    {
        GetWindow<MaterialEmission>("Material Emission");
    }
    private const string Filter = "*.mat";


    private List<Material> m_Materials;
    private Vector2 m_ScrollPos;

    private void OnGUI()
    {
        if (GUILayout.Button("Grab Selected") == true)
        {
            var paths = GetFilesSelected();
            m_Materials = paths.Extract(p => AssetDatabase.LoadAssetAtPath<Material>(p));
        }

        if (m_Materials != null)
        {
            EditorGUILayout.FloatField("Count", m_Materials.Count);
            m_ScrollPos = EditorGUILayout.BeginScrollView(m_ScrollPos);
            {
                for (int i = 0; i < m_Materials.Count; ++i)
                {
                    EditorHelper.ObjectField(m_Materials[i]);
                }
            }
            EditorGUILayout.EndScrollView();

            if (GUILayout.Button("Disable Emission") == true)
            {
                for (int i = 0; i < m_Materials.Count; ++i)
                {
                    if (ShouldDisableEmission(m_Materials[i]) == false)
                    {
                        m_Materials.RemoveAt(i);
                        --i;
                    }
                    else
                    {
                        m_Materials[i].DisableKeyword("_EMISSION");
                        m_Materials[i].globalIlluminationFlags = MaterialGlobalIlluminationFlags.EmissiveIsBlack;
                    }
                }
            }
        }

    }

    private static bool ShouldDisableEmission(Material mat)
    {
        if (mat.shader.name != "Standard")
        {
            return false;
        }

        const string EmissionMap = "_EmissionMap";
        if (mat.HasProperty(EmissionMap) && mat.GetTexture(EmissionMap) != null)
        {
            return false;
        }

        const string EmissionColor = "_EmissionColor";
        if (mat.HasProperty(EmissionColor) && mat.GetColor(EmissionColor).grayscale > 0)
        {
            return false;
        }

        return true;
    }

    private static List<string> GetFilesSelected()
    {
        var selection = Selection.objects.ToList();
        var paths = new List<string>();
        foreach (var obj in selection)
        {
            var path = AssetDatabase.GetAssetPath(obj);
            if (Directory.Exists(path) == true)
            {
                paths.AddRange(GetFilesAtPath(path));
            }
            paths.Add(path);
        }
        return GetFiles(paths);
    }

    private static List<string> GetFilesAtPath(string path)
    {
        return GetFiles(Directory.GetFiles(path, Filter, SearchOption.AllDirectories).ToList());
    }

    private static List<string> GetFiles(List<string> paths)
    {
        string filter = Filter.Strip("*");
        paths = paths.DistinctList();
        paths.RemoveAll(p => p == null);
        paths.RemoveAll(p => p.Contains(filter) == false);
        return paths.Extract(p => p.Replace("\\", "/"));
    }
}
