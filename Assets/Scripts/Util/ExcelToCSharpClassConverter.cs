#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using UnityEngine;
using UnityEditor;

public static class ExcelToCSharpClassConverter
{
    [MenuItem("Tools/Convert Excel To Json")]
    public static void ConvertExcelToJsonTest()
    {
        ConvertExcelToJson("Sample.CSV");
    }


    // Function to convert .xlsx files to json and save them to a specific path
    public static void ConvertExcelToJson(string fileName)
    {
        string excelFilePath = Path.Combine(PathDefine.ExcelPath, fileName);
        string jsonFilePath = Path.Combine(PathDefine.JsonPath, Path.GetFileNameWithoutExtension(fileName) + ".json");



        string[] lines = File.ReadAllLines(excelFilePath);
        Debug.Log($"Number of rows: {lines.Length}");

        // Extract data types and variable names from the header row
        string[] columnHeaders = lines[0].Split(',');
        Debug.Log($"Number of columns: {columnHeaders.Length}");

        for (int i = 1; i < lines.Length; i++)
        {
            string[] row = lines[i].Split(',');
            if (row.Length != columnHeaders.Length)
            {
                Debug.LogError($"Number of columns in row {i + 1} does not match the header row");
                return;
            }
        }

        string[] dataTypes = new string[columnHeaders.Length];
        string[] variableNames = new string[columnHeaders.Length];

        for (int i = 0; i < columnHeaders.Length; i++)
        {
            string[] columnData = columnHeaders[i].Split(':');
            dataTypes[i] = columnData[0];
            variableNames[i] = columnData[1];

            Debug.LogError($"columnData : {columnData}");
            Debug.LogError($"dataTypes : {dataTypes[i]}");
            Debug.LogError($"variableNames : {variableNames[i]}");
        }

        // Create a list to hold the dictionaries for each row
        List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();

        // Loop through each row in the excel file and add the data to the rows list
        for (int i = 1; i < lines.Length; i++)
        {
            string[] row = lines[i].Split(',');
            Dictionary<string, object> rowData = new Dictionary<string, object>();

            for (int j = 0; j < row.Length; j++)
            {
                // 배열인지 먼저 확인
                if (dataTypes[j].Contains("["))
                {
                    string arrayDataType = dataTypes[j].Substring(0, dataTypes[j].IndexOf("["));
                    string[] arrayData = row[j].Split(';');

                    rowData.Add(variableNames[j], GetArrayData(arrayDataType, arrayData));
                }
                else
                {
                    rowData.Add(variableNames[j], GetData(dataTypes[j], row[j]));
                }
            }

            rows.Add(rowData);
        }

        // Serialize the rows list to json and save it to a file
        string jsonData = JsonUtility.ToJson(rows);
        File.WriteAllText(jsonFilePath, jsonData);
    }

    // Function to create a .cs file with the file name as the class name based on the json file saved in 1
    public static void CreateClassFromJson(string fileName)
    {
        string jsonFilePath = Path.Combine(PathDefine.JsonPath, fileName);
        string className = Path.GetFileNameWithoutExtension(jsonFilePath);
        string classFilePath = Path.Combine(PathDefine.DataClassPath, className + ".cs");

        // Read the json file and deserialize it to a list of dictionaries
        string jsonData = File.ReadAllText(jsonFilePath);
        List<Dictionary<string, object>> rows = JsonUtility.FromJson<List<Dictionary<string, object>>>(jsonData);

        // Create the class string
        string classString = "public class " + className + "\n{\n";

        // Loop through each dictionary in the rows list and add the variable declarations to the class string
        foreach (Dictionary<string, object> row in rows)
        {
            foreach (KeyValuePair<string, object> kvp in row)
            {
                string variableName = kvp.Key;
                object variableValue = kvp.Value;
                string variableType = variableValue.GetType().ToString();

                classString += "    public " + variableType + " " + variableName + ";\n";
            }
        }

        // Finish the class string
        classString += "}";

        // Write the class string to a file
        File.WriteAllText(classFilePath, classString);
    }

    private static object GetArrayData(string type, string[] data)
    {
        switch (type)
        {
            case "int":
                return Array.ConvertAll(data, int.Parse);
            case "float":
                return Array.ConvertAll(data, float.Parse);
            case "bool":
                return Array.ConvertAll(data, bool.Parse);
            default:
                return data;
        }
    }

    private static object GetData(string type, string data)
    {
        switch (type)
        {
            case "int":
                int.TryParse(data, out int intValue);
                return intValue;
            case "float":
                float.TryParse(data, out float floatValue);
                return floatValue;
            case "bool":
                bool.TryParse(data, out bool boolValue);
                return boolValue;
            default:
                return data;
        }
    }
}
#endif