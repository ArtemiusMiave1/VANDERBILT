using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class HandBook : BookBase
{
    [Header("Book Canvas")]
    public GameObject bookCanvas;

    [Header("Pages")]
    public GameObject[] pages;

    [Header("Page Buttons")]
    public Button IndexPageButton;
    public Button LocationPageButton;
    public Button RequestsPageButton;
    public Button RoutesPageButton;
    public Button exitButton;


    [Header("Player")]
    public PlayerState playerState;

    private int currentPage = 0;

    private void Start()
    {
        // Start with the book closed
        if (bookCanvas != null)
        {
            bookCanvas.SetActive(false);
        }

        // Connect page buttons
        if (IndexPageButton != null)
        {
            IndexPageButton.onClick.AddListener(
                () => GoToPage(0)
            );
        }

        if (LocationPageButton != null)
        {
            LocationPageButton.onClick.AddListener(
                () => GoToPage(1)
            );
        }

        if (RequestsPageButton != null)
        {
            RequestsPageButton.onClick.AddListener(
                () => GoToPage(2)
            );
        }

        if (RoutesPageButton != null)
        {
            RoutesPageButton.onClick.AddListener(
                () => GoToPage(3)
            );
        }

        if (exitButton != null)
        {
            exitButton.onClick.AddListener(CloseBook);
        }
    }

    public override void OpenBook()
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
        if (bookCanvas != null)
        {
            bookCanvas.SetActive(false);
        }

        if (playerState != null)
        {
            playerState.SetState(PlayerState.State.Gameplay);
        }
    }

    // Go directly to a specific page
    public void GoToPage(int pageNumber)
    {
        // Make sure the page exists
        if (pageNumber < 0 || pageNumber >= pages.Length)
        {
            return;
        }

        currentPage = pageNumber;

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


    }
}