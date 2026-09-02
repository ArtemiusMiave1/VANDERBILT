[System.Serializable]
public class RouteConnection
{
    public Location destination;

    public float distance;

    //[Range(0, 5)]
    public int dangerLevel;

    public float fuelCost;

    public bool blocked;
}