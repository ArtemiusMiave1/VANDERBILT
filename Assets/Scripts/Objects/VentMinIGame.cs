using UnityEngine;

public class VentMinIGame : MonoBehaviour
{
    // credit to the script https://stackoverflow.com/questions/56152299/realistic-valve-wheel-rotation-in-unity
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public GameObject Vent;
    public GameObject ValveTurn;
    Animator ventAnimator;
    public V2WarningLight v2warningLight;

    public float rotationSpeed = 0.05f;
    [SerializeField] private float cur_HP;

    private const float max_HP = 25;
    private Vector3 PrevmousePos;
    private bool ClickedOn;

    // Start is called before the first frame update
    void Start()
    {
        cur_HP = max_HP;
       ventAnimator = Vent.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0)) // to fix: make it so it only moves when the mouse is ON THE OBJECT !!!! players can click on the valve and move their mouse the other way
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform != null)
                {
                    print(hit); // this doesnt really matter cause the handle is the only one with collision lol
                    if (hit.transform.name == "pipe_handle")
                    {
                        ClickedOn = true;
                    }
                    else
                    {
                        ClickedOn = false;
                    }
                }
            }
        }
        if (!Input.GetMouseButton(0))
        {
            ClickedOn = false;
        }
        Wheel();
    }

        void Wheel()
        {
            Vector3 mouseDelta = Input.mousePosition - PrevmousePos;

            if (mouseDelta.x > 0 && ClickedOn)
            {
                cur_HP -= mouseDelta.x * rotationSpeed;
            }
            else if (mouseDelta.x < 0 && ClickedOn)
            {
                cur_HP += mouseDelta.x * rotationSpeed;
            }
            if (cur_HP <= 0)
            {
                cur_HP = 0;
                print("open vent");
                OpenVent();
            }
            if (cur_HP > 100)
            {
                cur_HP = 100;
            }
            ValveTurn.transform.localRotation = Quaternion.Euler(0, 0, (720 / max_HP * cur_HP)); // to fix: move only 80 degrees
            PrevmousePos = Input.mousePosition;
        }

       public void ResetWheel()
        {
            cur_HP = max_HP;
            ValveTurn.transform.localRotation = Quaternion.Euler(0, 0, 0);
            ventAnimator.SetBool("VentOpenBool", false);
        
        }

        void OpenVent()
        {
            ventAnimator.SetBool("VentOpenBool", true);
            v2warningLight.CurrentColor = "Neither";
            ResetWheel();
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
