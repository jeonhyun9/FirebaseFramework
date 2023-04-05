#if UNITY_EDITOR
using System.IO;
using System.Linq;
using System;
using UnityEngine;

namespace Tools
{
    public class ContainerManagerGenerator : BaseGenerator
    {
        public ContainerManagerGenerator()
        {
            InitType(Type.ContainerManager);
        }
        
        public void Generate(string[] dataTypeList)
        {
            Array.Sort(dataTypeList);

            string types = string.Join(Environment.NewLine, dataTypeList.Select(dataType =>
                GetDataTemplate(PathDefine.AddContainerTypeTemplate, name: dataType)));

            string containerManager = GetDataTemplate(PathDefine.DataContainerManagerTemplate, type: types);

            OnEndGenerate(SavePath, containerManager);
        }
    }
}
#endif
