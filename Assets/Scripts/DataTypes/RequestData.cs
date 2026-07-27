using System;

[Serializable]
public class RequestData
{
    public string Title;

    public string Faction;

    public string RequestedResources;

    public string Reward;

    public float TimeLimit;

    public string Dialogue;

    public string Consequences;

    public string ConsequencesAmount;

    public bool Completed;

    public bool Repeatable;

    public string Unlockables;
}