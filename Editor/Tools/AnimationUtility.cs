using EditorTools;
using FilePathHelper;
using UnityEditor;
using UnityEngine;

public static class AnimationUtility
{
    [MenuItem("Assets/Create/Animation Clip from Pose")]
    public static void ConvertToAnimations()
    {
        foreach (var model in Selection.gameObjects)
        {
            var importer = model?.GetImporter<ModelImporter>();
            if (importer != null)
            {
                importer.animationType = ModelImporterAnimationType.Generic;
                importer.optimizeGameObjects = false;

                var clip = new AnimationClip
                {
                    frameRate = 30,
                    legacy = false,
                    name = model.name,
                    wrapMode = WrapMode.Loop,
                };
                foreach (var transformPath in importer.transformPaths)
                {
                    AddPoseToClip(model, clip, transformPath);
                }
                var path = FilePath.FromRelative(importer.assetPath);
                var animPath = path.RemoveExt().AppendExt(".anim");
                CreateOrOverwriteAsset(clip, animPath.Relative.Strip("@"));
            }
        }
    }

    private static void AddPoseToClip(GameObject model, AnimationClip clip, string transformPath)
    {
        var target = model.transform;
        if (string.IsNullOrEmpty(transformPath) == false)
        {
            target = target.Find(transformPath);
        }

        string position = nameof(Transform.localPosition);
        string rotation = nameof(Transform.localRotation);
        string scale = nameof(Transform.localScale);

        SetCurve(clip, transformPath, $"{position}.x", target.localPosition.x);
        SetCurve(clip, transformPath, $"{position}.y", target.localPosition.y);
        SetCurve(clip, transformPath, $"{position}.z", target.localPosition.z);

        SetCurve(clip, transformPath, $"{rotation}.x", target.localRotation.x);
        SetCurve(clip, transformPath, $"{rotation}.y", target.localRotation.y);
        SetCurve(clip, transformPath, $"{rotation}.z", target.localRotation.z);
        SetCurve(clip, transformPath, $"{rotation}.w", target.localRotation.w);

        SetCurve(clip, transformPath, $"{scale}.x", target.localScale.x);
        SetCurve(clip, transformPath, $"{scale}.y", target.localScale.y);
        SetCurve(clip, transformPath, $"{scale}.z", target.localScale.z);
    }

    private static void CreateOrOverwriteAsset<T>(T asset, string path) where T : Object
    {
        var loadedAsset = AssetDatabase.LoadAssetAtPath<T>(path);
        if (loadedAsset != null)
        {
            EditorUtility.CopySerialized(asset, loadedAsset);
        }
        else
        {
            AssetDatabase.CreateAsset(asset, path);
        }
    }

    private static void SetCurve(AnimationClip clip, string path, string property, float value)
    {
        clip.SetCurve(path, typeof(Transform), property, CreateCurve(value));
    }

    private static AnimationCurve CreateCurve(float value)
    {
        return new AnimationCurve(new Keyframe { value = value, time = 0, });
    }
}
