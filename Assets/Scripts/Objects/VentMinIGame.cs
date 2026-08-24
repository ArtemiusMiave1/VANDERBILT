using UnityEngine;

public class VentMinIGame : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public GameObject Vent;
    public GameObject ValveTurn;
    float rotationSpeed = 5f;
    [SerializeField] private float cur_HP;
    private const float max_HP = 100;
    private Vector3 PrevmousePos;
    //private bool SpaceIsPressed;
    // Start is called before the first frame update
    void Start()
    {
        cur_HP = max_HP;
    }

    // Update is called once per frame
    void Update()
    {
        //SpaceIsPressed = Input.GetKey(KeyCode.Space);
        Wheel();


    }

    void Wheel()
    {
        Vector3 mouseDelta = Input.mousePosition - PrevmousePos;

        if (mouseDelta.x > 0)
        {
            float amount = 0.01f;
            cur_HP -= mouseDelta.x * amount;
        }
        else if (mouseDelta.x < 0)
        {
            float amount = 0.01f;
            cur_HP += mouseDelta.x * amount;    
        }
        if (cur_HP <= 0)
        {
            cur_HP = 0;
            Debug.Log("You have unlocked");
        }
        if (cur_HP > 100)
        {
            cur_HP = 100;
        }
        ValveTurn.transform.localRotation = Quaternion.Euler(0, 0, (720 / max_HP * cur_HP));
        PrevmousePos = Input.mousePosition;
    }

    // Update is called once per frame
    //void Update()
    //{
    //    if (ValveTurn.transform.eulerAngles.z >= 30f && ValveTurn.transform.eulerAngles.z > -30f)
    //    {
    //        print("doneeee");
    //    }
    //}

    //void OnMouseDrag()
    //{
    //    float zRotation = Input.GetAxis("Mouse X") * rotationSpeed;
    //    ValveTurn.transform.Rotate(Vector3.forward, zRotation);
    //   // float xRotation = Input.GetAxis("Mouse Y") * rotationSpeed;
    //    //ValveTurn.transform.transform.Rotate(Vector3.forward, xRotation);
    //    print("spinninggggg");

    //}
}
