using EditorTools;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public class AnimatorEditor : EditorWindow
{
    [MenuItem("Window/Utility/Animator Editor")]
    public static void ShowWindow() => GetWindow<AnimatorEditor>("Animator Editor");

    private AnimatorController m_Controller;
    private List<AnimationClip> m_Animations;

    private string m_AnimationName;

    private void Awake()
    {
        OnAnimatorChanged();
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Animator Inspector", EditorStyles.boldLabel);

        var animator = EditorHelper.ObjectField(m_Controller);
        if (m_Controller != animator)
        {
            Selection.activeObject = animator;
        }

        foreach (var clip in m_Animations)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorHelper.ObjectField(clip);
                if (Utils.Gui.BtnUtility("X", true) == true)
                {
                    var stateMachine = m_Controller.layers[0].stateMachine;
                    var states = stateMachine.states.ToList();

                    var state = states.Find(s => s.state.motion == clip).state;
                    stateMachine.RemoveState(state);

                    clip.DestroyObjectImmediate(true);
                    m_Controller.GetImporter().SaveAndReimport();
                    OnAnimatorChanged();
                }
            }
        }

        using (new EditorGUILayout.HorizontalScope())
        {
            m_AnimationName = EditorGUILayout.TextField(m_AnimationName);

            using (new GuiEnabledScope(!m_Animations.Exists(a => a.name == m_AnimationName)))
            {
                if (Utils.Gui.BtnUtility("+", true) == true)
                {
                    var clip = AnimatorController.AllocateAnimatorClip(m_AnimationName);
                    var settings = UnityEditor.AnimationUtility.GetAnimationClipSettings(clip);
                    settings.loopTime = false;
                    UnityEditor.AnimationUtility.SetAnimationClipSettings(clip, settings);
                    AssetDatabase.AddObjectToAsset(clip, m_Controller);
                    var state = m_Controller.AddMotion(clip);
                    state.name = m_AnimationName;

                    m_Controller.GetImporter().SaveAndReimport();
                    OnAnimatorChanged();
                }
            }
        }
    }

    private void OnSelectionChange()
    {
        var controller = Selection.activeObject as AnimatorController;
        if (controller != null)
        {
            m_Controller = controller;
            OnAnimatorChanged();
            Repaint();
        }
    }

    private void OnAnimatorChanged()
    {
        if (m_Controller == null)
        {
            m_Animations = new List<AnimationClip>();
        }
        else
        {
            m_Animations = m_Controller.animationClips.ToList();
        }
    }
}
