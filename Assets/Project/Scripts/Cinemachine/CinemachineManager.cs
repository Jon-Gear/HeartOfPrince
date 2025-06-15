using Unity.Cinemachine;
using UnityEngine;

public class CinemachineManager : Singleton<CinemachineManager>
{
    CinemachineBrain brain;

    public CinemachineCamera longShot;
    public CinemachineCamera closeUpShot;

    [SerializeField] public CameraTarget cameraTarget;

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

    private void Awake()
    {   
        brain = Camera.main.GetComponent<CinemachineBrain>();

        longShot.Follow = cameraTarget.transform;
        closeUpShot.Follow = cameraTarget.transform;

    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
