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

    private void Start()
    {
        Setup();
    }

    public void Setup()
    {
        resourceNameText.text = resourceType;
        priceText.text = price + " Gold";

        quantityInput.text = "0";

        // Update the depot whenever the player changes the number
        quantityInput.onValueChanged.AddListener(OnQuantityChanged);
    }

    private void OnQuantityChanged(string value)
    {
        // Tell the depot paper that something changed
        ResourceDepotPaper paper = GetComponentInParent<ResourceDepotPaper>();

        if (paper != null)
        {
            paper.UpdateTotal();
        }
    }

    public int GetAmount()
    {
        if (int.TryParse(quantityInput.text, out int amount))
        {
            return Mathf.Max(0, amount);
        }

        return 0;
    }

    public int GetTotalCost()
    {
        return GetAmount() * price;
    }

    public void ClearAmount()
    {
        quantityInput.text = "0";
    }
}