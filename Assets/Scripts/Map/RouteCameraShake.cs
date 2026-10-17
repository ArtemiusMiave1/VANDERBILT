using UnityEngine;

public class RouteCameraShake : MonoBehaviour
{
    [Header("Shake Settings")]
    [Tooltip("Maximum camera movement caused by shaking.")]
    public float shakeIntensity = 0.035f;

    [Tooltip("How quickly the camera changes shake direction.")]
    public float shakeSpeed = 15f;

    [Tooltip("How quickly the shake fades in.")]
    public float fadeInSpeed = 1.5f;

    [Tooltip("How quickly the shake fades out.")]
    public float fadeOutSpeed = 1f;

    private Vector3 originalLocalPosition;

    private bool shakeActive = false;
    private float currentShakeAmount = 0f;

    private void Awake()
    {
        originalLocalPosition = transform.localPosition;
    }

    private void LateUpdate()
    {
        float targetAmount = shakeActive ? 1f : 0f;

        float fadeSpeed = shakeActive
            ? fadeInSpeed
            : fadeOutSpeed;

        currentShakeAmount = Mathf.MoveTowards(
            currentShakeAmount,
            targetAmount,
            fadeSpeed * Time.deltaTime
        );

        if (currentShakeAmount > 0f)
        {
            float x = Mathf.PerlinNoise(
                Time.time * shakeSpeed, 0f
            ) - 0.5f;

            float y = Mathf.PerlinNoise(
                0f, Time.time * shakeSpeed
            ) - 0.5f;

            float z = Mathf.PerlinNoise(
                Time.time * shakeSpeed,
                Time.time * shakeSpeed
            ) - 0.5f;

            Vector3 offset = new Vector3(x, y, z)
                * shakeIntensity
                * currentShakeAmount;

            transform.localPosition =
                originalLocalPosition + offset;
        }
        else
        {
            transform.localPosition = originalLocalPosition;
        }
    }

    public void SetShakeActive(bool active)
    {
        shakeActive = active;
    }
}