using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourceDepotPaper : MonoBehaviour
{
    [Header("Ship")]
    public ShipCargo shipCargo;

    [Header("Order Rows")]
    public ResourceOrderRow[] orderRows;

    [Header("UI")]
    public TMP_Text totalText;
    public TMP_Text goldText;
    public TMP_Text remainingGoldText;
    public TMP_Text warningText;

    [Header("Purchase")]
    public Button purchaseButton;


    private void Start()
    {
        UpdateTotal();
    }


    public void UpdateTotal()
    {
        if (shipCargo == null)
        {
            Debug.LogWarning("Ship Cargo has not been assigned.");
            return;
        }

        int total = CalculateTotal();
        int remainingGold = shipCargo.gold - total;

        totalText.text = "TOTAL: " + total + " Gold";

        goldText.text = "GOLD: " + shipCargo.gold;

        remainingGoldText.text =
            "AFTER PURCHASE: " + remainingGold + " Gold";


        // Check whether the player can afford the order
        if (remainingGold < 0)
        {
            warningText.text = "NOT ENOUGH GOLD!";
            purchaseButton.interactable = false;
        }
        else
        {
            warningText.text = "";
            purchaseButton.interactable = true;
        }
    }


    public int CalculateTotal()
    {
        int total = 0;

        foreach (ResourceOrderRow row in orderRows)
        {
            if (row != null)
            {
                total += row.GetTotalCost();
            }
        }

        return total;
    }


    public void PurchaseOrder()
    {
        if (shipCargo == null)
        {
            Debug.LogError("Ship Cargo has not been assigned!");
            return;
        }

        int total = CalculateTotal();


        // Make sure the player can afford it
        if (shipCargo.gold < total)
        {
            Debug.Log("Not enough gold!");
            return;
        }


        // Buy every resource
        foreach (ResourceOrderRow row in orderRows)
        {
            if (row == null)
                continue;

            int amount = row.GetAmount();

            if (amount <= 0)
                continue;

            shipCargo.AddOrRemoveResource(
                row.resourceType,
                amount
            );
        }


        // Remove gold
        shipCargo.AddOrRemoveResource("gold", -total);


        Debug.Log("Purchase completed for " + total + " Gold");


        // Clear the order
        foreach (ResourceOrderRow row in orderRows)
        {
            if (row != null)
            {
                row.ClearAmount();
            }
        }


        UpdateTotal();
    }
}