using Unity.VisualScripting;
using UnityEngine;

public class V2WarningLight : MonoBehaviour
{

    //public bool RedLightToggle;
    //public bool PurpleLightToggle;
    public string CurrentColor; // change color here
    public Material materialVar;
    public Animator animatorVar;
    public AudioSource AlarmSnd;
    int RedHash;
    int PurpleHash;
    int OffHash;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Animator animatorVar = GetComponent<Animator>();
        PurpleHash = Animator.StringToHash("Base Layer.PurpleLight");
        RedHash = Animator.StringToHash("Base Layer.RedLight");
        OffHash = Animator.StringToHash("Base Layer.BothOff");


    }

    // Update is called once per frame
    void Update()

    {   switch (CurrentColor)
        {
            case "Purple":
                Light("Purple");
                break;
            case "Red":
                Light("Red");
                break;
            case "Neither":
                LightOff("Both");
                break;

        }
        
        // debug toggles in editor
        //if (RedOurPurple)
        //{
           // Light("Red");
           // PurpleLightToggle = false;
        //}

        //if (PurpleLightToggle)
        //{
            //Light("Purple");
            //RedLightToggle = false;
       // }

        //else
        //{
        //    LightOff("Both");
        //}
    }

    void Light(string colorType)
    {
        switch (colorType)
        {
            case "Purple":
                //print("lol");
                LightOff("Red");
                animatorVar.Play(PurpleHash);
                break;
            case "Red":
               // print("aaa");
                LightOff("Purple");
                animatorVar.Play(RedHash);
                break;
                AlarmSnd.Play(); 
        }

    }

    void LightOff(string TurnOff)
    {
        switch (TurnOff)
        {
            case "Purple":
               // print("a");
                break;
            case "Red":
               // print("b");
                break;
            case "Both":
               // print("both offff");
                animatorVar.Play(OffHash);
                break;

                AlarmSnd.Stop();

        }
    }
}
