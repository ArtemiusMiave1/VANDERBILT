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

    [Header("Estimated Arrival")]
    public TMP_Text arrivalTimeText;

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

    // The actual deadline for this individual request.
    private int deadlineMinutes;

    private bool deadlineSet = false;

    private void Awake()
    {
        corkBoard = FindObjectOfType<CorkBoard>();
    }

    private void Update()
    {
        if (!activeRequest)
            return;

        if (!deadlineSet)
            return;

        CheckDeadline();
    }

    // --------------------------------------------------
    // DISPLAY REQUEST
    // --------------------------------------------------

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

        rewardText.text =
            "Reward: " +
            data.Reward +
            " x" +
            data.RewardAmount;

        dialogueText.text =
            data.Dialogue;

        SetLocationIcon();
        SetResourceIcon();
    }
    public void AssignLocation(Location location)
    {
        targetLocation = location;

        SetLocationIcon();
    }
    // --------------------------------------------------
    // SET DEADLINE
    // --------------------------------------------------

    public void SetEstimatedArrival()
    {
        if (GameClock.Instance == null)
        {
            Debug.LogError(
                "RequestPaper: GameClock not found!"
            );

            return;
        }

        int currentTime =
            GameClock.Instance.GetTotalMinutes();

        // Round UP to the next 15-minute interval.
        int roundedTime =
            Mathf.CeilToInt(
                currentTime / 15f
            ) * 15;

        // If rounding reaches the next day.
        roundedTime %= 1440;

        // Add exactly 3 in-game hours.
        deadlineMinutes =
            (roundedTime + 180) % 1440;

        deadlineSet = true;

        UpdateArrivalDisplay();
    }

    // --------------------------------------------------
    // ARRIVAL DISPLAY
    // --------------------------------------------------

    private void UpdateArrivalDisplay()
    {
        if (arrivalTimeText == null)
            return;

        if (!deadlineSet)
        {
            arrivalTimeText.text =
                "EST. ARRIVAL: --:--";

            return;
        }

        int hour =
            deadlineMinutes / 60;

        int minute =
            deadlineMinutes % 60;

        string period =
            hour >= 12 ? "PM" : "AM";

        int displayHour =
            hour % 12;

        if (displayHour == 0)
            displayHour = 12;

        arrivalTimeText.text =
            string.Format(
                "ETA:\n{0}:{1:00} {2}",
                displayHour,
                minute,
                period
            );
    }

    // --------------------------------------------------
    // ACCEPT REQUEST
    // --------------------------------------------------

    public void AcceptRequest()
    {
        if (activeRequest)
            return;

        activeRequest = true;

        Debug.Log(
            "Accepted Request: " +
            request.Title
        );

        if (audioSource != null &&
            requestAcceptedSound != null)
        {
            audioSource.PlayOneShot(
                requestAcceptedSound
            );
        }

        if (targetLocation != null)
            targetLocation.Highlight();

        if (corkBoard != null)
            corkBoard.AddRequest(this);
    }

    public void AcceptFromBoard()
    {
        if (activeRequest)
            return;

        activeRequest = true;

        Debug.Log(
            "Accepted Request: " +
            request.Title
        );

        if (audioSource != null &&
            requestAcceptedSound != null)
        {
            audioSource.PlayOneShot(
                requestAcceptedSound
            );
        }

        if (targetLocation != null)
            targetLocation.Highlight();
    }

    // --------------------------------------------------
    // CHECK DEADLINE
    // --------------------------------------------------

    private void CheckDeadline()
    {
        if (GameClock.Instance == null)
            return;

        int currentTime =
            GameClock.Instance.GetTotalMinutes();

        if (currentTime == deadlineMinutes)
        {
            ExpireRequest();
        }
    }

    // --------------------------------------------------
    // REQUEST INFORMATION
    // --------------------------------------------------

    public string GetRequestTitle()
    {
        if (request == null)
            return "Unknown Request";

        return request.Title;
    }

    // --------------------------------------------------
    // SHIP ARRIVAL
    // --------------------------------------------------

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

    // --------------------------------------------------
    // COMPLETE REQUEST
    // --------------------------------------------------

    private void CompleteRequest(
        ShipCargo shipCargo
    )
    {
        Debug.Log(
            "Completed Request: " +
            request.Title
        );

        activeRequest = false;
        deadlineSet = false;

        if (audioSource != null &&
            requestCompletedSound != null)
        {
            audioSource.PlayOneShot(
                requestCompletedSound
            );
        }

        shipCargo.AddOrRemoveResource(
            request.RequestedResources,
            -request.RequestedAmount
        );

        shipCargo.AddOrRemoveResource(
            request.Reward,
            request.RewardAmount
        );

        if (targetLocation != null)
            targetLocation.ClearHighlight();

        if (corkBoard != null)
            corkBoard.RemoveRequest(this);

        Destroy(gameObject);
    }

    // --------------------------------------------------
    // EXPIRE REQUEST
    // --------------------------------------------------

    private void ExpireRequest()
    {
        Debug.Log(
            "Request expired: " +
            request.Title
        );

        activeRequest = false;
        deadlineSet = false;

        if (targetLocation != null)
            targetLocation.ClearHighlight();

        if (corkBoard != null)
            corkBoard.RemoveRequest(this);

        Destroy(gameObject);
    }

    // --------------------------------------------------
    // LOCATION ICON
    // --------------------------------------------------

    private void SetLocationIcon()
    {
        if (targetLocation == null)
            return;

        if (targetLocation.locationType == null)
            return;

        if (locationIcon == null)
            return;

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
                iconName
            );

            return;
        }

        locationIcon.material =
            iconMaterial;
    }

    // --------------------------------------------------
    // RESOURCE ICON
    // --------------------------------------------------

    private void SetResourceIcon()
    {
        if (resourceIcon == null)
            return;

        if (request == null)
            return;

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
                iconName
            );

            return;
        }

        resourceIcon.material =
            iconMaterial;
    }
}