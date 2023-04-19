#if UNITY_EDITOR
using System.IO;
using System.Linq;
using System;
using UnityEngine;

namespace Tools
{
    public class DataManagerGenerator : BaseGenerator
    {
        private string ContainerManagerName => Path.GetFileName(PathDefine.DataManager);
        public DataManagerGenerator()
        {
            folderPath = PathDefine.Manager;
        }
        
        public void Generate(string[] dataTypeList)
        {
            Array.Sort(dataTypeList);

            string types = string.Join(Environment.NewLine, dataTypeList.Select(dataType =>
                GetDataTemplate(TemplatePathDefine.AddContainerTypeTemplate, ("name", dataType))));

            string containerManager = GetDataTemplate(TemplatePathDefine.DataManagerTemplate, ("type", types));

            SaveFileAtPath(folderPath, ContainerManagerName, containerManager);
        }
    }
}
#endif
