using System.Collections.Generic;
using UnityEngine;

public class LocationManager : MonoBehaviour
{
    public static LocationManager Instance { get; private set; }


    [Header("Settings")]
    public float connectionDistance = 20f;


    [Header("Locations")]
    public List<Location> locations =
        new List<Location>();


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }


    public void FindLocations()
    {
        locations.Clear();

        locations.AddRange(
            FindObjectsOfType<Location>()
        );


        Debug.Log(
            $"Found {locations.Count} locations."
        );
    }


    public void CreateConnections()
    {
        if (locations.Count == 0)
        {
            Debug.LogWarning(
                "Cannot create connections. No locations found!"
            );

            return;
        }


        // Clear old connections
        foreach (Location location in locations)
        {
            location.connections.Clear();
            location.distances.Clear();
        }


        // Compare every location
        for (int i = 0; i < locations.Count; i++)
        {
            for (int j = i + 1;
                 j < locations.Count;
                 j++)
            {
                float distance =
                    Vector3.Distance(
                        locations[i].transform.position,
                        locations[j].transform.position
                    );


                if (distance <= connectionDistance)
                {
                    locations[i].connections.Add(
                        locations[j]
                    );

                    locations[i].distances.Add(
                        distance
                    );


                    locations[j].connections.Add(
                        locations[i]
                    );

                    locations[j].distances.Add(
                        distance
                    );
                }
            }
        }


        Debug.Log(
            $"Connections created for {locations.Count} locations."
        );
    }
}