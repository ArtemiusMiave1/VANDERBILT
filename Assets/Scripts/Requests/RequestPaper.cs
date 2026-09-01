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

    [Header("Timer")]
    public TMP_Text timerText;

    [Header("Icons")]
    public Renderer locationIcon;
    public Renderer resourceIcon;

    [Header("Request Location")]
    public Location targetLocation;

    [Header("Cork Board")]
    public CorkBoard corkBoard;

    // Request data
    private RequestData request;

    // Is this request currently active?
    public bool activeRequest = false;

    // Timer
    private float timeRemaining;
    private bool timerRunning = false;


    private void Awake()
    {
        corkBoard = FindObjectOfType<CorkBoard>();
    }


    private void Update()
    {
        if (!activeRequest || !timerRunning)
            return;

        // Count down
        timeRemaining -= Time.deltaTime;

        // Update UI
        UpdateTimerDisplay();

        // Request expired
        if (timeRemaining <= 0f)
        {
            timeRemaining = 0f;
            timerRunning = false;

            ExpireRequest();
        }
    }


    public void DisplayRequest(RequestData data)
    {
        request = data;

        titleText.text = data.Title;SetLocationIcon();

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

        // Show the time limit before the request is accepted
        timeRemaining = data.TimeLimit;
        SetResourceIcon();
        UpdateTimerDisplay();
    }


    public void AssignLocation(Location location)
    {
        targetLocation = location;

        // Don't highlight yet.
        // The request isn't active until accepted.
        SetLocationIcon();
    }


    // Called when the player clicks the request
    public void AcceptRequest()
    {
        if (activeRequest)
            return;

        activeRequest = true;

        Debug.Log(
            "Accepted Request: " +
            request.Title
        );

        // Start timer
        timeRemaining = request.TimeLimit;
        timerRunning = true;

        UpdateTimerDisplay();

        // Highlight destination
        if (targetLocation != null)
        {
            targetLocation.Highlight();
        }

        // Move request onto corkboard
        if (corkBoard != null)
        {
            corkBoard.AddRequest(this);
        }
        else
        {
            Debug.LogWarning(
                "No CorkBoard found!"
            );
        }
    }


    private void UpdateTimerDisplay()
    {
        if (timerText == null)
            return;

        int minutes =
            Mathf.FloorToInt(timeRemaining / 60f);

        int seconds =
            Mathf.FloorToInt(timeRemaining % 60f);

        timerText.text =
            string.Format(
                "{0:00}:{1:00}",
                minutes,
                seconds
            );
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

        // Ignore wrong location
        if (location != targetLocation)
            return;


        ShipCargo shipCargo =
            FindObjectOfType<ShipCargo>();

        if (shipCargo == null)
        {
            Debug.LogError(
                "No ShipCargo found in the scene!"
            );

            return;
        }


        int cargoAmount =
            shipCargo.GetResourceAmount(
                request.RequestedResources
            );


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


    private void CompleteRequest(
        ShipCargo shipCargo
    )
    {
        Debug.Log(
            "Completed Request: " +
            request.Title
        );

        // Stop timer
        timerRunning = false;
        activeRequest = false;


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


        // Remove highlight
        if (targetLocation != null)
        {
            targetLocation.ClearHighlight();
        }


        // Remove from corkboard
        if (corkBoard != null)
        {
            corkBoard.RemoveRequest(this);
        }


        // Destroy request
        Destroy(gameObject);
    }


    private void ExpireRequest()
    {
        Debug.Log(
            "Request expired: " +
            request.Title
        );

        activeRequest = false;


        // Remove location highlight
        if (targetLocation != null)
        {
            targetLocation.ClearHighlight();
        }


        // Remove from corkboard
        if (corkBoard != null)
        {
            corkBoard.RemoveRequest(this);
        }


        // Destroy request
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
