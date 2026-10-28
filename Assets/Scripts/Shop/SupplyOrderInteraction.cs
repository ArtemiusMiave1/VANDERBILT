using UnityEngine;

public class SupplyOrderInteraction : MonoBehaviour
{
    [Header("Cameras")]
    public Camera playerCamera;
    public Camera uiCamera;

    [Header("Supply Order UI")]
    public GameObject supplyOrderUI;

    [Header("Player")]
    public FirstPersonDrifter playerMovement;
    public MouseLook playerMouseLook;
    public MouseLook cameraMouseLook;
    public PlayerState playerState;


    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        // The Fax itself is controlled by ShipMovement.
        // ShipMovement activates it when the ship stops
        // at a Resource Depot.

        if (uiCamera != null)
        {
            uiCamera.gameObject.SetActive(false);
        }
    }


    // =========================================================
    // OPEN SUPPLY ORDER
    // =========================================================

    public void OpenSupplyOrder()
    {
        // Disable player movement.
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        // Disable player looking.
        if (playerMouseLook != null)
        {
            playerMouseLook.enabled = false;
        }

        if (cameraMouseLook != null)
        {
            cameraMouseLook.enabled = false;
        }

        // Disable player camera.
        if (playerCamera != null)
        {
            playerCamera.gameObject.SetActive(false);
        }

        // Enable close-up UI camera.
        if (uiCamera != null)
        {
            uiCamera.gameObject.SetActive(true);
        }

        // Show the Order Form.
        if (supplyOrderUI != null)
        {
            supplyOrderUI.SetActive(true);
        }

        // Switch to Book state.
        // This disables gameplay camera controls and
        // shows the regular mouse cursor.
        if (playerState != null)
        {
            playerState.SetState(PlayerState.State.Book);
        }
        else
        {
            // Fallback if PlayerState is not assigned.
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }


    // =========================================================
    // CLOSE SUPPLY ORDER
    // =========================================================

    public void CloseSupplyOrder()
    {
        // Disable close-up camera.
        if (uiCamera != null)
        {
            uiCamera.gameObject.SetActive(false);
        }

        // Enable player camera.
        if (playerCamera != null)
        {
            playerCamera.gameObject.SetActive(true);
        }

        // Re-enable player movement.
        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }

        // Re-enable player looking.
        if (playerMouseLook != null)
        {
            playerMouseLook.enabled = true;
        }

        if (cameraMouseLook != null)
        {
            cameraMouseLook.enabled = true;
        }

        // Return to Gameplay state.
        // This restores the gameplay cursor behavior
        // and stops the reading crosshair state.
        if (playerState != null)
        {
            playerState.SetState(PlayerState.State.Gameplay);
        }
        else
        {
            // Fallback if PlayerState is not assigned.
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        // IMPORTANT:
        // Do NOT hide the Fax here.
        // ShipMovement handles hiding it when travelling.
    }
}