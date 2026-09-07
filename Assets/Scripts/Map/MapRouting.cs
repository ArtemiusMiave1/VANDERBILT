using UnityEngine;

public class MapRouting : MonoBehaviour
{
    [Header("References")]
    public Camera mapCamera;
    public ShipMovement shipMovement;

    [Header("Settings")]
    public float clickDistance = 100f;

    [Header("Map")]
    public LayerMask mapPlaneLayer;

    private Location lastHoveredLocation;


    private void Start()
    {
        if (mapCamera == null)
        {
            mapCamera = Camera.main;
        }
    }


    private void Update()
    {
        // =====================================================
        // LEFT CLICK - ADD LOCATIONS + DRAG ROUTE
        // =====================================================

        if (Input.GetMouseButton(0))
        {
            CheckLocation(false);

            // Floating line follows mouse
            UpdateDragLine();
        }


        // =====================================================
        // LEFT CLICK RELEASED
        // REMOVE FLOATING DRAG STRING
        // =====================================================

        if (Input.GetMouseButtonUp(0))
        {
            shipMovement.UpdateRouteLine();
            lastHoveredLocation = null;
        }


        // =====================================================
        // RIGHT CLICK - REMOVE END LOCATION
        // =====================================================

        if (Input.GetMouseButton(1))
        {
            CheckLocation(true);
        }


        // =====================================================
        // RIGHT CLICK RELEASED
        // =====================================================

        if (Input.GetMouseButtonUp(1))
        {
            lastHoveredLocation = null;
        }
    }


    // =========================================================
    // CHECK LOCATION
    // =========================================================

    private void CheckLocation(bool rightClick)
    {
        if (mapCamera == null)
            return;

        if (shipMovement == null)
            return;


        Ray ray =
            mapCamera.ScreenPointToRay(
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


            // Don't repeatedly process the same location
            if (location == lastHoveredLocation)
                return;


            lastHoveredLocation = location;


            // Send location to ShipMovement
            shipMovement.HandleRouteLocation(
                location,
                rightClick
            );
        }
        else
        {
            lastHoveredLocation = null;
        }
    }


    // =========================================================
    // DRAG ROUTE LINE
    // =========================================================

    private void UpdateDragLine()
    {
        if (mapCamera == null)
            return;

        if (shipMovement == null)
            return;

        if (shipMovement.routeLine == null)
            return;

        if (shipMovement.currentLocation == null)
            return;


        // We need a point on the map for the mouse
        Ray ray =
            mapCamera.ScreenPointToRay(
                Input.mousePosition
            );

        if (!Physics.Raycast(
            ray,
            out RaycastHit hit,
            clickDistance,
            mapPlaneLayer
        ))
        {
            return;
        }


        Vector3 mousePosition =
            hit.point;


        // =====================================================
        // CREATE ROUTE LINE
        // =====================================================

        int routeCount =
            shipMovement.route.Count;


        shipMovement.routeLine.positionCount =
            routeCount + 2;


        // =====================================================
        // START POSITION
        // =====================================================

        Vector3 startPosition =
            shipMovement.currentLocation.transform.position;

        startPosition.y +=
            shipMovement.routeLineHeightOffset;


        shipMovement.routeLine.SetPosition(
            0,
            startPosition
        );


        // =====================================================
        // ROUTE LOCATIONS
        // =====================================================

        for (int i = 0; i < routeCount; i++)
        {
            Vector3 position =
                shipMovement.route[i].transform.position;

            position.y +=
                shipMovement.routeLineHeightOffset;


            shipMovement.routeLine.SetPosition(
                i + 1,
                position
            );
        }


        // =====================================================
        // MOUSE END
        // =====================================================

        mousePosition.y +=
            shipMovement.routeLineHeightOffset;


        shipMovement.routeLine.SetPosition(
            routeCount + 1,
            mousePosition
        );
    }
}