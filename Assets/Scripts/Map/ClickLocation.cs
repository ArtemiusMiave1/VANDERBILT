using UnityEngine;

public class ClickLocation : MonoBehaviour
{
    Location location;

    private void Awake()
    {
        location = GetComponent<Location>();
    }

    private void OnMouseDown()
    {
        ShipMovement ship = FindObjectOfType<ShipMovement>();

        //if (ship.currentLocation.connections.Contains(location))
        //{
        //    //ship.TravelTo(location);
        //}
    }

    //private void OnMouseDown()
    //{
    //    ShipMovement ship = FindObjectOfType<ShipMovement>();

    //    ship.TravelTo(GetComponent<Location>());
    //}
}