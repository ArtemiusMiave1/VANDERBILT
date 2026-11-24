using UnityEngine;

public class TutorialHighlight : MonoBehaviour
{
    [Header("Highlight Objects")]
    public GameObject[] highlights;

    private void Start()
    {
        HideAllHighlights();
    }

    // Show a specific highlight using its array index
    public void ShowHighlight(int index)
    {
        // Turn off all existing highlights first
        HideAllHighlights();

        // Check that the index is valid
        if (index >= 0 && index < highlights.Length && highlights[index] != null)
        {
            highlights[index].SetActive(true);
        }
    }

    // Turn off all highlights
    public void HideAllHighlights()
    {
        foreach (GameObject highlight in highlights)
        {
            if (highlight != null)
            {
                highlight.SetActive(false);
            }
        }
    }
}
