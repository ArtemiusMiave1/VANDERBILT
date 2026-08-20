using UnityEngine;

public class BootUpGame : MonoBehaviour
{
    public RandomLocationSpawner locationSpawner;
    public LocationManager manager;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        locationSpawner.SpawnLocations();
        manager.FindLocations();
    }
    void Start()
    {
        manager.CreateConnections();
        Debug.Log("test");
        //locationSpawner.AssignLocationTypes();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
