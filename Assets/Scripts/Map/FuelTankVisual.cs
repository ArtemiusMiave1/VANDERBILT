using UnityEngine;

public class FuelTankVisual : MonoBehaviour
{
    [Header("References")]
    public ShipCargo shipCargo;
    public Transform fuelLiquid;

    [Header("Fuel")]
    [Tooltip("Maximum amount of fuel the tank can hold.")]
    public float maximumFuel = 100f;

    [Header("Liquid")]
    public float minimumHeight = 0.05f;
    public float maximumHeight = 1f;

    private Vector3 originalScale;


    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        if (shipCargo == null)
        {
            shipCargo =
                FindObjectOfType<ShipCargo>();
        }


        if (fuelLiquid != null)
        {
            originalScale =
                fuelLiquid.localScale;
        }


        UpdateFuelVisual();
    }


    // =========================================================
    // UPDATE
    // =========================================================

    private void Update()
    {
        UpdateFuelVisual();
    }


    // =========================================================
    // UPDATE FUEL VISUAL
    // =========================================================

    private void UpdateFuelVisual()
    {
        if (shipCargo == null)
            return;


        if (fuelLiquid == null)
            return;


        if (maximumFuel <= 0f)
            return;


        float fuelPercentage =
            shipCargo.fuel /
            maximumFuel;


        fuelPercentage =
            Mathf.Clamp01(
                fuelPercentage
            );


        float height =
            Mathf.Lerp(
                minimumHeight,
                maximumHeight,
                fuelPercentage
            );


        Vector3 newScale =
            originalScale;


        newScale.y =
            height;


        fuelLiquid.localScale =
            newScale;
    }
}