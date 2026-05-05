using UnityEngine;

public class DailyRotation : MonoBehaviour
{
    [SerializeField] private float yRotationOffset = 0f;

    // Update is called once per frame
    void Update()
    {
        RotateSun();
    }

    private void RotateSun()
    {
        // float _sunAngle = GameManager.Instance.GetSystem<TimeManager>().GetSunAngle();
        // transform.localRotation = Quaternion.Euler(new Vector3(_sunAngle, yRotationOffset, 0f));
    }
}
