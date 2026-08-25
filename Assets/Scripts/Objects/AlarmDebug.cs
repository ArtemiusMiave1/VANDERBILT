using System.Collections;
using UnityEngine;

public class AlarmDebug : MonoBehaviour
{
    public V2WarningLight v2warningLight;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(ChangeText(30f));
    }

    IEnumerator ChangeText(float halt)
    {

        //Start of text change
        yield return new WaitForSeconds(halt);
        debugalarm();
        print("ALARMMMMMM");
    }


    void debugalarm()
    {
        v2warningLight.CurrentColor = "Purple";
    }

}
