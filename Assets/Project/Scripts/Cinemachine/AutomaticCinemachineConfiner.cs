using UnityEngine;

public class AutomaticCinemachineConfiner : MonoBehaviour
{
    void Start()
    {
        CinemachineManager.Instance.UpdateCinemachineConfiner(GetComponent<BoxCollider>());
    }
}
