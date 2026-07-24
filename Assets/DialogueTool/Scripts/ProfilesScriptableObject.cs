using UnityEngine;

[CreateAssetMenu(fileName = "ProfilesScriptableObject", menuName = "ScriptableObjects/CharacterProfiles")]
public class ProfilesScriptableObject : ScriptableObject
{
    public string displayName;
    public Texture2D portraitImage;
    public Texture2D portraitBorder;
    public string layoutType;
    public bool portraitOn, portaitBorderOn, defaultBorderOn = true;
}
