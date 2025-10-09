using UnityEngine;

public class AutomaticCinemachineConfiner : MonoBehaviour
{
    void Start()
    {
        GameManager.Instance.GetSystem<CinemachineManager>().UpdateCinemachineConfiner(GetComponent<BoxCollider>());
    }
}
