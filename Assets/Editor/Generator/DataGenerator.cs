#if UNITY_EDITOR
using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using ExcelDataReader;
using System.Data;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace Tools
{
    public static class DataGenerator
    {
        private const string ExcelFileExtension = ".xlsx";

        private static StructGenerator structGenerator = new();
        private static ContainerGenerator containerGenerator = new();
        private static JsonGenerator jsonGenerator = new();

        private static string JsonListTextName => Path.GetFileName(PathDefine.JsonListText);
        private static string VersionTextName => Path.GetFileName(PathDefine.VersionText);


        //public static void GenerateDataFromExcelFileWithRefresh(string assetPath)
        //{
        //    if (!assetPath.Contains(ExcelFileExtension))
        //    {
        //        Debug.LogError(".xlsx ������ ��ȯ�ؾ� �մϴ�.");
        //        return;
        //    }
        //
        //    GenerateDataFromExcelFile(assetPath);
        //    AssetDatabase.Refresh();
        //}

        public static void GenerateDataFromExcelFoler(string folderPath, string jsonPath, int version)
        {
            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                Debug.LogError("��ȿ�� ���� ��ΰ� �ƴմϴ�.");
                return;
            }

            if (string.IsNullOrEmpty(jsonPath))
            {
                Debug.LogError("Json ���� ��ΰ� �����ϴ�.");
                return;
            }

            string[] excelFiles = Directory.GetFiles(folderPath, $"*{ExcelFileExtension}");
            float progress = 0f;

            if (excelFiles.Length == 0)
            {
                Debug.LogError($"{folderPath} �� ���� ������ �����ϴ�.");
                return;
            }

            foreach (string excelPath in excelFiles)
            {
                EditorUtility.DisplayProgressBar(folderPath, $"{excelPath} ��ȯ��..", progress);
                GenerateDataFromExcelFile(excelPath.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar), jsonPath);
                progress += 1f / excelFiles.Length;
            }

            GenerateJsonList(folderPath, jsonPath, progress);

            GenerateVersion(folderPath, jsonPath, version, progress);

            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
        }

        private static void GenerateDataFromExcelFile(string readExcelPath, string saveJsonPath)
        {
            try
            {
                using (FileStream fileStream = File.Open(readExcelPath, FileMode.Open, FileAccess.Read))
                {
                    using (IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(fileStream))
                    {
                        var dataSet = excelReader.AsDataSet();

                        //��Ʈ�� �ϳ��� ����� ��
                        DataTable sheet = dataSet.Tables[0];

                        string excelFileName = Path.GetFileNameWithoutExtension(readExcelPath);

                        StringBuilder log = new();

                        GenerateStructFromExcelSheet(readExcelPath, sheet, ref log);
                        GenerateDataContainer(readExcelPath, ref log);
                        GenerateJsonFromExcelSheet(readExcelPath, sheet, ref log, saveJsonPath);

                        if (log.Length > 0)
                        {
                            Debug.Log($"======== {excelFileName} ���� ========");
                            Debug.Log(log.ToString());
                        }
                        else
                        {
                            Debug.Log($"======== {excelFileName} ������ ���� ========");
                        }
                    }
                }    
            }
            catch (Exception e)
            {
                Debug.LogError(e.StackTrace);
            }
        }

        private static void GenerateStructFromExcelSheet(string filePath, DataTable sheet, ref StringBuilder log)
        {
            structGenerator.Init(filePath, sheet);
            structGenerator.Generate(ref log);
        }
        private static void GenerateDataContainer(string filePath, ref StringBuilder log)
        {
            containerGenerator.Init(filePath);
            containerGenerator.Generate(ref log);
        }

        private static void GenerateJsonFromExcelSheet(string filePath, DataTable sheet, ref StringBuilder log, string jsonSavePath)
        {
            jsonGenerator.Init(filePath, sheet, jsonSavePath);
            jsonGenerator.Generate(ref log);
        }

        private static void GenerateJsonList(string folderPath, string jsonPath, float progress)
        {
            string jsonListPath = Path.Combine(jsonPath, JsonListTextName);
            string[] jsonFiles = Directory.GetFiles(jsonPath, $"*.json").Select(Path.GetFileName).ToArray();
            string jsonListDesc = string.Empty;

            if (jsonFiles.Length > 0)
            {
                EditorUtility.DisplayProgressBar(folderPath, $"JsonList.txt �ۼ���..", progress);
                jsonListDesc = string.Join(",", jsonFiles);

                if (File.Exists(jsonListPath))
                {
                    if (File.ReadAllText(jsonListPath) == jsonListDesc)
                    {
                        Debug.Log("JsonList ���� ����");
                        return;
                    }
                }
            }
            else
            {
                Debug.LogError("��ο� json�� ����!");
                return;
            }

            File.WriteAllText(jsonListPath, jsonListDesc);
            Debug.Log("JsonList ���� �Ϸ�");
        }

        private static void GenerateVersion(string folderPath, string jsonPath, int version, float progress)
        {
            EditorUtility.DisplayProgressBar(folderPath, $"Version.txt �ۼ���..", progress);
            File.WriteAllText(Path.Combine(jsonPath, VersionTextName), version.ToString());
            Debug.Log($"Version : {version}");
        }
    }
}
#endif