using System.IO;
using System.Text;
using UnityEngine;

public class ContainerGenerator : BaseGenerator
{
    public void Init(string pathValue)
    {
        GeneratorType = Type.Container;
        FilePath = pathValue;
    }

    public bool Generate(ref StringBuilder log)
    {
        string dataContainer = GetDataTemplate(PathDefine.DataContainerTemplate, name:FileName);

        bool changed = false;

        if (File.Exists(SavePath))
        {
            if (File.ReadAllText(SavePath).Equals(dataContainer))
            {
                return false;
            }
            else
            {
                changed = true;
            }
        }

        File.WriteAllText(SavePath, dataContainer);

        log.AppendLine($"{FileNameWithExtension} {(changed ? "수정" : "생성")} 완료");

        return true;
    }
}
