#if UNITY_EDITOR
using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using ExcelDataReader;
using System.Data;
using System.Text;
using System.Linq;

namespace Tools
{
    public static class DataGenerator
    {
        private const string ExcelFileExtension = ".xlsx";

        private static readonly StructGenerator structGenerator = new();
        private static readonly JsonGenerator jsonGenerator = new();
        private static readonly ContainerManagerGenerator containerManagerGenerator = new();

        private static string JsonListTextName => Path.GetFileName(PathDefine.JsonListText);
        private static string VersionTextName => Path.GetFileName(PathDefine.VersionText);

        public static void GenerateDataFromExcelFoler(string folderPath, string jsonPath, int version)
        {
            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                Debug.LogError("유효한 폴더 경로가 아닙니다.");
                return;
            }

            if (string.IsNullOrEmpty(jsonPath))
            {
                Debug.LogError("Json 저장 경로가 없습니다.");
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
                GenerateDataFromExcelFile(excelPath.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar), jsonPath);
                progress += 1f / excelFiles.Length;
            }

            GenerateContainerManager(jsonPath, progress);
            GenerateJsonList(jsonPath, progress);
            GenerateVersion(jsonPath, version, progress);

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

                        //시트는 하나만 사용할 것
                        DataTable sheet = dataSet.Tables[0];

                        string excelFileName = Path.GetFileNameWithoutExtension(readExcelPath);

                        StringBuilder log = new();

                        GenerateStructFromExcelSheet(readExcelPath, sheet, ref log);
                        GenerateJsonFromExcelSheet(readExcelPath, sheet, ref log, saveJsonPath);

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

        private static void GenerateJsonFromExcelSheet(string filePath, DataTable sheet, ref StringBuilder log, string jsonSavePath)
        {
            jsonGenerator.Init(filePath, sheet, jsonSavePath);
            jsonGenerator.Generate(ref log);
        }

        private static void GenerateContainerManager(string jsonPath, float progress)
        {
            EditorUtility.DisplayProgressBar(PathDefine.Manager, $"DataContainerManager 작성중..", progress);

            string[] dataNames = Directory.GetFiles(jsonPath, $"*.json").Select(x => $"Data{Path.GetFileNameWithoutExtension(x)}").ToArray();

            containerManagerGenerator.Init();
            containerManagerGenerator.Generate(dataNames);
        }

        private static void GenerateJsonList(string jsonPath, float progress)
        {
            EditorUtility.DisplayProgressBar("Finisihing", $"JsonList.txt 작성중..", progress);

            string jsonListPath = Path.Combine(jsonPath, JsonListTextName);
            string[] jsonFiles = Directory.GetFiles(jsonPath, $"*.json").Select(Path.GetFileName).ToArray();
            string jsonListDesc = string.Empty;

            if (jsonFiles.Length == 0)
            {
                Debug.LogError("경로에 json이 없음!");
                return;
            }

            jsonListDesc = string.Join(",", jsonFiles);

            if (File.Exists(jsonListPath))
            {
                if (File.ReadAllText(jsonListPath) == jsonListDesc)
                {
                    Debug.Log("JsonList 변경 없음");
                    return;
                }
            }

            File.WriteAllText(jsonListPath, jsonListDesc);
            Debug.Log("JsonList 생성 완료");
        }

        private static void GenerateVersion(string jsonPath, int version, float progress)
        {
            EditorUtility.DisplayProgressBar("Finisihing", $"Version.txt 작성중..", progress);

            File.WriteAllText(Path.Combine(jsonPath, VersionTextName), version.ToString());
            Debug.Log($"Version : {version}");
        }
    }
}
#endif