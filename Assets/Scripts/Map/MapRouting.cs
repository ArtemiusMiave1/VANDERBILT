using UnityEngine;

public class MapRouting : MonoBehaviour
{
    [Header("References")]
    public Camera mapCamera;
    public ShipMovement shipMovement;

    [Header("Settings")]
    public float clickDistance = 100f;

    private Location lastHoveredLocation;


    private void Update()
    {
        // Only route while holding right mouse
        if (Input.GetMouseButton(1))
        {
            CheckLocation();
        }
        else
        {
            // Reset when mouse button is released
            lastHoveredLocation = null;
        }
    }


    private void CheckLocation()
    {
        Ray ray = mapCamera.ScreenPointToRay(
            Input.mousePosition
        );

        if (Physics.Raycast(
            ray,
            out RaycastHit hit,
            clickDistance
        ))
        {
            Location location =
                hit.collider.GetComponent<Location>();

            if (location == null)
                return;

            // Don't process the same location repeatedly
            if (location == lastHoveredLocation)
                return;

            lastHoveredLocation = location;

            shipMovement.HandleRouteLocation(location);
        }
        else
        {
            lastHoveredLocation = null;
        }
    }
}