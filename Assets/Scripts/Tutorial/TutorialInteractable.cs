using UnityEngine;

public class TutorialInteractable : MonoBehaviour
{
    [Header("Interaction Settings")]
    public bool useMouseDown = true;


    // =========================================================
    // MOUSE CLICK
    // =========================================================

    private void OnMouseDown()
    {
        if (useMouseDown)
        {
            Interact();
        }
    }


    // =========================================================
    // REPORT INTERACTION
    // =========================================================

    public void Interact()
    {
        if (TutorialManager.Instance == null)
            return;

        if (!TutorialManager.Instance.IsTutorialActive())
            return;

        TutorialManager.Instance.ReportInteraction(this);
    }
}