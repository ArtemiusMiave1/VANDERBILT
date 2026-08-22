using UnityEngine;

public class VentMinIGame : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public GameObject Vent;
    public GameObject ValveTurn;
    float rotationSpeed = 5f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (ValveTurn.transform.eulerAngles.z >= 30f && ValveTurn.transform.eulerAngles.z > -30f)
        {
            print("doneeee");
        }
    }

    void OnMouseDrag()
    {
        float zRotation = Input.GetAxis("Mouse X") * rotationSpeed;
        ValveTurn.transform.Rotate(Vector3.forward, zRotation);
        print("spinninggggg");

    }
}
