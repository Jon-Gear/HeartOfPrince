using GameCreator.Runtime.Stats;
using UnityEngine;

public class PlayerCharacterBrain : MonoBehaviour
{
    private TraitsOperator traits;
    public TraitsOperator Traits() => traits;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        traits = GetComponent<TraitsOperator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
