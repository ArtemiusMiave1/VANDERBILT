using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
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

    // When mouse enters obj
    public void OnPointerEnter(PointerEventData eventData)
    {
        text.fontSize = hoverFontSize;
        text.fontStyle |= FontStyles.Underline;
    }

    // When mouse leaves obj
    public void OnPointerExit(PointerEventData eventData)
    {
        text.fontSize = originalFontSize;
        text.fontStyle &= ~FontStyles.Underline;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        text.color = clickedColor;
    }
}
