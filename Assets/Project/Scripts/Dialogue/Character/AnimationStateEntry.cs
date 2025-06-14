using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using System;
using UnityEngine;

[Serializable]
public class AnimationStateEntry
{
    [SerializeField] public string m_Name = string.Empty;
    [Space]
    [SerializeField] private StateData m_State = new StateData(StateData.StateType.State);
    [SerializeField] private int m_Layer = 1;
    [SerializeField] private BlendMode m_BlendMode = BlendMode.Blend;

    [Space]
    [SerializeField]
    private float m_Delay = 0f;

    [SerializeField]
    private float m_Speed = 1f;

    [SerializeField]
    [Range(0f, 1f)]
    private float m_Weight = 1f;

    [SerializeField]
    private float m_Transition = 0.1f;
    public void EnterAnimationState(Character character, Args args)
    {
        if (character == null) return;

        if (!this.m_State.IsValid(character)) return;

        ConfigState configuration = new ConfigState(
            this.m_Delay, this.m_Speed, this.m_Weight,
            this.m_Transition, 0f
        );

        int layer = (int)this.m_Layer;

        _ = character.States.SetState(
            this.m_State, layer,
            this.m_BlendMode, configuration
        );
    }

    public void ExitAnimationState(Character character, Args args)
    {
        if (character == null) return;
        int layer = (int)this.m_Layer;
        character.States.Stop(layer, this.m_Delay, this.m_Transition);
    }
}