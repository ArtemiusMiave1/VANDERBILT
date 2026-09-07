using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class Book : MonoBehaviour
{
    [Header("Book Canvas")]
    public GameObject bookCanvas;

    [Header("Pages")]
    public GameObject[] pages;

    [Header("Buttons")]
    public Button leftPageButton;
    public Button rightPageButton;
    public Button exitButton;

    [Header("Player")]
    public PlayerState playerState;

    private int currentPage = 0;

    private void Start()
    {
        // Make sure the book starts closed
        if (bookCanvas != null)
        {
            bookCanvas.SetActive(false);
        }

        // Connect buttons
        if (leftPageButton != null)
        {
            leftPageButton.onClick.AddListener(PreviousPage);
        }

        if (rightPageButton != null)
        {
            rightPageButton.onClick.AddListener(NextPage);
        }

        if (exitButton != null)
        {
            exitButton.onClick.AddListener(CloseBook);
        }
    }

    public void OpenBook()
    {
        bookCanvas.SetActive(true);

        currentPage = 0;

        UpdatePages();

        if (playerState != null)
        {
            playerState.SetState(
                PlayerState.State.Book
            );
        }
    }

    public void CloseBook()
    {
        bookCanvas.SetActive(false);

        if (playerState != null)
        {
            playerState.SetState(
                PlayerState.State.Gameplay
            );
        }
    }

    public void NextPage()
    {
        if (currentPage >= pages.Length - 1)
            return;

        currentPage++;

        UpdatePages();
    }

    public void PreviousPage()
    {
        if (currentPage <= 0)
            return;

        currentPage--;

        UpdatePages();
    }

    private void UpdatePages()
    {
        // Turn every page off
        for (int i = 0; i < pages.Length; i++)
        {
            if (pages[i] != null)
            {
                pages[i].SetActive(false);
            }
        }

        // Turn the current page on
        if (pages.Length > 0 &&
            pages[currentPage] != null)
        {
            pages[currentPage].SetActive(true);
        }

        // Disable buttons at the ends
        if (leftPageButton != null)
        {
            leftPageButton.interactable =
                currentPage > 0;
        }

        if (rightPageButton != null)
        {
            rightPageButton.interactable =
                currentPage < pages.Length - 1;
        }
    }
}