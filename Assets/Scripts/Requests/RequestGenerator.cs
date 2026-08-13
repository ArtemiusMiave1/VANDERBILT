using UnityEngine;

public class RequestGenerator : MonoBehaviour
{
    public bool buttonPressed = false;

    [Header("Request Paper")]
    public RequestPaper requestPaperPrefab;
    public Transform spawnLocation;


    private void Update()
    {
        if (buttonPressed)
        {
            buttonPressed = false;
            GenerateRequest();
        }
    }


    public void GenerateRequest()
    {
        // Check requests exist
        if (GameDatabase.Instance.Requests.Count == 0)
        {
            Debug.LogWarning("No requests loaded!");
            return;
        }


        // Check locations exist
        if (LocationManager.Instance.locations.Count == 0)
        {
            Debug.LogWarning("No locations found!");
            return;
        }


        // Pick random request
        int requestIndex = Random.Range(
            0,
            GameDatabase.Instance.Requests.Count
        );

        RequestData request = GameDatabase.Instance.Requests[requestIndex];


        // Pick random location
        int locationIndex = Random.Range(
            0,
            LocationManager.Instance.locations.Count
        );

        Location targetLocation = LocationManager.Instance.locations[locationIndex];
        targetLocation.activeRequests.Add(requestPaperPrefab);


        // Spawn request paper
        RequestPaper paper = Instantiate(
            requestPaperPrefab,
            spawnLocation.position,
            Quaternion.identity
        );


        // Give paper request information
        paper.DisplayRequest(request);
        paper.targetLocation = targetLocation;
        targetLocation.Highlight();


        Debug.Log(
            $"Generated {request.Title} at {targetLocation.name}"
        );
    }
}