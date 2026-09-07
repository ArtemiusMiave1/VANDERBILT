using UnityEngine;

public class LockMouse : MonoBehaviour
{
    [Header("Cursor")]
    public bool cursorLocked = true;

    private void Start()
    {
        LockCursor(cursorLocked);
    }

    public void LockCursor(bool lockCursor)
    {
        cursorLocked = lockCursor;

        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}