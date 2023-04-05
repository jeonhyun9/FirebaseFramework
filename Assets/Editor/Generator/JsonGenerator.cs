using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace Tools
{
    public class JsonGenerator : BaseGenerator
    {
        public void Init(string pathValue, string jsonSavePathValue)
        {
            InitType(Type.Json);
            SetFolderPath(jsonSavePathValue);
            SetFileNameWithoutExtension(pathValue);
        }

        public void Generate(DataTable sheet)
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
                    string name = GetNaming(nameRow[j].ToString(), dataType);
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

            OnEndGenerate(SavePath, newJson);
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
                    System.Type type = System.Type.GetType(s.Replace("enum:", ""));
                    if (type == null)
                        Logger.Warning($"Undefined enum : {columnType}");
                    return type;
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
}
