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

    public LocationManager locationManager;


    private void Start()
    {
        SpawnLocations();
    }


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

        // Get plane dimensions
        Renderer planeRenderer = spawnPlane.GetComponent<Renderer>();

        if (planeRenderer == null)
        {
            Debug.LogError("Spawn Plane needs a Renderer!");
            return;
        }

        Bounds bounds = planeRenderer.bounds;

        int spawned = 0;
        int attempts = 0;

        // Prevent infinite loops if the plane is too small
        int maxAttempts = locationAmount * 100;

        while (spawned < locationAmount && attempts < maxAttempts)
        {
            attempts++;

            // Random position inside plane
            float x = Random.Range(bounds.min.x, bounds.max.x);
            float z = Random.Range(bounds.min.z, bounds.max.z);

            Vector3 spawnPosition = new Vector3(
                x,
                bounds.center.y,
                z
            );

            // Check distance from existing locations
            bool tooClose = false;

            //Location[] existingLocations = FindObjectsOfType<Location>();

            foreach (Location location in locationManager.locations)
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

            // Spawn location
            GameObject newLocation = Instantiate(
                locationPrefab,
                spawnPosition,
                Quaternion.identity,
                locationParent
            ); 
            locationManager.locations.Add(newLocation.GetComponent<Location>());

            spawned++;
        }

        Debug.Log($"Spawned {spawned} locations.");
        locationManager.CreateConnections();
    }
}