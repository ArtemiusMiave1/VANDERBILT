using UnityEngine;

public class TutorialArrowMarker : MonoBehaviour
{
    [Header("Bobbing")]
    public float bobHeight = 0.25f;
    public float bobSpeed = 2f;

    [Header("Player Tracking")]
    public Transform player;
    public float rotationSpeed = 5f;

    private Vector3 startPosition;

    private void Awake()
    {
        startPosition = transform.localPosition;
    }

    private void Update()
    {
        // Bob up and down
        float bobOffset =
            Mathf.Sin(Time.time * bobSpeed) * bobHeight;

        transform.localPosition =
            startPosition + Vector3.up * bobOffset;

        // Rotate horizontally toward the player
        if (player != null)
        {
            Vector3 direction =
                player.position - transform.position;

            direction.y = 0f;

            if (direction.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation =
                    Quaternion.LookRotation(direction);

                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    rotationSpeed * Time.deltaTime
                );
            }
        }
    }
}
