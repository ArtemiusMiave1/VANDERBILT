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
    public Color paymentReadyLightColour =
        Color.green;

    [Header("Light")]
    public GameObject requestLight;

    private Light lightComponent;
    private Color originalLightColour;


    // =========================================================
    // AWAKE
    // =========================================================

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
        if (
            targetRenderer != null &&
            paymentReadyMaterial != null
        )
        {
            targetRenderer.material =
                paymentReadyMaterial;
        }


        if (requestLight != null)
        {
            requestLight.SetActive(true);
        }


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


    // =========================================================
    // SET ALL VANDERBILT PAYMENT LIGHTS
    // =========================================================

    public static void SetAllVanderbiltPaymentLights(
        bool paymentReady
    )
    {
        if (LocationManager.Instance == null)
            return;


        foreach (
            Location location
            in LocationManager.Instance.locations
        )
        {
            if (location == null)
                continue;

            if (location.locationType == null)
                continue;


            // Only Vanderbilt collection locations.
            if (
                location.locationType.LocationType !=
                "Vanderbilt"
            )
            {
                continue;
            }


            RequestVisualIndicator indicator =
                location.GetComponentInChildren
                <RequestVisualIndicator>(true);


            if (indicator == null)
            {
                Debug.LogWarning(
                    "RequestVisualIndicator: " +
                    "No indicator found on Vanderbilt " +
                    "location " +
                    location.GetDisplayName()
                );

                continue;
            }


            if (paymentReady)
            {
                indicator.SetPaymentReady();
            }
            else
            {
                indicator.SetInactive();
            }
        }
    }


    // =========================================================
    // REFRESH VANDERBILT PAYMENT LIGHTS
    // =========================================================

    public static void RefreshVanderbiltPaymentLights()
    {
        RequestPaper[] requests =
            FindObjectsOfType<RequestPaper>();


        bool paymentWaiting =
            false;


        // Check whether ANY request has
        // money waiting to be collected.
        foreach (
            RequestPaper request
            in requests
        )
        {
            if (request == null)
                continue;


            if (request.HasPendingGold())
            {
                paymentWaiting = true;
                break;
            }
        }


        SetAllVanderbiltPaymentLights(
            paymentWaiting
        );
    }
}