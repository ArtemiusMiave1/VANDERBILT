using UnityEngine;

public class RouteDangerSystem : MonoBehaviour
{
    [Header("Danger Chances")]
    [Range(0f, 1f)]
    public float dangerLevel1Chance = 0.25f;

    [Range(0f, 1f)]
    public float dangerLevel2Chance = 0.50f;

    [Header("Cargo Damage")]
    public int minimumCargoLoss = 5;
    public int maximumCargoLoss = 20;

    [Header("Fuel Damage")]
    public int minimumFuelLoss = 1;
    public int maximumFuelLoss = 5;

    private ShipCargo shipCargo;


    private void Start()
    {
        shipCargo = FindObjectOfType<ShipCargo>();

        if (shipCargo == null)
        {
            Debug.LogError(
                "RouteDangerSystem could not find ShipCargo!"
            );
        }
    }


    public void CheckRouteDanger(RouteConnection route)
    {
        if (route == null)
            return;

        // Safe route
        if (route.dangerLevel <= 0)
            return;


        float chance = 0f;


        // Determine chance based on danger
        if (route.dangerLevel == 1)
        {
            chance = dangerLevel1Chance;
        }
        else if (route.dangerLevel >= 2)
        {
            chance = dangerLevel2Chance;
        }


        // Roll for danger
        float roll = Random.value;


        if (roll <= chance)
        {
            TriggerDangerEvent(route);
        }
    }


    private void TriggerDangerEvent(
        RouteConnection route)
    {
        int eventType =
            Random.Range(0, 3);

        print("event type "+eventType);

        switch (eventType)
        {
            case 0:
                CargoDamage();
                break;

            case 1:
                FuelDamage();
                break;

            case 2:
                ShipIncident();
                break;
        }
    }


    private void CargoDamage()
    {
        if (shipCargo == null)
            return;


        string[] resources =
        {
            "food",
            "medicine",
            "machinery",
            "weapons"
        };


        string resource =
            resources[
                Random.Range(
                    0,
                    resources.Length
                )
            ];


        int amount =
            Random.Range(
                minimumCargoLoss,
                maximumCargoLoss + 1
            );


        int currentAmount =
            shipCargo.GetResourceAmount(resource);


        // Don't remove more than the player owns
        amount =
            Mathf.Min(
                amount,
                currentAmount
            );


        shipCargo.AddOrRemoveResource(
            resource,
            -amount
        );


        Debug.Log(
            "DANGER EVENT: Lost " +
            amount +
            " " +
            resource
        );
    }


    private void FuelDamage()
    {
        if (shipCargo == null)
            return;


        int amount =
            Random.Range(
                minimumFuelLoss,
                maximumFuelLoss + 1
            );


        shipCargo.AddOrRemoveResource(
            "fuel",
            -amount
        );


        Debug.Log(
            "DANGER EVENT: Lost " +
            amount +
            " fuel"
        );
    }


    private void ShipIncident()
    {
        Debug.Log(
            "DANGER EVENT: Ship encountered " +
            "a dangerous incident!"
        );
    }
}