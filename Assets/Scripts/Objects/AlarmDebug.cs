using System.Collections;
using UnityEngine;

public class AlarmDebug : MonoBehaviour
{
    public V2WarningLight v2warningLight;
    public VentMinIGame ventScript;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(StartAlarm(20f));
    }

    IEnumerator StartAlarm(float halt)
    {

        //Start of text change
        yield return new WaitForSeconds(halt);
        debugalarm();
        print("ALARMMMMMM");
    }


    void debugalarm()
    {   if (v2warningLight.CurrentColor != "Purple")
        {
            v2warningLight.CurrentColor = "Purple";
            ventScript.ResetWheel();
            print("vent reset");
        }
        StartCoroutine(StartAlarm(20f));
    }

}
