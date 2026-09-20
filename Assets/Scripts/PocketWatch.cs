using UnityEngine;

public class PocketWatch : MonoBehaviour
{
    [Header("Game Clock")]
    public GameClock gameClock;

    [Header("Clock Hands")]
    public Transform hourHand;
    public Transform minuteHand;

    [Header("Hand Rotation")]
    [Tooltip("Axis the hands rotate around.")]
    public Vector3 rotationAxis = Vector3.forward;

    [Tooltip("Rotation offset for the hour hand.")]
    public float hourHandOffset = 0f;

    [Tooltip("Rotation offset for the minute hand.")]
    public float minuteHandOffset = 0f;

    [Header("Direction")]
    [Tooltip("Flip this if the hands rotate the wrong way.")]
    public bool clockwise = true;

    private void Start()
    {
        if (gameClock == null)
        {
            gameClock = FindObjectOfType<GameClock>();
        }

        UpdateWatch();
    }

    private void Update()
    {
        if (gameClock == null)
            return;

        UpdateWatch();
    }

    private void UpdateWatch()
    {
        int hour = gameClock.GetHour();
        int minute = gameClock.GetMinute();

        // -----------------------------------------
        // MINUTE HAND
        // -----------------------------------------

        // 60 minutes = 360 degrees
        float minuteAngle =
            (minute / 60f) * 360f;

        // -----------------------------------------
        // HOUR HAND
        // -----------------------------------------

        // The hour hand moves gradually between hours.
        float hourAngle =
            ((hour % 12) / 12f) * 360f;

        hourAngle +=
            (minute / 60f) * 30f;

        // Flip direction if necessary.
        if (!clockwise)
        {
            minuteAngle = -minuteAngle;
            hourAngle = -hourAngle;
        }

        // Apply rotation.
        if (minuteHand != null)
        {
            minuteHand.localRotation =
                Quaternion.AngleAxis(
                    minuteAngle + minuteHandOffset,
                    rotationAxis
                );
        }

        if (hourHand != null)
        {
            hourHand.localRotation =
                Quaternion.AngleAxis(
                    hourAngle + hourHandOffset,
                    rotationAxis
                );
        }
    }
}