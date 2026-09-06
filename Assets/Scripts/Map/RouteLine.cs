using UnityEngine;

public class RouteLine : MonoBehaviour
{
    [Header("Line Renderer")]
    public LineRenderer lineRenderer;

    [Header("Map")]
    public Transform mapObject;

    [Tooltip("How far above the map the route should sit.")]
    public float heightOffset = 0.01f;

    private void Awake()
    {
        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();
        }

        // Force world-space coordinates
        if (lineRenderer != null)
        {
            lineRenderer.useWorldSpace = true;
        }
    }

    public void Setup(
        Location locationA,
        Location locationB)
    {
        if (locationA == null || locationB == null)
        {
            Debug.LogError("RouteLine: Missing location!");
            return;
        }

        if (lineRenderer == null)
        {
            Debug.LogError("RouteLine: Missing LineRenderer!");
            return;
        }

        // Get actual WORLD positions
        Vector3 positionA =
            locationA.transform.position;

        Vector3 positionB =
            locationB.transform.position;

        // Put the line at the map's height
        if (mapObject != null)
        {
            positionA.y =
                mapObject.position.y + heightOffset;

            positionB.y =
                mapObject.position.y + heightOffset;
        }

        lineRenderer.positionCount = 2;

        lineRenderer.SetPosition(
            0,
            positionA
        );

        lineRenderer.SetPosition(
            1,
            positionB
        );
    }
}