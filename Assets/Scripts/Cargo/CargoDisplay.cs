using TMPro;
using UnityEngine;

public class CargoDisplay : MonoBehaviour
{
    [Header("Ship Cargo")]
    public ShipCargo shipCargo;

    [Header("Cargo Text")]
    public TMP_Text foodText;
    public TMP_Text coalText;
    public TMP_Text medicineText;
    public TMP_Text machineryText;
    public TMP_Text oreText;
    public TMP_Text fuelText;
    public TMP_Text goldText;
    public TMP_Text weaponsText;


    private void Update()
    {
        UpdateCargoDisplay();
    }


    public void UpdateCargoDisplay()
    {
        foodText.text = "Food: " + shipCargo.food;
        coalText.text = "Coal: " + shipCargo.coal;
        medicineText.text = "Medicine: " + shipCargo.medicine;
        machineryText.text = "Machinery: " + shipCargo.machinery;
        oreText.text = "Ore: " + shipCargo.ore;
        fuelText.text = "Fuel: " + shipCargo.fuel;
        goldText.text = "Gold: " + shipCargo.gold;
        weaponsText.text = "Weapons: " + shipCargo.weapons;
    }
}