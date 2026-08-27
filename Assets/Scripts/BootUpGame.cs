using UnityEngine;

public class BootUpGame : MonoBehaviour
{
    public RandomLocationSpawner locationSpawner;
    public LocationManager manager;
    public RequestGenerator requestGenerator;


    private void Start()
    {
        if (locationSpawner == null)
        {
            Debug.LogError("LocationSpawner is not assigned!");
            return;
        }

        if (manager == null)
        {
            Debug.LogError("LocationManager is not assigned!");
            return;
        }


        // 1. Spawn locations
        locationSpawner.SpawnLocations();


        // 2. Find the newly spawned locations
        manager.FindLocations();


        // 3. Assign location types
        locationSpawner.AssignLocationTypes();


        // 4. Create connections
        manager.CreateConnections();

        requestGenerator.GenerateRequests();

        Debug.Log("Vanderbilt map generated.");
    }
}