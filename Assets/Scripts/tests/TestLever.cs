using UnityEngine;

public class TestLever : MonoBehaviour
{
    [Header("Handle")]
    public Transform handle;

    [Header("Pivot")]
    public Transform pivot;

    [Header("Return Point")]
    public Transform returnPoint;

    public float returnSpeed = 5f;

    [Header("Handle Movement")]
    public float minimumX = -0.5f;
    public float maximumX = 0.5f;

    [Header("Pivot Rotation Limit")]
    public float minimumPivotAngle = -45f;
    public float maximumPivotAngle = 45f;

    [Header("Speed")]
    [Range(0f, 100f)]
    public float speed;

    [Header("Rotation Offset")]
    public float pivotRotationOffset = 0f;
    public float handleRotationOffset = 0f;

    [Header("Player Camera")]
    public Camera playerCamera;

    private bool dragging = false;
    private bool returning = false;

    private Vector3 grabOffset;

    private Vector2 currentValue =
        new Vector2(0.5f, 0f);

    private void Start()
    {
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }

        if (handle == null)
        {
            Debug.LogError(
                "TestLever: Handle has not been assigned!"
            );

            return;
        }

        if (pivot == null)
        {
            Debug.LogError(
                "TestLever: Pivot has not been assigned!"
            );

            return;
        }

        if (returnPoint == null)
        {
            Debug.LogError(
                "TestLever: Return Point has not been assigned!"
            );

            return;
        }

        SetHandleToReturnPoint();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TryGrabHandle();
        }

        if (dragging)
        {
            DragHandle();
        }

        if (Input.GetMouseButtonUp(0))
        {
            dragging = false;
            returning = true;
        }

        if (returning && !dragging)
        {
            ReturnHandle();
        }

        RotatePivotTowardsHandle();
        RotateHandleTowardsPivot();
    }

    // =========================================================
    // HANDLE DRAGGING
    // =========================================================

    private void TryGrabHandle()
    {
        if (playerCamera == null)
            return;

        Ray ray =
            playerCamera.ScreenPointToRay(
                Input.mousePosition
            );

        RaycastHit hit;

        if (!Physics.Raycast(ray, out hit))
            return;

        if (
            hit.transform != handle &&
            !hit.transform.IsChildOf(handle)
        )
        {
            return;
        }

        dragging = true;
        returning = false;

        Plane plane =
            new Plane(
                playerCamera.transform.forward,
                handle.position
            );

        float distance;

        if (
            plane.Raycast(
                ray,
                out distance
            )
        )
        {
            Vector3 mousePosition =
                ray.GetPoint(distance);

            grabOffset =
                handle.position -
                mousePosition;
        }
    }

    private void DragHandle()
    {
        if (playerCamera == null)
            return;

        Ray ray =
            playerCamera.ScreenPointToRay(
                Input.mousePosition
            );

        Plane plane =
            new Plane(
                playerCamera.transform.forward,
                handle.position
            );

        float distance;

        if (
            !plane.Raycast(
                ray,
                out distance
            )
        )
        {
            return;
        }

        Vector3 mouseWorldPosition =
            ray.GetPoint(distance);

        Vector3 targetWorldPosition =
            mouseWorldPosition +
            grabOffset;

        // Convert the mouse position
        // into this object's local space.
        Vector3 localPosition =
            transform.InverseTransformPoint(
                targetWorldPosition
            );

        // Only X can move.
        float x =
            Mathf.Clamp(
                localPosition.x,
                minimumX,
                maximumX
            );

        Vector3 newPosition =
            handle.localPosition;

        newPosition.x = x;

        // Y and Z remain unchanged.
        handle.localPosition =
            newPosition;

        float xValue =
            Mathf.InverseLerp(
                minimumX,
                maximumX,
                x
            );

        currentValue =
            new Vector2(
                xValue,
                0f
            );
    }

    // =========================================================
    // RETURN HANDLE
    // =========================================================

    private void ReturnHandle()
    {
        if (returnPoint == null)
            return;

        handle.position =
            Vector3.MoveTowards(
                handle.position,
                returnPoint.position,
                returnSpeed *
                Time.deltaTime
            );

        if (
            Vector3.Distance(
                handle.position,
                returnPoint.position
            ) < 0.001f
        )
        {
            handle.position =
                returnPoint.position;

            returning = false;

            UpdateCurrentValue();
        }
    }

    private void SetHandleToReturnPoint()
    {
        if (returnPoint == null)
            return;

        handle.position =
            returnPoint.position;

        UpdateCurrentValue();
    }

    private void UpdateCurrentValue()
    {
        float x =
            handle.localPosition.x;

        float xValue =
            Mathf.InverseLerp(
                minimumX,
                maximumX,
                x
            );

        currentValue =
            new Vector2(
                xValue,
                0f
            );
    }

    // =========================================================
    // PIVOT ROTATION
    // =========================================================

    private void RotatePivotTowardsHandle()
    {
        if (
            pivot == null ||
            handle == null
        )
        {
            return;
        }

        if (pivot.parent == null)
            return;

        // Convert both objects into the same
        // local coordinate space.
        Vector3 localHandlePosition =
            pivot.parent.InverseTransformPoint(
                handle.position
            );

        Vector3 localPivotPosition =
            pivot.parent.InverseTransformPoint(
                pivot.position
            );

        Vector3 direction =
            localHandlePosition -
            localPivotPosition;

        if (direction.sqrMagnitude < 0.001f)
            return;

        float angle =
            Mathf.Atan2(
                direction.y,
                direction.x
            ) *
            Mathf.Rad2Deg;

        angle +=
            pivotRotationOffset;

        // Clamp LOCAL rotation.
        angle =
            Mathf.Clamp(
                angle,
                minimumPivotAngle,
                maximumPivotAngle
            );

        pivot.localRotation =
            Quaternion.Euler(
                0f,
                0f,
                angle
            );

        // Convert angle to 0-100 speed.
        speed =
            Mathf.InverseLerp(
                minimumPivotAngle,
                maximumPivotAngle,
                angle
            ) *
            100f;
    }

    // =========================================================
    // HANDLE ROTATION
    // =========================================================

    private void RotateHandleTowardsPivot()
    {
        if (
            handle == null ||
            pivot == null
        )
        {
            return;
        }

        Vector3 direction =
            pivot.position -
            handle.position;

        if (direction.sqrMagnitude < 0.001f)
            return;

        float angle =
            Mathf.Atan2(
                direction.y,
                direction.x
            ) *
            Mathf.Rad2Deg;

        // Flip the handle so it points
        // in the same visual direction.
        angle +=
            handleRotationOffset +
            180f;

        handle.rotation =
            Quaternion.Euler(
                0f,
                0f,
                angle
            );
    }

    // =========================================================
    // PUBLIC VALUES
    // =========================================================

    public Vector2 GetValue()
    {
        return currentValue;
    }

    public float GetSpeed()
    {
        return speed;
    }
}