using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RequestGenerator : MonoBehaviour
{
    [Header("Request Paper")]
    public GameObject requestPaperPrefab;

    [Header("Spawn Settings")]
    public Transform spawnLocation;
    public int requestsToSpawn = 3;
    public float spawnInterval = 5f;

    [Header("Request Line")]
    public RequestLineManager requestLineManager;

    [Header("Automatic Spawning")]
    public bool automaticSpawning = true;
    public float automaticSpawnInterval = 45f;

    [Header("Manual Spawn")]
    public bool spawn = false;

    private bool currentlySpawning = false;


    // --------------------------------------------------
    // START
    // --------------------------------------------------

    private void Start()
    {
        if (requestLineManager == null)
        {
            requestLineManager =
                FindObjectOfType<RequestLineManager>();
        }

        StartCoroutine(
            SpawnRequestGroup()
        );

        if (automaticSpawning)
        {
            InvokeRepeating(
                nameof(StartAutomaticSpawn),
                automaticSpawnInterval,
                automaticSpawnInterval
            );
        }
    }


    // --------------------------------------------------
    // UPDATE
    // --------------------------------------------------

    private void Update()
    {
        if (spawn)
        {
            spawn = false;

            if (!currentlySpawning)
            {
                StartCoroutine(
                    SpawnRequestGroup()
                );
            }
        }
    }


    // --------------------------------------------------
    // DESTROY
    // --------------------------------------------------

    private void OnDestroy()
    {
        CancelInvoke(
            nameof(StartAutomaticSpawn)
        );
    }


    // --------------------------------------------------
    // AUTOMATIC SPAWN
    // --------------------------------------------------

    private void StartAutomaticSpawn()
    {
        if (!currentlySpawning)
        {
            StartCoroutine(
                SpawnRequestGroup()
            );
        }
    }


    // --------------------------------------------------
    // SPAWN REQUEST GROUP
    // --------------------------------------------------

    private IEnumerator SpawnRequestGroup()
    {
        if (currentlySpawning)
            yield break;

        currentlySpawning = true;

        for (
            int i = 0;
            i < requestsToSpawn;
            i++
        )
        {
            if (requestLineManager == null)
            {
                Debug.LogError(
                    "RequestGenerator: " +
                    "RequestLineManager not found!"
                );

                break;
            }


            // Stop if the top request line is full.
            if (!requestLineManager.HasSpace())
            {
                Debug.Log(
                    "Request line is full. " +
                    "Stopping request generation."
                );

                break;
            }


            if (spawnLocation == null)
            {
                Debug.LogError(
                    "RequestGenerator: " +
                    "Spawn Location has not been assigned!"
                );

                break;
            }


            Vector3 spawnPoint =
                spawnLocation.position;


            SpawnAvailableRequest(
                spawnPoint
            );


            // Wait between individual requests.
            if (
                i <
                requestsToSpawn - 1
            )
            {
                yield return new WaitForSeconds(
                    spawnInterval
                );
            }
        }

        currentlySpawning = false;
    }


    // --------------------------------------------------
    // PUBLIC GENERATE METHOD
    // --------------------------------------------------

    public void GenerateRequests()
    {
        if (!currentlySpawning)
        {
            StartCoroutine(
                SpawnRequestGroup()
            );
        }
    }


    // --------------------------------------------------
    // SPAWN AVAILABLE REQUEST
    // --------------------------------------------------

    private GameObject SpawnAvailableRequest(
        Vector3 spawnPoint
    )
    {
        // ----------------------------------------------
        // GAME DATABASE
        // ----------------------------------------------

        if (GameDatabase.Instance == null)
        {
            Debug.LogError(
                "RequestGenerator: " +
                "GameDatabase not found!"
            );

            return null;
        }


        if (
            GameDatabase.Instance.Requests == null ||
            GameDatabase.Instance.Requests.Count == 0
        )
        {
            Debug.LogWarning(
                "RequestGenerator: " +
                "No requests loaded!"
            );

            return null;
        }


        // ----------------------------------------------
        // LOCATION MANAGER
        // ----------------------------------------------

        if (LocationManager.Instance == null)
        {
            Debug.LogError(
                "RequestGenerator: " +
                "LocationManager not found!"
            );

            return null;
        }


        if (
            LocationManager.Instance.locations == null ||
            LocationManager.Instance.locations.Count == 0
        )
        {
            Debug.LogWarning(
                "RequestGenerator: " +
                "No locations found!"
            );

            return null;
        }


        // ----------------------------------------------
        // SPAWN LOCATION
        // ----------------------------------------------

        if (spawnLocation == null)
        {
            Debug.LogError(
                "RequestGenerator: " +
                "Spawn Location has not been assigned!"
            );

            return null;
        }


        // ----------------------------------------------
        // REQUEST PAPER PREFAB
        // ----------------------------------------------

        if (requestPaperPrefab == null)
        {
            Debug.LogError(
                "RequestGenerator: " +
                "Request Paper Prefab has not been assigned!"
            );

            return null;
        }


        // ----------------------------------------------
        // REQUEST LINE MANAGER
        // ----------------------------------------------

        if (requestLineManager == null)
        {
            Debug.LogError(
                "RequestGenerator: " +
                "Request Line Manager has not been assigned!"
            );

            return null;
        }


        // ----------------------------------------------
        // FIND VALID REQUESTS
        // ----------------------------------------------

        List<RequestData> validRequests =
            new List<RequestData>();


        foreach (
            RequestData request
            in GameDatabase.Instance.Requests
        )
        {
            if (request == null)
                continue;


            List<Location> matchingLocations =
                FindLocationsForRequest(
                    request
                );


            if (
                matchingLocations.Count >
                0
            )
            {
                validRequests.Add(
                    request
                );
            }
        }


        if (validRequests.Count == 0)
        {
            Debug.LogWarning(
                "RequestGenerator: " +
                "No requests have a matching LocationType!"
            );

            return null;
        }


        // ----------------------------------------------
        // SELECT RANDOM REQUEST
        // ----------------------------------------------

        RequestData selectedRequest =
            validRequests[
                Random.Range(
                    0,
                    validRequests.Count
                )
            ];


        // ----------------------------------------------
        // FIND MATCHING LOCATIONS
        // ----------------------------------------------

        List<Location> matchingLocationsForRequest =
            FindLocationsForRequest(
                selectedRequest
            );


        if (
            matchingLocationsForRequest.Count ==
            0
        )
        {
            Debug.LogWarning(
                "Could not find location for request: " +
                selectedRequest.Title
            );

            return null;
        }


        // ----------------------------------------------
        // SELECT RANDOM LOCATION
        // ----------------------------------------------

        Location targetLocation =
            matchingLocationsForRequest[
                Random.Range(
                    0,
                    matchingLocationsForRequest.Count
                )
            ];


        // ----------------------------------------------
        // CREATE REQUEST PAPER
        // ----------------------------------------------

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
                "Request Paper prefab does not have " +
                "a RequestPaper component!"
            );

            Destroy(
                paper
            );

            return null;
        }


        // ----------------------------------------------
        // DISPLAY REQUEST
        // ----------------------------------------------

        requestPaper.DisplayRequest(
            selectedRequest
        );


        // ----------------------------------------------
        // ASSIGN DESTINATION
        // ----------------------------------------------

        requestPaper.AssignLocation(
            targetLocation
        );


        // ----------------------------------------------
        // SET ESTIMATED ARRIVAL
        // ----------------------------------------------

        requestPaper.SetEstimatedArrival();


        // ----------------------------------------------
        // ADD TO LOCATION REQUESTS
        // ----------------------------------------------

        if (
            targetLocation.activeRequests == null
        )
        {
            targetLocation.activeRequests =
                new List<RequestPaper>();
        }


        if (
            !targetLocation.activeRequests.Contains(
                requestPaper
            )
        )
        {
            targetLocation.activeRequests.Add(
                requestPaper
            );
        }


        // ----------------------------------------------
        // ADD TO REQUEST LINE
        // ----------------------------------------------

        bool added =
            requestLineManager.AddNewRequest(
                paper
            );


        if (!added)
        {
            // Remove from the location if the
            // request could not enter the top line.
            if (
                targetLocation.activeRequests.Contains(
                    requestPaper
                )
            )
            {
                targetLocation.activeRequests.Remove(
                    requestPaper
                );
            }


            Destroy(
                paper
            );

            return null;
        }


        // ----------------------------------------------
        // LOCATION NAME
        // ----------------------------------------------

        string locationName =
            targetLocation.locationType != null
                ? targetLocation.locationType.LocationType
                : "Unknown";


        // ----------------------------------------------
        // DEBUG
        // ----------------------------------------------

        Debug.Log(
            "Generated Request: " +
            selectedRequest.Title +
            " → " +
            targetLocation.GetDisplayName() +
            " (" +
            locationName +
            ")"
        );


        return paper;
    }


    // --------------------------------------------------
    // FIND LOCATIONS FOR REQUEST
    // --------------------------------------------------

    private List<Location> FindLocationsForRequest(
        RequestData request
    )
    {
        List<Location> matchingLocations =
            new List<Location>();


        if (request == null)
            return matchingLocations;


        if (LocationManager.Instance == null)
            return matchingLocations;


        if (
            LocationManager.Instance.locations == null
        )
        {
            return matchingLocations;
        }


        foreach (
            Location location
            in LocationManager.Instance.locations
        )
        {
            if (location == null)
                continue;


            if (location.locationType == null)
                continue;


            // Match the RequestData LocationType
            // against the new LocationData LocationType.
            if (
                location.locationType.LocationType ==
                request.LocationType
            )
            {
                matchingLocations.Add(
                    location
                );
            }
        }


        return matchingLocations;
    }
}