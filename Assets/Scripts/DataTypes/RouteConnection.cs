using System;
using UnityEngine;

[Serializable]
public class RouteConnection
{
    public Location locationA;
    public Location locationB;

    [Header("Route")]
    public float distance;

    public int dangerLevel;

    public float fuelCost;

    public bool blocked;
}