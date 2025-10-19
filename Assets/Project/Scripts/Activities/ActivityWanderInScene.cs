using GameCreator.Runtime.Common;
using UnityEngine;

public class ActivityWanderInScene : Activity
{
    BoxCollider wanderArea;
    Marker destinationMarker;
    public ActivityWanderInScene(BoxCollider wanderArea)
    {
        this.wanderArea = wanderArea;
    }

    public override float EvaluateScore(CharacterBrain brain)
    {
        return 0.01f;
    }

    protected override void CreateSteps(CharacterBrain brain)
    {
        destinationMarker = CreateRandomMarker();
        steps.Add(new GoToMarkerStep(destinationMarker));
    }

    protected override void Init(CharacterBrain brain)
    {
    }

    protected override void Shutdown(CharacterBrain brain)
    {
        if(destinationMarker != null)
        {
            UnityEngine.Object.Destroy(destinationMarker.gameObject);
        }
    }


    private Marker CreateRandomMarker()
    {
        if (wanderArea == null)
        {
            Debug.LogError("No BoxCollider assigned!");
            return null;
        }

        // --- Random position inside BoxCollider ---
        Vector3 size = wanderArea.size;
        Vector3 center = wanderArea.center;

        Vector3 localRandom = new Vector3(
            Random.Range(-size.x / 2f, size.x / 2f),
            Random.Range(-size.y / 2f, size.y / 2f),
            Random.Range(-size.z / 2f, size.z / 2f)
        );

        Vector3 worldPos = wanderArea.transform.TransformPoint(center + localRandom);

        // Force y = 0 if you want it flat on ground
        worldPos.y = 0f;

        // --- Random rotation around Y axis ---
        Quaternion randomRot = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

        // --- Create Game Creator 2 Marker ---
        GameObject markerObj = new GameObject("RandomMarker");
        Marker marker = markerObj.AddComponent<Marker>();

        marker.transform.position = worldPos;
        marker.transform.rotation = randomRot;

        return marker;
    }
}
