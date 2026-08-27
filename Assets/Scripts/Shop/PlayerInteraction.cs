using UnityEngine;
using UnityEngine.UI;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Player")]
    public Camera playerCamera;
    public float interactionDistance = 3f;

    [Header("Interaction UI")]
    public Image interactionImage;

    public Color normalColor = Color.white;
    public Color interactableColor = Color.red;

    [Header("Request Generator")]
    public RequestGenerator requestGenerator;


    private void Start()
    {
        if (requestGenerator == null)
        {
            requestGenerator = FindObjectOfType<RequestGenerator>();
        }

        // Start with normal UI colour
        if (interactionImage != null)
        {
            interactionImage.color = normalColor;
        }
    }


    private void Update()
    {
        CheckForInteractable();

        if (Input.GetMouseButtonDown(0))
        {
            Interact();
        }
    }


    void CheckForInteractable()
    {
        Ray ray = playerCamera.ScreenPointToRay(
            Input.mousePosition
        );

        RaycastHit hit;

        bool canInteract = false;

        if (Physics.Raycast(
            ray,
            out hit,
            interactionDistance
        ))
        {
            // Check if object has the Interactable tag
            if (hit.collider.CompareTag("Interactable"))
            {
                canInteract = true;
            }
        }


        // Change UI colour
        if (interactionImage != null)
        {
            if (canInteract)
            {
                interactionImage.color = interactableColor;
            }
            else
            {
                interactionImage.color = normalColor;
            }
        }
    }


    void Interact()
    {
        Ray ray = playerCamera.ScreenPointToRay(
            Input.mousePosition
        );

        RaycastHit hit;

        if (!Physics.Raycast(
            ray,
            out hit,
            interactionDistance
        ))
        {
            return;
        }


        // Supply Order
        SupplyOrderInteraction supplyOrder =
            hit.collider.GetComponent<SupplyOrderInteraction>();

        if (supplyOrder != null)
        {
            supplyOrder.OpenSupplyOrder();
            return;
        }


        // Request Paper
        RequestPaper request =
            hit.collider.GetComponent<RequestPaper>();

        if (request != null)
        {
            request.AcceptRequest();

            if (requestGenerator != null)
            {
                requestGenerator.removeRequest(request);
            }

            return;
        }
    }
}