using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public Camera playerCamera;

    public float interactionDistance = 3f;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = playerCamera.ScreenPointToRay(
                Input.mousePosition
            );

            RaycastHit hit;

            if (Physics.Raycast(
                ray,
                out hit,
                interactionDistance
            ))
            {
                SupplyOrderInteraction supplyOrder =
                    hit.collider.GetComponent<SupplyOrderInteraction>();
                RequestPaper request = 
                    hit.collider.GetComponent<RequestPaper>();

                if (supplyOrder != null)
                {
                    supplyOrder.OpenSupplyOrder();
                }
                if (request != null)
                {
                    print(request.name);
                    request.AcceptRequest();
                }
            }
        }
    }
}