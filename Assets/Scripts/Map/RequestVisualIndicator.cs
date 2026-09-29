using UnityEngine;

public class RequestVisualIndicator : MonoBehaviour
{
    [Header("Object")]
    public Renderer targetRenderer;

    [Header("Materials")]
    public Material originalMaterial;
    public Material activeMaterial;

    [Header("Payment")]
    public Material paymentReadyMaterial;
    public Color paymentReadyLightColour = Color.green;

    [Header("Light")]
    public GameObject requestLight;

    private Light lightComponent;
    private Color originalLightColour;


    private void Awake()
    {
        if (targetRenderer == null)
        {
            targetRenderer =
                GetComponent<Renderer>();
        }

        // Automatically remember starting material.
        if (
            targetRenderer != null &&
            originalMaterial == null
        )
        {
            originalMaterial =
                targetRenderer.material;
        }


        // -----------------------------------------------------
        // GET LIGHT
        // -----------------------------------------------------

        if (requestLight != null)
        {
            lightComponent =
                requestLight.GetComponent<Light>();

            if (lightComponent != null)
            {
                originalLightColour =
                    lightComponent.color;
            }
        }


        // Start inactive.
        SetInactive();
    }


    // =========================================================
    // REQUEST ACCEPTED
    // =========================================================

    public void SetActive()
    {
        if (
            targetRenderer != null &&
            activeMaterial != null
        )
        {
            targetRenderer.material =
                activeMaterial;
        }


        if (requestLight != null)
        {
            requestLight.SetActive(true);
        }


        // Restore normal request light colour.
        if (lightComponent != null)
        {
            lightComponent.color =
                originalLightColour;
        }
    }


    // =========================================================
    // PAYMENT READY
    // =========================================================

    public void SetPaymentReady()
    {
        // Change material if one has been assigned.
        if (
            targetRenderer != null &&
            paymentReadyMaterial != null
        )
        {
            targetRenderer.material =
                paymentReadyMaterial;
        }


        // Turn light on.
        if (requestLight != null)
        {
            requestLight.SetActive(true);
        }


        // Make light green.
        if (lightComponent != null)
        {
            lightComponent.color =
                paymentReadyLightColour;
        }
    }


    // =========================================================
    // INACTIVE
    // =========================================================

    public void SetInactive()
    {
        if (
            targetRenderer != null &&
            originalMaterial != null
        )
        {
            targetRenderer.material =
                originalMaterial;
        }


        if (lightComponent != null)
        {
            lightComponent.color =
                originalLightColour;
        }


        if (requestLight != null)
        {
            requestLight.SetActive(false);
        }
    }
}