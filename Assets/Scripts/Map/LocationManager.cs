using UnityEngine;

public class LocationManager : MonoBehaviour
{
    public float connectionDistance = 20f;

    private void Start()
    {
        CreateConnections();
    }

    void CreateConnections()
    {
        Location[] locations = FindObjectsOfType<Location>();

        // Clear old connections
        foreach (Location location in locations)
        {
            location.connections.Clear();
            location.distances.Clear();
        }

        // Compare every location with every other location
        for (int i = 0; i < locations.Length; i++)
        {
            for (int j = i + 1; j < locations.Length; j++)
            {
                float distance = Vector3.Distance(
                    locations[i].transform.position,
                    locations[j].transform.position);

                if (distance <= connectionDistance)
                {
                    locations[i].connections.Add(locations[j]);
                    locations[i].distances.Add(distance);

                    locations[j].connections.Add(locations[i]);
                    locations[j].distances.Add(distance);
                }
            }
        }

        Debug.Log("Connections created.");
    }
}