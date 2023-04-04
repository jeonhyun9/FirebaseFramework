#if UNITY_EDITOR
using System.IO;
using System.Text;
using UnityEngine;

namespace Tools
{
    public class ContainerManagerGenerator : BaseGenerator
    {
        public void Init()
        {
            GeneratorType = Type.Container;
        }

        public bool Generate(string[] dataTypeList)
        {
            System.Array.Sort(dataTypeList);

            StringBuilder typesBuilder = new();
            StringBuilder cotainerManagerBuilder = new();
            string containerManager;

            foreach (string dataType in dataTypeList)
            {
                typesBuilder.AppendLine(GetDataTemplate(PathDefine.AddContainerTypeTemplate, name: dataType));
            }

            cotainerManagerBuilder.AppendLine(GetDataTemplate(PathDefine.DataContainerManagerTemplate, name: FileName));

            containerManager = cotainerManagerBuilder.AppendLine
                (GetDataTemplate(PathDefine.AddContainerTemplate, type: typesBuilder.ToString())).ToString();

            if (File.Exists(SavePath) && File.ReadAllText(SavePath) == containerManager)
            {
                Debug.Log($"======== {FileNameWithExtension} 변경점 없음 ========");
                return false;
            }

            Debug.Log($"{FileNameWithExtension} {(File.Exists(SavePath) ? "수정" : "생성")} 완료");

            File.WriteAllText(SavePath, containerManager);

            return true;
        }
    }
}
#endif
