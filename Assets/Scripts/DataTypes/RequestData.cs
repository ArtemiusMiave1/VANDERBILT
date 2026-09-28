using System;
using UnityEngine;

[Serializable]
public class RequestData
{
    public string Title;

    public string Faction;

    public string RequestedResources;
    
    public int RequestedAmount;

    public string Reward;

    public int RewardAmount;

    public float TimeLimit;

    public string Dialogue;

    public string Consequences;

    public string ConsequencesAmount;

    public bool Completed;

    public bool Repeatable;

    public string LocationType;

    public string Unlockables;

    [Header("Requirement")]
    public RequestRequirement requirement;
}