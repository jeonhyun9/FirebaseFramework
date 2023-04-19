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
        }

        private void GenerateScript(string templatePath, string name, string saveName)
        {
            string contents = GetDataTemplate(templatePath, ("name", name));
            SaveFileAtPath(templatePath, saveName, contents);
        }

        #region GenerateScript By Type
        public void Generate(ScriptType type, string name)
        {
            switch (type)
            {
                case ScriptType.MVC:
                    GenerateMVC(name);
                    break;
                case ScriptType.Unit:
                    GenerateUnit(name);
                    break;
            }
        }

        private void GenerateMVC(string name)
        {
            string controllerName = $"{name}Controller.cs";
            string modelName = $"{name}ViewModel.cs";
            string viewName = $"{name}View.cs";

            GenerateScript(TemplatePathDefine.MVC_ControllerTemplate, name, controllerName);
            GenerateScript(TemplatePathDefine.MVC_ModelTemplate, name, modelName);
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

            string prefabPath = $"{prefabFolderPath}/{prefabName}";

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
