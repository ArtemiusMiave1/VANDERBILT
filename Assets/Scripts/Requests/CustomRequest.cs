using UnityEngine;

public class CustomRequest : MonoBehaviour
{
    [Header("Request Data")]
    public RequestData requestData = new RequestData();

    [Header("Destination")]
    public Location targetLocation;

    [Header("References")]
    public RequestGenerator requestGenerator;

    [Header("Spawn Settings")]
    public bool spawnOnStart = false;
    public bool allowMultipleSpawns = false;

    private bool hasSpawned = false;

    private void Start()
    {
        if (spawnOnStart)
        {
            SpawnRequest();
        }
    }

    public void SpawnRequest()
    {
        if (hasSpawned && !allowMultipleSpawns)
            return;

        if (requestGenerator == null)
        {
            requestGenerator =
                FindObjectOfType<RequestGenerator>();
        }

        if (requestGenerator == null)
        {
            Debug.LogError("CustomRequest: RequestGenerator not found!");
            return;
        }

        GameObject paper = requestGenerator.SpawnCustomRequest(
            requestData,
            targetLocation
        );

        if (paper != null)
        {
            hasSpawned = true;
        }
    }

    public void ResetRequest()
    {
        hasSpawned = false;
    }
}