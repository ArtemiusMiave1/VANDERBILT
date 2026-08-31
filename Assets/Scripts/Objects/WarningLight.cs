//using UnityEditorInternal;
using UnityEngine;


public class WarningLight : MonoBehaviour
{
    public Material LightMaterial;
    Animator Animator;
    int RedHash;
    int PurpleHash;
    public bool toggle;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Animator = GetComponent<Animator>();
        PurpleHash = Animator.StringToHash("Base Layer.LightFlashPurple");
        RedHash = Animator.StringToHash("Base Layer.LightFlashRed");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && toggle == false)
        {
            RedLight();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && toggle == false)
        {
            PurpleLight();
        }


        if (!toggle)
        {
            RedHash = Animator.StringToHash("Base Layer.New State");
            PurpleHash = Animator.StringToHash("Base Layer.New State");
        }
    }


    void RedLight()
    {
        //var material = GetComponent<WarningLight>();
        LightMaterial.EnableKeyword("_EMISSION");
        LightMaterial.SetColor("_EmissionColor", Color.red);
        LightMaterial.SetColor("_Color", Color.red);
        Animator.Play(RedHash);
        toggle = true;
    }

    void PurpleLight()
    {
        LightMaterial.EnableKeyword("_EMISSION");
        LightMaterial.SetColor("_EmissionColor", Color.purple);
        LightMaterial.SetColor("_Color", Color.purple);
        Animator.Play(PurpleHash);
        toggle = true;
    }
}
