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

    private float orbitAngle;


    private void Start()
    {
        if (centerPoint == null)
        {
            Debug.LogWarning(
                "NucleusShipBoid: No Center Point assigned!"
            );

            return;
        }

        // Find starting angle based on the ship's
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


    private void Update()
    {
        if (centerPoint == null)
            return;


        // =====================================================
        // UPDATE ORBIT
        // =====================================================

        orbitAngle +=
            orbitSpeed * Time.deltaTime;


        // =====================================================
        // CALCULATE ORBIT POSITION
        // =====================================================

        float angle =
            orbitAngle * Mathf.Deg2Rad;


        Vector3 targetPosition =
            centerPoint.position +
            new Vector3(
                Mathf.Cos(angle) * orbitRadius,
                transform.position.y - centerPoint.position.y,
                Mathf.Sin(angle) * orbitRadius
            );


        // =====================================================
        // MOVE
        // =====================================================

        transform.position =
            Vector3.Lerp(
                transform.position,
                targetPosition,
                10f * Time.deltaTime
            );


        // =====================================================
        // CALCULATE ACTUAL MOVEMENT DIRECTION
        // =====================================================

        Vector3 movementDirection =
            new Vector3(
                -Mathf.Sin(angle),
                0f,
                Mathf.Cos(angle)
            );


        // Reverse if orbit is going backwards
        if (orbitSpeed < 0f)
        {
            movementDirection *= -1f;
        }


        // =====================================================
        // FACE MOVEMENT DIRECTION
        // =====================================================

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
}