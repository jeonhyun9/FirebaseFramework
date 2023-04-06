#if UNITY_EDITOR
using System.IO;
using System.Linq;
using System;
using UnityEngine;

namespace Tools
{
    public class ContainerManagerGenerator : BaseGenerator
    {
        private string ContainerManagerName => Path.GetFileName(PathDefine.DataContainerManager);
        public ContainerManagerGenerator()
        {
            InitType(Type.ContainerManager);
            folderPath = PathDefine.Manager;
        }
        
        public void Generate(string[] dataTypeList)
        {
            Array.Sort(dataTypeList);

            string types = string.Join(Environment.NewLine, dataTypeList.Select(dataType =>
                GetDataTemplate(PathDefine.AddContainerTypeTemplate, name: dataType)));

            string containerManager = GetDataTemplate(PathDefine.DataContainerManagerTemplate, type: types);

            OnEndGenerate(folderPath, ContainerManagerName, containerManager);
        }
    }
}
#endif
