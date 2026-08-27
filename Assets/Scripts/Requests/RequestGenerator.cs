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

    [Header("Automatic Spawning")]
    public bool automaticSpawning = true;

    public float spawnInterval = 45f;

    [Header("Manual Spawn")]
    public bool spawn = false;

    [Header("Active Requests")]
    public List<GameObject> requests = new List<GameObject>();


    private void Start()
    {
        // Spawn the initial requests
        GenerateRequests();

        // Start automatic spawning
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
        // Manual testing spawn
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


        // Make sure our request list is large enough
        while (requests.Count < requestsToSpawn)
        {
            requests.Add(null);
        }


        // Spawn requests into empty slots
        for (int i = 0; i < requestsToSpawn; i++)
        {
            if (requests[i] != null)
                continue;


            Vector3 spawnPoint = spawnLocation.position;

            spawnPoint.z += i * spawnDistance;


            GameObject newRequest =
                SpawnAvailableRequest(spawnPoint);


            if (newRequest != null)
            {
                requests[i] = newRequest;
            }
        }
    }


    private GameObject SpawnAvailableRequest(Vector3 spawnPoint)
    {
        // Find requests that have at least one matching location
        List<RequestData> validRequests =
            new List<RequestData>();


        foreach (RequestData request in GameDatabase.Instance.Requests)
        {
            List<Location> matchingLocations =
                FindLocationsForRequest(request);


            if (matchingLocations.Count > 0)
            {
                validRequests.Add(request);
            }
        }


        // No valid requests
        if (validRequests.Count == 0)
        {
            Debug.LogWarning(
                "No requests have a matching LocationType!"
            );

            return null;
        }


        // Pick random request
        RequestData selectedRequest =
            validRequests[
                Random.Range(0, validRequests.Count)
            ];


        // Find ALL locations matching this request
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


        // Pick a RANDOM matching location
        Location targetLocation =
            matchingLocationsForRequest[
                Random.Range(
                    0,
                    matchingLocationsForRequest.Count
                )
            ];


        // Spawn paper
        GameObject paper = Instantiate(
            requestPaperPrefab,
            spawnPoint,
            Quaternion.identity,
            spawnLocation
        );


        // Get RequestPaper
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


        // Give paper the request
        requestPaper.DisplayRequest(selectedRequest);


        // Give paper its location
        requestPaper.AssignLocation(targetLocation);


        // Add request to location
        targetLocation.activeRequests.Add(requestPaper);


        Debug.Log(
            $"Available request: {selectedRequest.Title} " +
            $"→ {targetLocation.name} " +
            $"({targetLocation.locationType.Name})"
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


    public void ClearRequests()
    {
        foreach (GameObject request in requests)
        {
            if (request != null)
            {
                Destroy(request);
            }
        }


        requests.Clear();
    }


    public void removeRequest(RequestPaper requestPaper)
    {
        if (requestPaper != null)
        {
            requests.Remove(requestPaper.gameObject);
        }
    }
}