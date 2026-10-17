using UnityEngine;

public class TutorialArrowMarker : MonoBehaviour
{
    [Header("Bobbing")]
    public float bobHeight = 0.25f;
    public float bobSpeed = 2f;

    [Header("Player Tracking")]
    public Transform player;
    public float rotationSpeed = 5f;

    [Header("Arrow Orientation")]
    [Tooltip("Fixed downward tilt of the arrow, in degrees.")]
    public float downwardTilt = 90f;

    private Vector3 startPosition;

    private void Awake()
    {
        startPosition = transform.localPosition;
    }

    private void Update()
    {
        // Bob up and down without changing the starting position.
        float bobOffset =
            Mathf.Sin(Time.time * bobSpeed) * bobHeight;

        transform.localPosition =
            startPosition + Vector3.up * bobOffset;

        // Rotate horizontally toward the player.
        if (player != null)
        {
            Vector3 direction =
                player.position - transform.position;

            direction.y = 0f;

            if (direction.sqrMagnitude > 0.001f)
            {
                float targetY =
                    Quaternion.LookRotation(direction).eulerAngles.y;

                Vector3 currentEuler = transform.eulerAngles;

                float newY = Mathf.LerpAngle(
                    currentEuler.y,
                    targetY,
                    rotationSpeed * Time.deltaTime
                );

                // Keep the downward tilt fixed.
                transform.rotation = Quaternion.Euler(
                    downwardTilt,
                    newY,
                    0f
                );
            }
        }
    }
}
