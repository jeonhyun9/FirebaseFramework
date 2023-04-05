#if UNITY_EDITOR
using System.IO;
using System.Linq;
using UnityEngine;
namespace Tools
{
    public class JsonListGenerator : BaseGenerator
    {
        public JsonListGenerator()
        {
            InitType(Type.JsonList);
        }

        public void Generate(string jsonFolderPathValue)
        {
            SetFolderPath(jsonFolderPathValue);

            string[] jsonFiles = Directory.GetFiles(jsonFolderPathValue, $"*.json").Select(Path.GetFileName).ToArray();
            string jsonListDesc;

            if (jsonFiles.Length == 0)
            {
                Logger.Error("No json files in path!");
                return;
            }

            jsonListDesc = string.Join(",", jsonFiles);

            OnEndGenerate(SavePath, jsonListDesc);
        }
    }
}
#endif
