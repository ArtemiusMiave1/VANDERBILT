using TMPro;
using UnityEngine;

public class CargoDisplay : MonoBehaviour
{
    [Header("Ship Cargo")]
    public ShipCargo shipCargo;

    [Header("Cargo Text")]
    public TMP_Text foodText;
    public TMP_Text medicineText;
    public TMP_Text machineryText;
    public TMP_Text fuelText;
    public TMP_Text goldText;
    public TMP_Text weaponsText;

    [Header("Cargo Weight")]
    public TMP_Text cargoWeightText;

    [Header("Cargo Settings")]
    public float maximumCargoWeight = 750f;

    [Header("Resource Weights")]
    public float foodWeight = 1f;
    public float medicineWeight = 1f;
    public float machineryWeight = 3f;
    public float fuelWeight = 1f;
    public float weaponsWeight = 2f;

    private void Update()
    {
        UpdateCargoDisplay();
    }

    public void UpdateCargoDisplay()
    {
        if (shipCargo == null)
            return;

        foodText.text =
            "Food: " + shipCargo.food;

        medicineText.text =
            "Medicine: " + shipCargo.medicine;

        machineryText.text =
            "Machinery: " + shipCargo.machinery;

        fuelText.text =
            "Fuel: " + shipCargo.fuel + " / 100";

        goldText.text =
            "Gold: " + shipCargo.gold;

        weaponsText.text =
            "Weapons: " + shipCargo.weapons;

        // Calculate total cargo weight
        float totalWeight = CalculateCargoWeight();

        cargoWeightText.text =
            "CARGO WEIGHT: " +
            totalWeight.ToString("0") +
            " / " +
            maximumCargoWeight.ToString("0");
    }

    private float CalculateCargoWeight()
    {
        float weight = 0f;

        weight += shipCargo.food * foodWeight;
        weight += shipCargo.medicine * medicineWeight;
        weight += shipCargo.machinery * machineryWeight;
        weight += shipCargo.fuel * fuelWeight;
        weight += shipCargo.weapons * weaponsWeight;

        return weight;
    }
}