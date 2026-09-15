using UnityEngine;

public class RequestDropZone : MonoBehaviour
{
    [Header("Request Line")]
    public RequestLineManager requestLineManager;

    private void Start()
    {
        if (requestLineManager == null)
        {
            requestLineManager =
                FindObjectOfType<RequestLineManager>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        RequestPaper request =
            other.GetComponent<RequestPaper>();

        if (request == null)
            return;

        if (requestLineManager == null)
        {
            Debug.LogWarning(
                "RequestDropZone: RequestLineManager not found!"
            );

            return;
        }

        requestLineManager.MoveRequestToOldLine(
            request.gameObject
        );
    }
}