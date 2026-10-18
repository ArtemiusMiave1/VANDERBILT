
using UnityEngine;
using UnityEngine.UI;

public class CrosshairManager : MonoBehaviour
{
    public static CrosshairManager Instance;

    [Header("Crosshair Image")]
    public Image crosshairImage;

    [Header("Crosshair Sprites")]
    public Sprite defaultCrosshair;
    public Sprite interactCrosshair;
    public Sprite grabbingCrosshair;

    [Header("Interactable Detection")]
    public Camera playerCamera;

    [Tooltip("First tag that changes the crosshair.")]
    public string interactableTag = "Interactable";

    [Tooltip("Tag used by request papers and pickups.")]
    public string pickupTag = "canPickUp";

    public float interactionDistance = 3f;
    public LayerMask interactionLayers = ~0;

    [Header("Mouse Cursor Textures")]
    public Texture2D normalCursor;
    public Texture2D handCursor;

    public Vector2 normalCursorHotspot = Vector2.zero;
    public Vector2 handCursorHotspot = Vector2.zero;

    [Header("Grabbing")]
    public bool showCrosshairWhenGrabbing = true;
    public bool followGrabbedObject = true;

    private RectTransform crosshairRect;
    private Vector2 originalAnchoredPosition;

    private Transform grabbedObject;
    private Vector3 localGrabPosition;

    private bool grabbing;
    private bool reading;

    // =========================================================
    // AWAKE
    // =========================================================

    private void Awake()
    {
        Instance = this;

        if (crosshairImage == null)
        {
            crosshairImage = GetComponent<Image>();
        }

        if (crosshairImage != null)
        {
            crosshairRect = crosshairImage.rectTransform;

            originalAnchoredPosition =
                crosshairRect.anchoredPosition;

            crosshairImage.raycastTarget = false;
        }

        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }
    }

    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        SetNormalMouseCursor();

        if (crosshairImage != null)
        {
            crosshairImage.enabled = true;
            crosshairImage.sprite = defaultCrosshair;
        }
    }

    // =========================================================
    // UPDATE
    // =========================================================

    private void Update()
    {
        if (grabbing || reading)
            return;

        UpdateCrosshairHover();
    }

    private void LateUpdate()
    {
        if (grabbing && followGrabbedObject)
        {
            UpdateGrabbedCrosshairPosition();
        }
    }

    // =========================================================
    // HOVER DETECTION
    // =========================================================

    private void UpdateCrosshairHover()
    {
        if (playerCamera == null ||
            crosshairImage == null)
        {
            return;
        }

        Ray ray = playerCamera.ViewportPointToRay(
            new Vector3(0.5f, 0.5f, 0f)
        );

        if (Physics.Raycast(
            ray,
            out RaycastHit hit,
            interactionDistance,
            interactionLayers))
        {
            if (HasInteractableTag(hit.transform))
            {
                crosshairImage.sprite = interactCrosshair;
                return;
            }
        }

        crosshairImage.sprite = defaultCrosshair;
    }

    private bool HasInteractableTag(Transform target)
    {
        while (target != null)
        {
            if (target.CompareTag(interactableTag) ||
                target.CompareTag(pickupTag))
            {
                return true;
            }

            target = target.parent;
        }

        return false;
    }

    // =========================================================
    // START GRABBING
    // =========================================================

    public void StartGrabbing()
    {
        grabbing = true;
        reading = false;

        grabbedObject = null;

        if (crosshairImage != null)
        {
            crosshairImage.enabled =
                showCrosshairWhenGrabbing;

            crosshairImage.sprite =
                grabbingCrosshair != null
                    ? grabbingCrosshair
                    : interactCrosshair;
        }

        SetNormalMouseCursor();
    }

    // =========================================================
    // SET GRABBED OBJECT
    // =========================================================

    public void SetGrabbedObject(
        Transform target,
        Vector3 worldHitPoint)
    {
        grabbedObject = target;

        if (grabbedObject == null)
            return;

        // Store the exact point clicked in local space.
        localGrabPosition =
            grabbedObject.InverseTransformPoint(
                worldHitPoint
            );

        UpdateGrabbedCrosshairPosition();
    }

    // =========================================================
    // FOLLOW GRABBED OBJECT
    // =========================================================

    private void UpdateGrabbedCrosshairPosition()
    {
        if (!grabbing ||
            !followGrabbedObject ||
            grabbedObject == null ||
            crosshairRect == null ||
            playerCamera == null)
        {
            return;
        }

        // Convert the original clicked point back
        // into world space as the object moves.
        Vector3 worldPosition =
            grabbedObject.TransformPoint(
                localGrabPosition
            );

        // Convert world position to screen position.
        Vector3 screenPosition =
            playerCamera.WorldToScreenPoint(
                worldPosition
            );

        if (screenPosition.z <= 0f)
        {
            if (crosshairImage != null)
                crosshairImage.enabled = false;

            return;
        }

        if (crosshairImage != null)
        {
            crosshairImage.enabled =
                showCrosshairWhenGrabbing;
        }

        Canvas canvas =
            crosshairImage.GetComponentInParent<Canvas>();

        if (canvas == null)
            return;

        RectTransform parentRect =
            crosshairRect.parent as RectTransform;

        if (parentRect == null)
            return;

        Camera uiCamera =
            canvas.renderMode == RenderMode.ScreenSpaceOverlay
                ? null
                : canvas.worldCamera;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRect,
            screenPosition,
            uiCamera,
            out Vector2 localPosition
        );

        // Account for the crosshair's anchor.
        Vector2 anchorReference = new Vector2(
            (crosshairRect.anchorMin.x - parentRect.pivot.x)
                * parentRect.rect.width,

            (crosshairRect.anchorMin.y - parentRect.pivot.y)
                * parentRect.rect.height
        );

        crosshairRect.anchoredPosition =
            localPosition - anchorReference;
    }

    // =========================================================
    // STOP GRABBING
    // =========================================================

    public void StopGrabbing()
    {
        grabbing = false;
        grabbedObject = null;

        if (crosshairRect != null)
        {
            crosshairRect.anchoredPosition =
                originalAnchoredPosition;
        }

        if (crosshairImage != null)
        {
            crosshairImage.enabled = true;
            crosshairImage.sprite = defaultCrosshair;
        }
    }

    // =========================================================
    // START READING / UI
    // =========================================================

    public void StartReading()
    {
        reading = true;
        grabbing = false;
        grabbedObject = null;

        if (crosshairImage != null)
        {
            crosshairImage.enabled = false;
        }

        SetHandMouseCursor();
    }

    // =========================================================
    // STOP READING / UI
    // =========================================================

    public void StopReading()
    {
        reading = false;

        if (crosshairRect != null)
        {
            crosshairRect.anchoredPosition =
                originalAnchoredPosition;
        }

        if (crosshairImage != null)
        {
            crosshairImage.enabled = true;
            crosshairImage.sprite = defaultCrosshair;
        }

        SetNormalMouseCursor();
    }

    // =========================================================
    // MOUSE CURSORS
    // =========================================================

    public void SetHandMouseCursor()
    {
        if (handCursor == null)
            return;

        Cursor.SetCursor(
            handCursor,
            handCursorHotspot,
            CursorMode.Auto
        );
    }

    public void SetNormalMouseCursor()
    {
        Cursor.SetCursor(
            normalCursor,
            normalCursorHotspot,
            CursorMode.Auto
        );
    }

    // =========================================================
    // PUBLIC VALUES
    // =========================================================

    public bool IsGrabbing()
    {
        return grabbing;
    }

    public bool IsReading()
    {
        return reading;
    }
}
