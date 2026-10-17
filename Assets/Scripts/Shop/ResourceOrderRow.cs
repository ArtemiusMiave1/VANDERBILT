using TMPro;
using UnityEngine;

public class ResourceOrderRow : MonoBehaviour
{
    [Header("Resource")]
    public string resourceType;
    public int price;

    [Header("UI")]
    public TMP_Text resourceNameText;
    public TMP_Text priceText;
    public TMP_InputField quantityInput;


    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        Setup();
    }


    // =========================================================
    // SETUP
    // =========================================================

    public void Setup()
    {
        resourceNameText.text = resourceType;

        priceText.text = price + " Gold";

        // Allow integer input only.
        quantityInput.contentType =
            TMP_InputField.ContentType.IntegerNumber;

        // Limit input to 3 characters (0-999).
        quantityInput.characterLimit = 3;

        // Prevent non-numeric characters.
        quantityInput.onValidateInput -= ValidateNumberInput;
        quantityInput.onValidateInput += ValidateNumberInput;

        // Set initial quantity.
        quantityInput.text = "0";

        // Update depot whenever quantity changes.
        quantityInput.onValueChanged.RemoveListener(OnQuantityChanged);
        quantityInput.onValueChanged.AddListener(OnQuantityChanged);
    }


    // =========================================================
    // VALIDATE INPUT
    // =========================================================

    private char ValidateNumberInput(
        string text,
        int charIndex,
        char addedChar
    )
    {
        return char.IsDigit(addedChar)
            ? addedChar
            : '\0';
    }


    // =========================================================
    // QUANTITY CHANGED
    // =========================================================

    private void OnQuantityChanged(string value)
    {
        ResourceDepotPaper paper =
            GetComponentInParent<ResourceDepotPaper>();

        if (paper != null)
        {
            paper.UpdateTotal();
        }
    }


    // =========================================================
    // GET AMOUNT
    // =========================================================

    public int GetAmount()
    {
        if (int.TryParse(quantityInput.text, out int amount))
        {
            return Mathf.Clamp(amount, 0, 999);
        }

        return 0;
    }


    // =========================================================
    // GET TOTAL COST
    // =========================================================

    public int GetTotalCost()
    {
        return GetAmount() * price;
    }


    // =========================================================
    // CLEAR AMOUNT
    // =========================================================

    public void ClearAmount()
    {
        quantityInput.text = "0";
    }
}