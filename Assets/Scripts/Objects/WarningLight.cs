using UnityEditorInternal;
using UnityEngine;


public class WarningLight : MonoBehaviour
{
    public Material LightMaterial;
    Animator Animator;
    int RedHash;
    int PurpleHash;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            RedLight();
            RedHash = Animator.StringToHash("Base Layer.LightFlashRed");
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            PurpleLight();
            PurpleHash = Animator.StringToHash("Base Layer.LightFlashPurple");
        }
    }


    void RedLight()
    {
        //var material = GetComponent<WarningLight>();
        LightMaterial.EnableKeyword("_EMISSION");
        LightMaterial.SetColor("_EmissionColor", Color.red);
        LightMaterial.SetColor("_Color", Color.red);
        Animator.Play(RedHash);
    }

    void PurpleLight()
    {
        LightMaterial.EnableKeyword("_EMISSION");
        LightMaterial.SetColor("_EmissionColor", Color.purple);
        LightMaterial.SetColor("_Color", Color.purple);
        Animator.Play(PurpleHash);
    }
}
