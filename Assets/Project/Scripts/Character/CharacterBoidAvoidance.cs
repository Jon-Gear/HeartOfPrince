using GameCreator.Runtime.Characters;
using UnityEngine;

public class CharacterBoidAvoidance : MonoBehaviour
{
    public float separationRadius = 2f;   // Minimum distance to other NPCs
    public float separationStrength = 2f; // How strongly they push away
    public LayerMask npcLayer;            // Layer that NPCs are on

    private Character character;

    void Start()
    {
        character = GetComponent<Character>();
    }

    void Update()
    {
        Vector3 separation = Vector3.zero;

        // Detect nearby NPCs
        Collider[] hits = Physics.OverlapSphere(transform.position, separationRadius, npcLayer);
        foreach (var hit in hits)
        {
            if (hit.gameObject == gameObject) continue;

            Vector3 away = transform.position - hit.transform.position;
            float distance = away.magnitude;
            if (distance > 0)
            {
                separation += away.normalized * (separationRadius - distance);
            }
        }

        // Apply separation movement
        if (separation != Vector3.zero)
        {
            Vector3 move = separation * separationStrength * Time.deltaTime;

            // Move using Character Controller
            // character..SetVelocity(character.CharacterLocomotion.GetVelocity() + move);
        }
    }
}
