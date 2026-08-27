using System;
using System.Collections.Generic;
using System.Reflection;
using System.Globalization;
using UnityEngine;

public static class CSVLoader
{
    public static List<T> Load<T>(string fileName)
    {
        List<T> data = new List<T>();

        TextAsset file =
            Resources.Load<TextAsset>("CSV/" + fileName);

        if (file == null)
        {
            Debug.LogError("CSV Missing: " + fileName);
            return data;
        }

        // Split file into rows
        string[] rows =
            file.text.Split(
                new string[] { "\r\n", "\n" },
                StringSplitOptions.RemoveEmptyEntries
            );

        if (rows.Length == 0)
        {
            Debug.LogWarning("CSV is empty: " + fileName);
            return data;
        }

        // Read headers using the same CSV parser
        string[] headers = ParseCSVLine(rows[0]);

        // Loop through rows
        for (int row = 1; row < rows.Length; row++)
        {
            string[] values = ParseCSVLine(rows[row]);

            // Ignore empty rows
            if (IsRowEmpty(values))
            {
                continue;
            }

            T item = Activator.CreateInstance<T>();

            FieldInfo[] fields =
                typeof(T).GetFields();

            foreach (FieldInfo field in fields)
            {
                // Find matching column
                int column =
                    Array.IndexOf(
                        headers,
                        field.Name
                    );

                if (column == -1)
                {
                    Debug.LogWarning(
                        $"Missing column '{field.Name}' in {fileName}"
                    );

                    continue;
                }

                // Make sure the row actually contains this column
                if (column >= values.Length)
                {
                    Debug.LogWarning(
                        $"Row {row} is missing value for '{field.Name}'"
                    );

                    continue;
                }

                string value =
                    values[column].Trim();

                // Empty value
                if (string.IsNullOrWhiteSpace(value))
                {
                    continue;
                }

                try
                {
                    object converted =
                        ConvertValue(
                            value,
                            field.FieldType
                        );

                    field.SetValue(
                        item,
                        converted
                    );
                }
                catch (Exception e)
                {
                    Debug.LogError(
                        $"CSV Error in {fileName}, " +
                        $"row {row}, field '{field.Name}', " +
                        $"value '{value}': {e.Message}"
                    );
                }
            }

            data.Add(item);
        }

        return data;
    }


    // --------------------------------------------------
    // CSV PARSER
    // --------------------------------------------------

    private static string[] ParseCSVLine(string line)
    {
        List<string> values = new List<string>();

        bool insideQuotes = false;
        string currentValue = "";

        for (int i = 0; i < line.Length; i++)
        {
            char character = line[i];

            // Quote
            if (character == '"')
            {
                // Double quote inside quoted text
                if (insideQuotes &&
                    i + 1 < line.Length &&
                    line[i + 1] == '"')
                {
                    currentValue += '"';
                    i++;
                }
                else
                {
                    insideQuotes = !insideQuotes;
                }

                continue;
            }

            // Comma outside quotes = new column
            if (character == ',' && !insideQuotes)
            {
                values.Add(currentValue);
                currentValue = "";
                continue;
            }

            currentValue += character;
        }

        // Add final value
        values.Add(currentValue);

        return values.ToArray();
    }


    // --------------------------------------------------
    // DATATYPE CONVERSION
    // --------------------------------------------------

    private static object ConvertValue(
        string value,
        Type type)
    {
        value = value.Trim();

        // INT
        if (type == typeof(int))
        {
            if (int.TryParse(
                value,
                NumberStyles.Integer,
                CultureInfo.InvariantCulture,
                out int result))
            {
                return result;
            }

            throw new FormatException(
                $"'{value}' is not a valid integer."
            );
        }


        // FLOAT
        if (type == typeof(float))
        {
            if (float.TryParse(
                value,
                NumberStyles.Float,
                CultureInfo.InvariantCulture,
                out float result))
            {
                return result;
            }

            throw new FormatException(
                $"'{value}' is not a valid float."
            );
        }


        // BOOL
        if (type == typeof(bool))
        {
            if (bool.TryParse(
                value,
                out bool result))
            {
                return result;
            }

            throw new FormatException(
                $"'{value}' is not a valid boolean. " +
                "Use TRUE or FALSE."
            );
        }


        // STRING
        if (type == typeof(string))
        {
            return value;
        }


        // ENUM
        if (type.IsEnum)
        {
            return Enum.Parse(
                type,
                value,
                true
            );
        }


        // Unsupported type
        Debug.LogWarning(
            $"Unsupported datatype: {type}"
        );

        return null;
    }


    // --------------------------------------------------
    // EMPTY ROW CHECK
    // --------------------------------------------------

    private static bool IsRowEmpty(string[] values)
    {
        foreach (string value in values)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                return false;
            }
        }

        return true;
    }
}