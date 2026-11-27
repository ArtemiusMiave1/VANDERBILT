using System;
using UnityEngine;

public enum RouteDangerLevel
{
    Normal = 0,
    Dangerous = 1,
    VeryDangerous = 2
}

[Serializable]
public class CustomRouteDanger
{
    public Location locationA;
    public Location locationB;

    public RouteDangerLevel dangerLevel =
        RouteDangerLevel.Normal;

    public bool Matches(Location a, Location b)
    {
        return
            (locationA == a && locationB == b) ||
            (locationA == b && locationB == a);
    }
}