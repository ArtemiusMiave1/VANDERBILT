using System.Collections.Generic;
using UnityEngine;

public class Location : MonoBehaviour
{
    public List<Location> connections = new List<Location>();
    public List<float> distances = new List<float>();


    // Draw connections in the Scene view
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        foreach (Location location in connections)
        {
            if (location != null)
            {
                Gizmos.DrawLine(
                    transform.position,
                    location.transform.position
                );
            }
        }
    }
}