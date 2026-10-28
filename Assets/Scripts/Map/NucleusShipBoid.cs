using UnityEngine;

public class NucleusShipBoid : MonoBehaviour
{
    [Header("Orbit Center")]
    public Transform centerPoint;

    [Header("Orbit")]
    public float orbitRadius = 1.5f;
    public float orbitSpeed = 20f;

    [Header("Rotation")]
    public float rotationSpeed = 8f;
    public float modelRotationOffset = 0f;

    [Header("Ship Movement Connection")]
    public ShipMovement shipMovement;

    [Tooltip("How quickly the blimp turns toward the route.")]
    public float travelRotationSpeed = 5f;

    private float orbitAngle;

    // Remember the last known route direction.
    private Vector3 lastRouteDirection;
    private bool hasLastRouteDirection = false;


    // =====================================================
    // START
    // =====================================================

    private void Start()
    {
        // Find ShipMovement if it has not been assigned.
        if (shipMovement == null)
        {
            shipMovement = FindObjectOfType<ShipMovement>();
        }

        if (centerPoint == null)
        {
            Debug.LogWarning(
                "NucleusShipBoid: No Center Point assigned!"
            );

            return;
        }

        // Find starting angle based on the blimp's
        // current position around the centre.
        Vector3 offset =
            transform.position - centerPoint.position;

        offset.y = 0f;

        if (offset.sqrMagnitude > 0.001f)
        {
            orbitAngle =
                Mathf.Atan2(
                    offset.z,
                    offset.x
                ) * Mathf.Rad2Deg;
        }
        else
        {
            orbitAngle = 0f;
        }
    }


    // =====================================================
    // UPDATE
    // =====================================================

    private void Update()
    {
        if (centerPoint == null)
            return;

        if (shipMovement == null)
        {
            OrbitCentre();
            return;
        }

        // Remember the route direction whenever a
        // destination is available.
        if (
            shipMovement.targetLocation != null &&
            shipMovement.currentLocation != null
        )
        {
            Vector3 direction =
                shipMovement.targetLocation.transform.position -
                shipMovement.currentLocation.transform.position;

            direction.y = 0f;

            if (direction.sqrMagnitude > 0.001f)
            {
                lastRouteDirection = direction.normalized;
                hasLastRouteDirection = true;
            }
        }

        bool hasDestination =
            shipMovement.targetLocation != null;

        bool isStopped =
            !shipMovement.IsMoving();

        // Check whether the actual ship has arrived
        // at its current landmark.
        bool isAtLandmark = false;

        if (shipMovement.currentLocation != null)
        {
            isAtLandmark =
                Vector3.Distance(
                    shipMovement.transform.position,
                    shipMovement.currentLocation.transform.position
                ) < 0.1f;
        }

        // Orbit only when stopped at a landmark
        // with no remaining destination.
        if (
            isStopped &&
            isAtLandmark &&
            !hasDestination
        )
        {
            OrbitCentre();
            return;
        }

        // Otherwise, face the route and do not orbit.
        FaceRouteDirection();
    }


    // =====================================================
    // ORBIT
    // =====================================================

    private void OrbitCentre()
    {
        // Advance the orbit angle.
        orbitAngle += orbitSpeed * Time.deltaTime;

        float angle =
            orbitAngle * Mathf.Deg2Rad;

        // Calculate target position around the centre.
        Vector3 targetPosition =
            centerPoint.position +
            new Vector3(
                Mathf.Cos(angle) * orbitRadius,
                transform.position.y - centerPoint.position.y,
                Mathf.Sin(angle) * orbitRadius
            );

        // Smoothly move around the orbit.
        transform.position =
            Vector3.Lerp(
                transform.position,
                targetPosition,
                10f * Time.deltaTime
            );

        // Calculate direction of movement.
        Vector3 movementDirection =
            new Vector3(
                -Mathf.Sin(angle),
                0f,
                Mathf.Cos(angle)
            );

        // Reverse if the orbit runs backwards.
        if (orbitSpeed < 0f)
        {
            movementDirection *= -1f;
        }

        // Face the direction of the orbit.
        if (movementDirection.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(
                    movementDirection.normalized,
                    Vector3.up
                );

            targetRotation *=
                Quaternion.Euler(
                    0f,
                    modelRotationOffset,
                    0f
                );

            transform.rotation =
                Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    rotationSpeed * Time.deltaTime
                );
        }
    }


    // =====================================================
    // FACE ROUTE DIRECTION
    // =====================================================

    private void FaceRouteDirection()
    {
        // Refresh the saved direction when a destination
        // is available.
        if (
            shipMovement != null &&
            shipMovement.currentLocation != null &&
            shipMovement.targetLocation != null
        )
        {
            Vector3 direction =
                shipMovement.targetLocation.transform.position -
                shipMovement.currentLocation.transform.position;

            direction.y = 0f;

            if (direction.sqrMagnitude > 0.001f)
            {
                lastRouteDirection = direction.normalized;
                hasLastRouteDirection = true;
            }
        }

        // Keep the last known direction when paused.
        if (!hasLastRouteDirection)
            return;

        Quaternion targetRotation =
            Quaternion.LookRotation(
                lastRouteDirection,
                Vector3.up
            );

        // Correct for the model's forward-facing axis.
        targetRotation *=
            Quaternion.Euler(
                0f,
                modelRotationOffset,
                0f
            );

        // Smoothly face the route.
        transform.rotation =
            Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                travelRotationSpeed * Time.deltaTime
            );
    }
}
