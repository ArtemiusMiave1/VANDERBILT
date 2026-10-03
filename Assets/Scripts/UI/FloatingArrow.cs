using UnityEngine;

public class FloatingArrow : MonoBehaviour
{
    public float height = 0.2f;
    public float speed = 2f;

    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.localPosition;
    }

    private void Update()
    {
        float newY = startPosition.y +
                     Mathf.Sin(Time.time * speed) * height;

        transform.localPosition = new Vector3(
            startPosition.x,
            newY,
            startPosition.z
        );
    }
}
