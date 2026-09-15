using System.Collections.Generic;
using UnityEngine;

public class RequestGenerator : MonoBehaviour
{
    [Header("Request Paper")]
    public GameObject requestPaperPrefab;

    [Header("Spawn Settings")]
    public Transform spawnLocation;

    public int requestsToSpawn = 3;
    public float spawnDistance = 2f;

    [Header("Request Line")]
    public RequestLineManager requestLineManager;

    [Header("Automatic Spawning")]
    public bool automaticSpawning = true;
    public float spawnInterval = 45f;

    [Header("Manual Spawn")]
    public bool spawn = false;

    private void Start()
    {
        if (requestLineManager == null)
            requestLineManager = FindObjectOfType<RequestLineManager>();

        GenerateRequests();

        if (automaticSpawning)
        {
            InvokeRepeating(
                nameof(GenerateRequests),
                spawnInterval,
                spawnInterval
            );
        }
    }

    private void Update()
    {
        if (spawn)
        {
            spawn = false;
            GenerateRequests();
        }
    }

    private void OnDestroy()
    {
        CancelInvoke(nameof(GenerateRequests));
    }

    public void GenerateRequests()
    {

        // Spawn the requested number of new requests
        int availableSlots =
            requestLineManager.maxRequests -
            requestLineManager.requests.Count;

        int amountToSpawn =
            Mathf.Min(requestsToSpawn, availableSlots);

        for (int i = 0; i < amountToSpawn; i++)
        {
            Vector3 spawnPoint =
                spawnLocation.position;

            SpawnAvailableRequest(spawnPoint);
        }
    }

    private GameObject SpawnAvailableRequest(Vector3 spawnPoint)
    {
        List<RequestData> validRequests =
            new List<RequestData>();

        // Find requests that have a matching location
        foreach (RequestData request in GameDatabase.Instance.Requests)
        {
            List<Location> matchingLocations =
                FindLocationsForRequest(request);

            if (matchingLocations.Count > 0)
            {
                validRequests.Add(request);
            }
        }

        if (validRequests.Count == 0)
        {
            Debug.LogWarning(
                "RequestGenerator: No requests have a matching LocationType!"
            );

            return null;
        }

        // Pick a random request
        RequestData selectedRequest =
            validRequests[
                Random.Range(
                    0,
                    validRequests.Count
                )
            ];

        // Find locations that can receive this request
        List<Location> matchingLocationsForRequest =
            FindLocationsForRequest(selectedRequest);

        if (matchingLocationsForRequest.Count == 0)
        {
            Debug.LogWarning(
                "Could not find location for request: " +
                selectedRequest.Title
            );

            return null;
        }

        // Pick a random target location
        Location targetLocation =
            matchingLocationsForRequest[
                Random.Range(
                    0,
                    matchingLocationsForRequest.Count
                )
            ];

        // Create the request paper
        GameObject paper =
            Instantiate(
                requestPaperPrefab,
                spawnPoint,
                Quaternion.identity,
                spawnLocation
            );

        RequestPaper requestPaper =
            paper.GetComponent<RequestPaper>();

        if (requestPaper == null)
        {
            Debug.LogError(
                "Request Paper prefab does not have a RequestPaper component!"
            );

            Destroy(paper);
            return null;
        }

        // Give the paper its request data
        requestPaper.DisplayRequest(selectedRequest);

        // Give it a target location
        requestPaper.AssignLocation(targetLocation);

        // Add request to target location
        if (!targetLocation.activeRequests.Contains(requestPaper))
        {
            targetLocation.activeRequests.Add(requestPaper);
        }

        bool added =
            requestLineManager.AddNewRequest(paper);

        if (!added)
        {
            Destroy(paper);
            return null;
        }

        Debug.Log(
            "Generated Request: " +
            selectedRequest.Title +
            " → " +
            targetLocation.name +
            " (" +
            targetLocation.locationType.Name +
            ")"
        );

        return paper;
    }

    private List<Location> FindLocationsForRequest(
        RequestData request)
    {
        List<Location> matchingLocations =
            new List<Location>();

        foreach (Location location
                 in LocationManager.Instance.locations)
        {
            if (location == null)
                continue;

            if (location.locationType == null)
                continue;

            if (location.locationType.LocationType ==
                request.LocationType)
            {
                matchingLocations.Add(location);
            }
        }

        return matchingLocations;
    }
}