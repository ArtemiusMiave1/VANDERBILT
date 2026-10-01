using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [Header("Audio Source")]
    [SerializeField]
    private AudioSource SFXSource;


    [Header("Request Sounds")]
    public AudioClip Request;
    public AudioClip RequestComplete;


    [Header("Fax Sounds")]
    public AudioClip FaxPrint;
    public AudioClip FaxPurchase;


    [Header("World Sounds")]
    public AudioClip Thunder;
    public AudioClip Landing;


    // =========================================================
    // PLAY SOUND EFFECT
    // =========================================================

    public void PlaySFX(
        AudioClip clip
    )
    {
        if (SFXSource == null)
            return;

        if (clip == null)
            return;

        SFXSource.PlayOneShot(
            clip
        );
    }
}