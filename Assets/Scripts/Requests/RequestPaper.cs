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

    [Header("Sound")]
    public AudioSource audioSource;
    public AudioClip requestAcceptedSound;
    public AudioClip requestCompletedSound;

    private RequestData request;

    public bool activeRequest = false;

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

        timeRemaining -= Time.deltaTime;

        UpdateTimerDisplay();

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

        timeRemaining =
            data.TimeLimit;

        SetLocationIcon();
        SetResourceIcon();
        UpdateTimerDisplay();
    }

    public void AssignLocation(Location location)
    {
        targetLocation = location;

        SetLocationIcon();
    }

    // ---------------------------------------------------------
    // OLD DIRECT ACCEPT METHOD
    // ---------------------------------------------------------
    // This can still exist, but with the new pickup system
    // the player should normally use E to pick up the paper
    // and E again while looking at the corkboard.
    public void AcceptRequest()
    {
        if (activeRequest)
            return;

        activeRequest = true;

        Debug.Log(
            "Accepted Request: " +
            request.Title
        );

        timeRemaining =
            request.TimeLimit;

        timerRunning = true;

        if (audioSource != null &&
            requestAcceptedSound != null)
        {
            audioSource.PlayOneShot(
                requestAcceptedSound
            );
        }

        UpdateTimerDisplay();

        if (targetLocation != null)
        {
            targetLocation.Highlight();
        }

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
        //soundManager.PlaySFX(soundManager.Request);
    }

    // ---------------------------------------------------------
    // ACCEPT FROM CORK BOARD
    // ---------------------------------------------------------

    public void AcceptFromBoard()
    {
        if (activeRequest)
            return;

        activeRequest = true;

        Debug.Log(
            "Accepted Request: " +
            request.Title
        );

        timeRemaining =
            request.TimeLimit;

        timerRunning = true;

        if (audioSource != null &&
            requestAcceptedSound != null)
        {
            audioSource.PlayOneShot(
                requestAcceptedSound
            );
        }

        UpdateTimerDisplay();

        if (targetLocation != null)
        {
            targetLocation.Highlight();
        }
    }

    // ---------------------------------------------------------
    // TIMER
    // ---------------------------------------------------------

    private void UpdateTimerDisplay()
    {
        if (timerText == null)
            return;

        int minutes =
            Mathf.FloorToInt(
                timeRemaining / 60f
            );

        int seconds =
            Mathf.FloorToInt(
                timeRemaining % 60f
            );

        timerText.text =
            string.Format(
                "{0:00}:{1:00}",
                minutes,
                seconds
            );
    }

    // ---------------------------------------------------------
    // GET REQUEST TITLE
    // ---------------------------------------------------------

    public string GetRequestTitle()
    {
        if (request == null)
            return "Unknown Request";

        return request.Title;
    }

    // ---------------------------------------------------------
    // LOCATION ICON
    // ---------------------------------------------------------

    private void SetLocationIcon()
    {
        if (targetLocation == null)
        {
            Debug.LogWarning(
                "Request has no target location."
            );

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

        string iconName =
            targetLocation.locationType.Name;

        Material iconMaterial =
            Resources.Load<Material>(
                "Icons/" + iconName
            );

        if (iconMaterial == null)
        {
            Debug.LogError(
                "Could not find icon material: " +
                iconName +
                " in Resources/Icons/"
            );

            return;
        }

        locationIcon.material =
            iconMaterial;
    }

    // ---------------------------------------------------------
    // SHIP ARRIVES
    // ---------------------------------------------------------

    public void OnShipArrived(Location location)
    {
        if (!activeRequest)
            return;

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

    // ---------------------------------------------------------
    // COMPLETE REQUEST
    // ---------------------------------------------------------

    private void CompleteRequest(
        ShipCargo shipCargo
    )
    {
        Debug.Log(
            "Completed Request: " +
            request.Title
        );

        timerRunning = false;
        activeRequest = false;

        if (audioSource != null &&
            requestCompletedSound != null)
        {
            audioSource.PlayOneShot(
                requestCompletedSound
            );
        }

        // Remove requested resources
        shipCargo.AddOrRemoveResource(
            request.RequestedResources,
            -request.RequestedAmount
        );

        // Add reward
        shipCargo.AddOrRemoveResource(
            request.Reward,
            request.RewardAmount
        );

        if (targetLocation != null)
        {
            targetLocation.ClearHighlight();
        }

        if (corkBoard != null)
        {
            corkBoard.RemoveRequest(this);
        }

        Destroy(gameObject);
    }

    // ---------------------------------------------------------
    // REQUEST EXPIRES
    // ---------------------------------------------------------

    private void ExpireRequest()
    {
        Debug.Log(
            "Request expired: " +
            request.Title
        );

        activeRequest = false;

        if (targetLocation != null)
        {
            targetLocation.ClearHighlight();
        }

        if (corkBoard != null)
        {
            corkBoard.RemoveRequest(this);
        }

        Destroy(gameObject);
    }

    // ---------------------------------------------------------
    // RESOURCE ICON
    // ---------------------------------------------------------

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

        string iconName =
            request.RequestedResources;

        Material iconMaterial =
            Resources.Load<Material>(
                "ResourceIcons/" + iconName
            );

        if (iconMaterial == null)
        {
            Debug.LogWarning(
                "Could not find resource icon: " +
                iconName +
                " in Resources/ResourceIcons/"
            );

            return;
        }

        resourceIcon.material =
            iconMaterial;
    }
}