using System;
using System.Collections.Generic;
using UnityEngine;

public static class CSVManager2
{
    public static List<RequestData> ReadRequests(string fileName)
    {
        List<RequestData> requests = new List<RequestData>();

        // Load CSV from Resources/CSV/
        TextAsset csvFile = Resources.Load<TextAsset>("CSV/" + fileName);

        if (csvFile == null)
        {
            Debug.LogError("CSV file not found: " + fileName);
            return requests;
        }

        // Split into rows
        string[] rows = csvFile.text.Split(
            new string[] { "\r\n", "\n" },
            StringSplitOptions.RemoveEmptyEntries);

        // Skip header row
        for (int i = 1; i < rows.Length; i++)
        {
            string[] columns = rows[i].Split(',');

            if (columns.Length < 8)
            {
                Debug.LogWarning("Skipping invalid row: " + rows[i]);
                continue;
            }

            RequestData request = new RequestData();

            request.id = int.Parse(columns[0]);
            request.faction = columns[1];
            request.title = columns[2];
            request.resource = columns[3];
            request.amount = int.Parse(columns[4]);
            request.reward = int.Parse(columns[5]);
            request.pollution = int.Parse(columns[6]);
            request.hope = int.Parse(columns[7]);

            requests.Add(request);
        }

        return requests;
    }
}