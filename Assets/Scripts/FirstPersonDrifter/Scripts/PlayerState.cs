
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    public enum State
    {
        Gameplay,
        Book,
        Menu,
        Dialogue,
        Grabbing
    }

    [Header("Current State")]
    public State currentState = State.Gameplay;

    [Header("Lock Mouse")]
    public LockMouse lockMouse;

    [Header("Mouse Look")]
    public MonoBehaviour mouseLook1;
    public MonoBehaviour mouseLook2;

    [Header("Grabbing Settings")]
    [Range(0.01f, 1f)]
    [Tooltip("Multiplier for mouse movement when interacting with machinery.")]
    public float grabbingSensitivity = 0.35f;

    private void Start()
    {
        if (lockMouse == null)
        {
            lockMouse = GetComponent<LockMouse>();
        }

        SetState(State.Gameplay);
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

            case State.Grabbing:
                EnableGrabbing();
                break;
        }
    }

    // =========================================================
    // GAMEPLAY
    // =========================================================

    private void EnableGameplay()
    {
        if (mouseLook1 != null)
            mouseLook1.enabled = true;

        if (mouseLook2 != null)
            mouseLook2.enabled = true;

        if (lockMouse != null)
            lockMouse.LockCursor(true);
    }

    // =========================================================
    // BOOK
    // =========================================================

    private void EnableBook()
    {
        if (mouseLook1 != null)
            mouseLook1.enabled = false;

        if (mouseLook2 != null)
            mouseLook2.enabled = false;

        if (lockMouse != null)
            lockMouse.LockCursor(false);
    }

    // =========================================================
    // MENU / DIALOGUE
    // =========================================================

    private void EnableUI()
    {
        if (mouseLook1 != null)
            mouseLook1.enabled = false;

        if (mouseLook2 != null)
            mouseLook2.enabled = false;

        if (lockMouse != null)
            lockMouse.LockCursor(false);
    }

    // =========================================================
    // GRABBING
    // =========================================================

    private void EnableGrabbing()
    {
        // Disable camera movement.
        if (mouseLook1 != null)
            mouseLook1.enabled = false;

        if (mouseLook2 != null)
            mouseLook2.enabled = false;

        // Unlock and show cursor.
        if (lockMouse != null)
            lockMouse.LockCursor(false);
    }

    // =========================================================
    // GRABBING SENSITIVITY
    // =========================================================

    public float GetGrabbingSensitivity()
    {
        return grabbingSensitivity;
    }

    // =========================================================
    // STATE CHECKS
    // =========================================================

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

    public bool IsGrabbing()
    {
        return currentState == State.Grabbing;
    }
}
