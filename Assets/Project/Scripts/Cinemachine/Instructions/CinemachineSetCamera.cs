using GameCreator.Runtime.Common;
using GameCreator.Runtime.VisualScripting;
using System;
using System.Threading.Tasks;
using Unity.Cinemachine;
using UnityEngine;

[Title("Cinemachine/Set Active Camera")]
[Category("Cinemachine/Activation/Set Active Camera")]
[Description("Makes the specified Cinemachine Camera live by bumping its priority " +
                 "or moving it to the top of the priority subqueue.")]
[Keywords("Cinemachine", "Camera", "Active", "Priority", "Blend", "CM3")]
[Version(1, 0, 0)]
[Serializable]
public class CinemachineSetCamera : Instruction
{
    [SerializeField] private PropertyGetGameObject m_Camera;
    protected override Task Run(Args args)
    {
        var cam = m_Camera.Get(args);
        if (cam == null) return DefaultResult;

        var vcam = cam.GetComponent<CinemachineCamera>();
        if (vcam != null) vcam.Priority = 100; // bring it to the front
        return DefaultResult;
    }
}


[Title("Cinemachine/Remove Active Camera")]
[Category("Cinemachine/Deactivation/Remove Active Camera")]
[Description("Removes the specified Cinemachine Camera by setting its priority to 0, effectively deactivating it.")]
[Keywords("Cinemachine", "Camera", "Deactive", "Priority", "Blend", "CM3")]
[Version(1, 0, 0)]
[Serializable]
public class CinemachineRemoveCamera : Instruction
{
    [SerializeField] private PropertyGetGameObject m_Camera;
    protected override Task Run(Args args)
    {
        var cam = m_Camera.Get(args);
        if (cam == null) return DefaultResult;
        var vcam = cam.GetComponent<CinemachineCamera>();
        if (vcam != null) vcam.Priority = 0; // set it to default priority
        return DefaultResult;
    }
}
