#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using ExcelDataReader;
using Newtonsoft.Json;
using System.Data;
using Newtonsoft.Json.Linq;
using System.Text;

namespace Tools
{
    public static class DataGenerator
    {
        private const int dataTypeIndex = 1;

        private static int NameIndex
        {
            get
            {
                return dataTypeIndex + 1;
            }
        }
        private static int ValueIndex
        {
            get
            {
                return NameIndex + 1;
            }
        }
        private static FileStream fileStream = null;
        private static IExcelDataReader excelReader = null;

        public static void GenerateDataFromExcel(string assetPath)
        {
            if (!assetPath.Contains(".xlsx"))
            {
                Debug.LogError(".xlsx 파일을 변환해야 합니다.");
                return;
            }

            GenerateDataFromExcel(assetPath, true);
        }

        public static void GenerateDataFromExcelFoler(string folderPath)
        {
            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                Debug.LogError("유효한 폴더 경로가 아닙니다.");
                return;
            }

            foreach (string excelPath in Directory.GetFiles(folderPath, ".xlsx"))
                GenerateDataFromExcel(excelPath);

            AssetDatabase.Refresh();
        }

        private static void GenerateDataFromExcel(string readExcelPath, bool refreshAssetDatabase = false)
        {
            try
            {
                string fileName = Path.GetFileName(readExcelPath);
                fileStream = File.Open(readExcelPath, FileMode.Open, FileAccess.Read);
                excelReader = ExcelReaderFactory.CreateOpenXmlReader(fileStream);

                var dataSet = excelReader.AsDataSet();

                //시트는 하나만 사용할 것
                DataTable sheet = dataSet.Tables[0];

                GenerateJsonFromExcelSheet(fileName, sheet);
                GenerateStructFromExcelSheet(fileName, sheet);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                Debug.LogError(e.StackTrace);
            }
            finally
            {
                //안하면 에러 계속 발생
                if (excelReader != null)
                    excelReader.Close();

                if (fileStream != null)
                    fileStream.Close();
            }

            if (refreshAssetDatabase)
                AssetDatabase.Refresh();
        }

        private static void GenerateJsonFromExcelSheet(string fileName, DataTable sheet)
        {
            string jsonName = Path.GetFileNameWithoutExtension(fileName);
            string saveJsonPath = Path.Combine(PathDefine.JsonPath, $"{jsonName}.json");

            List<Dictionary<string, object>> dataDicList = new();

            DataRow dataTypeRow = sheet.Rows[dataTypeIndex];
            DataRow nameRow = sheet.Rows[NameIndex];

            for (int i = ValueIndex; i < sheet.Rows.Count; i++)
            {
                Dictionary<string, object> dataDic = new();
                for (int j = 0; j < sheet.Columns.Count; j++)
                {
                    string dataType = dataTypeRow[j].ToString();
                    string name = nameRow[j].ToString();
                    string value = sheet.Rows[i][j].ToString();

                    if (dataType.Contains("[]"))
                    {
                        string[] values = value.Split(',');
                        Type arrayType = GetDataType(dataType);
                        Type valueType = GetDataType(dataType.Replace("[]", ""));

                        Array array = Array.CreateInstance(arrayType, values.Length);
                        for (int k = 0; k < values.Length; k++)
                        {
                            object arrayValue = GetConvertValue(valueType, values[k]);
                            array.SetValue(arrayValue, k);
                        }
                        dataDic.Add(name, array);
                    }
                    else
                    {
                        Type type = GetDataType(dataTypeRow[j].ToString());
                        object convertValue = GetConvertValue(type, value);
                        dataDic.Add(name, convertValue);
                    }
                }
                dataDicList.Add(dataDic);
            }

            string newJson = JsonConvert.SerializeObject(dataDicList);

            if (File.Exists(saveJsonPath))
            {
                string oldJson = File.ReadAllText(saveJsonPath);

                if (CheckJsonChange(newJson, oldJson))
                {
                    Debug.LogError($"Json 변경점이 없습니다. {saveJsonPath}");
                    return;
                }
            }

            File.WriteAllText(saveJsonPath, newJson);

            Debug.Log($"{jsonName}.json 생성 완료");
        }

        private static void GenerateStructFromExcelSheet(string fileName, DataTable sheet)
        {
            string className = $"Data{Path.GetFileNameWithoutExtension(fileName)}";
            string saveDataStructPath = Path.Combine(PathDefine.DataStructPath, $"{className}.cs");

            //시트에서 데이터 타입과 이름만 뽑아놓기
            List<string> columnNames = new();
            List<string> columnTypes = new();

            DataRow dataTypeRow = sheet.Rows[dataTypeIndex];
            DataRow nameRow = sheet.Rows[NameIndex];

            for (int j = 0; j < sheet.Columns.Count; j++)
            {
                string dataType = dataTypeRow[j].ToString();
                string name = nameRow[j].ToString();
                columnTypes.Add(dataType);
                columnNames.Add(name);
            }

            StringBuilder sb = new();

            sb.AppendLine($"//Generated Date : {DateTime.Now}");
            sb.AppendLine($"//Author : Jhg");

            //할당안했다고 waring 뜨는 것 제거용
            sb.AppendLine("#pragma warning disable 0649");

            sb.AppendLine("using Newtonsoft.Json;");
            sb.AppendLine();
            sb.AppendLine($"public struct {className} : IBaseData");
            sb.AppendLine("{");

            for (int i = 0; i < columnNames.Count; i++)
            {
                string type = columnTypes[i];
                string name = columnNames[i].Replace("enum:", "");
                string propertyName = $"\"{name}\"";

                sb.AppendLine($"    [JsonProperty(PropertyName = {propertyName})]");
                sb.AppendLine($"    public readonly {type} {name};");
            }

            sb.AppendLine("}");

            if (File.Exists(saveDataStructPath))
            {
                if (File.ReadAllText(saveDataStructPath).Equals(sb.ToString()))
                {
                    Debug.LogError($"Struct 변경점이 없습니다. {saveDataStructPath}");
                    return;
                }
            }

            File.WriteAllText(saveDataStructPath, sb.ToString());

            Debug.Log($"{className}.cs 생성 완료");
        }

        private static Type GetDataType(string columnType)
        {
            if (columnType.Contains("[]"))
                columnType = columnType.Replace("[]", "");

            switch (columnType)
            {
                case "int":
                    return typeof(int);
                case "float":
                    return typeof(float);
                case "string":
                    return typeof(string);
                case "int[]":
                    return typeof(int[]);
                case "float[]":
                    return typeof(float[]);
                case "string[]":
                    return typeof(string[]);
                case string s when s.StartsWith("enum:"):
                    return Type.GetType(s.Replace("enum:", ""));
                default:
                    throw new Exception("Invalid column type: " + columnType);
            }
        }

        private static object GetConvertValue(Type type, string value)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Int32:
                    return int.Parse(value);
                case TypeCode.Single:
                    return float.Parse(value);
                case TypeCode.String:
                    return value;
                case TypeCode.Object when type.IsEnum:
                    return Enum.Parse(type, value);
                default:
                    throw new Exception("Invalid column type: " + type.FullName);
            }
        }

        public static bool CheckJsonChange(string newJson, string oldJson)
        {
            return JToken.DeepEquals(newJson, oldJson);
        }
    }
}
#endif