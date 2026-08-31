using UnityEngine;

public class RandomLocationSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject locationPrefab;
    public GameObject spawnPlane;

    [Min(1)]
    public int locationAmount = 10;

    [Header("Spacing")]
    public float minimumDistance = 2f;

    [Header("Parent")]
    public Transform locationParent;

    [Header("References")]
    public LocationManager locationManager;
    public GameDatabase gameData;


    public void SpawnLocations()
    {
        if (locationPrefab == null)
        {
            Debug.LogError("Location Prefab has not been assigned!");
            return;
        }

        if (spawnPlane == null)
        {
            Debug.LogError("Spawn Plane has not been assigned!");
            return;
        }


        Renderer planeRenderer =
            spawnPlane.GetComponent<Renderer>();

        if (planeRenderer == null)
        {
            Debug.LogError("Spawn Plane needs a Renderer!");
            return;
        }


        Bounds bounds = planeRenderer.bounds;

        int spawned = 0;
        int attempts = 0;

        int maxAttempts = locationAmount * 100;


        while (spawned < locationAmount &&
               attempts < maxAttempts)
        {
            attempts++;


            // Random position
            float x = Random.Range(
                bounds.min.x,
                bounds.max.x
            );

            float z = Random.Range(
                bounds.min.z,
                bounds.max.z
            );


            Vector3 spawnPosition = new Vector3(
                x,
                bounds.center.y,
                z
            );


            // Check distance from existing locations
            bool tooClose = false;

            Location[] existingLocations =
                FindObjectsOfType<Location>();


            foreach (Location location in existingLocations)
            {
                if (Vector3.Distance(
                    spawnPosition,
                    location.transform.position
                ) < minimumDistance)
                {
                    tooClose = true;
                    break;
                }
            }


            if (tooClose)
                continue;


            // Spawn
            Instantiate(
                locationPrefab,
                spawnPosition,
                Quaternion.identity,
                locationParent
            );


            spawned++;
        }


        //Debug.Log(
        //    $"Spawned {spawned} / {locationAmount} locations."
        //);
    }


    public void AssignLocationTypes()
    {
        if (gameData == null)
        {
            Debug.LogError("GameDatabase reference is missing!");
            return;
        }

        if (gameData.LocationData == null)
        {
            Debug.LogError("LocationData list is NULL!");
            return;
        }

        if (gameData.LocationData.Count == 0)
        {
            Debug.LogError("LocationData contains 0 entries!");
            return;
        }

        if (locationManager == null)
        {
            Debug.LogError("LocationManager reference is missing!");
            return;
        }


        //Debug.Log(
        //    "Assigning location types from " +
        //    gameData.LocationData.Count +
        //    " available types."
        //);


        foreach (Location place in locationManager.locations)
        {
            LocationData randomType =
                gameData.LocationData[
                    Random.Range(
                        0,
                        gameData.LocationData.Count
                    )
                ];

            place.SetLocationType(randomType);
            //Debug.Log(
            //    place.name +
            //    " assigned type: " +
            //    randomType.Name
            //);
        }
    }
}