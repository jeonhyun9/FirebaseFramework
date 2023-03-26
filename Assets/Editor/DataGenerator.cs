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
        private const string ExcelFileExtension = ".xlsx";

        public static void GenerateDataFromExcelFileWithRefresh(string assetPath)
        {
            if (!assetPath.Contains(ExcelFileExtension))
            {
                Debug.LogError(".xlsx 파일을 변환해야 합니다.");
                return;
            }

            GenerateDataFromExcelFile(assetPath);
            AssetDatabase.Refresh();
        }

        public static void GenerateDataFromExcelFoler(string folderPath)
        {
            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                Debug.LogError("유효한 폴더 경로가 아닙니다.");
                return;
            }

            string[] excelFiles = Directory.GetFiles(folderPath, "*.xlsx");
            float progress = 0f;

            foreach (string excelPath in excelFiles)
            {
                EditorUtility.DisplayProgressBar(folderPath, $"{excelPath} 변환중..", progress);
                GenerateDataFromExcelFile(excelPath.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
                progress += 1f / excelFiles.Length;
            }

            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
        }

        private static void GenerateDataFromExcelFile(string readExcelPath)
        {
            try
            {
                string fileName = Path.GetFileName(readExcelPath);

                using (FileStream fileStream = File.Open(readExcelPath, FileMode.Open, FileAccess.Read))
                {
                    using (IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(fileStream))
                    {
                        var dataSet = excelReader.AsDataSet();

                        //시트는 하나만 사용할 것
                        DataTable sheet = dataSet.Tables[0];

                        GenerateJsonFromExcelSheet(fileName, sheet);
                        GenerateStructFromExcelSheet(fileName, sheet);
                        GenerateDataContainer(fileName);
                    }
                }    
            }
            catch (Exception e)
            {
                Debug.LogError(e.StackTrace);
            }
        }

        private static void GenerateJsonFromExcelSheet(string filePath, DataTable sheet)
        {
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            string jsonName = $"{fileName}.json";
            string saveJsonPath = Path.Combine(PathDefine.Json, jsonName);

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

                    if (dataType.Contains("struct:"))
                        name += $"_{dataType.Replace("struct:", "")}";

                    string value = sheet.Rows[i][j].ToString();

                    if (dataType.Contains("[]"))
                    {
                        string[] values = value.Split(',');
                        Type arrayType = GetDataType(dataType.Replace("[]", ""));

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
            if (File.Exists(saveJsonPath))
            {
                string oldJson = File.ReadAllText(saveJsonPath);

                if (JToken.DeepEquals(newJson, oldJson))
                {
                    return;
                }
                else
                {
                    changed = true;
                }
            }

            File.WriteAllText(saveJsonPath, newJson);

            Debug.Log($"{jsonName}.json {(changed ? "수정" : "생성")} 완료");
        }

        private static void GenerateStructFromExcelSheet(string filePath, DataTable sheet)
        {
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            string structName = $"Data{fileName}";
            string saveDataStructPath = Path.Combine(PathDefine.DataStruct, $"{structName}.cs");

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

            sb.AppendLine(GetDataTemplate(PathDefine.StartDataTemplate, name: structName));

            for (int i = 0; i < columnNames.Count; i++)
            {
                string type = columnTypes[i].Replace("enum:", "");
                string name = columnNames[i];

                if (type.Contains("struct:"))
                {
                    type = type.Replace("struct:", "");
                    sb.AppendLine(GetDataTemplate(PathDefine.StructValueTemplate, type, name));
                }
                else
                {
                    sb.AppendLine(GetDataTemplate(PathDefine.DataValueTemplate, type, name));
                }
            }

            sb.AppendLine(GetDataTemplate(PathDefine.EndDateTemplate));

            bool changed = false;

            if (File.Exists(saveDataStructPath))
            {
                if (File.ReadAllText(saveDataStructPath).Equals(sb.ToString()))
                {
                    return;
                }
                else
                {
                    changed = true;
                }
            }

            File.WriteAllText(saveDataStructPath, sb.ToString());

            Debug.Log($"{structName}.cs {(changed ? "수정" : "생성")} 완료");
        }

        private static void GenerateDataContainer(string filePath)
        {
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            string structName = $"Data{fileName}";
            string containerName = $"{structName}Container";
            string saveDataContainerPath = Path.Combine(PathDefine.DataContainer, $"{containerName}.cs");

            string dataContainerText = File.ReadAllText(PathDefine.DataContainerTemplate);

            dataContainerText = dataContainerText.Replace("#fileName#", Path.GetFileNameWithoutExtension(fileName));

            bool changed = false;

            if (File.Exists(saveDataContainerPath))
            {
                if (File.ReadAllText(saveDataContainerPath).Equals(dataContainerText))
                {
                    return;
                }
                else
                {
                    changed = true;
                }
            }

            File.WriteAllText(saveDataContainerPath, dataContainerText);

            Debug.Log($"{containerName}.cs {(changed ? "수정" : "생성")} 완료");
        }

        private static Type GetDataType(string columnType)
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
                    return Type.GetType(s.Replace("enum:", ""));
                default:
                    return Type.GetType(columnType);
            }
        }

        private static object GetConvertValue(string columnType, string value)
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

        private static string GetDataTemplate(string path, string type = null, string name = null)
        {
            if (!File.Exists(path))
            {
                Debug.LogError($"{path}에 해당 템플릿이 없습니다.");
                return null;
            }

            var template = File.ReadAllText(path);

            if (type != null)
            {
                template = template.Replace("#type#", type);
            }

            if (name != null)
            {
                template = template.Replace("#name#", name);
            }

            return template;
        }
    }
}
#endif