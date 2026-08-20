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

    // Is this request currently active?
    private bool activeRequest = false;


    public void DisplayRequest(RequestData data)
    {
        request = data;

        titleText.text = data.Title;
        factionText.text = "Faction: " + data.Faction;
        resourceText.text =
            "Requested " +
            data.RequestedResources +
            ": " +
            data.RequestedAmount;

        rewardText.text = "Reward: " + data.Reward;
        dialogueText.text = data.Dialogue;
    }


    public void AssignLocation(Location location)
    {
        targetLocation = location;

        // Do NOT highlight the location yet.
        // The request isn't active until the player accepts it.
    }


    // Called when the player clicks the request paper
    public void AcceptRequest()
    {
        // Don't accept it twice
        if (activeRequest)
            return;

        activeRequest = true;

        Debug.Log("Accepted Request: " + request.Title);

        // Highlight the destination
        if (targetLocation != null)
        {
            targetLocation.Highlight();
        }

        // Hide the paper from the request board
        gameObject.SetActive(false);
    }


    public void OnShipArrived(Location location)
    {
        // Ignore inactive requests
        if (!activeRequest)
            return;

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
        int cargoAmount =
            shipCargo.GetResourceAmount(request.RequestedResources);


        // Check if there is enough
        if (cargoAmount >= request.RequestedAmount)
        {
            CompleteRequest(shipCargo);
        }
        else
        {
            Debug.Log(
                "Not enough " +
                request.RequestedResources +
                "! Required: " +
                request.RequestedAmount +
                ", Have: " +
                cargoAmount
            );
        }
    }


    private void CompleteRequest(ShipCargo shipCargo)
    {
        Debug.Log(
            "Completed Request: " +
            request.Title
        );


        // Remove requested resources
        shipCargo.AddOrRemoveResource(
            request.RequestedResources,
            -request.RequestedAmount
        );


        // Give reward
        shipCargo.AddOrRemoveResource(
            request.Reward,
            request.RewardAmount
        );


        // Remove yellow highlight
        if (targetLocation != null)
        {
            targetLocation.ClearHighlight();
        }


        // Remove request
        Destroy(gameObject);
    }
}