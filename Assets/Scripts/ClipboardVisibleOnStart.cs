using UnityEngine;

public class ClipboardVisibleOnStart : MonoBehaviour
{
    public GameObject clipboard;
    private bool TabToggle = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        clipboard.SetActive(TabToggle);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            TabToggle = !TabToggle;
            clipboard.SetActive(TabToggle);
        }

    }
}
