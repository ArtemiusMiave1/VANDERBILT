using UnityEngine;

public class TutorialHighlight : MonoBehaviour
{
    [Header("Highlight Objects")]
    public GameObject[] highlights;

    private int currentHighlight = -1;

    private void Start()
    {
        HideAllHighlights();
    }

    private void Update()
    {
        // Press 1, 2, 3, etc. to test highlights
        if (Input.GetKeyDown(KeyCode.Alpha1))
            ShowHighlight(0);

        if (Input.GetKeyDown(KeyCode.Alpha2))
            ShowHighlight(1);

        if (Input.GetKeyDown(KeyCode.Alpha3))
            ShowHighlight(2);

        if (Input.GetKeyDown(KeyCode.Alpha4))
            ShowHighlight(3);

        // Press 0 to hide all highlights
        if (Input.GetKeyDown(KeyCode.Alpha0))
            HideAllHighlights();
    }

    public void ShowHighlight(int index)
    {
        HideAllHighlights();

        if (index >= 0 && index < highlights.Length && highlights[index] != null)
        {
            highlights[index].SetActive(true);
            currentHighlight = index;
        }
    }

    public void HideAllHighlights()
    {
        foreach (GameObject highlight in highlights)
        {
            if (highlight != null)
                highlight.SetActive(false);
        }

        currentHighlight = -1;
    }
}
