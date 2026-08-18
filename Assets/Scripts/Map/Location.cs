using System.Collections.Generic;
using UnityEngine;

public class Location : MonoBehaviour
{
    public List<Location> connections = new List<Location>();
    public List<float> distances = new List<float>();

    [Header("Visual")]
    public Renderer locationRenderer;

    public List<RequestPaper> activeRequests;
    public LocationData locationType;

    private void Awake()
    {
        locationRenderer = GetComponent<Renderer>();
    }

    public void Highlight()
    {
        if (locationRenderer != null)
        {
            locationRenderer.material.color = Color.yellow;
        }
    }


    public void ClearHighlight()
    {
        if (locationRenderer != null)
        {
            locationRenderer.material.color = Color.white;
        }
    }


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

//using UnityEngine;

//public class Location : MonoBehaviour
//{
//    public List<Location> connections = new();
//    public List<float> distances = new();

//    SpriteRenderer sprite;

//    private void Awake()
//    {
//        sprite = GetComponent<SpriteRenderer>();
//    }

//    public void Highlight()
//    {
//        sprite.color = Color.yellow;
//    }

//    public void ClearHighlight()
//    {
//        sprite.color = Color.white;
//    }
//}