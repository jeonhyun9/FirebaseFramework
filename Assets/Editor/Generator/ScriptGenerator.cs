using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Tools
{
    public class ScriptGenerator : BaseGenerator
    {
        public enum ScriptType
        {
            MVC,
            Unit,
            Manager,
        }

        private void GenerateScript(string templatePath, string name, string saveName)
        {
            string contents = GetDataTemplate(templatePath, ("name", name));
            SaveFileAtPath(folderPath, saveName, contents);
        }

        #region GenerateScript By Type
        private string GetFolderPathByScriptType(ScriptType type)
        {
            switch (type)
            {
                case ScriptType.MVC:
                case ScriptType.Unit:
                    return PathDefine.ContentsScriptsFolderPath;

                case ScriptType.Manager:
                    return PathDefine.Manager;
            }

            return null;
        }

        public void Generate(ScriptType type, string name)
        {
            folderPath = GetFolderPathByScriptType(type);

            if (string.IsNullOrEmpty(folderPath))
            {
                Logger.Null("folder Path");
                return;
            }

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            switch (type)
            {
                case ScriptType.MVC:
                    GenerateMVC(name);
                    break;
                case ScriptType.Unit:
                    GenerateUnit(name);
                    break;
                case ScriptType.Manager:
                    GenerateScript(TemplatePathDefine.ManagerTemplate, name, $"{name}Manager.cs");
                    AssetDatabase.Refresh();
                    break;
            }
        }

        private void GenerateMVC(string name)
        {
            string controllerName = $"{name}Controller.cs";
            string modelName = $"{name}ViewModel.cs";
            string viewName = $"{name}View.cs";

            GenerateScript(TemplatePathDefine.MVC_ControllerTemplate, name, controllerName);
            GenerateScript(TemplatePathDefine.MVC_ViewModelTemplate, name, modelName);
            GenerateScript(TemplatePathDefine.MVC_ViewTemplate, name, viewName);

            CreatePrefab(name, Path.GetFileNameWithoutExtension(viewName));
        }

        private void GenerateUnit(string name)
        {
            string unitName = $"{name}Unit.cs";
            string modelName = $"{name}UnitModel.cs";

            GenerateScript(TemplatePathDefine.UnitTemplate, name, unitName);
            GenerateScript(TemplatePathDefine.UnitModelTemplate, name, modelName);

            CreatePrefab(name, Path.GetFileNameWithoutExtension(unitName));
        }

        private void CreatePrefab(string folderName, string prefabName)
        {
            AssetDatabase.Refresh();

            GameObject newPrefab = new (prefabName);

            string prefabFolderPath = $"{PathDefine.PrefabResourcesFullPath}/{folderName}";

            if (!Directory.Exists(prefabFolderPath))
                Directory.CreateDirectory(prefabFolderPath);

            string prefabPath = $"{prefabFolderPath}/{prefabName}.prefab";

            Object existingPrefab = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject));

            if (existingPrefab != null)
            {
                PrefabUtility.SaveAsPrefabAssetAndConnect(newPrefab, prefabPath, InteractionMode.UserAction);
            }
            else
            {
                PrefabUtility.SaveAsPrefabAsset(newPrefab, prefabPath);
            }

            if (newPrefab != null)
                Object.DestroyImmediate(newPrefab);

            AssetDatabase.Refresh();
        }
        #endregion
    }
}
