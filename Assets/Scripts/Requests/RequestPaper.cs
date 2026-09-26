using TMPro;
using UnityEngine;

public class RequestPaper : MonoBehaviour
{
    [Header("Request Data")]
    public RequestData requestData;

    [Header("UI")]
    public TMP_Text titleText;
    public TMP_Text factionText;
    public TMP_Text resourceText;
    public TMP_Text rewardText;
    public TMP_Text dialogueText;
    public TMP_Text arrivalTimeText;
    public TMP_Text destinationText;

    [Header("Delivered Stamp")]
    [Tooltip("Image/GameObject shown when the request is successfully delivered.")]
    public GameObject deliveredStamp;

    [Header("Request Destination")]
    public Location targetLocation;

    [Header("Request State")]
    public bool activeRequest = false;
    public bool completed = false;
    public bool acceptedFromBoard = false;

    [Header("Payment")]
    [Tooltip("Gold waiting to be collected at Vanderbilt.")]
    public int pendingGold = 0;

    [Header("Deadline")]
    [Tooltip("Deadline in total GameClock minutes.")]
    private int deadlineMinutes;

    private bool deadlineSet = false;

    [Header("Sound")]
    public AudioSource audioSource;
    public AudioClip requestAcceptedSound;
    public AudioClip requestCompletedSound;

    private RequestVisualIndicator requestVisual;


    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        if (deliveredStamp != null)
        {
            deliveredStamp.SetActive(false);
        }
    }


    // =========================================================
    // DISPLAY REQUEST
    // =========================================================

    public void DisplayRequest(
        RequestData data
    )
    {
        if (data == null)
        {
            Debug.LogError(
                "RequestPaper: RequestData is null!"
            );

            return;
        }

        requestData = data;

        completed = false;
        activeRequest = false;
        acceptedFromBoard = false;

        pendingGold = 0;

        requestVisual = null;

        if (deliveredStamp != null)
        {
            deliveredStamp.SetActive(false);
        }

        if (titleText != null)
            titleText.text = data.Title;

        if (factionText != null)
            factionText.text = data.Faction;

        if (resourceText != null)
        {
            resourceText.text =
                data.RequestedResources +
                " x " +
                data.RequestedAmount;
        }

        if (rewardText != null)
        {
            rewardText.text =
                "REWARD: " +
                data.Reward +
                " x " +
                data.RewardAmount;
        }

        if (dialogueText != null)
            dialogueText.text = data.Dialogue;

        UpdateDestinationDisplay();

        deadlineSet = false;

        UpdateArrivalDisplay();
    }


    // =========================================================
    // ASSIGN LOCATION
    // =========================================================

    public void AssignLocation(
        Location location
    )
    {
        if (location == null)
        {
            Debug.LogWarning(
                "RequestPaper: Tried to assign a null location."
            );

            return;
        }

        targetLocation = location;

        requestVisual =
            targetLocation.GetComponentInChildren<RequestVisualIndicator>(
                true
            );

        if (requestVisual == null)
        {
            Debug.LogWarning(
                "RequestPaper: No RequestVisualIndicator " +
                "found on location " +
                targetLocation.GetDisplayName()
            );
        }

        UpdateDestinationDisplay();

        Debug.Log(
            "Request assigned to " +
            location.GetDisplayName()
        );
    }


    private void UpdateDestinationDisplay()
    {
        if (destinationText == null)
            return;

        if (targetLocation == null)
        {
            destinationText.text =
                "DESTINATION: --";

            return;
        }

        destinationText.text =
            "DESTINATION: " +
            targetLocation.GetDisplayName();
    }


    // =========================================================
    // ARRIVAL TIME
    // =========================================================

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

        int roundedTime =
            Mathf.CeilToInt(
                currentTime / 15f
            ) * 15;

        roundedTime %= 1440;

        deadlineMinutes =
            (roundedTime + 180) % 1440;

        deadlineSet = true;

        UpdateArrivalDisplay();
    }


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
            hour >= 12
                ? "PM"
                : "AM";

        int displayHour =
            hour % 12;

        if (displayHour == 0)
            displayHour = 12;

        arrivalTimeText.text =
            string.Format(
                "EST. ARRIVAL: {0}:{1:00} {2}",
                displayHour,
                minute,
                period
            );
    }


    // =========================================================
    // DEADLINE
    // =========================================================

    private void Update()
    {
        if (!deadlineSet)
            return;

        if (completed)
            return;

        CheckDeadline();
    }


    private void CheckDeadline()
    {
        if (GameClock.Instance == null)
            return;

        int currentTime =
            GameClock.Instance.GetTotalMinutes();

        int minutesUntilDeadline =
            (deadlineMinutes - currentTime + 1440)
            % 1440;

        if (minutesUntilDeadline == 0)
            DeadlineReached();
    }


    private void DeadlineReached()
    {
        if (completed)
            return;

        Debug.Log(
            "Request deadline reached: " +
            GetRequestTitle()
        );

        // Add failure/consequence system here later.
    }


    // =========================================================
    // ACCEPT REQUEST
    // =========================================================

    public void AcceptRequest()
    {
        if (completed)
            return;

        activeRequest = true;

        if (requestVisual != null)
            requestVisual.SetActive();

        if (
            audioSource != null &&
            requestAcceptedSound != null
        )
        {
            audioSource.PlayOneShot(
                requestAcceptedSound
            );
        }

        Debug.Log(
            "Request accepted: " +
            GetRequestTitle()
        );
    }


    public void AcceptFromBoard()
    {
        if (completed)
            return;

        acceptedFromBoard = true;
        activeRequest = true;

        if (requestVisual != null)
            requestVisual.SetActive();

        if (
            audioSource != null &&
            requestAcceptedSound != null
        )
        {
            audioSource.PlayOneShot(
                requestAcceptedSound
            );
        }

        Debug.Log(
            "Request accepted from corkboard: " +
            GetRequestTitle()
        );
    }


    // =========================================================
    // ARRIVE AT REQUEST DESTINATION
    // =========================================================

    public void OnShipArrived(
        Location arrivedLocation
    )
    {
        if (arrivedLocation == null)
            return;

        if (targetLocation != arrivedLocation)
            return;

        if (completed)
            return;

        CompleteRequest();
    }


    // =========================================================
    // COMPLETE REQUEST
    // =========================================================

    public void CompleteRequest()
    {
        if (completed)
            return;

        if (requestData == null)
        {
            Debug.LogError(
                "RequestPaper: Cannot complete request. " +
                "RequestData is missing."
            );

            return;
        }

        ShipCargo shipCargo =
            FindObjectOfType<ShipCargo>();

        if (shipCargo == null)
        {
            Debug.LogError(
                "RequestPaper: ShipCargo not found!"
            );

            return;
        }

        /*
         * -----------------------------------------------------
         * DELIVER THE REQUESTED RESOURCE
         * -----------------------------------------------------
         *
         * Gold is deliberately NOT handled here.
         *
         * Example:
         *
         * Food request = 20
         *
         * Cargo:
         * Food 100 -> Food 80
         */

        string requestedResource =
            requestData.RequestedResources;

        int requestedAmount =
            requestData.RequestedAmount;

        if (
            !string.IsNullOrEmpty(
                requestedResource
            ) &&
            requestedAmount > 0
        )
        {
            /*
             * Gold is a reward, not a delivered cargo resource.
             */
            if (
                !requestedResource.Equals(
                    "gold",
                    System.StringComparison.OrdinalIgnoreCase
                )
            )
            {
                int currentAmount =
                    shipCargo.GetResourceAmount(
                        requestedResource
                    );

                if (currentAmount < requestedAmount)
                {
                    Debug.LogWarning(
                        "RequestPaper: Not enough " +
                        requestedResource +
                        " to complete request."
                    );

                    return;
                }

                shipCargo.AddOrRemoveResource(
                    requestedResource,
                    -requestedAmount
                );

                Debug.Log(
                    "Delivered " +
                    requestedAmount +
                    " " +
                    requestedResource +
                    " for request " +
                    GetRequestTitle()
                );
            }
        }


        // -----------------------------------------------------
        // REQUEST COMPLETED
        // -----------------------------------------------------

        completed = true;
        activeRequest = false;


        // -----------------------------------------------------
        // STORE GOLD AS PENDING
        // -----------------------------------------------------

        pendingGold =
            requestData.RewardAmount;

        Debug.Log(
            "Request completed. " +
            pendingGold +
            " gold is waiting at Vanderbilt."
        );


        // -----------------------------------------------------
        // SHOW DELIVERED STAMP
        // -----------------------------------------------------

        if (deliveredStamp != null)
        {
            deliveredStamp.SetActive(true);
        }


        // -----------------------------------------------------
        // LOCATION VISUAL
        // -----------------------------------------------------

        if (requestVisual != null)
        {
            requestVisual.SetInactive();
        }


        if (targetLocation != null)
        {
            if (
                targetLocation.activeRequests != null &&
                targetLocation.activeRequests.Contains(this)
            )
            {
                targetLocation.activeRequests.Remove(this);
            }

            targetLocation.ClearHighlight();
        }


        // -----------------------------------------------------
        // SOUND
        // -----------------------------------------------------

        if (
            audioSource != null &&
            requestCompletedSound != null
        )
        {
            audioSource.PlayOneShot(
                requestCompletedSound
            );
        }

        Debug.Log(
            "Request delivered: " +
            GetRequestTitle()
        );
    }


    // =========================================================
    // VANDERBILT PAYMENT
    // =========================================================

    public int CollectGold()
    {
        if (pendingGold <= 0)
            return 0;

        int goldToCollect =
            pendingGold;

        pendingGold = 0;

        Debug.Log(
            "Collected " +
            goldToCollect +
            " gold from request: " +
            GetRequestTitle()
        );

        return goldToCollect;
    }


    public bool HasPendingGold()
    {
        return pendingGold > 0;
    }


    // =========================================================
    // GETTERS
    // =========================================================

    public string GetRequestTitle()
    {
        if (requestData == null)
            return gameObject.name;

        return requestData.Title;
    }


    public Location GetTargetLocation()
    {
        return targetLocation;
    }


    public string GetDestinationID()
    {
        if (targetLocation == null)
            return "";

        return targetLocation.GetDisplayName();
    }


    public int GetDeadlineMinutes()
    {
        return deadlineMinutes;
    }


    public bool IsCompleted()
    {
        return completed;
    }


    public bool IsActive()
    {
        return activeRequest;
    }


    public int GetPendingGold()
    {
        return pendingGold;
    }
}