using TMPro;
using UnityEngine;

public class RequestPaper : MonoBehaviour
{
    [Header("UI Text")]
    public TMP_Text titleText;
    public TMP_Text factionText;
    public TMP_Text resourceText;
    public TMP_Text rewardText;
    public TMP_Text dialogueText;

    [Header("Request Location")]
    public Location targetLocation;

    private RequestData request;


    public void DisplayRequest(RequestData data)
    {
        request = data;

        titleText.text = data.Title;
        factionText.text = "Faction: " + data.Faction;
        resourceText.text = "Requested " + data.RequestedResources + ": " + data.RequestedAmount;
        rewardText.text = "Reward: " + data.Reward;
        dialogueText.text = data.Dialogue;
    }


    public void AssignLocation(Location location)
    {
        targetLocation = location;

        targetLocation.Highlight();
    }


    public void OnShipArrived(Location location)
    {
        // Ignore if ship did not arrive at this request's location
        if (location != targetLocation)
            return;

        // Find the ship
        ShipCargo shipCargo = FindObjectOfType<ShipCargo>();

        if (shipCargo == null)
        {
            Debug.LogError("No ShipCargo found in the scene!");
            return;
        }

        // Check how much of the requested resource the ship has
        int cargoAmount = shipCargo.GetResourceAmount(request.RequestedResources);

        // Check if there is enough
        if (cargoAmount >= request.RequestedAmount)
        {
            CompleteRequest(shipCargo);
        }
        else
        {
            Debug.Log(
                "Not enough " + request.RequestedResources +
                "! Required: " + request.RequestedAmount +
                ", Have: " + cargoAmount
            );
        }
    }


    void CompleteRequest(ShipCargo shipCargo)
    {
        Debug.Log("Completed Request: " + request.Title);

        // Remove the requested resources from the ship
        shipCargo.AddOrRemoveResource(
            request.RequestedResources,
            -request.RequestedAmount
        );
        shipCargo.AddOrRemoveResource(
            request.Reward,
            request.RewardAmount
        );

        // Remove yellow highlight
        if (targetLocation != null)
        {
            targetLocation.ClearHighlight();
        }

        // Remove request paper
        Destroy(gameObject);
    }
}