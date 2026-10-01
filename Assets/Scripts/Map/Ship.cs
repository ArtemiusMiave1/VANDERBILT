using UnityEngine;

public class Ship : MonoBehaviour
{
    public static Ship Instance;


    // =========================================================
    // SHIP STATE
    // =========================================================

    public enum ShipState
    {
        Stopped,
        Moving
    }

    public enum ShipEnvironment
    {
        Clear,
        Storm
    }


    [Header("Ship State")]
    public ShipState currentState =
        ShipState.Stopped;


    // =========================================================
    // REFERENCES
    // =========================================================

    [Header("References")]
    public ShipMovement shipMovement;


    // =========================================================
    // PRIVATE VARIABLES
    // =========================================================

    private ShipState previousState;


    // =========================================================
    // AWAKE
    // =========================================================

    private void Awake()
    {
        if (
            Instance != null &&
            Instance != this
        )
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }


    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        // Try finding ShipMovement on this object.
        if (shipMovement == null)
        {
            shipMovement =
                GetComponent<ShipMovement>();
        }


        // Try finding ShipMovement in the scene.
        if (shipMovement == null)
        {
            shipMovement =
                FindObjectOfType<ShipMovement>();
        }


        // -----------------------------------------------------
        // START STOPPED
        // -----------------------------------------------------

        currentState =
            ShipState.Stopped;

        previousState =
            currentState;


        // IMPORTANT:
        // Force the GameClock to update immediately.
        //
        // This means the game starts with time stopped.
        UpdateGameClock();


        Debug.Log(
            "Ship starting state: " +
            currentState
        );
    }


    // =========================================================
    // UPDATE
    // =========================================================

    private void Update()
    {
        UpdateShipState();
    }


    // =========================================================
    // UPDATE SHIP STATE
    // =========================================================

    private void UpdateShipState()
    {
        if (shipMovement == null)
            return;


        bool physicallyMoving =
            shipMovement.IsMoving() &&
            shipMovement.GetCurrentSpeed() > 0f;


        // -----------------------------------------------------
        // MOVING
        // -----------------------------------------------------

        if (physicallyMoving)
        {
            SetShipState(
                ShipState.Moving
            );
        }


        // -----------------------------------------------------
        // STOPPED
        // -----------------------------------------------------

        else
        {
            SetShipState(
                ShipState.Stopped
            );
        }
    }


    // =========================================================
    // SET SHIP STATE
    // =========================================================

    public void SetShipState(
        ShipState newState
    )
    {
        // Don't repeatedly set the same state.
        if (
            currentState ==
            newState
        )
        {
            return;
        }


        previousState =
            currentState;

        currentState =
            newState;


        Debug.Log(
            "Ship State changed: " +
            previousState +
            " -> " +
            currentState
        );


        UpdateGameClock();
    }


    // =========================================================
    // GAME CLOCK
    // =========================================================

    private void UpdateGameClock()
    {
        if (GameClock.Instance == null)
        {
            Debug.LogWarning(
                "Ship: GameClock not found."
            );

            return;
        }


        // -----------------------------------------------------
        // MOVING
        // -----------------------------------------------------

        if (
            currentState ==
            ShipState.Moving
        )
        {
            GameClock.Instance.StartClock();

            Debug.Log(
                "Ship moving - GameClock started."
            );

            return;
        }


        // -----------------------------------------------------
        // STOPPED
        // -----------------------------------------------------

        if (
            currentState ==
            ShipState.Stopped
        )
        {
            GameClock.Instance.StopClock();

            Debug.Log(
                "Ship stopped - GameClock stopped."
            );
        }
    }


    // =========================================================
    // GETTERS
    // =========================================================

    public ShipState GetShipState()
    {
        return currentState;
    }


    public bool IsMoving()
    {
        return currentState ==
               ShipState.Moving;
    }


    public bool IsStopped()
    {
        return currentState ==
               ShipState.Stopped;
    }
}