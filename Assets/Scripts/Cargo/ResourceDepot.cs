using UnityEngine;

public class ResourceDepot : MonoBehaviour
{
    [Header("Depot Settings")]
    public string depotName = "Resource Depot";

    [Header("Prices")]
    public int foodPrice = 2;
    public int coalPrice = 3;
    public int medicinePrice = 5;
    public int machineryPrice = 8;
    public int orePrice = 4;
    public int fuelPrice = 3;
    public int weaponsPrice = 10;


    private ShipCargo shipCargo;


    public void OpenDepot(ShipCargo cargo)
    {
        shipCargo = cargo;

        Debug.Log("Opened " + depotName);
    }


    public void BuyResource(string resourceType, int amount)
    {
        if (shipCargo == null)
        {
            Debug.LogWarning("No ship cargo connected to depot.");
            return;
        }

        int price = GetResourcePrice(resourceType);

        if (price < 0)
        {
            Debug.LogWarning("Unknown resource: " + resourceType);
            return;
        }

        int totalCost = price * amount;


        // Check if the ship has enough gold
        if (shipCargo.gold < totalCost)
        {
            Debug.Log("Not enough gold!");
            return;
        }


        // Remove gold
        shipCargo.AddOrRemoveResource("gold", -totalCost);

        // Add resource
        shipCargo.AddOrRemoveResource(resourceType, amount);

        Debug.Log(
            "Bought " + amount + " " + resourceType +
            " for " + totalCost + " gold."
        );
    }


    private int GetResourcePrice(string resourceType)
    {
        switch (resourceType.ToLower())
        {
            case "food":
                return foodPrice;

            case "coal":
                return coalPrice;

            case "medicine":
                return medicinePrice;

            case "machinery":
                return machineryPrice;

            case "ore":
                return orePrice;

            case "fuel":
                return fuelPrice;

            case "weapons":
                return weaponsPrice;

            default:
                return -1;
        }
    }
}