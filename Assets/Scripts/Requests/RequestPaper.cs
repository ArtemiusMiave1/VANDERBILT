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

    [Header("Request Destination")]
    public Location targetLocation;

    [Header("Request State")]
    public bool activeRequest = false;
    public bool completed = false;
    public bool acceptedFromBoard = false;

    [Header("Deadline")]
    [Tooltip("Deadline in total GameClock minutes.")]
    private int deadlineMinutes;

    private bool deadlineSet = false;

    [Header("Sound")]
    public AudioSource audioSource;
    public AudioClip requestAcceptedSound;
    public AudioClip requestCompletedSound;

    // Visual indicator belonging to the assigned location
    private RequestVisualIndicator requestVisual;


    // --------------------------------------------------
    // DISPLAY REQUEST
    // --------------------------------------------------

    public void DisplayRequest(
        RequestData data)
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

        requestVisual = null;


        // ----------------------------------------------
        // TITLE
        // ----------------------------------------------

        if (titleText != null)
        {
            titleText.text =
                data.Title;
        }


        // ----------------------------------------------
        // FACTION
        // ----------------------------------------------

        if (factionText != null)
        {
            factionText.text =
                data.Faction;
        }


        // ----------------------------------------------
        // RESOURCE
        // ----------------------------------------------

        if (resourceText != null)
        {
            resourceText.text =
                data.RequestedResources +
                " x " +
                data.RequestedAmount;
        }


        // ----------------------------------------------
        // REWARD
        // ----------------------------------------------

        if (rewardText != null)
        {
            rewardText.text =
                "REWARD: " +
                data.Reward +
                " x " +
                data.RewardAmount;
        }


        // ----------------------------------------------
        // DIALOGUE
        // ----------------------------------------------

        if (dialogueText != null)
        {
            dialogueText.text =
                data.Dialogue;
        }


        // ----------------------------------------------
        // DESTINATION
        // ----------------------------------------------

        UpdateDestinationDisplay();


        // ----------------------------------------------
        // ARRIVAL TIME
        // ----------------------------------------------

        deadlineSet = false;

        UpdateArrivalDisplay();
    }


    // --------------------------------------------------
    // ASSIGN LOCATION
    // --------------------------------------------------

    public void AssignLocation(
        Location location)
    {
        if (location == null)
        {
            Debug.LogWarning(
                "RequestPaper: Tried to assign a null location."
            );

            return;
        }

        targetLocation =
            location;


        // ----------------------------------------------
        // GET VISUAL FROM LOCATION
        // ----------------------------------------------

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


    // --------------------------------------------------
    // DESTINATION DISPLAY
    // --------------------------------------------------

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


    // --------------------------------------------------
    // SET ESTIMATED ARRIVAL
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


        // ----------------------------------------------
        // ROUND UP TO NEXT 15 MINUTES
        // ----------------------------------------------

        int roundedTime =
            Mathf.CeilToInt(
                currentTime / 15f
            ) * 15;


        roundedTime %= 1440;


        // ----------------------------------------------
        // ADD 3 GAME HOURS
        // ----------------------------------------------

        deadlineMinutes =
            (roundedTime + 180) % 1440;

        deadlineSet = true;


        UpdateArrivalDisplay();
    }


    // --------------------------------------------------
    // UPDATE ARRIVAL DISPLAY
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
            hour >= 12
                ? "PM"
                : "AM";


        int displayHour =
            hour % 12;


        if (displayHour == 0)
        {
            displayHour = 12;
        }


        arrivalTimeText.text =
            string.Format(
                "ETA: {0}:{1:00} {2}",
                displayHour,
                minute,
                period
            );
    }


    // --------------------------------------------------
    // UPDATE DEADLINE
    // --------------------------------------------------

    private void Update()
    {
        if (!deadlineSet)
            return;

        if (completed)
            return;

        CheckDeadline();
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


        int minutesUntilDeadline =
            (deadlineMinutes - currentTime + 1440)
            % 1440;


        if (minutesUntilDeadline == 0)
        {
            DeadlineReached();
        }
    }


    // --------------------------------------------------
    // DEADLINE REACHED
    // --------------------------------------------------

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


    // --------------------------------------------------
    // ACCEPT REQUEST
    // --------------------------------------------------

    public void AcceptRequest()
    {
        if (completed)
            return;


        activeRequest = true;


        // Turn on visual at destination
        if (requestVisual != null)
        {
            requestVisual.SetActive();
        }


        if (audioSource != null &&
            requestAcceptedSound != null)
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


    // --------------------------------------------------
    // ACCEPT FROM CORKBOARD
    // --------------------------------------------------

    public void AcceptFromBoard()
    {
        if (completed)
            return;


        acceptedFromBoard = true;
        activeRequest = true;


        // Turn on visual at destination
        if (requestVisual != null)
        {
            requestVisual.SetActive();
        }


        if (audioSource != null &&
            requestAcceptedSound != null)
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


    // --------------------------------------------------
    // SHIP ARRIVES
    // --------------------------------------------------

    public void OnShipArrived(
        Location arrivedLocation)
    {
        if (arrivedLocation == null)
            return;


        if (targetLocation != arrivedLocation)
            return;


        if (completed)
            return;


        CompleteRequest();
    }


    // --------------------------------------------------
    // COMPLETE REQUEST
    // --------------------------------------------------

    public void CompleteRequest()
    {
        if (completed)
            return;


        completed = true;
        activeRequest = false;


        // Turn off visual at destination
        if (requestVisual != null)
        {
            requestVisual.SetInactive();
        }


        // Remove from location's active requests.
        if (targetLocation != null)
        {
            if (
                targetLocation.activeRequests != null &&
                targetLocation.activeRequests.Contains(
                    this
                )
            )
            {
                targetLocation.activeRequests.Remove(
                    this
                );
            }


            targetLocation.ClearHighlight();
        }


        // Play completion sound.
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
            "Request completed: " +
            GetRequestTitle()
        );
    }


    // --------------------------------------------------
    // GET REQUEST TITLE
    // --------------------------------------------------

    public string GetRequestTitle()
    {
        if (requestData == null)
            return gameObject.name;


        return requestData.Title;
    }


    // --------------------------------------------------
    // GET TARGET LOCATION
    // --------------------------------------------------

    public Location GetTargetLocation()
    {
        return targetLocation;
    }


    // --------------------------------------------------
    // GET DESTINATION ID
    // --------------------------------------------------

    public string GetDestinationID()
    {
        if (targetLocation == null)
            return "";


        return targetLocation.GetDisplayName();
    }


    // --------------------------------------------------
    // GET DEADLINE
    // --------------------------------------------------

    public int GetDeadlineMinutes()
    {
        return deadlineMinutes;
    }


    // --------------------------------------------------
    // IS COMPLETE
    // --------------------------------------------------

    public bool IsCompleted()
    {
        return completed;
    }


    // --------------------------------------------------
    // IS ACTIVE
    // --------------------------------------------------

    public bool IsActive()
    {
        return activeRequest;
    }
}