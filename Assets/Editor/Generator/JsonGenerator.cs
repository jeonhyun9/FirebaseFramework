using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using UnityEngine;
using System.Text;
using System.Linq;

public class JsonGenerator : BaseGenerator
{
    private DataTable sheet;
    private string jsonSavePath;

    public void Init(string pathValue, DataTable sheetValue, string jsonSavePathValue)
    {
        GeneratorType = Type.Json;
        FilePath = pathValue;
        sheet = sheetValue;
        jsonSavePath = Path.Combine(jsonSavePathValue, FileNameWithExtension);
    }

    public bool Generate(ref StringBuilder log)
    {
        List<Dictionary<string, object>> dataDicList = new();

        DataRow dataTypeRow = sheet.Rows[DataTypeIndex];
        DataRow nameRow = sheet.Rows[NameIndex];

        for (int i = ValueIndex; i < sheet.Rows.Count; i++)
        {
            Dictionary<string, object> dataDic = new();
            for (int j = 0; j < sheet.Columns.Count; j++)
            {
                string dataType = dataTypeRow[j].ToString();
                string name = nameRow[j].ToString();

                if (name.Contains("Id") || name.Contains("NameId"))
                    name = $"_{name}";

                if (dataType.Contains("struct:"))
                    name += $"_{dataType.Replace("struct:", "")}";

                string value = sheet.Rows[i][j].ToString();

                if (dataType.Contains("[]"))
                {
                    string[] values = value.Split(',');
                    System.Type arrayType = GetDataType(dataType.Replace("[]", ""));

                    Array array = Array.CreateInstance(arrayType, values.Length);
                    for (int k = 0; k < values.Length; k++)
                    {
                        object arrayValue = GetConvertValue(dataType.Replace("[]", ""), values[k]);
                        array.SetValue(arrayValue, k);
                    }
                    dataDic.Add(name, array);
                }
                else
                {
                    object convertValue = GetConvertValue(dataType, value);
                    dataDic.Add(name, convertValue);
                }
            }
            dataDicList.Add(dataDic);
        }

        string newJson = JsonConvert.SerializeObject(dataDicList);

        bool changed = false;
        if (File.Exists(jsonSavePath))
        {
            string oldJson = File.ReadAllText(jsonSavePath);

            if (JToken.DeepEquals(newJson, oldJson))
            {
                return false;
            }
            else
            {
                changed = true;
            }
        }

        File.WriteAllText(jsonSavePath, newJson);

        log.AppendLine($"{FileNameWithExtension} {(changed ? "수정" : "생성")} 완료");

        return true;
    }

    private System.Type GetDataType(string columnType)
    {
        switch (columnType)
        {
            case "int":
                return typeof(int);
            case "float":
                return typeof(float);
            case "string":
            case string s when s.StartsWith("struct:"):
                return typeof(string);
            case string s when s.StartsWith("enum:"):
                return System.Type.GetType(s.Replace("enum:", ""));
            default:
                return System.Type.GetType(columnType);
        }
    }

    private object GetConvertValue(string columnType, string value)
    {
        switch (columnType)
        {
            case "int":
                return int.Parse(value);
            case "float":
                return float.Parse(value);
            case string s when s.StartsWith("struct:"):
                return value;
            default:
                return value;
        }
    }
}
