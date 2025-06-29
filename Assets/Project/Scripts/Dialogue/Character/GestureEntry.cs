using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using System;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class GestureEntry
{
    [SerializeField] public string m_Name = string.Empty;
    [Space]
    [SerializeField] private AnimationClip m_AnimationClip;
    [SerializeField] private AvatarMask m_AvatarMask = null;
    [SerializeField] private BlendMode m_BlendMode = BlendMode.Blend;

    [Space]
    [SerializeField] private float m_Delay = 0.0f;
    [SerializeField] private float m_Speed = 1.0f;
    [SerializeField] private bool m_UseRootMotion = false;
    [SerializeField] private float m_TransitionIn = 0.1f;
    [SerializeField] private float m_TransitionOut = 0.1f;

    public void PlayGesture(Character character, Args args)
    {
        if (character == null) return;

        if (m_AnimationClip == null) return;

        ConfigGesture configuration = new ConfigGesture(
            (float)this.m_Delay, m_AnimationClip.length,
            (float)this.m_Speed, this.m_UseRootMotion,
            (float)this.m_TransitionIn,
            (float)this.m_TransitionOut
        );

        Task gestureTask = character.Gestures.CrossFade(
            m_AnimationClip, this.m_AvatarMask, this.m_BlendMode,
            configuration, false
        );
    }
}