using UnityEngine;

public class TutorialBookPrompt : MonoBehaviour
{
    [Header("Prompt Objects")]
    public GameObject arrow;
    public GameObject highlight;

    private bool promptActive = true;

    private void Start()
    {
        ShowPrompt();
    }

    public void ShowPrompt()
    {
        promptActive = true;

        if (arrow != null)
            arrow.SetActive(true);

        if (highlight != null)
            highlight.SetActive(true);
    }

    public void HidePrompt()
    {
        promptActive = false;

        if (arrow != null)
            arrow.SetActive(false);

        if (highlight != null)
            highlight.SetActive(false);
    }
}
