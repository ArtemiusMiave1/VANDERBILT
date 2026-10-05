using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
    public TMP_Text purchaseWeight;
    public TMP_Text warningText;

    [Header("Purchase")]
    public Button purchaseButton;

    private SoundManager soundManager;


    // =========================================================
    // AWAKE
    // =========================================================

    private void Awake()
    {
        GameObject audioObject =
            GameObject.FindGameObjectWithTag("Audio");

        if (audioObject != null)
        {
            soundManager =
                audioObject.GetComponent<SoundManager>();
        }
    }


    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        UpdateTotal();
    }


    // =========================================================
    // UPDATE TOTAL
    // =========================================================

    public void UpdateTotal()
    {
        if (shipCargo == null)
        {
            Debug.LogWarning(
                "Ship Cargo has not been assigned."
            );

            return;
        }


        int total =
            CalculateTotal();


        int remainingGold =
            shipCargo.gold - total;


        totalText.text =
            "TOTAL: " +
            total +
            " Gold";


        goldText.text =
            "GOLD: " +
            shipCargo.gold;


        remainingGoldText.text =
            "AFTER PURCHASE: " +
            remainingGold +
            " Gold";


        purchaseWeight.text =
            "weight " +
            CalculateTotalWeight();


        // -----------------------------------------------------
        // CHECK GOLD
        // -----------------------------------------------------

        if (remainingGold < 0)
        {
            warningText.text =
                "NOT ENOUGH GOLD!";

            purchaseButton.interactable =
                false;
        }


        // -----------------------------------------------------
        // CHECK WEIGHT
        // -----------------------------------------------------

        else if (CheckOverWeight())
        {
            warningText.text =
                "Over Weight!";

            purchaseButton.interactable =
                false;
        }


        // -----------------------------------------------------
        // EVERYTHING IS OK
        // -----------------------------------------------------

        else
        {
            warningText.text =
                "";

            purchaseButton.interactable =
                true;
        }
    }


    // =========================================================
    // CALCULATE TOTAL COST
    // =========================================================

    public int CalculateTotal()
    {
        int total = 0;


        foreach (
            ResourceOrderRow row
            in orderRows
        )
        {
            if (row != null)
            {
                total +=
                    row.GetTotalCost();
            }
        }


        return total;
    }


    // =========================================================
    // CALCULATE TOTAL WEIGHT
    // =========================================================

    public float CalculateTotalWeight()
    {
        float tempWeight = 0f;


        foreach (
            ResourceOrderRow row
            in orderRows
        )
        {
            if (row == null)
                continue;


            int amount =
                row.GetAmount();


            float resourceWeight =
                shipCargo.GetResourceWeight(
                    row.resourceType
                );


            tempWeight +=
                amount *
                resourceWeight;
        }


        return tempWeight;
    }


    // =========================================================
    // CHECK OVERWEIGHT
    // =========================================================

    public bool CheckOverWeight()
    {
        if (
            CalculateTotalWeight() >
            shipCargo.GetRemainingCargoCapacity()
        )
        {
            return true;
        }


        return false;
    }


    // =========================================================
    // PURCHASE ORDER
    // =========================================================

    public void PurchaseOrder()
    {
        if (CheckOverWeight())
            return;


        if (shipCargo == null)
        {
            Debug.LogError(
                "Ship Cargo has not been assigned!"
            );

            return;
        }


        int total =
            CalculateTotal();


        // -----------------------------------------------------
        // CHECK GOLD
        // -----------------------------------------------------

        if (shipCargo.gold < total)
        {
            Debug.Log(
                "Not enough gold!"
            );

            return;
        }


        // -----------------------------------------------------
        // BUY EVERY RESOURCE
        // -----------------------------------------------------

        foreach (
            ResourceOrderRow row
            in orderRows
        )
        {
            if (row == null)
                continue;


            int amount =
                row.GetAmount();


            if (amount <= 0)
                continue;


            shipCargo.AddOrRemoveResource(
                row.resourceType,
                amount
            );


            Debug.Log(
                "Amount " +
                amount +
                " row " +
                row.name
            );
        }


        // -----------------------------------------------------
        // REMOVE GOLD
        // -----------------------------------------------------

        shipCargo.AddOrRemoveResource(
            "gold",
            -total
        );


        Debug.Log(
            "Purchase completed for " +
            total +
            " Gold"
        );


        // -----------------------------------------------------
        // CLEAR ORDER
        // -----------------------------------------------------

        foreach (
            ResourceOrderRow row
            in orderRows
        )
        {
            if (row != null)
            {
                row.ClearAmount();
            }
        }


        // -----------------------------------------------------
        // PURCHASE SOUND
        // -----------------------------------------------------

        if (soundManager != null)
        {
            soundManager.PlaySFX(
                soundManager.FaxPurchase
            );
        }


        // -----------------------------------------------------
        // UPDATE UI
        // -----------------------------------------------------

        UpdateTotal();
    }
}