using UnityEngine;

public class GUIClipboard : MonoBehaviour
{
    Animator clipboardAnimator;
    private bool TabToggle = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
      clipboardAnimator = GetComponent<Animator>(); 
      this.GetComponentInChildren<MeshRenderer>().enabled = false;
        clipboardAnimator.SetBool("Toggle", false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (clipboardAnimator)
            {
                clipboardAnimator.SetBool("Toggle", TabToggle);
                TabToggle = !TabToggle;
                // this.gameObject.SetActive(TabToggle);
                this.GetComponentInChildren<MeshRenderer>().enabled = !TabToggle;
                print(TabToggle);
            }
        }
    }
}
