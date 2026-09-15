using System.Collections.Generic;
using UnityEngine;

public class RequestLineManager : MonoBehaviour
{
    [Header("Active Requests - Top Line")]
    public List<GameObject> requests =
        new List<GameObject>();

    [Header("Request Limit")]
    public int maxRequests = 3;

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
    public List<GameObject> bottomRequests =
        new List<GameObject>();

    [Header("Movement")]
    public float moveSpeed = 2f;

    [Header("Request Rotation")]
    [Tooltip("Rotation of requests while they are on the request lines.")]
    public Vector3 requestRotation =
        new Vector3(180f, 0f, 0f);

    [Header("Bottom Request Lifetime")]
    [Tooltip("How long a request stays on the bottom line before being destroyed.")]
    public float oldRequestTime = 30f;

    private List<RequestMovement> movingRequests =
        new List<RequestMovement>();


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
                requests.Count +
                "/" +
                maxRequests
            );

            return false;
        }

        if (requests.Contains(request))
            return false;

        requests.Add(request);

        RequestMovement movement =
            request.GetComponent<RequestMovement>();

        if (movement == null)
        {
            movement =
                request.AddComponent<RequestMovement>();
        }

        if (!movingRequests.Contains(movement))
        {
            movingRequests.Add(movement);
        }

        int queuePosition =
            requests.IndexOf(request);

        Vector3 targetPosition =
            GetTopQueuePositionWorld(queuePosition);

        movement.Setup(
            request,
            topSpawnPoint.position,
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
            requests.Count +
            "/" +
            maxRequests
        );

        return true;
    }


    // =========================================================
    // GET TOP QUEUE POSITION
    // =========================================================

    private Vector3 GetTopQueuePositionWorld(
        int queuePosition
    )
    {
        Vector3 direction =
            (
                topEndPoint.position -
                topSpawnPoint.position
            ).normalized;

        return topEndPoint.position -
               direction *
               (
                   queuePosition *
                   topRequestSpacing
               );
    }


    // =========================================================
    // UPDATE TOP LINE
    // =========================================================

    public void UpdateTopLine()
    {
        for (int i = 0; i < requests.Count; i++)
        {
            GameObject request =
                requests[i];

            if (request == null)
                continue;

            RequestMovement movement =
                request.GetComponent<RequestMovement>();

            if (movement == null)
                continue;

            Vector3 targetPosition =
                GetTopQueuePositionWorld(i);

            movement.SetTargetPosition(
                targetPosition
            );

            movement.SetLineRotation(
                requestRotation
            );
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
            movingRequests.Remove(
                movement
            );

            movement.StopMovement();
        }

        if (requests.Contains(request))
        {
            requests.Remove(request);
        }

        UpdateTopLine();

        Debug.Log(
            "Request removed from top line: " +
            request.name +
            " | Active: " +
            requests.Count +
            "/" +
            maxRequests
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

    public void MoveRequestToOldLine(
        GameObject request
    )
    {
        if (request == null)
            return;

        if (bottomRequests.Contains(request))
            return;

        // Remove from top line if it is still there.
        if (requests.Contains(request))
        {
            requests.Remove(request);
            UpdateTopLine();
        }

        bottomRequests.Add(request);

        RequestMovement movement =
            request.GetComponent<RequestMovement>();

        if (movement == null)
        {
            movement =
                request.AddComponent<RequestMovement>();
        }

        if (!movingRequests.Contains(movement))
        {
            movingRequests.Add(movement);
        }

        Vector3 startPosition =
            request.transform.position;

        int queuePosition =
            bottomRequests.IndexOf(request);

        Vector3 targetPosition =
            GetBottomQueuePositionWorld(
                queuePosition
            );

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
            "Request moved to bottom line: " +
            request.name
        );
    }


    // =========================================================
    // GET BOTTOM QUEUE POSITION
    // =========================================================

    private Vector3 GetBottomQueuePositionWorld(
        int queuePosition
    )
    {
        Vector3 direction =
            (
                bottomEndPoint.position -
                bottomStartPoint.position
            ).normalized;

        return bottomStartPoint.position +
               direction *
               (
                   queuePosition *
                   bottomRequestSpacing
               );
    }


    // =========================================================
    // UPDATE BOTTOM LINE
    // =========================================================

    public void UpdateBottomLine()
    {
        for (int i = 0; i < bottomRequests.Count; i++)
        {
            GameObject request =
                bottomRequests[i];

            if (request == null)
                continue;

            RequestMovement movement =
                request.GetComponent<RequestMovement>();

            if (movement == null)
                continue;

            Vector3 targetPosition =
                GetBottomQueuePositionWorld(i);

            movement.SetTargetPosition(
                targetPosition
            );

            movement.SetLineRotation(
                requestRotation
            );
        }
    }


    // =========================================================
    // REMOVE BOTTOM REQUEST
    // =========================================================

    public void RemoveBottomRequest(
        GameObject request
    )
    {
        if (request == null)
            return;

        RequestMovement movement =
            request.GetComponent<RequestMovement>();

        if (movement != null)
        {
            movingRequests.Remove(
                movement
            );
        }

        bottomRequests.Remove(
            request
        );

        UpdateBottomLine();

        Debug.Log(
            "Removed bottom-line request: " +
            request.name
        );
    }


    // =========================================================
    // REMOVE MOVEMENT
    // =========================================================

    public void RemoveMovement(
        RequestMovement movement
    )
    {
        if (movement == null)
            return;

        movingRequests.Remove(
            movement
        );

        UpdateTopLine();
        UpdateBottomLine();
    }


    // =========================================================
    // CLEAR ALL REQUESTS
    // =========================================================

    public void ClearRequests()
    {
        foreach (
            GameObject request
            in requests
        )
        {
            if (request != null)
            {
                Destroy(request);
            }
        }

        foreach (
            GameObject request
            in bottomRequests
        )
        {
            if (request != null)
            {
                Destroy(request);
            }
        }

        requests.Clear();
        bottomRequests.Clear();
        movingRequests.Clear();
    }
}