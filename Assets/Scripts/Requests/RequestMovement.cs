using UnityEngine;

public class RequestMovement : MonoBehaviour
{
    private GameObject request;

    private Vector3 startPosition;
    private Vector3 endPosition;

    private float moveSpeed;

    private bool movingToOldLine;
    private bool onTopLine;

    private float oldRequestTimer;

    private RequestLineManager manager;

    private Quaternion targetRotation;

    private bool moving = false;


    // =========================================================
    // SETUP
    // =========================================================

    public void Setup(
        GameObject requestObject,
        Vector3 start,
        Vector3 end,
        float speed,
        bool oldLine,
        RequestLineManager lineManager,
        Vector3 lineRotation
    )
    {
        request = requestObject;

        startPosition = start;
        endPosition = end;

        moveSpeed = speed;

        movingToOldLine = oldLine;
        onTopLine = !oldLine;

        manager = lineManager;

        oldRequestTimer = 0f;

        // Use the rotation supplied by RequestLineManager.
        targetRotation =
            Quaternion.Euler(
                lineRotation
            );

        transform.position =
            startPosition;

        transform.rotation =
            targetRotation;

        moving = true;
    }


    // =========================================================
    // UPDATE
    // =========================================================

    private void Update()
    {
        if (request == null)
            return;

        if (moving)
        {
            MoveRequest();
        }

        // =====================================================
        // BOTTOM LINE TIMER
        // =====================================================

        if (movingToOldLine)
        {
            oldRequestTimer +=
                Time.deltaTime;

            if (
                manager != null &&
                oldRequestTimer >=
                manager.oldRequestTime
            )
            {
                RemoveRequest();
            }
        }
    }


    // =========================================================
    // MOVE REQUEST
    // =========================================================

    private void MoveRequest()
    {
        transform.position =
            Vector3.MoveTowards(
                transform.position,
                endPosition,
                moveSpeed *
                Time.deltaTime
            );

        // Keep the selected rotation.
        transform.rotation =
            targetRotation;

        if (
            Vector3.Distance(
                transform.position,
                endPosition
            ) < 0.01f
        )
        {
            transform.position =
                endPosition;

            transform.rotation =
                targetRotation;

            moving = false;
        }
    }


    // =========================================================
    // SET TARGET POSITION
    // =========================================================

    public void SetTargetPosition(
        Vector3 newTarget
    )
    {
        endPosition =
            newTarget;

        moving = true;
    }


    // =========================================================
    // SET ROTATION
    // =========================================================

    public void SetLineRotation(
        Vector3 newRotation
    )
    {
        targetRotation =
            Quaternion.Euler(
                newRotation
            );

        transform.rotation =
            targetRotation;
    }


    // =========================================================
    // STOP MOVEMENT
    // =========================================================

    public void StopMovement()
    {
        moving = false;
    }


    // =========================================================
    // CHECK TOP LINE
    // =========================================================

    public bool IsOnTopLine()
    {
        return onTopLine &&
               !movingToOldLine;
    }


    // =========================================================
    // CHECK BOTTOM LINE
    // =========================================================

    public bool IsOnBottomLine()
    {
        return movingToOldLine;
    }


    // =========================================================
    // REMOVE OLD REQUEST
    // =========================================================

    private void RemoveRequest()
    {
        Debug.Log(
            "Old request removed: " +
            gameObject.name
        );

        if (manager != null)
        {
            manager.RemoveBottomRequest(
                gameObject
            );
        }

        Destroy(
            gameObject
        );
    }
}