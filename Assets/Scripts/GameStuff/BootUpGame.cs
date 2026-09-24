using UnityEngine;

public class BootUpGame : MonoBehaviour
{
    [Header("Game Systems")]
    public GameDatabase gameDatabase;
    public RandomLocationSpawner locationSpawner;
    public LocationManager locationManager;
    public TradeRouteVisualManager tradeRouteVisualManager;
    public RequestGenerator requestGenerator;
    public GameClock gameClock;
    public ShipMovement shipMovement;

    [Header("Startup")]
    public bool bootGameOnStart = true;

    private void Start()
    {
        if (!bootGameOnStart)
            return;

        BootGame();
    }

    public void BootGame()
    {
        Debug.Log("================================");
        Debug.Log("VANDERBILT - BOOTING GAME");
        Debug.Log("================================");

        FindSystems();

        if (!ValidateSystems())
        {
            Debug.LogError(
                "BootUpGame: Game boot failed!"
            );

            return;
        }

        // --------------------------------
        // BOOT 1
        // Load Game Database
        // --------------------------------

        Debug.Log(
            "BOOT 1: Loading Game Database..."
        );

        gameDatabase.LoadDatabase();


        // --------------------------------
        // BOOT 2
        // Spawn Locations
        // --------------------------------

        Debug.Log(
            "BOOT 2: Spawning Locations..."
        );

        locationSpawner.SpawnLocations();


        // --------------------------------
        // BOOT 3
        // Find Vanderbilt
        // --------------------------------

        Debug.Log(
            "BOOT 3: Finding Vanderbilt starting location..."
        );

        Location vanderbiltLocation =
            FindVanderbiltLocation();

        if (vanderbiltLocation == null)
        {
            Debug.LogError(
                "BootUpGame: Could not find a Vanderbilt location!"
            );

            return;
        }


        // --------------------------------
        // BOOT 4
        // Set Ship Starting Location
        // --------------------------------

        Debug.Log(
            "BOOT 4: Setting ship starting location..."
        );

        SetShipStartingLocation(
            vanderbiltLocation
        );


        // --------------------------------
        // BOOT 5
        // Generate Connections
        // --------------------------------

        Debug.Log(
            "BOOT 5: Generating Connections..."
        );

        locationManager.GenerateConnections();


        // --------------------------------
        // BOOT 6
        // Generate Trade Route Visuals
        // --------------------------------

        Debug.Log(
            "BOOT 6: Generating Trade Route Visuals..."
        );

        if (tradeRouteVisualManager != null)
        {
            tradeRouteVisualManager.GenerateVisuals();
        }
        else
        {
            Debug.LogWarning(
                "BootUpGame: TradeRouteVisualManager not found. " +
                "Trade route visuals will not be generated."
            );
        }


        // --------------------------------
        // BOOT 7
        // Start Game Clock
        // --------------------------------

        Debug.Log(
            "BOOT 7: Starting Game Clock..."
        );

        if (gameClock != null)
        {
            gameClock.StartClock();
        }


        // --------------------------------
        // BOOT 8
        // Start Request System
        // --------------------------------

        Debug.Log(
            "BOOT 8: Starting Request System..."
        );

        if (requestGenerator != null)
        {
            requestGenerator.GenerateRequests();
        }


        // --------------------------------
        // GAME READY
        // --------------------------------

        Debug.Log(
            "================================"
        );

        Debug.Log(
            "VANDERBILT - GAME READY"
        );

        Debug.Log(
            "Starting Location: " +
            vanderbiltLocation.GetDisplayName()
        );

        Debug.Log(
            "================================"
        );
    }

    private void FindSystems()
    {
        if (gameDatabase == null)
        {
            gameDatabase =
                FindObjectOfType<GameDatabase>();
        }

        if (locationSpawner == null)
        {
            locationSpawner =
                FindObjectOfType<RandomLocationSpawner>();
        }

        if (locationManager == null)
        {
            locationManager =
                FindObjectOfType<LocationManager>();
        }

        if (tradeRouteVisualManager == null)
        {
            tradeRouteVisualManager =
                FindObjectOfType<TradeRouteVisualManager>();
        }

        if (requestGenerator == null)
        {
            requestGenerator =
                FindObjectOfType<RequestGenerator>();
        }

        if (gameClock == null)
        {
            gameClock =
                FindObjectOfType<GameClock>();
        }

        if (shipMovement == null)
        {
            shipMovement =
                FindObjectOfType<ShipMovement>();
        }
    }

    private bool ValidateSystems()
    {
        bool valid = true;

        if (gameDatabase == null)
        {
            Debug.LogError(
                "BootUpGame: GameDatabase not found!"
            );

            valid = false;
        }

        if (locationSpawner == null)
        {
            Debug.LogError(
                "BootUpGame: RandomLocationSpawner not found!"
            );

            valid = false;
        }

        if (locationManager == null)
        {
            Debug.LogError(
                "BootUpGame: LocationManager not found!"
            );

            valid = false;
        }

        if (shipMovement == null)
        {
            Debug.LogError(
                "BootUpGame: ShipMovement not found!"
            );

            valid = false;
        }

        if (tradeRouteVisualManager == null)
        {
            Debug.LogWarning(
                "BootUpGame: TradeRouteVisualManager not found. " +
                "Trade route visuals will not be generated."
            );
        }

        if (gameClock == null)
        {
            Debug.LogWarning(
                "BootUpGame: GameClock not found!"
            );
        }

        if (requestGenerator == null)
        {
            Debug.LogWarning(
                "BootUpGame: RequestGenerator not found!"
            );
        }

        return valid;
    }

    private Location FindVanderbiltLocation()
    {
        if (locationManager == null)
            return null;

        if (locationManager.locations == null)
            return null;

        foreach (
            Location location
            in locationManager.locations
        )
        {
            if (location == null)
                continue;

            if (location.locationType == null)
                continue;

            if (
                location.locationType.LocationType ==
                "Vanderbilt"
            )
            {
                return location;
            }
        }

        return null;
    }

    private void SetShipStartingLocation(
        Location vanderbiltLocation
    )
    {
        if (shipMovement == null)
            return;

        if (vanderbiltLocation == null)
            return;

        shipMovement.currentLocation =
            vanderbiltLocation;

        shipMovement.transform.position =
            vanderbiltLocation.transform.position;

        shipMovement.targetLocation =
            null;

        //shipMovement.route.Clear();

        Debug.Log(
            "Ship starting location set to: " +
            vanderbiltLocation.GetDisplayName()
        );
    }
}