using UnityEngine;

public class ShipMovement : MonoBehaviour
{
    public float speed = 5f;

    public Location currentLocation;
    public Location targetLocation;

    bool moving;

    private void Update()
    {
        if (!moving || targetLocation == null)
            return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetLocation.transform.position,
            speed * Time.deltaTime);

        if (Vector3.Distance(
            transform.position,
            targetLocation.transform.position) < 0.05f)
        {
            transform.position = targetLocation.transform.position;

            currentLocation = targetLocation;
            targetLocation = null;

            moving = false;

            Debug.Log("Arrived at " + currentLocation.name);
        }
    }

    public void TravelTo(Location destination)
    {
        if (moving)
            return;

        // Can only travel to connected locations
        if (!currentLocation.connections.Contains(destination))
        {
            Debug.Log(destination.name + " is not connected to " + currentLocation.name);
            return;
        }

        targetLocation = destination;
        moving = true;
    }
    //public void TravelTo(Location destination)
    //{
    //    if (moving)
    //        return;

    //    targetLocation = destination;
    //    moving = true;
    //}
}