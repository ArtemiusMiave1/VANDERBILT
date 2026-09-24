using System.Collections.Generic;
using UnityEngine;

public class ShipRouteSystem : MonoBehaviour
{
    [Header("Ship")]
    public ShipMovement shipMovement;

    [Header("Route")]
    public List<Location> route =
        new List<Location>();

    [Header("Route Line")]
    public LineRenderer routeLine;

    [Tooltip("Height of the route line above the map.")]
    public float routeLineHeightOffset = 0.5f;

    [Header("Map")]
    public Transform mapObject;

    [Tooltip("Layer containing the map surface.")]
    public LayerMask mapLayer;

    [Header("Locations")]
    [Tooltip("Layer containing the location colliders.")]
    public LayerMask locationLayer;

    [Header("Camera")]
    public Camera mapCamera;

    private bool addingRoute = false;
    private bool removingRoute = false;

    private bool mouseOnMap = false;

    private Location lastDraggedLocation;

    private Location hoveredLocation;

    private Vector3 currentMapMousePosition;


    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        if (shipMovement == null)
        {
            shipMovement =
                GetComponent<ShipMovement>();

            if (shipMovement == null)
            {
                shipMovement =
                    FindObjectOfType<ShipMovement>();
            }
        }

        if (mapCamera == null)
        {
            mapCamera =
                Camera.main;
        }

        if (routeLine != null)
        {
            routeLine.useWorldSpace = true;
        }

        UpdateRouteLine();
    }


    // =========================================================
    // UPDATE
    // =========================================================

    private void Update()
    {
        HandleMouseInput();
    }


    // =========================================================
    // MOUSE INPUT
    // =========================================================

    private void HandleMouseInput()
    {
        // -----------------------------------------------------
        // LEFT CLICK - ADD TO ROUTE
        // -----------------------------------------------------

        if (Input.GetMouseButtonDown(0))
        {
            if (IsMouseOverMap())
            {
                StartAddingRoute();
            }
        }

        if (
            addingRoute &&
            Input.GetMouseButton(0)
        )
        {
            ContinueAddingRoute();
        }

        if (
            addingRoute &&
            Input.GetMouseButtonUp(0)
        )
        {
            StopAddingRoute();
        }


        // -----------------------------------------------------
        // RIGHT CLICK - REMOVE FROM ROUTE
        // -----------------------------------------------------

        if (Input.GetMouseButtonDown(1))
        {
            if (IsMouseOverMap())
            {
                StartRemovingRoute();
            }
        }

        if (
            removingRoute &&
            Input.GetMouseButton(1)
        )
        {
            ContinueRemovingRoute();
        }

        if (
            removingRoute &&
            Input.GetMouseButtonUp(1)
        )
        {
            StopRemovingRoute();
        }
    }


    // =========================================================
    // ADDING TO ROUTE
    // =========================================================

    private void StartAddingRoute()
    {
        if (shipMovement == null)
            return;

        Location currentLocation =
            shipMovement.GetCurrentLocation();

        if (currentLocation == null)
        {
            Debug.LogWarning(
                "ShipRouteSystem: Ship has no current location."
            );

            return;
        }

        /*
         * If there isn't a route yet,
         * start it at the ship's current location.
         */
        if (route.Count == 0)
        {
            route.Add(
                currentLocation
            );
        }

        /*
         * Continue from the last location
         * already in the route.
         */
        lastDraggedLocation =
            route[route.Count - 1];

        addingRoute = true;

        mouseOnMap =
            TryGetMapMousePosition(
                out currentMapMousePosition
            );

        //Debug.Log(
        //    "Adding to route from " +
        //    lastDraggedLocation.GetDisplayName()
        //);

        UpdateDraggingLine();
    }


    private void ContinueAddingRoute()
    {
        /*
         * Check whether the mouse is still
         * over the map.
         */
        mouseOnMap =
            TryGetMapMousePosition(
                out currentMapMousePosition
            );

        /*
         * If the mouse leaves the map,
         * don't move the temporary line.
         */
        if (!mouseOnMap)
        {
            hoveredLocation = null;

            UpdateDraggingLine();

            return;
        }

        Location hitLocation;

        /*
         * Look specifically for a Location
         * collider.
         */
        if (
            !GetLocationFromMouse(
                out hitLocation
            )
        )
        {
            hoveredLocation = null;

            UpdateDraggingLine();

            return;
        }

        hoveredLocation =
            hitLocation;

        TryAddLocation(
            hitLocation
        );

        UpdateDraggingLine();
    }


    private void TryAddLocation(
        Location location
    )
    {
        if (location == null)
            return;

        if (lastDraggedLocation == null)
            return;

        /*
         * Don't add a location that is already
         * somewhere in the route.
         */
        if (route.Contains(location))
            return;

        if (LocationManager.Instance == null)
        {
            Debug.LogError(
                "ShipRouteSystem: LocationManager not found!"
            );

            return;
        }

        /*
         * Check if the location is directly
         * connected to the last route location.
         */
        RouteConnection connection =
            LocationManager.Instance.GetConnection(
                lastDraggedLocation,
                location
            );

        if (connection == null)
        {
            return;
        }

        /*
         * Don't allow blocked routes.
         */
        if (connection.blocked)
        {
            //Debug.Log(
            //    "Route blocked between " +
            //    lastDraggedLocation.GetDisplayName() +
            //    " and " +
            //    location.GetDisplayName()
            //);

            return;
        }

        /*
         * Add the location.
         */
        route.Add(
            location
        );

        lastDraggedLocation =
            location;

        Debug.Log(
            "Added location to route: " +
            location.GetDisplayName()
        );
    }


    private void StopAddingRoute()
    {
        addingRoute = false;

        hoveredLocation = null;

        lastDraggedLocation = null;

        mouseOnMap = false;

        UpdateRouteLine();

        Debug.Log(
            "Finished adding route. Locations: " +
            route.Count
        );
    }


    // =========================================================
    // REMOVING FROM ROUTE
    // =========================================================

    private void StartRemovingRoute()
    {
        if (route.Count <= 1)
        {
            Debug.Log(
                "Nothing to remove from route."
            );

            return;
        }

        removingRoute = true;

        mouseOnMap =
            TryGetMapMousePosition(
                out currentMapMousePosition
            );

        Debug.Log(
            "Removing locations from route."
        );

        ContinueRemovingRoute();
    }


    private void ContinueRemovingRoute()
    {
        mouseOnMap =
            TryGetMapMousePosition(
                out currentMapMousePosition
            );

        /*
         * If the mouse leaves the map,
         * don't change the route.
         */
        if (!mouseOnMap)
        {
            hoveredLocation = null;

            UpdateRouteLine();

            return;
        }

        Location hitLocation;

        /*
         * Look specifically for a Location
         * collider.
         */
        if (
            !GetLocationFromMouse(
                out hitLocation
            )
        )
        {
            hoveredLocation = null;

            UpdateRouteLine();

            return;
        }

        hoveredLocation =
            hitLocation;

        TryRemoveLocation(
            hitLocation
        );

        UpdateRouteLine();
    }


    private void TryRemoveLocation(
        Location location
    )
    {
        if (location == null)
            return;

        /*
         * Never remove the ship's current
         * location.
         */
        if (route.Count <= 1)
            return;

        int locationIndex =
            route.IndexOf(location);

        /*
         * Location isn't in the route.
         */
        if (locationIndex == -1)
            return;

        /*
         * Never remove the first location.
         */
        if (locationIndex == 0)
            return;

        /*
         * Remove this location and everything
         * after it.
         *
         * Example:
         *
         * A -> B -> C -> D -> E
         *
         * Right-click D:
         *
         * A -> B -> C
         */
        while (
            route.Count >
            locationIndex
        )
        {
            Location removedLocation =
                route[
                    route.Count - 1
                ];

            route.RemoveAt(
                route.Count - 1
            );

            Debug.Log(
                "Removed location from route: " +
                removedLocation.GetDisplayName()
            );
        }
    }


    private void StopRemovingRoute()
    {
        removingRoute = false;

        hoveredLocation = null;

        mouseOnMap = false;

        UpdateRouteLine();

        Debug.Log(
            "Finished removing route. Locations: " +
            route.Count
        );
    }


    // =========================================================
    // LOCATION RAYCAST
    // =========================================================

    private bool GetLocationFromMouse(
        out Location location
    )
    {
        location = null;

        if (mapCamera == null)
            return false;

        Ray ray =
            mapCamera.ScreenPointToRay(
                Input.mousePosition
            );

        /*
         * Only hit objects on the Location layer.
         */
        if (
            Physics.Raycast(
                ray,
                out RaycastHit hit,
                1000f,
                locationLayer
            )
        )
        {
            /*
             * The Location component can be
             * on the collider itself or a parent.
             */
            location =
                hit.collider.GetComponentInParent<Location>();

            if (location != null)
            {
                return true;
            }
        }

        return false;
    }


    // =========================================================
    // MAP RAYCAST
    // =========================================================

    private bool IsMouseOverMap()
    {
        if (mapCamera == null)
            return false;

        Ray ray =
            mapCamera.ScreenPointToRay(
                Input.mousePosition
            );

        /*
         * Only hit objects on the Map layer.
         */
        return Physics.Raycast(
            ray,
            1000f,
            mapLayer
        );
    }


    private bool TryGetMapMousePosition(
        out Vector3 position
    )
    {
        position = Vector3.zero;

        if (mapCamera == null)
            return false;

        Ray ray =
            mapCamera.ScreenPointToRay(
                Input.mousePosition
            );

        /*
         * Only hit the map layer.
         */
        if (
            Physics.Raycast(
                ray,
                out RaycastHit hit,
                1000f,
                mapLayer
            )
        )
        {
            position =
                hit.point;

            return true;
        }

        return false;
    }


    // =========================================================
    // ROUTE LINE
    // =========================================================

    private void UpdateDraggingLine()
    {
        if (routeLine == null)
            return;

        if (route.Count == 0)
        {
            routeLine.positionCount = 0;

            return;
        }

        /*
         * Route locations + temporary mouse point.
         */
        routeLine.positionCount =
            route.Count + 1;

        for (
            int i = 0;
            i < route.Count;
            i++
        )
        {
            Vector3 position =
                route[i].transform.position;

            position.y =
                GetRouteLineHeight();

            routeLine.SetPosition(
                i,
                position
            );
        }

        /*
         * If the mouse is on the map,
         * extend the line toward it.
         *
         * If the mouse is outside the map,
         * keep the temporary point at the
         * final route location.
         */
        if (mouseOnMap)
        {
            Vector3 mousePosition =
                currentMapMousePosition;

            mousePosition.y =
                GetRouteLineHeight();

            routeLine.SetPosition(
                route.Count,
                mousePosition
            );
        }
        else
        {
            Vector3 lastPosition =
                route[
                    route.Count - 1
                ].transform.position;

            lastPosition.y =
                GetRouteLineHeight();

            routeLine.SetPosition(
                route.Count,
                lastPosition
            );
        }
    }


    private void UpdateRouteLine()
    {
        if (routeLine == null)
            return;

        if (route.Count == 0)
        {
            routeLine.positionCount = 0;

            return;
        }

        routeLine.positionCount =
            route.Count;

        for (
            int i = 0;
            i < route.Count;
            i++
        )
        {
            Vector3 position =
                route[i].transform.position;

            position.y =
                GetRouteLineHeight();

            routeLine.SetPosition(
                i,
                position
            );
        }
    }


    private float GetRouteLineHeight()
    {
        if (mapObject != null)
        {
            return
                mapObject.position.y +
                routeLineHeightOffset;
        }

        return
            transform.position.y +
            routeLineHeightOffset;
    }


    // =========================================================
    // DIRECT LOCATION SELECTION
    // =========================================================

    public void SelectLocation(
        Location location
    )
    {
        if (location == null)
            return;

        if (shipMovement == null)
            return;

        if (LocationManager.Instance == null)
            return;

        Location currentLocation =
            shipMovement.GetCurrentLocation();

        if (currentLocation == null)
            return;

        /*
         * If there is no route yet,
         * start at the ship's current location.
         */
        if (route.Count == 0)
        {
            route.Add(
                currentLocation
            );
        }

        /*
         * Continue from the last location
         * in the existing route.
         */
        Location lastLocation =
            route[
                route.Count - 1
            ];

        if (lastLocation == location)
            return;

        /*
         * Don't add locations already
         * in the route.
         */
        if (route.Contains(location))
            return;

        RouteConnection connection =
            LocationManager.Instance.GetConnection(
                lastLocation,
                location
            );

        if (connection == null)
        {
            Debug.Log(
                location.GetDisplayName() +
                " is not connected to " +
                lastLocation.GetDisplayName()
            );

            return;
        }

        if (connection.blocked)
        {
            Debug.Log(
                "Route is blocked."
            );

            return;
        }

        route.Add(
            location
        );

        UpdateRouteLine();

        Debug.Log(
            "Added route: " +
            lastLocation.GetDisplayName() +
            " -> " +
            location.GetDisplayName()
        );
    }


    // =========================================================
    // ROUTE CONTROL
    // =========================================================

    public void ClearRoute()
    {
        route.Clear();

        addingRoute = false;
        removingRoute = false;

        lastDraggedLocation = null;
        hoveredLocation = null;

        mouseOnMap = false;

        if (routeLine != null)
        {
            routeLine.positionCount = 0;
        }

        Debug.Log(
            "Ship route cleared."
        );
    }


    public Location GetNextLocation()
    {
        if (route.Count < 2)
            return null;

        return route[1];
    }


    public void OnArrivedAtLocation(
    Location arrivedLocation
)
    {
        if (arrivedLocation == null)
            return;

        if (route.Count < 2)
            return;

        if (route[1] != arrivedLocation)
        {
            Debug.LogWarning(
                "ShipRouteSystem: Arrived at " +
                arrivedLocation.GetDisplayName() +
                " but expected " +
                route[1].GetDisplayName()
            );

            return;
        }

        route.RemoveAt(0);

        UpdateRouteLine();

        Debug.Log(
            "Route updated after arriving at " +
            arrivedLocation.GetDisplayName() +
            ". Remaining route: " +
            route.Count
        );
    }


    public bool HasRoute()
    {
        return route.Count > 1;
    }


    public int GetRouteLength()
    {
        return route.Count;
    }
}