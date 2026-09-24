using UnityEngine;

[CreateAssetMenu(
    fileName = "LocationData",
    menuName = "Vanderbilt/Location Data"
)]
public class LocationData : ScriptableObject
{
    [Header("Location")]
    public string LocationType;

    [Header("District")]
    public string DistrictType;

    public bool District;

    [Header("Limit")]
    [Tooltip("Maximum number of this location type that can exist.")]
    public int Limit = 0;
}