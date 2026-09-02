using UnityEngine;

public class RouteLine : MonoBehaviour
{
    public LineRenderer lineRenderer;

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

    public void Setup(Location locationA, Location locationB)
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
            mousePosition.y += heightAbovePlane;

            // Make sure the line has 3 points
            lineRenderer.positionCount = 3;

            Vector3 lastPosition =
                lineRenderer.GetPosition(1);

            lineRenderer.SetPosition(
                2,
                mousePosition
            );
        }
    }
}