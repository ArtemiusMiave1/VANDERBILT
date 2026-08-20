using System.Collections.Generic;
using UnityEngine;

public class LocationManager : MonoBehaviour
{
    public static LocationManager Instance { get; private set; }

    public RandomLocationSpawner spawner;

    [Header("Settings")]
    public float connectionDistance = 20f;

    [Header("Locations")]
    public List<Location> locations = new List<Location>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        //spawner.SpawnLocations();
        //FindLocations();
        //Debug.Log("test");

        //CreateConnections();
        //Debug.Log("asdf" + locations.Count);

    }
    //private void 

    public void FindLocations()
    {
        locations.Clear();
        locations.AddRange(FindObjectsOfType<Location>());
    }

    public void CreateConnections()
    {
        // Clear old connections
        foreach (Location location in locations)
        {
            location.connections.Clear();
            location.distances.Clear();
        }

        // Compare every location with every other location
        for (int i = 0; i < locations.Count; i++)
        {
            for (int j = i + 1; j < locations.Count; j++)
            {
                float distance = Vector3.Distance(
                    locations[i].transform.position,
                    locations[j].transform.position);
                //print(distance);

                if (distance <= connectionDistance)
                {
                    locations[i].connections.Add(locations[j]);
                    locations[i].distances.Add(distance);

                    locations[j].connections.Add(locations[i]);
                    locations[j].distances.Add(distance);
                }
            }
        }

        Debug.Log($"Connections created for {locations.Count} locations.");
    }
}