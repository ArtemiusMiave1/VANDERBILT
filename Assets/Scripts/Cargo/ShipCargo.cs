using UnityEngine;

public class ShipCargo : MonoBehaviour
{
    [Header("Current Cargo Weight")]
    public float totalCargoWeight;

    [Header("Cargo")]
    public int food = 100;
    public int medicine = 100;
    public int machinery = 100;
    public int fuel = 100;
    public int gold = 300;
    public int weapons = 100;


    [Header("Cargo Capacity")]
    public float maxCargoWeight = 1000f;


    [Header("Resource Weights")]
    public float foodWeight = .3f;
    public float medicineWeight = 1f;
    public float machineryWeight = 3f;
    public float fuelWeight = .5f;
    public float goldWeight = .1f;
    public float weaponsWeight = 3f;


    // =========================================================
    // ADD / REMOVE RESOURCE
    // =========================================================

    public void AddOrRemoveResource(
        string resourceType,
        int amount
    )
    {
        resourceType = resourceType.ToLower();


        // If adding resources, check cargo capacity first
        if (amount > 0)
        {
            if (!CanCarry(resourceType, amount))
            {
                Debug.Log(
                    "Not enough cargo space for " +
                    amount +
                    " " +
                    resourceType
                );

                return;
            }
        }


        switch (resourceType)
        {
            case "food":
                food += amount;
                break;

            case "medicine":
                medicine += amount;
                break;

            case "machinery":
                machinery += amount;
                break;

            case "fuel":
                fuel += amount;
                break;

            case "gold":
                gold += amount;
                break;

            case "weapons":
                weapons += amount;
                break;

            default:
                Debug.LogWarning(
                    "Unknown resource type: " +
                    resourceType
                );

                return;
        }


        ClampResources();


        Debug.Log(
            $"{resourceType}: " +
            $"{GetResourceAmount(resourceType)}"
        );

        Debug.Log(
            "Cargo Weight: " +
            GetCurrentCargoWeight() +
            " / " +
            maxCargoWeight
        );
    }


    // =========================================================
    // CHECK CARGO CAPACITY
    // =========================================================

    public bool CanCarry(
        string resourceType,
        int amount
    )
    {
        float weightToAdd =
            GetResourceWeight(resourceType) * amount;


        float newWeight =
            GetCurrentCargoWeight() + weightToAdd;


        return newWeight <= maxCargoWeight;
    }


    // =========================================================
    // GET CURRENT CARGO WEIGHT
    // =========================================================

    public float GetCurrentCargoWeight()
    {
        float totalWeight = 0f;


        totalWeight += food * foodWeight;
        totalWeight += medicine * medicineWeight;
        totalWeight += machinery * machineryWeight;
        totalWeight += fuel * fuelWeight;
        totalWeight += gold * goldWeight;
        totalWeight += weapons * weaponsWeight;


        return totalWeight;
    }


    // =========================================================
    // GET REMAINING CAPACITY
    // =========================================================

    public float GetRemainingCargoCapacity()
    {
        return maxCargoWeight -
               GetCurrentCargoWeight();
    }


    // =========================================================
    // GET RESOURCE WEIGHT
    // =========================================================

    public float GetResourceWeight(
        string resourceType
    )
    {
        switch (resourceType.ToLower())
        {
            case "food":
                return foodWeight;

            case "medicine":
                return medicineWeight;

            case "machinery":
                return machineryWeight;

            case "fuel":
                return fuelWeight;

            case "gold":
                return goldWeight;

            case "weapons":
                return weaponsWeight;

            default:
                Debug.LogWarning(
                    "Unknown resource type: " +
                    resourceType
                );

                return 0f;
        }
    }


    // =========================================================
    // GET RESOURCE AMOUNT
    // =========================================================

    public int GetResourceAmount(
        string resourceType
    )
    {
        switch (resourceType.ToLower())
        {
            case "food":
                return food;

            case "medicine":
                return medicine;

            case "machinery":
                return machinery;

            case "fuel":
                return fuel;

            case "gold":
                return gold;

            case "weapons":
                return weapons;

            default:
                Debug.LogWarning(
                    "Unknown resource type: " +
                    resourceType
                );

                return 0;
        }
    }


    // =========================================================
    // CLAMP RESOURCES
    // =========================================================

    private void ClampResources()
    {
        food = Mathf.Max(0, food);
        medicine = Mathf.Max(0, medicine);
        machinery = Mathf.Max(0, machinery);
        fuel = Mathf.Max(0, fuel);
        gold = Mathf.Max(0, gold);
        weapons = Mathf.Max(0, weapons);
    }
    public float GetTotalWeight()
    {
        totalCargoWeight = 0f;

        totalCargoWeight += food * foodWeight;
        totalCargoWeight += medicine * medicineWeight;
        totalCargoWeight += machinery * machineryWeight;
        totalCargoWeight += fuel * fuelWeight;
        totalCargoWeight += gold * goldWeight;
        totalCargoWeight += weapons * weaponsWeight;

        return totalCargoWeight;
    }
}