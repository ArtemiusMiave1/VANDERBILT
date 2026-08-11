using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;


// Loads CSV files and converts them into C# objects
public static class CSVLoader
{

    // Loads a CSV file and converts each row into an object of type T
    public static List<T> Load<T>(string fileName)
    {
        // Create an empty list to store the loaded data
        List<T> data = new List<T>();

        // Load CSV file from Resources/CSV folder
        TextAsset file =
            Resources.Load<TextAsset>("CSV/" + fileName);


        // Check if the file exists
        if (file == null)
        {
            Debug.LogError(
                "CSV Missing: " + fileName
            );

            return data;
        }


        // Split the CSV into separate rows
        string[] rows =
            file.text.Split(
                new string[] { "\n" },
                StringSplitOptions.RemoveEmptyEntries);



        // The first row contains the column names
        string[] headers =
            rows[0].Split(',');



        // Loop through every row after the header
        for (int row = 1; row < rows.Length; row++)
        {

            // Split the current row into individual values
            string[] values =
                rows[row].Split(',');

            // Ignore completely empty rows
            if (IsRowEmpty(values))
            {
//                Debug.Log("Ignored empty CSV row: " + row);

                continue;
            }

            // Create a new object of type T
            T item = Activator.CreateInstance<T>();


            // Get all variables inside the class
            // Example: id, name, reward, etc.
            FieldInfo[] fields =
                typeof(T).GetFields();



            // Fill each variable with CSV data
            foreach (FieldInfo field in fields)
            {

                // Find which CSV column matches this variable name
                int column =
                    Array.IndexOf(
                        headers,
                        field.Name);

                // If the column does not exist, show warning
                if (column == -1)
                {
                    Debug.LogWarning(
                        "Missing column: "
                        + field.Name
                    );

                    continue;
                }

                // Get the value from the CSV cell
                string value =
                    values[column];

                // Check if the cell is empty
                if (CSVValidator.CheckEmpty(
                    value,
                    fileName,
                    field.Name,
                    row))
                {
                    continue;
                }

                // Convert the text value into the correct datatype
                // Example: "10" becomes int 10
                object converted =
                    ConvertValue(
                        value,
                        field.FieldType);

                // Put the converted value into the object variable
                field.SetValue(
                    item,
                    converted);
            }
            // Add the completed object to the list
            data.Add(item);
        }
        // Return all loaded data
        return data;
    }





    // Converts CSV text into the correct C# datatype
    static object ConvertValue(
        string value,
        Type type)
    {

        // Convert text into an integer
        if (type == typeof(int))
            return int.Parse(value);


        // Convert text into a decimal number
        if (type == typeof(float))
            return float.Parse(value);


        // Convert text into true/false
        if (type == typeof(bool))
            return bool.Parse(value);


        // Keep text as a string
        if (type == typeof(string))
            return value;


        // Convert text into an enum value
        if (type.IsEnum)
            return Enum.Parse(type, value);


        // Return nothing if the datatype is unsupported
        return null;
    }

    // Checks if every cell in a row is empty
    static bool IsRowEmpty(string[] values)
    {
        foreach (string value in values)
        {
            // If any cell contains text, the row is valid
            if (!string.IsNullOrWhiteSpace(value))
            {
                return false;
            }
        }

        // Every cell was empty
        return true;
    }
}