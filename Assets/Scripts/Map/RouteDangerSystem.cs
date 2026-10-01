using UnityEngine;

public class RouteDangerSystem : MonoBehaviour
{
    [Header("Danger Event Chances")]
    [Range(0f, 1f)]
    public float dangerLevel1Chance = 0.25f;

    [Range(0f, 1f)]
    public float dangerLevel2Chance = 0.50f;


    [Header("Fragile Cargo")]
    [Range(0f, 1f)]
    [Tooltip("Chance of damaging fragile requests on a dangerous route.")]
    public float dangerousFragileDamageChance = 0.25f;

    [Range(0f, 1f)]
    [Tooltip("Chance of damaging fragile requests on a very dangerous route.")]
    public float veryDangerousFragileDamageChance = 0.50f;


    [Header("Cargo Loss")]
    public int minimumCargoLoss = 5;
    public int maximumCargoLoss = 20;


    [Header("Fuel Loss")]
    public int minimumFuelLoss = 1;
    public int maximumFuelLoss = 5;


    [Header("Ship Incident")]
    public int shipIncidentFuelLoss = 5;


    [Header("Storm")]
    public StormTrigger stormTrigger;


    private ShipCargo shipCargo;

    private SoundManager soundManager;


    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        shipCargo =
            FindObjectOfType<ShipCargo>();

        if (stormTrigger == null)
        {
            stormTrigger =
                FindObjectOfType<StormTrigger>();
        }
        
        GameObject audioObject = GameObject.FindGameObjectWithTag("Audio");

        if (audioObject != null)
        {
            soundManager =
                audioObject.GetComponent<SoundManager>();
        }
    }


    // =========================================================
    // CHECK ROUTE DANGER
    // =========================================================

    public void CheckRouteDanger(
    RouteConnection route
)
    {
        if (route == null)
        {
            SetStorm(false);
            return;
        }


        // =========================================================
        // NORMAL ROUTE
        // =========================================================

        if (route.dangerLevel <= 0)
        {
            SetStorm(false);
            return;
        }


        // =========================================================
        // DANGEROUS ROUTE
        // =========================================================

        SetStorm(true);


        // Play thunder when entering
        // Dangerous OR Very Dangerous route.
        if (soundManager != null)
        {
            soundManager.PlaySFX(
                soundManager.Thunder
            );
        }


        // =========================================================
        // DANGER EVENT
        // =========================================================

        float eventChance = 0f;

        switch (route.dangerLevel)
        {
            case 1:
                eventChance =
                    dangerLevel1Chance;
                break;

            case 2:
                eventChance =
                    dangerLevel2Chance;
                break;
        }


        float roll =
            Random.value;


        if (roll <= eventChance)
        {
            TriggerRandomDangerEvent();
        }


        // =========================================================
        // FRAGILE CARGO
        // =========================================================

        CheckFragileCargo(
            route.dangerLevel
        );
    }


    // =========================================================
    // FRAGILE CARGO
    // =========================================================

    private void CheckFragileCargo(
        int dangerLevel
    )
    {
        float damageChance = 0f;


        // -----------------------------------------------------
        // GET DAMAGE CHANCE
        // -----------------------------------------------------

        switch (dangerLevel)
        {
            case 0:
                damageChance = 0f;
                break;


            case 1:
                damageChance =
                    dangerousFragileDamageChance;
                break;


            case 2:
                damageChance =
                    veryDangerousFragileDamageChance;
                break;
        }


        if (damageChance <= 0f)
            return;


        // -----------------------------------------------------
        // FIND REQUESTS CURRENTLY ON THE SHIP
        // -----------------------------------------------------

        RequestPaper[] requests =
            FindObjectsOfType<RequestPaper>();


        foreach (
            RequestPaper request
            in requests
        )
        {
            if (request == null)
                continue;


            // Request must currently be accepted.
            if (!request.IsAccepted())
                continue;


            // Only fragile requests are affected.
            if (!request.IsFragile())
                continue;


            // Completed requests cannot be damaged.
            if (request.IsCompleted())
                continue;


            // Already broken cargo can't get
            // any more broken.
            if (request.IsFragileBroken())
                continue;


            // -------------------------------------------------
            // DAMAGE ROLL
            // -------------------------------------------------

            float roll =
                Random.value;


            if (roll <= damageChance)
            {
                request.DamageFragileCargo();

                Debug.Log(
                    "Route danger damaged fragile cargo: " +
                    request.GetRequestTitle()
                );
            }
        }
    }


    // =========================================================
    // RANDOM DANGER EVENT
    // =========================================================

    private void TriggerRandomDangerEvent()
    {
        int eventType =
            Random.Range(
                0,
                3
            );


        switch (eventType)
        {
            // -------------------------------------------------
            // CARGO LOSS
            // -------------------------------------------------

            case 0:

                LoseRandomCargo();

                break;


            // -------------------------------------------------
            // FUEL LOSS
            // -------------------------------------------------

            case 1:

                LoseFuel();

                break;


            // -------------------------------------------------
            // SHIP INCIDENT
            // -------------------------------------------------

            case 2:

                TriggerShipIncident();

                break;
        }
    }


    // =========================================================
    // CARGO LOSS
    // =========================================================

    private void LoseRandomCargo()
    {
        if (shipCargo == null)
            return;


        string[] cargoTypes =
        {
            "food",
            "medicine",
            "machinery",
            "weapons"
        };


        // -----------------------------------------------------
        // FIND A RESOURCE THE SHIP ACTUALLY HAS
        // -----------------------------------------------------

        for (
            int attempt = 0;
            attempt < cargoTypes.Length;
            attempt++
        )
        {
            string resource =
                cargoTypes[
                    Random.Range(
                        0,
                        cargoTypes.Length
                    )
                ];


            int currentAmount =
                shipCargo.GetResourceAmount(
                    resource
                );


            if (currentAmount <= 0)
                continue;


            int amountToLose =
                Random.Range(
                    minimumCargoLoss,
                    maximumCargoLoss + 1
                );


            amountToLose =
                Mathf.Min(
                    amountToLose,
                    currentAmount
                );


            shipCargo.AddOrRemoveResource(
                resource,
                -amountToLose
            );


            Debug.Log(
                "Danger event: Lost " +
                amountToLose +
                " " +
                resource +
                "."
            );


            return;
        }


        Debug.Log(
            "Danger event tried to remove cargo, " +
            "but the ship had no valid cargo."
        );
    }


    // =========================================================
    // FUEL LOSS
    // =========================================================

    private void LoseFuel()
    {
        if (shipCargo == null)
            return;


        int currentFuel =
            shipCargo.GetResourceAmount(
                "fuel"
            );


        if (currentFuel <= 0)
            return;


        int amountToLose =
            Random.Range(
                minimumFuelLoss,
                maximumFuelLoss + 1
            );


        amountToLose =
            Mathf.Min(
                amountToLose,
                currentFuel
            );


        shipCargo.AddOrRemoveResource(
            "fuel",
            -amountToLose
        );


        Debug.Log(
            "Danger event: Lost " +
            amountToLose +
            " fuel."
        );
    }


    // =========================================================
    // SHIP INCIDENT
    // =========================================================

    private void TriggerShipIncident()
    {
        if (shipCargo == null)
            return;


        int currentFuel =
            shipCargo.GetResourceAmount(
                "fuel"
            );


        if (currentFuel <= 0)
            return;


        int amountToLose =
            Mathf.Min(
                shipIncidentFuelLoss,
                currentFuel
            );


        shipCargo.AddOrRemoveResource(
            "fuel",
            -amountToLose
        );


        Debug.Log(
            "Danger event: Ship incident. " +
            "Lost " +
            amountToLose +
            " fuel."
        );
    }


    // =========================================================
    // STORM
    // =========================================================

    public void SetStorm(
        bool active
    )
    {
        if (stormTrigger == null)
            return;


        if (active)
        {
            stormTrigger.ActivateStorm();
        }
        else
        {
            stormTrigger.DeactivateStorm();
        }
    }
}