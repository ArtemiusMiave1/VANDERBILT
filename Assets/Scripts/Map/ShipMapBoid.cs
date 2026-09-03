using UnityEngine;

public class ShipMapBoid : MonoBehaviour
{
    [Header("Orbit")]
    public float orbitRadius = 2f;
    public float orbitSpeed = 20f;

    [Header("Moving Centre")]
    public float centreMoveSpeed = 0.5f;
    public float centreMoveRadius = 3f;

    [Header("Movement")]
    public float movementSmoothness = 3f;

    [Header("Height")]
    public float heightAboveMap = 0.5f;

    private Vector3 centrePoint;
    private Vector3 targetCentrePoint;

    private float orbitAngle;


    private void Start()
    {
        // Start the centre point at the ship's position
        centrePoint = transform.position;

        // Start at a random point around the orbit
        orbitAngle = Random.Range(0f, 360f);

        ChooseNewCentrePoint();
    }


    private void Update()
    {
        MoveCentrePoint();
        MoveShipAroundCentre();
    }


    // =========================================================
    // MOVING CENTRE POINT
    // =========================================================

    private void MoveCentrePoint()
    {
        centrePoint = Vector3.MoveTowards(
            centrePoint,
            targetCentrePoint,
            centreMoveSpeed * Time.deltaTime
        );


        // Once we reach the target,
        // choose another random point
        if (Vector3.Distance(
            centrePoint,
            targetCentrePoint
        ) < 0.1f)
        {
            ChooseNewCentrePoint();
        }
    }


    private void ChooseNewCentrePoint()
    {
        Vector2 randomOffset =
            Random.insideUnitCircle *
            centreMoveRadius;


        targetCentrePoint =
            new Vector3(
                centrePoint.x + randomOffset.x,
                transform.position.y,
                centrePoint.z + randomOffset.y
            );
    }


    // =========================================================
    // SHIP ORBIT
    // =========================================================

    private void MoveShipAroundCentre()
    {
        // Increase orbit angle
        orbitAngle +=
            orbitSpeed * Time.deltaTime;


        if (orbitAngle >= 360f)
            orbitAngle -= 360f;


        float radians =
            orbitAngle * Mathf.Deg2Rad;


        // Calculate position around centre
        Vector3 orbitPosition =
            centrePoint +
            new Vector3(
                Mathf.Cos(radians) * orbitRadius,
                heightAboveMap,
                Mathf.Sin(radians) * orbitRadius
            );


        // Smoothly move ship
        transform.position =
            Vector3.Lerp(
                transform.position,
                orbitPosition,
                movementSmoothness * Time.deltaTime
            );


        // Rotate ship in direction of movement
        Vector3 direction =
            orbitPosition - transform.position;


        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(
                    direction
                );


            transform.rotation =
                Quaternion.Lerp(
                    transform.rotation,
                    targetRotation,
                    movementSmoothness *
                    Time.deltaTime
                );
        }
    }
}