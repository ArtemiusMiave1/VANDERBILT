using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public static class CSVManager
{
    
    public static List<DialogueSet> ReadCSV(string csvName)
    {
        List<DialogueSet> characterData = new List<DialogueSet>();
        string filePath = "DialogueCSV/" + csvName;
        var csvToLoad = Resources.Load<TextAsset>(filePath);
        string[] characterCSV = csvToLoad.text.Split(new string[] { "\n" }, StringSplitOptions.None);
        foreach (string s in characterCSV)
        {
            DialogueSet set = new DialogueSet();
            set.lines = new List<string>();
            set.lines.AddRange(s.Split(','));
            characterData.Add(set);
        }
        return characterData;
    }

    public static void SaveCSV(List<DialogueSet> profileText, string profileID)
    {
        string filePath = "DialogueCSV/" + profileID;
        var csvToSave = Resources.Load<TextAsset>(filePath);
        string csvPath = AssetDatabase.GetAssetPath(csvToSave);

        string textToSave = "";

        for (int i = 0; i < profileText.Count; i++)
        {
            for (int j = 0; j < profileText[i].lines.Count; j++)
            {
                textToSave += profileText[i].lines[j];
                if (j < profileText[i].lines.Count - 1)
                {
                    textToSave += ",";
                }
            }
            if (i < profileText.Count - 1)
            {
                textToSave += "\n";
            }
        }

        File.WriteAllText(csvPath, textToSave);
        AssetDatabase.Refresh();
    }

    public static bool NameFileChecker(string fileName)
    {
        TextAsset[] currentFiles = Resources.LoadAll<TextAsset>("DialogueCSV/");
        for (int i = 0; i < currentFiles.Length; i++)
        {
            if (currentFiles[i].name == fileName) { return true; }
        }
        return false;
    }
}

[System.Serializable]
public class DialogueSet
{
    public List<String> lines;
}
