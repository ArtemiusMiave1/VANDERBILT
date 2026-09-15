using System.Collections.Generic;
using UnityEngine;

public class CorkBoard : MonoBehaviour
{
    [Header("Request Placement")]
    public Transform requestPlane;

    [Header("Requests")]
    public List<RequestPaper> acceptedRequests =
        new List<RequestPaper>();

    [Header("Board Settings")]
    public int maxRequests = 10;

    [Header("Layout")]
    public int columns = 3;

    public float horizontalSpacing = 2f;
    public float verticalSpacing = 2f;

    [Header("Paper Offset")]
    public float requestOffset = 0.01f;


    // =========================================================
    // PLACE REQUEST
    // =========================================================

    public bool TryPlaceRequest(RequestPaper request)
    {
        if (request == null)
        {
            Debug.LogWarning(
                "CorkBoard: Tried to place a null request."
            );

            return false;
        }

        if (requestPlane == null)
        {
            Debug.LogError(
                "CorkBoard: Request Plane has not been assigned!"
            );

            return false;
        }

        // Remove destroyed/null requests first
        acceptedRequests.RemoveAll(
            r => r == null
        );

        // Check board capacity
        if (acceptedRequests.Count >= maxRequests)
        {
            Debug.Log(
                "CorkBoard is full! Maximum requests: " +
                maxRequests
            );

            return false;
        }

        // Prevent duplicate requests
        if (acceptedRequests.Contains(request))
        {
            Debug.Log(
                "This request is already on the corkboard."
            );

            return false;
        }

        // Add request to board
        acceptedRequests.Add(request);

        // Parent request to the placement plane
        request.transform.SetParent(
            requestPlane
        );

        // Arrange all requests
        ArrangeRequests();

        // Start request timer / activate request
        request.AcceptFromBoard();

        Debug.Log(
            "Request placed on corkboard: " +
            request.GetRequestTitle()
        );

        return true;
    }


    // =========================================================
    // ADD REQUEST
    // =========================================================
    // Compatibility with older RequestPaper methods.

    public void AddRequest(RequestPaper request)
    {
        if (request == null)
            return;

        if (acceptedRequests.Contains(request))
            return;

        if (acceptedRequests.Count >= maxRequests)
        {
            Debug.Log(
                "CorkBoard is full!"
            );

            return;
        }

        acceptedRequests.Add(request);

        request.transform.SetParent(
            requestPlane
        );

        ArrangeRequests();
    }


    // =========================================================
    // REMOVE REQUEST
    // =========================================================

    public void RemoveRequest(RequestPaper request)
    {
        if (request == null)
            return;

        if (!acceptedRequests.Contains(request))
            return;

        acceptedRequests.Remove(request);

        ArrangeRequests();

        Debug.Log(
            "Removed request from corkboard: " +
            request.GetRequestTitle()
        );
    }


    // =========================================================
    // ARRANGE REQUESTS
    // =========================================================

    private void ArrangeRequests()
    {
        if (requestPlane == null)
            return;

        acceptedRequests.RemoveAll(
            r => r == null
        );

        if (columns <= 0)
            columns = 1;


        for (
            int i = 0;
            i < acceptedRequests.Count;
            i++
        )
        {
            RequestPaper request =
                acceptedRequests[i];

            if (request == null)
                continue;


            // =================================================
            // WORK OUT ROW AND COLUMN
            // =================================================

            int column =
                i % columns;

            int row =
                i / columns;


            // =================================================
            // X POSITION
            // =================================================
            // Columns move left/right.

            float x =
                (
                    column -
                    (columns - 1) / 2f
                )
                * horizontalSpacing;


            // =================================================
            // Y POSITION
            // =================================================
            // Rows move up/down.

            float y =
                -row *
                verticalSpacing;


            // =================================================
            // POSITION ON PLANE
            // =================================================
            //
            // X = horizontal
            // Y = vertical
            // Z = depth/offset
            //

            request.transform.localPosition =
                new Vector3(
                    x,
                    y,
                    -requestOffset
                );


            // =================================================
            // ROTATION
            // =================================================
            //
            // Match the orientation of the request plane.
            //

            request.transform.localRotation =
                Quaternion.identity;
        }
    }
}