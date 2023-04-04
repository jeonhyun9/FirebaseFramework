using System.IO;
using UnityEngine;

public class BaseGenerator
{
    protected enum Type
    {
        Json,
        Struct,
        Container,
    }

    protected int DataTypeIndex
    {
        get
        {
            return 1;
        }
    }

    protected int NameIndex
    {
        get
        {
            return DataTypeIndex + 1;
        }
    }
    protected int ValueIndex
    {
        get
        {
            return NameIndex + 1;
        }
    }

    protected Type GeneratorType { get; set; }

    protected string FilePath { get; set; }

    protected string FileName
    {
        get
        {
            return Path.GetFileNameWithoutExtension(FilePath);
        }
    }

    protected string FileNameWithExtension
    {
        get
        {
            switch (GeneratorType)
            {
                case Type.Json:
                    return $"{FileName}.json";
                case Type.Struct:
                    return $"Data{FileName}.cs";
                case Type.Container:
                    return $"DataContainerManager.cs";
                default:
                    return null;
            }
        }
    }

    protected string FileNameWithoutExtension
    {
        get
        {
            return Path.GetFileNameWithoutExtension(FileNameWithExtension);
        }
    }

    protected string SavePath
    {
        get
        {
            string savePath = null;

            switch (GeneratorType)
            {
                case Type.Json:
                    savePath = PathDefine.Json;
                    break;
                case Type.Struct:
                    savePath = PathDefine.DataStruct;
                    break;
                case Type.Container:
                    savePath = PathDefine.Manager;
                    break;
            }

            return Path.Combine(savePath, FileNameWithExtension);
        }
    }

    protected string GetDataTemplate(string path, string type = null, string name = null)
    {
        if (!File.Exists(path))
        {
            Debug.LogError($"{path}에 해당 템플릿이 없습니다.");
            return null;
        }

        var template = File.ReadAllText(path);

        if (type != null)
        {
            template = template.Replace("#type#", type);
        }

        if (name != null)
        {
            template = template.Replace("#name#", name);
        }

        return template;
    }
}
