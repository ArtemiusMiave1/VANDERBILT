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


    private void Start()
    {
        // Start with the UI camera disabled
        uiCamera.gameObject.SetActive(false);

        //supplyOrderUI.SetActive(false);
    }


    public void OpenSupplyOrder()
    {
        // Disable player movement
        if (playerMovement != null)
            playerMovement.enabled = false;

        // Disable player looking
        if (playerMouseLook != null)
            playerMouseLook.enabled = false;

        if (cameraMouseLook != null)
            cameraMouseLook.enabled = false;


        // Disable player camera
        playerCamera.gameObject.SetActive(false);


        // Enable UI camera
        uiCamera.gameObject.SetActive(true);


        // Show supply order
        supplyOrderUI.SetActive(true);


        // Enable mouse
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }


    public void CloseSupplyOrder()
    {
        // Hide supply order
        supplyOrderUI.SetActive(false);


        // Disable UI camera
        uiCamera.gameObject.SetActive(false);


        // Enable player camera
        playerCamera.gameObject.SetActive(true);


        // Enable player movement
        if (playerMovement != null)
            playerMovement.enabled = true;

        // Enable player looking
        if (playerMouseLook != null)
            playerMouseLook.enabled = true;

        if (cameraMouseLook != null)
            cameraMouseLook.enabled = true;


        // Lock mouse again
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}