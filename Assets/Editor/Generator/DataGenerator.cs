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

        //�ݺ�����ϴ� generator�� ������
        private static readonly StructGenerator structGenerator = new();
        private static readonly JsonGenerator jsonGenerator = new();
        
        private static float progress;
        private static string excelFolderPath;
        private static string jsonFolderPath;
        private static int version;

        public static void GenerateDataFromExcelFoler(string excelFolderPathValue, string jsonFolderPathValue, int versionValue)
        {
            if (!AssetDatabase.IsValidFolder(excelFolderPathValue))
            {
                Logger.Error("Invalid folder path");
                return;
            }

            if (string.IsNullOrEmpty(jsonFolderPathValue))
            {
                Logger.Null("Json save path");
                return;
            }

            Initialize(excelFolderPathValue, jsonFolderPathValue, versionValue);

            string[] excelFiles = Directory.GetFiles(excelFolderPath, $"*{ExcelFileExtension}");

            if (excelFiles.Length == 0)
            {
                Logger.Error($"There is no excel file in {excelFolderPath}.");
                return;
            }

            if (GeneratedDataFromExcelFilePaths(excelFiles))
            {
                GenerateContainerManager();
                GenerateJsonList();
                GenerateVersion();
            }
            else
            {
                Logger.Error("Error occured while generated data.");
            }
            
            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
        }

        private static void Initialize(string folderPathValue, string jsonPathValue, int versionValue)
        {
            excelFolderPath = folderPathValue;
            jsonFolderPath = jsonPathValue;
            version = versionValue;

            progress = 0f;
        }

        /// <summary> ���������� ���������� ���� �߻� </summary>
        public static bool GeneratedDataFromExcelFilePaths(string[] excelFiles)
        {
            Logger.Log("----------------Check Excel Start-----------------");
            for (int i = 0; i < excelFiles.Length; i++)
            {
                string excelPath = excelFiles[i];
                EditorUtility.DisplayProgressBar(excelFolderPath, $"Converting {excelPath}..", progress);

                if (!GenerateDataFromExcelPath(excelPath.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)))
                    return false;

                progress += 1f / excelFiles.Length;
            }
            Logger.Log("----------------Check Excel End------------------");

            return true;
        }

        private static bool GenerateDataFromExcelPath(string readExcelPath)
        {
            try
            {
                using (FileStream fileStream = File.Open(readExcelPath, FileMode.Open, FileAccess.Read))
                using (IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(fileStream))
                {
                    var dataSet = excelReader.AsDataSet();

                    //��Ʈ�� �ϳ��� ����� ��
                    DataTable sheet = dataSet.Tables[0];

                    string excelFileName = Path.GetFileName(readExcelPath);

                    GenerateStructFromExcelSheet(readExcelPath, sheet);
                    GenerateJsonFromExcelSheet(readExcelPath, sheet);

                    return true;
                }
            }
            catch (Exception e)
            {
                Logger.Exception(e);
                return false;
            }
        }

        private static void GenerateStructFromExcelSheet(string readExcelPath, DataTable sheet)
        {
            structGenerator.Init(readExcelPath);
            structGenerator.Generate(sheet);
        }

        private static void GenerateJsonFromExcelSheet(string readExcelPath, DataTable sheet)
        {
            jsonGenerator.Init(readExcelPath, jsonFolderPath);
            jsonGenerator.Generate(sheet);
        }

        private static void GenerateContainerManager()
        {
            EditorUtility.DisplayProgressBar(PathDefine.Manager, $"Writing DataContainerManager.cs..", progress);

            string[] dataNames = Directory.GetFiles(jsonFolderPath, $"*.json").Select(x => $"Data{Path.GetFileNameWithoutExtension(x)}").ToArray();

            ContainerManagerGenerator containerManagerGenerator = new();
            containerManagerGenerator.Generate(dataNames);
        }

        private static void GenerateJsonList()
        {
            EditorUtility.DisplayProgressBar("Finisihing", $"Writing JsonList.txt..", progress);

            JsonListGenerator jsonListGenerator = new ();
            jsonListGenerator.Generate(jsonFolderPath);
        }

        private static void GenerateVersion()
        {
            EditorUtility.DisplayProgressBar("Finisihing", $"Version.txt �ۼ���..", progress);

            VersionTextGenerator versionTextGenerator = new ();
            versionTextGenerator.Generate(jsonFolderPath, version.ToString());
        }
    }
}
#endif