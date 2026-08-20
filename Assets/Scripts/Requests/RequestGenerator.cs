using UnityEngine;

public class RequestGenerator : MonoBehaviour
{
    [Header("Request Paper")]
    public RequestPaper requestPaperPrefab;

    [Header("Spawn Settings")]
    public Transform spawnLocation;

    public int requestsToSpawn = 3;


    private void Start()
    {
        GenerateRequests();
    }


    public void GenerateRequests()
    {
        // Check database
        if (GameDatabase.Instance == null)
        {
            Debug.LogError("GameDatabase not found!");
            return;
        }

        if (GameDatabase.Instance.Requests.Count == 0)
        {
            Debug.LogWarning("No requests loaded!");
            return;
        }


        // Check locations
        if (LocationManager.Instance == null)
        {
            Debug.LogError("LocationManager not found!");
            return;
        }

        if (LocationManager.Instance.locations.Count == 0)
        {
            Debug.LogWarning("No locations found!");
            return;
        }


        // Spawn the requested number of papers
        for (int i = 0; i < requestsToSpawn; i++)
        {
            SpawnAvailableRequest();
        }
    }


    private void SpawnAvailableRequest()
    {
        // Pick random request
        int requestIndex = Random.Range(
            0,
            GameDatabase.Instance.Requests.Count
        );

        RequestData request =
            GameDatabase.Instance.Requests[requestIndex];


        // Pick random location
        int locationIndex = Random.Range(
            0,
            LocationManager.Instance.locations.Count
        );

        Location targetLocation =
            LocationManager.Instance.locations[locationIndex];


        // Spawn paper
        RequestPaper paper = Instantiate(
            requestPaperPrefab,
            spawnLocation.position,
            Quaternion.identity,
            spawnLocation
        );


        // Give paper its request
        paper.DisplayRequest(request);

        // Give paper its location
        paper.targetLocation = targetLocation;



        Debug.Log(
            $"Available request: {request.Title} at {targetLocation.name}"
        );
    }
}