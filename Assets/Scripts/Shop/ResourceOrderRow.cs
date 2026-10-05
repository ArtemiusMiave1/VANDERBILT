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
        resourceNameText.text =
            resourceType;


        priceText.text =
            price +
            " Gold";


        quantityInput.contentType =
            TMP_InputField.ContentType.IntegerNumber;


        quantityInput.onValidateInput +=
            ValidateNumberInput;


        quantityInput.text =
            "0";


        // Update depot whenever the player
        // changes the quantity.

        quantityInput.onValueChanged.AddListener(
            OnQuantityChanged
        );
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

    private void OnQuantityChanged(
        string value
    )
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
        if (
            int.TryParse(
                quantityInput.text,
                out int amount
            )
        )
        {
            return Mathf.Max(
                0,
                amount
            );
        }


        return 0;
    }


    // =========================================================
    // GET TOTAL COST
    // =========================================================

    public int GetTotalCost()
    {
        return GetAmount() *
               price;
    }


    // =========================================================
    // CLEAR AMOUNT
    // =========================================================

    public void ClearAmount()
    {
        quantityInput.text =
            "0";
    }
}