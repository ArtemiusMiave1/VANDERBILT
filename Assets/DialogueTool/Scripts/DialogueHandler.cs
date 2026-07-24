using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueHandler : MonoBehaviour
{

    public static bool inDialogue;
    public static Dictionary<string, int> dialogueSetIndex = new Dictionary<string, int>();

    string[] profileIDs;
    List<DialogueSet> characterText;
    string characterName;
    int dialoguePage = 0;
    TextMeshProUGUI dialogueBox;
    GameObject spawnedLayout;



    void Start()
    {
        var profiles = Resources.LoadAll<ProfilesScriptableObject>("CharacterProfiles");
        profileIDs = new string[profiles.Length];
        for (int i = 0; i < profiles.Length; i++)
        {
            profileIDs[i] = profiles[i].name;
            dialogueSetIndex.Add(profileIDs[i], 0);
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!inDialogue)
        {
            
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    var selectedCharcter = hit.transform.gameObject;
                    if (selectedCharcter != null)
                    {
                        for (int i = 0; i < profileIDs.Length; i++)
                        {
                            if (selectedCharcter.gameObject.name == profileIDs[i])
                            {
                                characterName = selectedCharcter.gameObject.name;
                                LayoutSpawner(selectedCharcter.gameObject.name);
                                TextUpdater(characterName);
                                inDialogue = true;
                            }
                        }
                    }
                }
        }
            else
            {
                TextUpdater(characterName);
            }
        }

    }

    void LayoutSpawner(string selectedCharacter)
    {
        ProfilesScriptableObject profile = Resources.Load<ProfilesScriptableObject>("CharacterProfiles/" + selectedCharacter);
        GameObject UILayout = Resources.Load<GameObject>("UILayouts/" + profile.layoutType);

        spawnedLayout = Instantiate(UILayout, new Vector3(0, 0, 0), Quaternion.identity);

        RawImage[] UIImages;
        UIImages = spawnedLayout.GetComponentsInChildren<RawImage>();
        foreach (RawImage image in UIImages)
        {
            if(image.tag == "Portrait" && profile.portraitOn)
            {
                image.texture = profile.portraitImage;
            }
            else if (image.tag == "Portrait" && !profile.portraitOn)
            {
                image.enabled = false;
            }
            else if (image.tag == "PortraitBorder" && profile.portaitBorderOn && !profile.defaultBorderOn)
            {
                image.texture = profile.portraitBorder;
            }
            else if (image.tag == "PortraitBorder" && !profile.portaitBorderOn)
            {
                image.enabled = false;
            }
        }

        TextMeshProUGUI[] textBoxes;
        textBoxes = spawnedLayout.GetComponentsInChildren<TextMeshProUGUI>();
        foreach (TextMeshProUGUI textBox in textBoxes)
        {
            if (textBox.tag == "DialogueName")
            {
                textBox.text = profile.displayName;
            }
            else if (textBox.tag == "DialogueTextBox")
            {
                dialogueBox = textBox;
            }

        }
    }


    void TextUpdater(string characterName)
    {
        if (characterText == null)
        {
            characterText = CSVManager.ReadCSV(characterName);
        }
        int dialogueSet = dialogueSetIndex[characterName];
        if (dialoguePage >= characterText[dialogueSet].lines.Count)
        {
            Destroy(spawnedLayout);
            spawnedLayout = null;

            if (dialogueSet < characterText.Count - 1)
            {
                dialogueSetIndex[characterName]++;
            }

            inDialogue = false;
            characterText = null;

            dialoguePage = 0;

            return;
        }
        dialogueBox.text = characterText[dialogueSet].lines[dialoguePage];
        dialoguePage++;
    }
}

