using UnityEngine;

public class TutorialHighlight : MonoBehaviour
{
    [Header("Highlight Objects")]
    public GameObject[] highlights;

    [Header("Debug Settings")]
    public bool enableDebugControls = false;

    private int currentHighlight = -1;


    private void Awake()
    {
        HideAllHighlights();
    }


    private void Update()
    {
        if (!enableDebugControls)
            return;

        if (Input.GetKeyDown(KeyCode.Alpha1))
            ShowHighlight(0);

        if (Input.GetKeyDown(KeyCode.Alpha2))
            ShowHighlight(1);

        if (Input.GetKeyDown(KeyCode.Alpha3))
            ShowHighlight(2);

        if (Input.GetKeyDown(KeyCode.Alpha4))
            ShowHighlight(3);

        if (Input.GetKeyDown(KeyCode.Alpha0))
            HideAllHighlights();
    }


    // --------------------------------------------------
    // SHOW SINGLE HIGHLIGHT
    // --------------------------------------------------

    public void ShowHighlight(int index)
    {
        ShowHighlights(new int[] { index });
    }


    // --------------------------------------------------
    // SHOW MULTIPLE HIGHLIGHTS
    // --------------------------------------------------

    public void ShowHighlights(int[] indices)
    {
        HideAllHighlights();

        if (highlights == null || indices == null)
            return;

        foreach (int index in indices)
        {
            if (
                index >= 0 &&
                index < highlights.Length &&
                highlights[index] != null
            )
            {
                highlights[index].SetActive(true);

                if (currentHighlight == -1)
                {
                    currentHighlight = index;
                }
            }
        }
    }


    // --------------------------------------------------
    // HIDE ALL HIGHLIGHTS
    // --------------------------------------------------

    public void HideAllHighlights()
    {
        if (highlights != null)
        {
            foreach (GameObject highlight in highlights)
            {
                if (highlight != null)
                    highlight.SetActive(false);
            }
        }

        currentHighlight = -1;
    }


    public int GetCurrentHighlight()
    {
        return currentHighlight;
    }
}