using System.Collections;
using UnityEngine;

public class StormTrigger : MonoBehaviour

{
    public Material skybox;
    public bool Toggle;
    public float ShaderDuration = 2f;
    float cloudintense;
    float thunderlerp;
    Color colorlarp;


    public float cloudintensity;
    public bool togglestorm = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //cloudintensity = skybox.GetFloat("_CloudIntensity");

    }

    // Update is called once per frame
    void Update()
    {
        if (Toggle == true && togglestorm == false)
        {
            togglestorm = true;
            StartCoroutine(ToggleThing());
        }
        if (Toggle == false && togglestorm == true)
        {
            togglestorm = false;
            StartCoroutine(DONTToggleThing());
        }
    }

    public IEnumerator ToggleThing()
    {
        float elapsed = 0;

        while (elapsed < ShaderDuration)
        {
            elapsed += Time.deltaTime;

            cloudintense = Mathf.Lerp(0.89f, 1, elapsed / ShaderDuration);
            thunderlerp = Mathf.Lerp(0, 1, elapsed / ShaderDuration);
            colorlarp = Color.Lerp(skybox.GetColor("_ColorDark"), new Color(0.464813f, 0.5019016f, 0.5320753f), elapsed / ShaderDuration);
            colorlarp = Color.Lerp(skybox.GetColor("_ColorBright"), new Color (0.240833f, 0.2774992f, 0.2830189f), elapsed / ShaderDuration);
            skybox.SetFloat("_CloudIntensity", cloudintense);
            skybox.SetInt("_ThunderToggle", 1);
            skybox.SetColor("_ColorDark", colorlarp);
            skybox.SetColor("_ColorBright", colorlarp);

            yield return null;
            
        }
    }

    public IEnumerator DONTToggleThing()
    {
        float elapsed = 0;

        while(elapsed < ShaderDuration)
        {
            elapsed += Time.deltaTime;

            cloudintense = Mathf.Lerp(1, 0.89f, elapsed / ShaderDuration);
            thunderlerp = Mathf.Lerp(1, 0, elapsed / ShaderDuration);
            colorlarp = Color.Lerp(skybox.GetColor("_ColorDark"), new Color(0.3675542f, 0.6019195f, 0.7886792f), elapsed / ShaderDuration);
            colorlarp = Color.Lerp(skybox.GetColor("_ColorBright"), new Color(0.2055891f, 0.5685202f, 0.6226414f), elapsed / ShaderDuration);
            skybox.SetFloat("_CloudIntensity", cloudintense);
            skybox.SetInt("_ThunderToggle", 0);
            skybox.SetColor("_ColorDark", colorlarp);
            skybox.SetColor("_ColorBright", colorlarp);

            yield return null;
        }

    }
}
