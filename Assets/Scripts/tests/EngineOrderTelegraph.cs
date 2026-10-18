
using UnityEngine;
using UnityEngine.InputSystem;

public class EngineOrderTelegraph : MonoBehaviour
{
    [Header("Handle")]
    public Transform handle;

    [Header("Pivot")]
    public Transform pivot;

    [Header("Handle Movement - Y and Z")]
    public float minimumY = -0.5f;
    public float maximumY = 0.5f;

    public float minimumZ = -0.5f;
    public float maximumZ = 0.5f;

    [Header("Inner Movement Limit")]
    [Min(0f)]
    [Tooltip("Minimum Y/Z distance from the pivot.")]
    public float minimumDistanceFromPivot = 0.3f;

    [Header("Pivot Rotation Limit")]
    public float minimumPivotAngle = -90f;
    public float maximumPivotAngle = 90f;

    [Header("Rotation Offset")]
    public float pivotRotationOffset = 0f;
    public float handleRotationOffset = 0f;

    [Header("Player")]
    public Camera playerCamera;
    public PlayerState playerState;

    [Header("Movement Sensitivity")]
    public float movementSensitivity = 1f;

    [Header("Speed States")]
    [Range(0, 4)]
    public int speedState = 0;

    [Range(0f, 100f)]
    public float speed = 0f;

    [Header("Return Movement")]
    public float returnSpeed = 5f;
    public float snapSpeed = 180f;

    [Header("Cursor")]
    [Tooltip("Warp the actual mouse cursor to the clicked point on the handle. Disable this if only the Canvas crosshair should follow.")]
    public bool keepCursorOnGrabPoint = false;

    private Vector3 localGrabPoint;

    private Vector2 previousMousePosition;
    private bool skipNextMouseDelta;

    private Vector2 currentValue =
        new Vector2(0.5f, 0.5f);

    private bool dragging = false;
    private bool returning = false;

    private Vector3 originalHandlePivotPosition;
    private Quaternion originalHandlePivotRotation;

    private float currentPivotAngle;
    private float targetPivotAngle;

    private SoundManager soundManager;

    // =========================================================
    // AWAKE
    // =========================================================

    private void Awake()
    {
        GameObject audioObject =
            GameObject.FindGameObjectWithTag("Audio");

        if (audioObject != null)
        {
            soundManager =
                audioObject.GetComponent<SoundManager>();
        }
    }

    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;

        if (playerState == null)
            playerState =
                FindFirstObjectByType<PlayerState>();

        if (handle == null || pivot == null)
        {
            Debug.LogError(
                "EngineOrderTelegraph: Handle or Pivot missing!"
            );

            enabled = false;
            return;
        }

        // Store the handle's original attachment.
        originalHandlePivotPosition =
            pivot.InverseTransformPoint(handle.position);

        originalHandlePivotRotation =
            Quaternion.Inverse(pivot.rotation) *
            handle.rotation;

        speedState = Mathf.Clamp(speedState, 0, 4);

        targetPivotAngle =
            GetAngleForState(speedState);

        currentPivotAngle = targetPivotAngle;

        ApplyPivotRotation();
        SetHandleToPivot();

        speed = speedState * 25f;

        UpdateCurrentValue();
    }

    // =========================================================
    // UPDATE
    // =========================================================

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TryGrabHandle();
        }

        if (dragging)
        {
            if (playerState != null &&
                !playerState.IsGrabbing())
            {
                StopDragging();
            }
            else if (Input.GetMouseButton(0))
            {
                DragHandle();
            }
        }

        if (Input.GetMouseButtonUp(0) && dragging)
        {
            StopDragging();
        }

        if (returning && !dragging)
        {
            ReturnHandle();
        }
    }

    // =========================================================
    // GRAB HANDLE
    // =========================================================

    private void TryGrabHandle()
    {
        if (playerCamera == null || handle == null)
            return;

        if (playerState != null &&
            !playerState.IsGameplay())
            return;

        Ray ray =
            playerCamera.ScreenPointToRay(
                Input.mousePosition
            );

        if (!Physics.Raycast(ray, out RaycastHit hit))
            return;

        if (
            hit.transform != handle &&
            !hit.transform.IsChildOf(handle)
        )
        {
            return;
        }

        // Save the exact point clicked on the handle.
        localGrabPoint =
            handle.InverseTransformPoint(hit.point);

        previousMousePosition = Input.mousePosition;

        dragging = true;
        returning = false;
        skipNextMouseDelta = true;

        if (playerState != null)
        {
            playerState.SetState(
                PlayerState.State.Grabbing
            );
        }

        // Tell CrosshairManager to follow the clicked point.
        // This must happen AFTER setting PlayerState.Grabbing.
        if (CrosshairManager.Instance != null)
        {
            CrosshairManager.Instance.SetGrabbedObject(
                handle,
                hit.point
            );
        }
    }

    // =========================================================
    // DRAG HANDLE
    // =========================================================

    private void DragHandle()
    {
        if (playerCamera == null)
            return;

        Vector2 mousePosition = Input.mousePosition;

        Vector2 mouseDelta =
            mousePosition - previousMousePosition;

        previousMousePosition = mousePosition;

        if (skipNextMouseDelta)
        {
            mouseDelta = Vector2.zero;
            skipNextMouseDelta = false;
        }

        // =====================================================
        // GRABBING SENSITIVITY
        // =====================================================

        float sensitivity = movementSensitivity;

        if (playerState != null)
        {
            sensitivity *=
                playerState.GetGrabbingSensitivity();
        }

        mouseDelta *= sensitivity;

        // =====================================================
        // CONVERT MOUSE MOVEMENT TO Y/Z MOVEMENT
        // =====================================================

        Vector3 handleScreenPosition =
            playerCamera.WorldToScreenPoint(
                handle.position
            );

        Vector3 targetScreenPosition =
            handleScreenPosition +
            new Vector3(
                mouseDelta.x,
                mouseDelta.y,
                0f
            );

        Vector3 targetWorldPosition =
            playerCamera.ScreenToWorldPoint(
                targetScreenPosition
            );

        Vector3 currentLocalPosition =
            transform.InverseTransformPoint(
                handle.position
            );

        Vector3 targetLocalPosition =
            transform.InverseTransformPoint(
                targetWorldPosition
            );

        // =====================================================
        // OUTER Y/Z MOVEMENT LIMITS
        // =====================================================

        targetLocalPosition.y = Mathf.Clamp(
            targetLocalPosition.y,
            minimumY,
            maximumY
        );

        targetLocalPosition.z = Mathf.Clamp(
            targetLocalPosition.z,
            minimumZ,
            maximumZ
        );

        // =====================================================
        // INNER MOVEMENT LIMIT
        // =====================================================

        Vector3 localPivotPosition =
            transform.InverseTransformPoint(
                pivot.position
            );

        Vector2 direction = new Vector2(
            targetLocalPosition.y - localPivotPosition.y,
            targetLocalPosition.z - localPivotPosition.z
        );

        if (direction.magnitude < minimumDistanceFromPivot)
        {
            // If the handle is exactly at the pivot,
            // use its previous direction instead.
            if (direction.sqrMagnitude < 0.000001f)
            {
                direction = new Vector2(
                    currentLocalPosition.y -
                    localPivotPosition.y,

                    currentLocalPosition.z -
                    localPivotPosition.z
                );
            }

            if (direction.sqrMagnitude < 0.000001f)
            {
                direction = Vector2.up;
            }

            direction.Normalize();

            targetLocalPosition.y =
                localPivotPosition.y +
                direction.x * minimumDistanceFromPivot;

            targetLocalPosition.z =
                localPivotPosition.z +
                direction.y * minimumDistanceFromPivot;
        }

        // Clamp to outer limits again.
        targetLocalPosition.y = Mathf.Clamp(
            targetLocalPosition.y,
            minimumY,
            maximumY
        );

        targetLocalPosition.z = Mathf.Clamp(
            targetLocalPosition.z,
            minimumZ,
            maximumZ
        );

        // Keep X fixed.
        targetLocalPosition.x =
            currentLocalPosition.x;

        handle.position =
            transform.TransformPoint(
                targetLocalPosition
            );

        // Update pivot, throttle, and handle rotation.
        RotatePivotTowardsHandle();
        UpdateSpeedFromPivot();

        RotateHandleTowardsPivot();
        UpdateCurrentValue();

        // Optional: move the hardware cursor as well.
        // The Canvas crosshair follows independently.
        if (keepCursorOnGrabPoint)
        {
            KeepCursorOnGrabPoint();
        }
    }

    // =========================================================
    // OPTIONAL HARDWARE CURSOR FOLLOWING
    // =========================================================

    private void KeepCursorOnGrabPoint()
    {
        if (!dragging ||
            handle == null ||
            playerCamera == null ||
            Mouse.current == null)
        {
            return;
        }

        Vector3 worldGrabPoint =
            handle.TransformPoint(localGrabPoint);

        Vector3 screenPosition =
            playerCamera.WorldToScreenPoint(
                worldGrabPoint
            );

        if (screenPosition.z <= 0f)
            return;

        Vector2 cursorPosition =
            new Vector2(
                screenPosition.x,
                screenPosition.y
            );

        Mouse.current.WarpCursorPosition(
            cursorPosition
        );

        previousMousePosition = cursorPosition;
    }

    // =========================================================
    // STOP DRAGGING
    // =========================================================

    private void StopDragging()
    {
        dragging = false;
        returning = true;

        float normalizedAngle =
            Mathf.InverseLerp(
                minimumPivotAngle,
                maximumPivotAngle,
                currentPivotAngle
            );

        speedState = Mathf.Clamp(
            Mathf.RoundToInt(
                normalizedAngle * 4f
            ),
            0,
            4
        );

        speed = speedState * 25f;

        targetPivotAngle =
            GetAngleForState(speedState);

        if (playerState != null &&
            playerState.IsGrabbing())
        {
            playerState.SetState(
                PlayerState.State.Gameplay
            );
        }
        else if (CrosshairManager.Instance != null)
        {
            // Fallback when no PlayerState is assigned.
            CrosshairManager.Instance.StopGrabbing();
        }
    }

    // =========================================================
    // RETURN HANDLE TO PIVOT
    // =========================================================

    private void ReturnHandle()
    {
        currentPivotAngle = Mathf.MoveTowards(
            currentPivotAngle,
            targetPivotAngle,
            snapSpeed * Time.deltaTime
        );

        ApplyPivotRotation();

        Vector3 targetPosition =
            pivot.TransformPoint(
                originalHandlePivotPosition
            );

        Quaternion targetRotation =
            pivot.rotation *
            originalHandlePivotRotation;

        handle.position =
            Vector3.MoveTowards(
                handle.position,
                targetPosition,
                returnSpeed * Time.deltaTime
            );

        handle.rotation =
            Quaternion.RotateTowards(
                handle.rotation,
                targetRotation,
                snapSpeed * Time.deltaTime
            );

        UpdateCurrentValue();

        bool positionReached =
            Vector3.Distance(
                handle.position,
                targetPosition
            ) < 0.001f;

        bool rotationReached =
            Quaternion.Angle(
                handle.rotation,
                targetRotation
            ) < 0.1f;

        bool pivotReached =
            Mathf.Abs(
                currentPivotAngle -
                targetPivotAngle
            ) < 0.01f;

        if (
            positionReached &&
            rotationReached &&
            pivotReached
        )
        {
            currentPivotAngle = targetPivotAngle;

            ApplyPivotRotation();
            SetHandleToPivot();

            returning = false;

            UpdateCurrentValue();
        }
    }

    // =========================================================
    // SET HANDLE TO PIVOT
    // =========================================================

    private void SetHandleToPivot()
    {
        handle.position =
            pivot.TransformPoint(
                originalHandlePivotPosition
            );

        handle.rotation =
            pivot.rotation *
            originalHandlePivotRotation;
    }

    // =========================================================
    // PIVOT ROTATION
    // =========================================================

    private void RotatePivotTowardsHandle()
    {
        if (pivot.parent == null)
            return;

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
                direction.z,
                direction.y
            ) * Mathf.Rad2Deg;

        angle += pivotRotationOffset;

        currentPivotAngle = Mathf.Clamp(
            angle,
            minimumPivotAngle,
            maximumPivotAngle
        );

        ApplyPivotRotation();
    }

    private void ApplyPivotRotation()
    {
        pivot.localRotation =
            Quaternion.Euler(
                currentPivotAngle,
                0f,
                0f
            );
    }

    // =========================================================
    // REAL-TIME SPEED AND SOUND
    // =========================================================

    private void UpdateSpeedFromPivot()
    {
        float normalizedAngle =
            Mathf.InverseLerp(
                minimumPivotAngle,
                maximumPivotAngle,
                currentPivotAngle
            );

        int newSpeedState = Mathf.Clamp(
            Mathf.RoundToInt(
                normalizedAngle * 4f
            ),
            0,
            4
        );

        // Only play sound when the throttle state changes.
        if (newSpeedState != speedState)
        {
            speedState = newSpeedState;
            speed = speedState * 25f;

            if (soundManager != null)
            {
                soundManager.PlaySFX(
                    soundManager.Lever
                );
            }

            Debug.Log(
                "Engine Telegraph Speed: " +
                speed + "%"
            );
        }
    }

    // =========================================================
    // SPEED STATES
    // =========================================================

    private float GetAngleForState(int state)
    {
        float normalized =
            Mathf.Clamp(state, 0, 4) / 4f;

        return Mathf.Lerp(
            minimumPivotAngle,
            maximumPivotAngle,
            normalized
        );
    }

    public void SetSpeedState(int state)
    {
        speedState = Mathf.Clamp(state, 0, 4);

        speed = speedState * 25f;

        targetPivotAngle =
            GetAngleForState(speedState);

        returning = true;
    }

    // =========================================================
    // HANDLE ROTATION WHILE DRAGGING
    // =========================================================

    private void RotateHandleTowardsPivot()
    {
        Vector3 direction =
            pivot.position - handle.position;

        if (direction.sqrMagnitude < 0.001f)
            return;

        Vector3 localDirection =
            transform.InverseTransformDirection(
                direction
            );

        float angle =
            Mathf.Atan2(
                localDirection.z,
                localDirection.y
            ) * Mathf.Rad2Deg;

        angle +=
            handleRotationOffset + 180f;

        handle.rotation =
            transform.rotation *
            Quaternion.Euler(
                angle,
                0f,
                0f
            );
    }

    // =========================================================
    // CURRENT VALUE
    // =========================================================

    private void UpdateCurrentValue()
    {
        Vector3 localPosition =
            transform.InverseTransformPoint(
                handle.position
            );

        float yValue =
            Mathf.InverseLerp(
                minimumY,
                maximumY,
                localPosition.y
            );

        float zValue =
            Mathf.InverseLerp(
                minimumZ,
                maximumZ,
                localPosition.z
            );

        currentValue =
            new Vector2(
                yValue,
                zValue
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

    public int GetSpeedState()
    {
        return speedState;
    }

    public bool IsDragging()
    {
        return dragging;
    }
}
