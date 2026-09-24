using UnityEngine;

public class LocationClick : MonoBehaviour
{
    private Location location;

    private void Awake()
    {
        location = GetComponent<Location>();

        if (location == null)
        {
            Debug.LogError(
                gameObject.name +
                " does not have a Location component!"
            );
        }
    }

    private void OnMouseDown()
    {
        if (location == null)
            return;

        Debug.Log(
            "Clicked location: " +
            location.GetDisplayName()
        );

        // Tell the ShipRouteSystem that this location was clicked
        ShipRouteSystem routeSystem =
            FindObjectOfType<ShipRouteSystem>();

        if (routeSystem != null)
        {
            routeSystem.SelectLocation(location);
        }
        else
        {
            Debug.LogWarning(
                "LocationClick: ShipRouteSystem not found!"
            );
        }
    }
}