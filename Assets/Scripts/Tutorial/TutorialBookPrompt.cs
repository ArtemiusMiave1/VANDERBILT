using UnityEngine;

public class TutorialBookPrompt : MonoBehaviour
{
    [Header("Prompt Objects")]
    public GameObject arrow;
    public GameObject highlight;

    private void Start()
    {
        ShowPrompt();
    }

    public void ShowPrompt()
    {
        if (arrow != null)
            arrow.SetActive(true);

        if (highlight != null)
            highlight.SetActive(true);
    }

    public void HidePrompt()
    {
        if (arrow != null)
            arrow.SetActive(false);

        if (highlight != null)
            highlight.SetActive(false);
    }
}
