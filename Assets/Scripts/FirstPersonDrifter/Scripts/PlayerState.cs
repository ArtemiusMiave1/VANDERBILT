using UnityEngine;

public class PlayerState : MonoBehaviour
{
    public enum State
    {
        Gameplay,
        Book,
        Menu,
        Dialogue
    }

    [Header("Current State")]
    public State currentState = State.Gameplay;

    [Header("Lock Mouse")]
    public LockMouse lockMouse;

    [Header("Mouse Look")]
    public MonoBehaviour mouseLook1;
    public MonoBehaviour mouseLook2;

    private void Start()
    {
        if (lockMouse == null)
        {
            lockMouse = GetComponent<LockMouse>();
        }

        SetState(State.Gameplay);
    }

    private void Update()
    {
        // Escape opens the free cursor mode
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentState == State.Gameplay)
            {
                SetState(State.Menu);
            }
        }

        // Clicking the screen returns to gameplay
        // ONLY when in Menu state.
        //
        // This means clicking Book UI buttons will NOT
        // lock the cursor because Book is a separate state.
        if (currentState == State.Menu)
        {
            if (Input.GetMouseButtonDown(0))
            {
                SetState(State.Gameplay);
            }
        }
    }

    public void SetState(State newState)
    {
        currentState = newState;

        switch (currentState)
        {
            case State.Gameplay:
                EnableGameplay();
                break;

            case State.Book:
                EnableBook();
                break;

            case State.Menu:
                EnableUI();
                break;

            case State.Dialogue:
                EnableUI();
                break;
        }
    }

    private void EnableGameplay()
    {
        // Enable mouse look
        if (mouseLook1 != null)
            mouseLook1.enabled = true;

        if (mouseLook2 != null)
            mouseLook2.enabled = true;

        // Lock cursor
        if (lockMouse != null)
            lockMouse.LockCursor(true);
    }

    private void EnableBook()
    {
        // Disable mouse look
        if (mouseLook1 != null)
            mouseLook1.enabled = false;

        if (mouseLook2 != null)
            mouseLook2.enabled = false;

        // Unlock cursor
        if (lockMouse != null)
            lockMouse.LockCursor(false);
    }

    private void EnableUI()
    {
        // Disable mouse look
        if (mouseLook1 != null)
            mouseLook1.enabled = false;

        if (mouseLook2 != null)
            mouseLook2.enabled = false;

        // Unlock cursor
        if (lockMouse != null)
            lockMouse.LockCursor(false);
    }

    public bool IsGameplay()
    {
        return currentState == State.Gameplay;
    }

    public bool IsBook()
    {
        return currentState == State.Book;
    }

    public bool IsMenu()
    {
        return currentState == State.Menu;
    }
}