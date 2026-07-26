using UnityEngine;

public static class CSVValidator
{
    public static bool CheckEmpty(
        string value,
        string file,
        string column,
        int row)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            Debug.LogWarning(
                "EMPTY CSV CELL\n" +
                "File: " + file +
                "\nRow: " + row +
                "\nColumn: " + column
            );

            return true;
        }

        return false;
    }
}