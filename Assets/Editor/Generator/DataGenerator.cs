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

            string[] excelFiles = Directory.GetFiles(folderPath, $"*{ExcelFileExtension}");
            float progress = 0f;

            if (excelFiles.Length == 0)
            {
                Debug.LogError($"{folderPath} 에 엑셀 파일이 없습니다.");
                return;
            }

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
                using (FileStream fileStream = File.Open(readExcelPath, FileMode.Open, FileAccess.Read))
                {
                    using (IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(fileStream))
                    {
                        var dataSet = excelReader.AsDataSet();

                        //시트는 하나만 사용할 것
                        DataTable sheet = dataSet.Tables[0];

                        string excelFileName = Path.GetFileNameWithoutExtension(readExcelPath);

                        StringBuilder log = new();

                        GenerateStructFromExcelSheet(readExcelPath, sheet, ref log);
                        GenerateDataContainer(readExcelPath, ref log);
                        GenerateJsonFromExcelSheet(readExcelPath, sheet, ref log);

                        if (log.Length > 0)
                        {
                            Debug.Log($"======== {excelFileName} 갱신 ========");
                            Debug.Log(log.ToString());
                        }
                        else
                        {
                            Debug.Log($"======== {excelFileName} 변경점 없음 ========");
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