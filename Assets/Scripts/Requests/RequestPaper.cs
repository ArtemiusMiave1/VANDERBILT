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

    [Header("Icon Objects")]
    public Renderer locationIcon;
    public Renderer resourceIcon;

    [Header("Request Location")]
    public Location targetLocation;

    private RequestData request;

    [Header("Cork Board")]
    public CorkBoard corkBoard;

    // Is this request currently active?
    public bool activeRequest = false;

    private void Awake()
    {
        corkBoard = FindObjectOfType<CorkBoard>();
    }
    public void DisplayRequest(RequestData data)
    {
        request = data;

        // Text
        titleText.text = data.Title;

        factionText.text =
            "Faction: " + data.Faction;

        resourceText.text =
            "Requested " +
            data.RequestedResources +
            ": " +
            data.RequestedAmount;

        rewardText.text =
            "Reward: " +
            data.Reward +
            " x" +
            data.RewardAmount;

        dialogueText.text =
            data.Dialogue;


        // Resource icon
        SetResourceIcon();


    }


    public void AssignLocation(Location location)
    {
        targetLocation = location;

        // Location icon
        SetLocationIcon();
        // Do NOT highlight the location yet.
        // The request isn't active until the player accepts it.

        // Set the icon based on the location type
    }


    // Called when the player clicks the request paper
    public void AcceptRequest()
    {
        // Don't accept it twice
        if (activeRequest)
            return;

        activeRequest = true;

        Debug.Log("Accepted Request: " + request.Title);

        // Highlight destination
        if (targetLocation != null)
        {
            targetLocation.Highlight();
        }

        // Add request to cork board
        if (corkBoard != null)
        {
            corkBoard.AddRequest(this);
        }
        else
        {
            Debug.LogWarning(
                "No CorkBoard assigned to " +
                request.Title
            );
        }

        // Don't hide it anymore
        // gameObject.SetActive(false);
    }
    public string GetRequestTitle()
    {
        if (request == null)
            return "Unknown Request";

        return request.Title;
    }

    private void SetLocationIcon()
    {
        if (targetLocation == null)
        {
            Debug.LogWarning("Request has no target location.");
            return;
        }

        if (targetLocation.locationType == null)
        {
            Debug.LogWarning(
                "Target location has no LocationType assigned!"
            );

            return;
        }

        if (locationIcon == null)
        {
            Debug.LogWarning(
                "Location Icon Renderer has not been assigned!"
            );

            return;
        }


        // Get the location type name
        string iconName = targetLocation.locationType.Name;

        // Load material from Resources/Icons
        Material iconMaterial =
            Resources.Load<Material>("Icons/" + iconName);


        if (iconMaterial == null)
        {
            Debug.LogError(
                "Could not find icon material: " +
                iconName +
                " in Resources/Icons/"
            );

            return;
        }


        // Apply material to the plane
        locationIcon.material = iconMaterial;

        Debug.Log(
            "Set request icon to: " +
            iconName
        );
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


        // Remove location highlight
        if (targetLocation != null)
        {
            targetLocation.ClearHighlight();
        }


        // Remove from cork board
        if (corkBoard != null)
        {
            corkBoard.RemoveRequest(this);
        }


        // Destroy request paper
        Destroy(gameObject);
    }

    private void SetResourceIcon()
    {
        if (resourceIcon == null)
        {
            Debug.LogWarning(
                "Resource Icon Renderer has not been assigned!"
            );

            return;
        }


        if (request == null)
        {
            Debug.LogWarning(
                "Cannot set resource icon because request is null."
            );

            return;
        }


        // Get resource name from request
        string iconName =
            request.RequestedResources;


        // Load material from Resources/Icons
        Material iconMaterial =
            Resources.Load<Material>(
                "ResourceIcons/" + iconName
            );


        if (iconMaterial == null)
        {
            Debug.LogWarning(
                "Could not find resource icon: " +
                iconName +
                " in Resources/Icons/"
            );

            return;
        }


        // Apply material to plane
        resourceIcon.material =
            iconMaterial;
    }
}