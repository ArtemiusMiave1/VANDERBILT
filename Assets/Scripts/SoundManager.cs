using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] AudioSource SFXSource;

    public AudioClip Request;
    public AudioClip RequestComplete;
    public AudioClip FaxPrint;
    public AudioClip FaxPurchase;

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
