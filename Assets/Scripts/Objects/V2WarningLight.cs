using Unity.VisualScripting;
using UnityEngine;

public class V2WarningLight : MonoBehaviour
{

    public bool RedLightToggle;
    public bool PurpleLightToggle;
    public Material materialVar;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Animator animatorVar = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {   // debug toggles in editor
        if (RedLightToggle)
        {
            Light("Red");
            LightOff("Purple");
            PurpleLightToggle = false;
        }

        if (PurpleLightToggle)
        {
            Light("Purple");
            LightOff("Red");
            RedLightToggle = false;
        }

        if (!RedLightToggle && !PurpleLightToggle)
        {
            LightOff("Both");
        }
    }

    void Light(string colorType)
    {
        switch (colorType)
        {
            case "Purple":
                print("lol");
                break;
            case "Red":
                print("aaa");
                break;
        }

    }

    void LightOff(string TurnOff)
    {
        switch (TurnOff)
        {
            case "Purple":
                print("a");
                break;
            case "Red":
                print("b");
                break;
            case "Both":
                print("both offff");
                break;


        }
    }
}
