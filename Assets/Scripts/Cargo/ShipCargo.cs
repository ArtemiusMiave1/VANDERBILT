using UnityEngine;

public class ShipCargo : MonoBehaviour
{
    [Header("Cargo")]
    public int food = 100;
    public int coal = 100;
    public int medicine = 100;
    public int machinery = 100;
    public int ore = 100;
    public int fuel = 100;
    public int gold = 300;
    public int weapons = 100;



    public void AddOrRemoveResource(string resourceType, int amount)
    {
        switch (resourceType.ToLower())
        {
            case "food":
                food += amount;
                break;

            case "coal":
                coal += amount;
                break;

            case "medicine":
                medicine += amount;
                break;

            case "machinery":
                machinery += amount;
                break;

            case "ore":
                ore += amount;
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
                Debug.LogWarning("Unknown resource type: " + resourceType);
                return;
        }

        ClampResources();

        Debug.Log($"{resourceType}: {GetResourceAmount(resourceType)}");
    }


    private void ClampResources()
    {
        food = Mathf.Max(0, food);
        coal = Mathf.Max(0, coal);
        medicine = Mathf.Max(0, medicine);
        machinery = Mathf.Max(0, machinery);
        ore = Mathf.Max(0, ore);
        fuel = Mathf.Max(0, fuel);
        gold = Mathf.Max(0, gold);
        weapons = Mathf.Max(0, weapons);

    }


    public int GetResourceAmount(string resourceType)
    {
        switch (resourceType.ToLower())
        {
            case "food":
                return food;

            case "coal":
                return coal;

            case "medicine":
                return medicine;

            case "machinery":
                return machinery;

            case "ore":
                return ore;

            case "fuel":
                return fuel;

            case "gold":
                return gold;

            case "weapons":
                return weapons;

            default:
                Debug.LogWarning("Unknown resource type: " + resourceType);
                return 0;
        }
    }
}