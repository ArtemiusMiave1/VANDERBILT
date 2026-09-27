using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHighlightRawImage : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerDownHandler,
    IPointerUpHandler
{
    // Image shown when the button is highlighted
    public GameObject highlightRawImage;

    // Image shown when the button is pressed
    public GameObject pressedRawImage;

    private void Start()
    {
        highlightRawImage.SetActive(false);
        pressedRawImage.SetActive(false);
    }

    // Player highlights the button
    public void OnPointerEnter(PointerEventData eventData)
    {
        highlightRawImage.SetActive(true);
    }

    // Player stops highlighting the button
    public void OnPointerExit(PointerEventData eventData)
    {
        highlightRawImage.SetActive(false);
    }

    // Player presses the button
    public void OnPointerDown(PointerEventData eventData)
    {
        pressedRawImage.SetActive(true);
    }

    // Player releases the button
    public void OnPointerUp(PointerEventData eventData)
    {
        pressedRawImage.SetActive(false);
    }
}