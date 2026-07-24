using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Profiling;

public class DialogueWindowPopup : EditorWindow
{

    [MenuItem("Window/Dialogue Window")]

    static void Intitlize()
    {
        DialogueWindowPopup window = EditorWindow.GetWindowWithRect(typeof(DialogueWindowPopup), new Rect(0, 0, 960, 540)) as DialogueWindowPopup;
        window.Show();
    }
    private void OnEnable()
    {
        var layouts = Resources.LoadAll<GameObject>("UILayouts");
        UILayouts = new string[layouts.Length];
        for (int i = 0; i < layouts.Length; i++)
        {
            UILayouts[i] = layouts[i].name;
        }
        UpdateGUIInfo();
    }


    string[] UILayouts;

    string[] profileID;

    int currentPIndex;

    int layoutSelectedIndex;
    int lastLayoutSelectedIndex;

    int PageSelectedIndex;
    int dialogueSetIndex;

    bool hasBeenSaved;

    string lastTextEdited;


    string[] dialogueSetCount;
    string[] pageCount;

    public static List<DialogueSet> profileCSV;
    ProfilesScriptableObject[] profiles;

    int lastPIndex;
    int lastDSIndex;

    public static bool windowSaved;


    void OnGUI()
    {
        if (windowSaved)
        {
            UpdateGUIInfo();
            windowSaved = false;
        }
        EditorGUILayout.BeginHorizontal();
        currentPIndex = EditorGUILayout.Popup("Character Profile:", currentPIndex, profileID);

        if (GUILayout.Button("Edit"))
        {
            NameEditPopup.Intitlize(profiles[currentPIndex]);
        }
        else if (GUILayout.Button("New"))
        {
            ProfileCreatePopup.Intitlize(profiles[0]);
        }
        if (lastPIndex != currentPIndex)
        {
            lastPIndex = currentPIndex;
            UpdateGUIInfo();
        }
        EditorGUILayout.EndHorizontal();

        profiles[currentPIndex].displayName = EditorGUILayout.TextField("Display Name: ", profiles[currentPIndex].displayName);

        layoutSelectedIndex = EditorGUILayout.Popup("Profile Layout:", layoutSelectedIndex, UILayouts);
        if (layoutSelectedIndex != lastLayoutSelectedIndex)
        {
            profiles[currentPIndex].layoutType = UILayouts[layoutSelectedIndex];
            lastLayoutSelectedIndex = layoutSelectedIndex;
        }

        EditorGUILayout.BeginHorizontal();
        profiles[currentPIndex].portraitOn = EditorGUILayout.Toggle("Portrait", profiles[currentPIndex].portraitOn);
        if (profiles[currentPIndex].portraitOn)
        {
            profiles[currentPIndex].portaitBorderOn = EditorGUILayout.Toggle("Portrait Border", profiles[currentPIndex].portaitBorderOn);
            if (profiles[currentPIndex].portaitBorderOn)
            {
                profiles[currentPIndex].defaultBorderOn = EditorGUILayout.Toggle("Default Border", profiles[currentPIndex].defaultBorderOn);
            }
        }
        else
        {
            profiles[currentPIndex].portaitBorderOn = false;
            profiles[currentPIndex].defaultBorderOn = false;
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (profiles[currentPIndex].portraitOn)
        {
            profiles[currentPIndex].portraitImage = EditorGUILayout.ObjectField("Current Portrait", profiles[currentPIndex].portraitImage, typeof(Texture2D), false) as Texture2D;
            if (!profiles[currentPIndex].defaultBorderOn && profiles[currentPIndex].portaitBorderOn)
            {
                profiles[currentPIndex].portraitBorder = EditorGUILayout.ObjectField("Current Portrait Border", profiles[currentPIndex].portraitBorder, typeof(Texture2D), false) as Texture2D;
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (profiles[currentPIndex].portaitBorderOn)
        {
            //EditorGUILayout.LabelField(currentPortrait, GUILayout.MaxWidth(200f), GUILayout.MaxHeight(200f));
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();

        dialogueSetIndex = EditorGUILayout.Popup("Dialogue Set:", dialogueSetIndex, dialogueSetCount);
        if (GUILayout.Button("+"))
        {
            DialogueSet newSet = new DialogueSet();
            string[] pagetext = { "" };
            newSet.lines = new List<string>();
            newSet.lines.AddRange(pagetext);
            profileCSV.Add(newSet);
            if (dialogueSetIndex < profileCSV.Count - 1)
            {
                dialogueSetIndex++;
            }
        }
        if (lastDSIndex != dialogueSetIndex)
        {
            lastDSIndex = dialogueSetIndex;
            UpdatePageCount();
        }
        PageSelectedIndex = EditorGUILayout.Popup("Page:", PageSelectedIndex, pageCount);
        if (GUILayout.Button("+"))
        {
            profileCSV[dialogueSetIndex].lines.Add("");
            UpdatePageCount();
            if (PageSelectedIndex < profileCSV.Count - 1)
            {
                PageSelectedIndex++;
            }
        }
        EditorGUILayout.EndHorizontal();


        EditorGUILayout.BeginHorizontal();
        if (dialogueSetCount.Length > 1)
        {
            if (GUILayout.Button("Delete Set"))
            {
                profileCSV.Remove(profileCSV[dialogueSetIndex]);
                if (dialogueSetIndex != 0)
                {
                    dialogueSetIndex--;
                }
                UpdatePageCount();
                hasBeenSaved = false;
            }
        }
        if (pageCount.Length > 1)
        {
            if (GUILayout.Button("Delete Page"))
            {
                    UpdatePageCount();
                profileCSV[dialogueSetIndex].lines.Remove(profileCSV[dialogueSetIndex].lines[PageSelectedIndex]);
                if (dialogueSetIndex != 0) {
                    PageSelectedIndex--;
                }
                hasBeenSaved = false;
            }
        }
        EditorGUILayout.EndHorizontal();
        GUIStyle textAreaStyle = GUI.skin.textArea;
        textAreaStyle.stretchHeight = true;
        lastTextEdited = profileCSV[dialogueSetIndex].lines[PageSelectedIndex];

        profileCSV[dialogueSetIndex].lines[PageSelectedIndex] = EditorGUILayout.TextArea(profileCSV[dialogueSetIndex].lines[PageSelectedIndex], textAreaStyle);

        if(lastTextEdited != profileCSV[dialogueSetIndex].lines[PageSelectedIndex])
        {
            hasBeenSaved = false;
        }

        if (!hasBeenSaved)
        {
            if (GUILayout.Button("Save"))
            {
                hasBeenSaved = true;
                CSVManager.SaveCSV(profileCSV, profileID[currentPIndex]);
            }
        }

        if (Selection.activeGameObject)
        {
            this.Repaint();
        }
    }

    void UpdateGUIInfo()
    {
        profiles = Resources.LoadAll<ProfilesScriptableObject>("CharacterProfiles");
        dialogueSetIndex = 0;
        PageSelectedIndex = 0;
        hasBeenSaved = true;


        profileID = new string[profiles.Length];
        for (int i = 0; i < profiles.Length; i++)
        {
            profileID[i] = profiles[i].name;
        }

        profileCSV = CSVManager.ReadCSV(profileID[currentPIndex]);

        dialogueSetCount = new string[profileCSV.Count];
        for (int i = 0; i < profileCSV.Count; i++)
        {
            dialogueSetCount[i] = Convert.ToString(i + 1);
        }

        pageCount = new string[profileCSV[dialogueSetIndex].lines.Count];
        for (int i = 0; i < profileCSV[dialogueSetIndex].lines.Count; i++)
        {
            pageCount[i] = Convert.ToString(i + 1);
        }

        for (int i = 0; i < UILayouts.Length; i++)
        {
            if (UILayouts[i] == profileID[currentPIndex])
            {
                layoutSelectedIndex = i;
                lastLayoutSelectedIndex = layoutSelectedIndex;
            }
        }
    }

    void UpdatePageCount()
    {
        dialogueSetCount = new string[profileCSV.Count];
        for (int i = 0; i < profileCSV.Count; i++)
        {
            dialogueSetCount[i] = Convert.ToString(i + 1);
        }

        pageCount = new string[profileCSV[dialogueSetIndex].lines.Count];
        for (int i = 0; i < profileCSV[dialogueSetIndex].lines.Count; i++)
        {
            pageCount[i] = Convert.ToString(i + 1);
        }
        PageSelectedIndex = 0;
    }
}
public class NameEditPopup : EditorWindow
{

    ProfilesScriptableObject profileAsset;
    string assetName;
    string resourceCSVPath;
    bool nameExists;
    public static void Intitlize(ProfilesScriptableObject profile)
    {
        NameEditPopup window = EditorWindow.GetWindowWithRect(typeof(NameEditPopup), new Rect(0, 0, 400, 80)) as NameEditPopup;
        window.Show();
        window.profileAsset = profile;
        window.assetName = profile.name;
        window.resourceCSVPath = "DialogueCSV/" + profile.name;

    }


    void OnGUI()
    {

        assetName = EditorGUILayout.TextField("Asset Name: ", assetName);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Save"))
        {
            nameExists = CSVManager.NameFileChecker(assetName);
            if (!nameExists)
            {
                string assetPath = AssetDatabase.GetAssetPath(profileAsset);
                AssetDatabase.RenameAsset(assetPath, assetName);

                TextAsset csv = Resources.Load<TextAsset>(resourceCSVPath);
                string csvPath = AssetDatabase.GetAssetPath(csv);
                AssetDatabase.RenameAsset(csvPath, assetName);

                DialogueWindowPopup.windowSaved = true;
                this.Close();
            }
        }
        else if (GUILayout.Button("Cancel"))
        {
            this.Close();
        }
        EditorGUILayout.EndHorizontal();
        if (nameExists)
        {
            GUI.contentColor = Color.red;
            EditorGUILayout.LabelField("This name already exists");
        }
    }
}

public class ProfileCreatePopup : EditorWindow
{

    ProfilesScriptableObject profileAsset;
    string assetName;
    string resourceCSVPath;
    bool nameExists;

    public static void Intitlize(ProfilesScriptableObject profile)
    {
        ProfileCreatePopup window = EditorWindow.GetWindowWithRect(typeof(ProfileCreatePopup), new Rect(0, 0, 400, 80)) as ProfileCreatePopup;
        window.Show();
        window.profileAsset = profile;
        window.resourceCSVPath = "DialogueCSV/" + profile.name;
    }


    void OnGUI()
    {

        assetName = EditorGUILayout.TextField("Asset Name: ", assetName);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Save"))
        {
            nameExists = CSVManager.NameFileChecker(assetName);
            if (!nameExists)
            {
                string assetPath = AssetDatabase.GetAssetPath(profileAsset);
                assetPath = assetPath.Replace(profileAsset.name, assetName);
                Debug.Log(assetPath);
                ProfilesScriptableObject newObject = new ProfilesScriptableObject();
                AssetDatabase.CreateAsset(newObject, assetPath);

                TextAsset csv = Resources.Load<TextAsset>(resourceCSVPath);
                string csvPath = AssetDatabase.GetAssetPath(csv);
                csvPath = csvPath.Replace(csv.name, assetName);
                File.WriteAllText(csvPath, "");

                DialogueWindowPopup.windowSaved = true;
                this.Close();
            }
        }
        else if (GUILayout.Button("Cancel"))
        {
            this.Close();
        }
        EditorGUILayout.EndHorizontal();
        if (nameExists)
        {
            GUI.contentColor = Color.red;
            EditorGUILayout.LabelField("This name already exists");
        }
    }
}