using System;
using TMPro;
using UnityEngine;


// --------------------------------------------------
// TUTORIAL STEP
// --------------------------------------------------

public enum TutorialInteractionType
{
    SpecificObject,
    AnyRequest,
    PickUpRequest,
    PlaceRequestOnBoard,
    DiscardRequest,
    ChartRoute,
    OpenClipboard,
    CompleteDelivery,
    CollectPayment
}

[Serializable]
public class TutorialStep
{
    public string stepName;

    [TextArea(3, 6)]
    public string instruction;

    [Tooltip("Indices of all highlights to display.")]
    public int[] highlightIndices = new int[0];

    [Header("Progression")]
    public TutorialInteractionType interactionType =
        TutorialInteractionType.SpecificObject;

    public TutorialInteractable requiredObject;
    [Header("Route Settings")]

    public Location requiredDestination;
}


// --------------------------------------------------
// TUTORIAL MANAGER
// --------------------------------------------------

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;


    [Header("References")]
    public TutorialHighlight tutorialHighlight;


    [Header("Tutorial UI")]
    public GameObject tutorialPanel;

    public TMP_Text instructionText;

    public TMP_Text stepNameText;

    public TMP_Text progressText;


    [Header("Tutorial Settings")]
    public bool startOnAwake = true;

    public bool hidePanelOnCompletion = false;

    [TextArea(3, 6)]
    public string completionMessage =
        "Tutorial complete. You are ready to begin your journey.";


    [Header("Tutorial Steps")]
    public TutorialStep[] steps;


    [Header("Tutorial State")]
    [SerializeField]
    private int currentStep = -1;

    [SerializeField]
    private bool tutorialActive = false;

    [SerializeField]
    private bool tutorialCompleted = false;


    // --------------------------------------------------
    // AWAKE
    // --------------------------------------------------

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }


    // --------------------------------------------------
    // START
    // --------------------------------------------------

    private void Start()
    {
        if (startOnAwake)
        {
            StartTutorial();
        }
        else
        {
            if (tutorialPanel != null)
                tutorialPanel.SetActive(false);
        }
    }


    // --------------------------------------------------
    // START TUTORIAL
    // --------------------------------------------------

    public void StartTutorial()
    {
        if (steps == null || steps.Length == 0)
        {
            Debug.LogWarning(
                "TutorialManager: No tutorial steps assigned."
            );

            return;
        }

        currentStep = 0;

        tutorialActive = true;
        tutorialCompleted = false;

        if (tutorialPanel != null)
            tutorialPanel.SetActive(true);

        DisplayCurrentStep();
    }

    // =========================================================
    // REPORT DELIVERY COMPLETED
    // =========================================================

    public void ReportDeliveryCompleted(RequestPaper request)
    {
        if (!tutorialActive)
            return;

        if (request == null)
            return;

        if (!request.IsCompleted())
            return;

        if (
            currentStep < 0 ||
            currentStep >= steps.Length
        )
            return;

        TutorialStep step = steps[currentStep];

        if (step.interactionType !=
            TutorialInteractionType.CompleteDelivery)
            return;


        // =====================================================
        // CHECK REQUIRED DESTINATION
        // =====================================================

        if (
            step.requiredDestination != null &&
            request.GetTargetLocation() != step.requiredDestination
        )
        {
            return;
        }


        Debug.Log(
            "Tutorial: Delivery completed - " +
            request.GetRequestTitle()
        );

        NextStep();
    }
    // =========================================================
    // REPORT PAYMENT COLLECTED
    // =========================================================

    public void ReportPaymentCollected(int amount)
    {
        if (!tutorialActive)
            return;

        if (amount <= 0)
            return;

        if (
            currentStep < 0 ||
            currentStep >= steps.Length
        )
            return;

        TutorialStep step = steps[currentStep];

        if (step.interactionType !=
            TutorialInteractionType.CollectPayment)
            return;


        Debug.Log(
            "Tutorial: Payment collected - " +
            amount + " Gold"
        );

        NextStep();
    }

    // --------------------------------------------------
    // DISPLAY CURRENT STEP
    // --------------------------------------------------

    private void DisplayCurrentStep()
    {
        if (!tutorialActive)
            return;

        if (currentStep >= steps.Length)
        {
            CompleteTutorial();
            return;
        }

        TutorialStep step = steps[currentStep];


        // Update the instruction.
        if (instructionText != null)
        {
            instructionText.text =
                step.instruction;
        }


        // Update the step name.
        if (stepNameText != null)
        {
            stepNameText.text =
                step.stepName;
        }


        // Update the progress.
        if (progressText != null)
        {
            progressText.text =
                (currentStep + 1) +
                " / " +
                steps.Length;
        }


        // Update the highlighted object.
        if (tutorialHighlight != null)
        {
            tutorialHighlight.ShowHighlights(
                step.highlightIndices
            );
        }


        Debug.Log(
            "Tutorial Step: " +
            step.stepName
        );
    }

    // =========================================================
    // REPORT CLIPBOARD OPENED
    // =========================================================

    public void ReportClipboardOpened()
    {
        if (!tutorialActive)
            return;

        if (
            currentStep < 0 ||
            currentStep >= steps.Length
        )
            return;

        TutorialStep step = steps[currentStep];

        if (step.interactionType !=
            TutorialInteractionType.OpenClipboard)
            return;

        Debug.Log(
            "Tutorial Step Completed: " +
            step.stepName
        );

        NextStep();
    }

    public bool CanChartToLocation(Location location)
    {
        if (!tutorialActive)
            return true;

        if (currentStep < 0 ||
            currentStep >= steps.Length)
            return true;

        TutorialStep step = steps[currentStep];

        if (step.interactionType !=
            TutorialInteractionType.ChartRoute)
            return true;

        return location != null &&
               location == step.requiredDestination;
    }

    public void ReportRouteCharted(Location location)
    {
        if (!tutorialActive)
            return;

        if (currentStep < 0 ||
            currentStep >= steps.Length)
            return;

        TutorialStep step = steps[currentStep];

        if (step.interactionType !=
            TutorialInteractionType.ChartRoute)
            return;

        if (location == null ||
            location != step.requiredDestination)
            return;

        NextStep();
    }

    // --------------------------------------------------
    // REPORT INTERACTION
    // --------------------------------------------------

    public void ReportInteraction(
    TutorialInteractable interactedObject)
    {
        if (!tutorialActive)
            return;

        if (interactedObject == null)
            return;

        if (
            currentStep < 0 ||
            currentStep >= steps.Length
        )
            return;

        TutorialStep step = steps[currentStep];

        // Only check assigned objects for this interaction type.
        if (step.interactionType !=
            TutorialInteractionType.SpecificObject)
            return;

        if (step.requiredObject != interactedObject)
            return;

        Debug.Log(
            "Tutorial Step Completed: " +
            step.stepName
        );

        NextStep();
    }

    // --------------------------------------------------
    // REPORT REQUEST INTERACTION
    // --------------------------------------------------

    public void ReportRequestInteraction(
    RequestPaper request,
    TutorialInteractionType interactionType =
        TutorialInteractionType.AnyRequest)
    {
        if (!tutorialActive)
            return;

        if (request == null)
            return;

        if (
            currentStep < 0 ||
            currentStep >= steps.Length
        )
            return;

        TutorialStep step = steps[currentStep];

        if (step.interactionType ==
            TutorialInteractionType.SpecificObject)
            return;

        if (
            step.interactionType != interactionType &&
            step.interactionType != TutorialInteractionType.AnyRequest
        )
            return;

        Debug.Log(
            "Tutorial Step Completed: " +
            step.stepName
        );

        NextStep();
    }

    // --------------------------------------------------
    // NEXT STEP
    // --------------------------------------------------

    public void NextStep()
    {
        if (!tutorialActive)
            return;

        currentStep++;

        if (currentStep >= steps.Length)
        {
            CompleteTutorial();
            return;
        }

        DisplayCurrentStep();
    }


    // --------------------------------------------------
    // COMPLETE TUTORIAL
    // --------------------------------------------------

    public void CompleteTutorial()
    {
        tutorialActive = false;
        tutorialCompleted = true;

        if (tutorialHighlight != null)
        {
            tutorialHighlight.HideAllHighlights();
        }

        if (instructionText != null)
        {
            instructionText.text =
                completionMessage;
        }

        if (stepNameText != null)
        {
            stepNameText.text =
                "Tutorial Complete";
        }

        if (progressText != null)
        {
            progressText.text =
                steps.Length +
                " / " +
                steps.Length;
        }

        if (
            hidePanelOnCompletion &&
            tutorialPanel != null
        )
        {
            tutorialPanel.SetActive(false);
        }

        Debug.Log("Tutorial completed!");
    }


    // --------------------------------------------------
    // SKIP TUTORIAL
    // --------------------------------------------------

    public void SkipTutorial()
    {
        tutorialActive = false;

        if (tutorialHighlight != null)
        {
            tutorialHighlight.HideAllHighlights();
        }

        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(false);
        }
    }


    // --------------------------------------------------
    // GETTERS
    // --------------------------------------------------

    public bool IsTutorialActive()
    {
        return tutorialActive;
    }

    public bool IsTutorialCompleted()
    {
        return tutorialCompleted;
    }

    public int GetCurrentStep()
    {
        return currentStep;
    }


    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}