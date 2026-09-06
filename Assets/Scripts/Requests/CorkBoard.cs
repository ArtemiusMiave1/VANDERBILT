using System.Collections.Generic;
using UnityEngine;

public class CorkBoard : MonoBehaviour
{
    [Header("Accepted Requests")]
    public List<RequestPaper> acceptedRequests =
        new List<RequestPaper>();

    [Header("Board")]
    public Transform requestParent;

    [Header("Paper Layout")]
    public int columns = 3;
    public float horizontalSpacing = 2f;
    public float verticalSpacing = 2f;


    public void AddRequest(RequestPaper request)
    {
        if (request == null)
            return;

        // Don't add the same request twice
        if (acceptedRequests.Contains(request))
            return;

        acceptedRequests.Add(request);


        Debug.Log(
            "Added request to cork board: " +
            request.GetRequestTitle()
        );

        // Move request to cork board
        if (requestParent != null)
        {
            request.transform.SetParent(requestParent);
        }

        ArrangeRequests();
    }


    public void RemoveRequest(RequestPaper request)
    {
        if (request == null)
            return;

        // Remove request from the list
        acceptedRequests.Remove(request);

        // Rearrange the remaining requests
        ArrangeRequests();
    }


    private void ArrangeRequests()
    {
        // Remove destroyed/null requests first
        acceptedRequests.RemoveAll(
            request => request == null
        );


        // Position remaining requests
        for (int i = 0; i < acceptedRequests.Count; i++)
        {
            RequestPaper request = acceptedRequests[i];

            if (request == null)
                continue;


            // Calculate row and column
            int column = i % columns;
            int row = i / columns;


            // Calculate position
            float x = column * horizontalSpacing;
            float y = -row * verticalSpacing;


            request.transform.localPosition =
                new Vector3(x, y, 0f);


            request.transform.localRotation =
                Quaternion.Euler(0f, 45f, 0f);
        }
    }
}