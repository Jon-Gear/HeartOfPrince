using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.VisualScripting;
using UnityEngine;

[Title("Play Particle System")]
[Description("This will play a given particle system.")]
[Image(typeof(IconMagic), ColorTheme.Type.Green)]
[Category("Particles/Play Particle System")]
[Serializable]
public class PlayParticleSystem : Instruction
{
    [SerializeField] private ParticleSystem particleSystem;
    protected override Task Run(Args args)
    {
        particleSystem.Play();
        return DefaultResult;
    }
}
