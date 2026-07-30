using UnityEngine;

public class LocationLineRenderer : MonoBehaviour
{
    public Location location;
    public bool button = false;

    void Start()
    {
        location = GetComponent<Location>();

        DrawConnections();
    }

    private void Update()
    {
        if (button == true)
        {
            DrawConnections();
            button = false;
        }
    }


    void DrawConnections()
    {
        foreach (Location connected in location.connections)
        {
            GameObject lineObject = new GameObject(
                "Route Line"
            );

            LineRenderer line =
                lineObject.AddComponent<LineRenderer>();


            line.positionCount = 2;

            line.SetPosition(
                0,
                transform.position
            );

            line.SetPosition(
                1,
                connected.transform.position
            );


            line.startWidth = 0.05f;
            line.endWidth = 0.05f;


            line.material =
                new Material(
                    Shader.Find("Sprites/Default")
                );
        }
    }
}