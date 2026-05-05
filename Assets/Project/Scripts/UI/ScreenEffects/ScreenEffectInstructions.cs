// using GameCreator.Runtime.Common;
// using GameCreator.Runtime.VisualScripting;
// using System;
// using System.Threading.Tasks;
// using UnityEngine;
//
//
// [Title("Fade Out")]
// [Category("Screen Effects/Fade Out")]
// [Serializable]
// public class FadeOut : Instruction
// {
//     [SerializeField] private float m_Duration = 1.0f;
//     protected override async Task Run(Args args)
//     {
//         await GameManager.Instance.GetSystem<ScreenEffectsManager>().BasicFadeOut(m_Duration);
//     }
// }
//
//
// [Title("Fade In")]
// [Category("Screen Effects/Fade In")]
// [Serializable]
// public class FadeIn : Instruction
// {
//     [SerializeField] private float m_Duration = 1.0f;
//     protected override async Task Run(Args args)
//     {
//         await GameManager.Instance.GetSystem<ScreenEffectsManager>().BasicFadeIn(m_Duration);
//     }
// }
//
//
// [Title("Fade")]
// [Category("Screen Effects/Fade")]
// [Serializable]
// public class Fade : Instruction
// {
//     [SerializeField] private float m_Duration = 1.0f;
//     
//     protected override async Task Run(Args args)
//     {
//         await GameManager.Instance.GetSystem<ScreenEffectsManager>().BasicFadeOut(m_Duration);
//         await GameManager.Instance.GetSystem<ScreenEffectsManager>().BasicFadeIn(m_Duration);
//     }
// }