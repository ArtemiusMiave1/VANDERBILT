using UnityEngine;

public class RequestVisualIndicator : MonoBehaviour
{
    [Header("Object")]
    public Renderer targetRenderer;

    [Header("Materials")]
    public Material originalMaterial;
    public Material activeMaterial;

    [Header("Light")]
    public GameObject requestLight;

    private void Awake()
    {
        if (targetRenderer == null)
        {
            targetRenderer =
                GetComponent<Renderer>();
        }

        // Automatically remember the starting material
        if (
            targetRenderer != null &&
            originalMaterial == null
        )
        {
            originalMaterial =
                targetRenderer.material;
        }

        // Make sure the request starts inactive
        SetInactive();
    }


    // ==========================================
    // REQUEST ACCEPTED
    // ==========================================

    public void SetActive()
    {
        if (targetRenderer != null &&
            activeMaterial != null)
        {
            targetRenderer.material =
                activeMaterial;
        }

        if (requestLight != null)
        {
            requestLight.SetActive(true);
        }
    }


    // ==========================================
    // REQUEST COMPLETED
    // ==========================================

    public void SetInactive()
    {
        if (targetRenderer != null &&
            originalMaterial != null)
        {
            targetRenderer.material =
                originalMaterial;
        }

        if (requestLight != null)
        {
            requestLight.SetActive(false);
        }
    }
}