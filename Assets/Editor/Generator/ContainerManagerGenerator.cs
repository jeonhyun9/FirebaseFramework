using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class ContainerManagerGenerator : BaseGenerator
{
    public void Init()
    {
        GeneratorType = Type.Container;
    }

    public bool Generate(List<string> dataTypeList)
    {
        dataTypeList.Sort();

        StringBuilder typesBuilder = new();
        StringBuilder cotainerManagerBuilder = new();
        string containerManager;

        foreach(string dataType in dataTypeList)
        {
            typesBuilder.AppendLine(GetDataTemplate(PathDefine.AddContainerTypeTemplate, name: dataType));
        }

        cotainerManagerBuilder.AppendLine(GetDataTemplate(PathDefine.DataContainerManagerTemplate, name: FileName));

        containerManager = cotainerManagerBuilder.AppendLine
            (GetDataTemplate(PathDefine.AddContainerTemplate, type: typesBuilder.ToString())).ToString();

        bool changed = false;

        if (File.Exists(SavePath))
        {
            if (File.ReadAllText(SavePath).Equals(containerManager))
            {
                Debug.Log($"======== {FileNameWithExtension} 변경점 없음 ========");
                return false;
            }
            else
            {
                changed = true;
            }
        }

        File.WriteAllText(SavePath, containerManager);

        Debug.Log($"{FileNameWithExtension} {(changed ? "수정" : "생성")} 완료");

        return true;
    }
}
