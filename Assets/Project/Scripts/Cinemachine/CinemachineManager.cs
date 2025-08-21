using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CinemachineManager : Singleton<CinemachineManager>
{
    CinemachineBrain brain;

    public CinemachineCamera longShot;
    public CinemachineCamera closeUpShot;

    [SerializeField] public CinemachineTargetGroup targetGroup;
    
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

    protected override void Awake()
    {
        base.Awake();
        brain = Camera.main.GetComponent<CinemachineBrain>();
        //longShot.Follow = cameraTarget.transform;
        //closeUpShot.Follow = cameraTarget.transform;
        
    }
    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
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
