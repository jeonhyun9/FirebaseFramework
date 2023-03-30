#if UNITY_EDITOR
using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using ExcelDataReader;
using System.Data;
using System.Text;

namespace Tools
{
    public static class DataGenerator
    {
        private const string ExcelFileExtension = ".xlsx";

        private static StructGenerator structGenerator = new();
        private static ContainerGenerator containerGenerator = new();
        private static JsonGenerator jsonGenerator = new();

        public static void GenerateDataFromExcelFileWithRefresh(string assetPath)
        {
            if (!assetPath.Contains(ExcelFileExtension))
            {
                Debug.LogError(".xlsx ������ ��ȯ�ؾ� �մϴ�.");
                return;
            }

            GenerateDataFromExcelFile(assetPath);
            AssetDatabase.Refresh();
        }

        public static void GenerateDataFromExcelFoler(string folderPath)
        {
            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                Debug.LogError("��ȿ�� ���� ��ΰ� �ƴմϴ�.");
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
                        GenerateJsonFromExcelSheet(readExcelPath, sheet, ref log);

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

        private static void GenerateJsonFromExcelSheet(string filePath, DataTable sheet, ref StringBuilder log)
        {
            jsonGenerator.Init(filePath, sheet);
            jsonGenerator.Generate(ref log);
        }
    }
}
#endif