
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

        // Rotation relative to the request parent.
        targetRotation =
            Quaternion.Euler(
                lineRotation
            );

        // Use local position so requests follow the ship.
        transform.localPosition =
            startPosition;

        transform.localRotation =
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
        // Move relative to the parent, not world space.
        transform.localPosition =
            Vector3.MoveTowards(
                transform.localPosition,
                endPosition,
                moveSpeed *
                Time.deltaTime
            );

        // Keep the selected local rotation.
        transform.localRotation =
            targetRotation;

        if (
            Vector3.Distance(
                transform.localPosition,
                endPosition
            ) < 0.01f
        )
        {
            transform.localPosition =
                endPosition;

            transform.localRotation =
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

        transform.localRotation =
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
