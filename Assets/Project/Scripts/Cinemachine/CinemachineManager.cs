using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CinemachineManager : GameSystem
{
    CinemachineBrain brain;

    public CinemachineCamera longShot;
    public CinemachineCamera closeUpShot;

    [SerializeField] public CinemachineTargetGroup targetGroup;

    public override void Init()
    {
        brain = Camera.main.GetComponent<CinemachineBrain>();
    }

    public override void Shutdown()
    {
    }



    public void SetLongShot()
    {
        longShot.Priority = 1;
        closeUpShot.Priority = 0;
    }

    public void SetCloseUpShot()
    {
        longShot.Priority = 0;
        closeUpShot.Priority = 1;
    }

    public void UpdateCinemachineConfiner(BoxCollider newConfine)
    {
        longShot.GetComponent<CinemachineConfiner3D>().BoundingVolume = newConfine;
        closeUpShot.GetComponent<CinemachineConfiner3D>().BoundingVolume = newConfine;
    }
}
