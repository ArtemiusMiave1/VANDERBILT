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


    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        // The Fax itself is controlled by ShipMovement.
        //
        // ShipMovement will activate it when the ship has
        // stopped at a Resource Depot.

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
        // -----------------------------------------------------
        // Disable player movement
        // -----------------------------------------------------

        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }


        // -----------------------------------------------------
        // Disable player looking
        // -----------------------------------------------------

        if (playerMouseLook != null)
        {
            playerMouseLook.enabled = false;
        }


        if (cameraMouseLook != null)
        {
            cameraMouseLook.enabled = false;
        }


        // -----------------------------------------------------
        // Disable player camera
        // -----------------------------------------------------

        if (playerCamera != null)
        {
            playerCamera.gameObject.SetActive(false);
        }


        // -----------------------------------------------------
        // Enable close-up UI camera
        // -----------------------------------------------------

        if (uiCamera != null)
        {
            uiCamera.gameObject.SetActive(true);
        }


        // -----------------------------------------------------
        // Show the order form
        // -----------------------------------------------------
        //
        // This is safe because the player has interacted
        // with the Fax.
        //
        // We DO NOT hide it when CloseSupplyOrder()
        // is called.

        if (supplyOrderUI != null)
        {
            supplyOrderUI.SetActive(true);
        }


        // -----------------------------------------------------
        // Enable mouse
        // -----------------------------------------------------

        Cursor.lockState =
            CursorLockMode.None;

        Cursor.visible = true;
    }


    // =========================================================
    // CLOSE SUPPLY ORDER
    // =========================================================
    //
    // Called by the X button.
    //
    // IMPORTANT:
    //
    // This ONLY closes the close-up camera.
    //
    // It does NOT hide the Fax.
    //
    // The Fax stays visible because the ship is still
    // stopped at the Resource Depot.
    // =========================================================

    public void CloseSupplyOrder()
    {
        // -----------------------------------------------------
        // Disable close-up camera
        // -----------------------------------------------------

        if (uiCamera != null)
        {
            uiCamera.gameObject.SetActive(false);
        }


        // -----------------------------------------------------
        // Enable player camera
        // -----------------------------------------------------

        if (playerCamera != null)
        {
            playerCamera.gameObject.SetActive(true);
        }


        // -----------------------------------------------------
        // Enable player movement
        // -----------------------------------------------------

        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }


        // -----------------------------------------------------
        // Enable player looking
        // -----------------------------------------------------

        if (playerMouseLook != null)
        {
            playerMouseLook.enabled = true;
        }


        if (cameraMouseLook != null)
        {
            cameraMouseLook.enabled = true;
        }


        // -----------------------------------------------------
        // Lock mouse
        // -----------------------------------------------------

        Cursor.lockState =
            CursorLockMode.Locked;

        Cursor.visible = false;


        // -----------------------------------------------------
        // DO NOT HIDE THE FAX
        // -----------------------------------------------------
        //
        // Do NOT put this here:
        //
        // supplyOrderUI.SetActive(false);
        //
        // ShipMovement will hide the Fax automatically
        // when the player starts travelling.
    }
}