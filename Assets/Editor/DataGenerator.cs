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
                Debug.LogError(".xlsx ������ ��ȯ�ؾ� �մϴ�.");
                return;
            }

            GenerateDataFromExcel(assetPath, true);
        }

        public static void GenerateDataFromExcelFoler(string folderPath)
        {
            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                Debug.LogError("��ȿ�� ���� ��ΰ� �ƴմϴ�.");
                return;
            }

            int i = 0;

            foreach (string excelPath in Directory.GetFiles(folderPath, "*.xlsx"))
            {
                EditorUtility.DisplayProgressBar(folderPath, $"{excelPath} ��ȯ��..", (float)i / Directory.GetFiles(folderPath, "*.xlsx").Length);
                GenerateDataFromExcel(excelPath.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
                i++;
            }

            EditorUtility.ClearProgressBar();
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

                //��Ʈ�� �ϳ��� ����� ��
                DataTable sheet = dataSet.Tables[0];

                GenerateJsonFromExcelSheet(fileName, sheet);
                GenerateStructFromExcelSheet(fileName, sheet);
                GenerateDataContainer(fileName);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                Debug.LogError(e.StackTrace);
            }
            finally
            {
                //���ϸ� ���� ��� �߻�
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
                        Type arrayType = GetDataType(dataType.Replace("[]", ""));

                        Array array = Array.CreateInstance(arrayType, values.Length);
                        for (int k = 0; k < values.Length; k++)
                        {
                            object arrayValue = GetConvertValue(arrayType.ToString(),values[k]);
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
                    Debug.LogError($"Json �������� �����ϴ�. {saveJsonPath}");
                    return;
                }
                else
                {
                    changed = true;
                }
            }

            File.WriteAllText(saveJsonPath, newJson);

            Debug.Log($"{jsonName}.json {(changed ? "����" : "����")} �Ϸ�");
        }

        private static void GenerateStructFromExcelSheet(string fileName, DataTable sheet)
        {
            string structName = $"Data{Path.GetFileNameWithoutExtension(fileName)}";
            string saveDataStructPath = Path.Combine(PathDefine.DataStructPath, $"{structName}.cs");

            //��Ʈ���� ������ Ÿ�԰� �̸��� �̾Ƴ���
            List<string> columnNames = new();
            List<string> columnTypes = new();

            DataRow dataTypeRow = sheet.Rows[dataTypeIndex];
            DataRow nameRow = sheet.Rows[NameIndex];

            for (int j = 0; j < sheet.Columns.Count; j++)
            {
                string dataType = dataTypeRow[j].ToString().Replace("enum:", "");
                string name = nameRow[j].ToString();
                columnTypes.Add(dataType);
                columnNames.Add(name);
            }

            StringBuilder sb = new();

            //�Ҵ���ߴٰ� waring �ߴ� �� ���ſ�
            sb.AppendLine("#pragma warning disable 0649");

            sb.AppendLine("using Newtonsoft.Json;");
            sb.AppendLine();
            sb.AppendLine($"public struct {structName} : IBaseData");
            sb.AppendLine("{");

            for (int i = 0; i < columnNames.Count; i++)
            {
                string type = columnTypes[i];
                string name = columnNames[i].Replace("enum:", "");
                string propertyName = $"\"{name}\"";

                sb.AppendLine($"    [JsonProperty(PropertyName = {propertyName})]");
                sb.AppendLine($"    public readonly {type} {name};");
            }

            sb.AppendLine();
            sb.AppendLine("    public bool IsInit => Id == 0;");
            sb.AppendLine("}");

            bool changed = false;

            if (File.Exists(saveDataStructPath))
            {
                if (File.ReadAllText(saveDataStructPath).Equals(sb.ToString()))
                {
                    Debug.LogError($"Struct �������� �����ϴ�. {saveDataStructPath}");
                    return;
                }
                else
                {
                    changed = true;
                }
            }

            File.WriteAllText(saveDataStructPath, sb.ToString());

            Debug.Log($"{structName}.cs {(changed ? "����" : "����")} �Ϸ�");
        }

        private static void GenerateDataContainer(string fileName)
        {
            string structName = $"Data{Path.GetFileNameWithoutExtension(fileName)}";
            string containerName = $"{structName}Container";
            string saveDataContainerPath = Path.Combine(PathDefine.DataContainerPath, $"{containerName}.cs");

            string dataContainerText = File.ReadAllText(PathDefine.DataContainerTemplatePath);

            dataContainerText = dataContainerText.Replace("#structName#", structName);

            bool changed = false;

            if (File.Exists(saveDataContainerPath))
            {
                if (File.ReadAllText(saveDataContainerPath).Equals(dataContainerText))
                {
                    Debug.LogError($"Container �������� �����ϴ�. {saveDataContainerPath}");
                    return;
                }
                else
                {
                    changed = true;
                }
            }

            File.WriteAllText(saveDataContainerPath, dataContainerText);

            Debug.Log($"{containerName}.cs {(changed ? "����" : "����")} �Ϸ�");
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
                case "string":
                    return value;
                case string s when s.StartsWith("enum:"):
                    return value;
                default:
                    return Type.GetType(columnType);
            }
        }
    }
}
#endif