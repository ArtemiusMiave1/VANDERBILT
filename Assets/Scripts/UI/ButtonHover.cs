
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class ButtonHover : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerDownHandler,
    IPointerUpHandler
{
    [Header("Text")]
    [SerializeField] private TMP_Text text;
    [SerializeField] private float hoverFontSize = 40f;
    [SerializeField] private Color clickedColor = Color.yellow;

    private float originalFontSize;
    private Color originalColor;

    private void Awake()
    {
        originalFontSize = text.fontSize;
        originalColor = text.color;
    }

    // When mouse enters object
    public void OnPointerEnter(PointerEventData eventData)
    {
        text.fontSize = hoverFontSize;
        text.fontStyle |= FontStyles.Underline;
    }

    // When mouse leaves object
    public void OnPointerExit(PointerEventData eventData)
    {
        text.fontSize = originalFontSize;
        text.fontStyle &= ~FontStyles.Underline;
    }

    // When player presses and holds the button
    public void OnPointerDown(PointerEventData eventData)
    {
        text.color = clickedColor;
    }

    // When player releases the button
    public void OnPointerUp(PointerEventData eventData)
    {
        text.color = originalColor;
    }
}