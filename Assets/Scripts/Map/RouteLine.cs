using UnityEngine;

public class RouteLine : MonoBehaviour
{
    public LineRenderer lineRenderer;

    [Header("Route Materials")]
    public Material normalRouteMaterial;
    public Material dangerousRouteMaterial;
    public Material veryDangerousRouteMaterial;

    [Header("Mouse Route")]
    public Camera playerCamera;
    public LayerMask mapPlaneLayer;

    public float heightAbovePlane = 0.2f;

    private void Awake()
    {
        if (lineRenderer == null)
            lineRenderer = GetComponent<LineRenderer>();

        if (playerCamera == null)
            playerCamera = Camera.main;
    }

    private void Update()
    {
        // Only update the temporary line while
        // the player is holding right mouse button
        if (Input.GetMouseButton(1))
        {
            UpdateMousePosition();
        }
    }

    public void Setup(
        Location locationA,
        Location locationB,
        RouteConnection route)
    {
        if (locationA == null || locationB == null)
            return;

        if (lineRenderer == null)
        {
            Debug.LogError(
                "RouteLine has no LineRenderer!"
            );

            return;
        }

        // Set the route material
        SetRouteMaterial(route);

        lineRenderer.positionCount = 2;

        lineRenderer.SetPosition(
            0,
            locationA.transform.position
        );

        lineRenderer.SetPosition(
            1,
            locationB.transform.position
        );
    }

    private void SetRouteMaterial(RouteConnection route)
    {
        if (route == null)
            return;

        Material selectedMaterial = null;

        switch (route.dangerLevel)
        {
            case 0:
                selectedMaterial =
                    normalRouteMaterial;
                break;

            case 1:
                selectedMaterial =
                    dangerousRouteMaterial;
                break;

            default:
                selectedMaterial =
                    veryDangerousRouteMaterial;
                break;
        }

        if (selectedMaterial != null)
        {
            lineRenderer.material =
                selectedMaterial;
        }
    }

    private void UpdateMousePosition()
    {
        if (playerCamera == null)
            return;

        Ray ray =
            playerCamera.ScreenPointToRay(
                Input.mousePosition
            );

        RaycastHit hit;

        if (Physics.Raycast(
            ray,
            out hit,
            1000f,
            mapPlaneLayer
        ))
        {
            Vector3 mousePosition =
                hit.point;

            // Raise the line slightly above the plane
            mousePosition.y +=
                heightAbovePlane;

            // Add a temporary third point
            lineRenderer.positionCount = 3;

            lineRenderer.SetPosition(
                2,
                mousePosition
            );
        }
    }
}