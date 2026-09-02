using System.Collections.Generic;
using UnityEngine;

public class Location : MonoBehaviour
{
    [Header("Connections")]
    public List<RouteConnection> connections =
        new List<RouteConnection>();


    [Header("Requests")]
    public List<RequestPaper> activeRequests =
        new List<RequestPaper>();


    [Header("Location Type")]
    public LocationData locationType;


    [Header("Visual")]
    public Renderer locationRenderer;


    private Material locationMaterial;


    private void Awake()
    {
        // Automatically find the renderer on this object
        if (locationRenderer == null)
        {
            locationRenderer = GetComponent<Renderer>();
        }


        // Create a material instance so changing this
        // location doesn't change every other location.
        if (locationRenderer != null)
        {
            locationMaterial = locationRenderer.material;
        }
    }


    public void SetLocationType(LocationData type)
    {
        locationType = type;
        if (locationType == null)
        {
            Debug.LogWarning(
                gameObject.name +
                " was given a null LocationData."
            );

            return;
        }


        SetLocationIcon();
    }


    private void SetLocationIcon()
    {
        if (locationRenderer == null)
        {
            Debug.LogError(
                gameObject.name +
                " has no Renderer assigned."
            );

            return;
        }


        // LocationData name must match the PNG name
        string iconName = locationType.Name;


        // Load PNG from:
        // Assets/Resources/Icons/
        Texture2D icon =
            Resources.Load<Texture2D>(
                "Icons/" + iconName
            );


        if (icon == null)
        {
            Debug.LogError(
                "Could not find location icon: " +
                "Resources/Icons/" +
                iconName
            );

            return;
        }


        // Make sure we have a material
        if (locationMaterial == null)
        {
            locationMaterial = locationRenderer.material;
        }


        // Put PNG onto the material
        locationMaterial.mainTexture = icon;


        Debug.Log(
            gameObject.name +
            " assigned icon: " +
            iconName
        );
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
}