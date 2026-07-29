using System.Collections.Generic;
using UnityEngine;

public class GameDatabase : MonoBehaviour
{
    // Singleton instance
    public static GameDatabase Instance { get; private set; }

    // Loaded data
    public List<RequestData> Requests = new List<RequestData>();
    public List<FactionData> Factions = new List<FactionData>();
    //public List<ResourceData> Resources = new List<ResourceData>();
    //public List<EventData> Events = new List<EventData>();

    private void Awake()
    {
        // Make sure there is only one GameDatabase
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        LoadDatabase();
    }

    /// <summary>
    /// Loads all CSV files.
    /// </summary>
    public void LoadDatabase()
    {
        Debug.Log("Loading Game Database...");

        Requests = CSVLoader.Load<RequestData>("Requests");
        Factions = CSVLoader.Load<FactionData>("Factions");
        //Resources = CSVLoader.Load<ResourceData>("Resources");
        //Events = CSVLoader.Load<EventData>("Events");

        Debug.Log("Database Loaded!");
        Debug.Log($"Requests: {Requests.Count}");
        Debug.Log($"Factions: {Factions.Count}");
        //Debug.Log($"Resources: {Resources.Count}");
        //Debug.Log($"Events: {Events.Count}");
    }
}