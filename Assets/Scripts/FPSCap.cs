using UnityEngine;

public class FPSCap : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Application.targetFrameRate = 60;
        print("capped to" + Application.targetFrameRate);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
