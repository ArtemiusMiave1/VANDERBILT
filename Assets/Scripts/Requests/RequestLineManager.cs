
using System.Collections.Generic;
using UnityEngine;

public class RequestLineManager : MonoBehaviour
{
    [Header("Active Requests - Top Line")]
    public List<GameObject> requests = new List<GameObject>();

    [Header("Request Limit")]
    public int maxRequests = 3;

    [Header("Request Parent")]
    [Tooltip("Parent containing the requests. Should move and rock with the ship.")]
    public Transform requestParent;

    [Header("Top Line")]
    public Transform topSpawnPoint;
    public Transform topEndPoint;

    [Tooltip("Space between requests on the top line.")]
    public float topRequestSpacing = 1.5f;

    [Header("Bottom Line")]
    public Transform bottomStartPoint;
    public Transform bottomEndPoint;

    [Tooltip("Space between requests on the bottom line.")]
    public float bottomRequestSpacing = 1.5f;

    [Header("Bottom Line Requests")]
    public List<GameObject> bottomRequests = new List<GameObject>();

    [Header("Movement")]
    public float moveSpeed = 2f;

    [Header("Request Rotation")]
    public Vector3 requestRotation = new Vector3(180f, 0f, 0f);

    [Header("Bottom Request Lifetime")]
    public float oldRequestTime = 30f;

    private List<RequestMovement> movingRequests =
        new List<RequestMovement>();

    private void Awake()
    {
        if (requestParent == null)
            requestParent = transform;
    }

    // =========================================================
    // LOCAL POSITION HELPERS
    // =========================================================

    private Vector3 GetLocalPosition(Transform point)
    {
        return requestParent.InverseTransformPoint(point.position);
    }

    private Vector3 GetTopQueuePositionLocal(int queuePosition)
    {
        Vector3 start = GetLocalPosition(topSpawnPoint);
        Vector3 end = GetLocalPosition(topEndPoint);

        Vector3 direction = (end - start).normalized;

        return end - direction * (queuePosition * topRequestSpacing);
    }

    private Vector3 GetBottomQueuePositionLocal(int queuePosition)
    {
        Vector3 start = GetLocalPosition(bottomStartPoint);
        Vector3 end = GetLocalPosition(bottomEndPoint);

        Vector3 direction = (end - start).normalized;

        return start + direction * (queuePosition * bottomRequestSpacing);
    }

    // =========================================================
    // ADD NEW REQUEST
    // =========================================================

    public bool AddNewRequest(GameObject request)
    {
        if (request == null)
            return false;

        if (requests.Count >= maxRequests)
        {
            Debug.Log(
                "Request line is full! " +
                requests.Count + "/" + maxRequests
            );

            return false;
        }

        if (requests.Contains(request))
            return false;

        // Keep requests in the ship's moving coordinate space.
        request.transform.SetParent(requestParent, true);

        requests.Add(request);

        RequestMovement movement =
            request.GetComponent<RequestMovement>();

        if (movement == null)
            movement = request.AddComponent<RequestMovement>();

        if (!movingRequests.Contains(movement))
            movingRequests.Add(movement);

        int queuePosition = requests.IndexOf(request);

        Vector3 targetPosition =
            GetTopQueuePositionLocal(queuePosition);

        movement.Setup(
            request,
            GetLocalPosition(topSpawnPoint),
            targetPosition,
            moveSpeed,
            false,
            this,
            requestRotation
        );

        UpdateTopLine();

        Debug.Log(
            "Request added to top line: " +
            request.name +
            " | Active: " +
            requests.Count + "/" + maxRequests
        );

        return true;
    }

    // =========================================================
    // UPDATE TOP LINE
    // =========================================================

    public void UpdateTopLine()
    {
        for (int i = 0; i < requests.Count; i++)
        {
            GameObject request = requests[i];

            if (request == null)
                continue;

            RequestMovement movement =
                request.GetComponent<RequestMovement>();

            if (movement == null)
                continue;

            movement.SetTargetPosition(
                GetTopQueuePositionLocal(i)
            );

            movement.SetLineRotation(requestRotation);
        }
    }

    // =========================================================
    // REMOVE REQUEST FROM TOP LINE
    // =========================================================

    public void RemoveRequest(GameObject request)
    {
        if (request == null)
            return;

        RequestMovement movement =
            request.GetComponent<RequestMovement>();

        if (movement != null)
        {
            movingRequests.Remove(movement);
            movement.StopMovement();
        }

        if (requests.Contains(request))
            requests.Remove(request);

        UpdateTopLine();

        Debug.Log(
            "Request removed from top line: " +
            request.name +
            " | Active: " +
            requests.Count + "/" + maxRequests
        );
    }

    // =========================================================
    // CHECK IF TOP LINE HAS SPACE
    // =========================================================

    public bool HasSpace()
    {
        return requests.Count < maxRequests;
    }

    // =========================================================
    // GET REQUEST COUNT
    // =========================================================

    public int GetRequestCount()
    {
        return requests.Count;
    }

    // =========================================================
    // MOVE REQUEST TO BOTTOM LINE
    // =========================================================

    public void MoveRequestToOldLine(GameObject request)
    {
        if (request == null)
            return;

        if (bottomRequests.Contains(request))
            return;

        if (requests.Contains(request))
        {
            requests.Remove(request);
            UpdateTopLine();
        }

        request.transform.SetParent(requestParent, true);

        bottomRequests.Add(request);

        RequestMovement movement =
            request.GetComponent<RequestMovement>();

        if (movement == null)
            movement = request.AddComponent<RequestMovement>();

        if (!movingRequests.Contains(movement))
            movingRequests.Add(movement);

        Vector3 startPosition = GetLocalPosition(bottomEndPoint);

        int queuePosition = bottomRequests.IndexOf(request);

        Vector3 targetPosition =
            GetBottomQueuePositionLocal(queuePosition);

        movement.Setup(
            request,
            startPosition,
            targetPosition,
            moveSpeed,
            true,
            this,
            requestRotation
        );

        UpdateBottomLine();

        Debug.Log(
            "Request moved to bottom line: " + request.name
        );
    }

    // =========================================================
    // UPDATE BOTTOM LINE
    // =========================================================

    public void UpdateBottomLine()
    {
        for (int i = 0; i < bottomRequests.Count; i++)
        {
            GameObject request = bottomRequests[i];

            if (request == null)
                continue;

            RequestMovement movement =
                request.GetComponent<RequestMovement>();

            if (movement == null)
                continue;

            movement.SetTargetPosition(
                GetBottomQueuePositionLocal(i)
            );

            movement.SetLineRotation(requestRotation);
        }
    }

    // =========================================================
    // REMOVE BOTTOM REQUEST
    // =========================================================

    // =========================================================
    // REMOVE BOTTOM REQUEST
    // =========================================================

    public void RemoveBottomRequest(GameObject request)
    {
        if (request == null)
            return;

        // Only count requests actually on the bottom line.
        if (!bottomRequests.Contains(request))
            return;

        RequestPaper requestPaper =
            request.GetComponent<RequestPaper>();

        // Record discarded request.
        // Do not count requests already completed.
        if (requestPaper != null && !requestPaper.completed)
        {
            if (DailyStatsManager.Instance != null)
            {
                DailyStatsManager.Instance.RecordRequestDiscarded();
            }

            Debug.Log(
                "Request discarded: " +
                requestPaper.GetRequestTitle()
            );
        }

        RequestMovement movement =
            request.GetComponent<RequestMovement>();

        if (movement != null)
            movingRequests.Remove(movement);

        bottomRequests.Remove(request);

        UpdateBottomLine();

        Debug.Log(
            "Removed bottom-line request: " + request.name
        );
    }

    // =========================================================
    // REMOVE MOVEMENT
    // =========================================================

    public void RemoveMovement(RequestMovement movement)
    {
        if (movement == null)
            return;

        movingRequests.Remove(movement);

        UpdateTopLine();
        UpdateBottomLine();
    }

    // =========================================================
    // CLEAR ALL REQUESTS
    // =========================================================

    public void ClearRequests()
    {
        foreach (GameObject request in requests)
        {
            if (request != null)
                Destroy(request);
        }

        foreach (GameObject request in bottomRequests)
        {
            if (request != null)
                Destroy(request);
        }

        requests.Clear();
        bottomRequests.Clear();
        movingRequests.Clear();
    }
}
