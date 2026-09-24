using UnityEngine;

public class ShipControlButton : MonoBehaviour
{
    [Header("Ship")]
    public ShipMovement shipMovement;

    [Header("Button Visual")]
    public Renderer buttonRenderer;

    [Header("Materials")]
    public Material stoppedMaterial;
    public Material movingMaterial;

    private void Start()
    {
        if (shipMovement == null)
        {
            shipMovement =
                FindObjectOfType<ShipMovement>();
        }

        if (buttonRenderer == null)
        {
            buttonRenderer =
                GetComponent<Renderer>();
        }

        UpdateButtonVisual();
    }

    private void Update()
    {
        UpdateButtonVisual();
    }

    private void OnMouseDown()
    {
        if (shipMovement == null)
        {
            Debug.LogError(
                "ShipControlButton: ShipMovement not found!"
            );

            return;
        }

        if (shipMovement.IsMoving())
        {
            shipMovement.StopMovement();
        }
        else
        {
            shipMovement.StartRoute();
        }
    }

    private void UpdateButtonVisual()
    {
        if (
            buttonRenderer == null ||
            shipMovement == null
        )
        {
            return;
        }

        if (shipMovement.IsMoving())
        {
            if (movingMaterial != null)
            {
                buttonRenderer.material =
                    movingMaterial;
            }
        }
        else
        {
            if (stoppedMaterial != null)
            {
                buttonRenderer.material =
                    stoppedMaterial;
            }
        }
    }
}